using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Parse;
using com.bzr.puzzleBattle;

public class SinglePlayerProgression : MonoBehaviour {

	static SinglePlayerProgression instance;

	public static SinglePlayerProgression Instance
	{
		get { return SinglePlayerProgression.instance; }
	}

	////begin deprecated
	public static bool isDragUnlocked{
		get { return level>=instance.unlockDragAtLevel;}
	}
	public static bool isSwipeUnlocked{
		get { return level>=instance.unlockSwipeAtLevel;}
	}
	public static bool isTwistLeftUnlocked{
		get { return level>=instance.unlockTwistLeftAtLevel;}
	}
	public static bool isTwistRightUnlocked{
		get { return level>=instance.unlockTwistRightAtLevel;}
	}
	public static bool isShakeUnlocked{
		get { return level>=instance.unlockShakeAtLevel;}
	}

	public int startingPatternLength = 3;

	public int unlockSwipeAtLevel = 4;
	public int unlockDragAtLevel = 7;
	public int unlockTwistLeftAtLevel = 10;
	public int unlockTwistRightAtLevel = 13;
	public int unlockShakeAtLevel = 16;

	/// end deprecated 

	[SerializeField]
	public levelDefinition[] levels;

	[SerializeField]
	public List<achievement> _debugAchievements;

	public Dictionary<string,achievement> achievements;

/*
Pattern length 1-3: taps only
4-6: flick enabled
7-9: drag enabled
10-12: rotate clockwise enabled
13-15: rotate counterclockwise enabled
16+: colour swap enabled
*/

	//public int debugLevel;

	Queue<KeyValuePair<string,int>> thingsForTheMainThread;

	public static int level {
		get {return EncryptedPlayerPrefs.GetInt("level",0);}
		set {
			maxUnlockedLevel=Mathf.Max(value,maxUnlockedLevel);
			instance.SetAchievementProgress("levelnum", maxUnlockedLevel);
		    EncryptedPlayerPrefs.SetInt("level",value); 
		}
	}
	
	public static int maxUnlockedLevel {
		get {return instance._maxUnlockedLevel;}
		set {instance._maxUnlockedLevel = value;
			instance.SetIntLater("maxUnlockedLevel",value); 
			ParseUser.CurrentUser["maxUnlockedLevel"]=value;
			ParseUser.CurrentUser.SaveAsync();}
	}

	public static int progressInCurrentLevel{
		get {return EncryptedPlayerPrefs.GetInt("progressInCurrentLevel",0);}
		set {EncryptedPlayerPrefs.SetInt("progressInCurrentLevel",value); }
	}

	public static int money {
		get {
			return instance._money;
		}
		set { instance._money = value;
			instance.SetAchievementProgress("artifacts",instance._money);
			instance.SetIntLater("money",instance._money); 
			ParseUser.CurrentUser["artifacts"]=instance._money;
			ParseUser.CurrentUser.SaveAsync();
		}	
	}

	private int _money;
	private int _maxUnlockedLevel;

	public static int patternLength {
		get { 
			if(instance.levels[level].endLength==-1){
			instance.levels[level].endLength =  instance.levels[level].Min_ColourSwap
												+ instance.levels[level].Min_Tap
												+ instance.levels[level].Min_Drag
												+ instance.levels[level].Min_Rotation
												+ instance.levels[level].Min_Flick
												+ instance.levels[level].Min_Random;
			}

			int _patternLength = Mathf.Min((instance.levels[level].startLength +  progressInCurrentLevel) , instance.levels[level].endLength);

//			Debug.Log ("level " + (level+1).ToString() + " progress: " + progressInCurrentLevel + " startLength: " + instance.levels[level].startLength + " endLength: " + instance.levels[level].endLength + " currentLength: " + _patternLength);

			return _patternLength;
		}
	}

	// Use this for initialization
	void Awake () {
		if( instance == null )
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
			EncryptedPlayerPrefs.keys=new string[5];
			
			EncryptedPlayerPrefs.keys[0]="520155718c4";
			
			EncryptedPlayerPrefs.keys[1]="627a826d1ea";
			
			EncryptedPlayerPrefs.keys[2]="874164e7e61";
			
			EncryptedPlayerPrefs.keys[3]="377d2e8c4e9";
			
			EncryptedPlayerPrefs.keys[4]="181abed8dd48";

			LoadLevelsFromFile();

            thingsForTheMainThread = new Queue<KeyValuePair<string, int>>();
        }
		else if( instance != this )
		{
			Destroy( this.gameObject );
			return;
		}

	}

	void LoadLevelsFromFile(){
		TextAsset levelData= Resources.Load("Levels-Table 1") as TextAsset;
		var rows = levelData.text.Split('\n');
		levels = new levelDefinition[rows.Length-2];
		for(int i=2; i<rows.Length; i++){
//			Debug.Log(rows[i]);
			if(!string.IsNullOrEmpty(rows[i])){
				var strings = rows[i].Split(',');
				var newLevel = new levelDefinition();
				newLevel.nbOfPlates=int.Parse(strings[1]);
				newLevel.Min_Tap=int.Parse(strings[2]);
				newLevel.Min_Drag=int.Parse(strings[3]);
				newLevel.Min_Flick=int.Parse(strings[4]);
				newLevel.Min_Rotation=int.Parse(strings[5]);
				newLevel.Min_ColourSwap=int.Parse(strings[6]);
				newLevel.Min_Random=int.Parse(strings[7]);
				newLevel.endLength=int.Parse(strings[8]);
				newLevel.Rand_Tap=bool.Parse(strings[9]);
				newLevel.Rand_Drag=bool.Parse(strings[10]);
				newLevel.Rand_Flick=bool.Parse(strings[11]);
				newLevel.Rand_Rotation=bool.Parse(strings[12]);
				newLevel.Rand_ColourSwap=bool.Parse(strings[13]);
				newLevel.startLength=int.Parse(strings[14]);
				newLevel.replayCost=int.Parse(strings[15]);
				newLevel.winReward=int.Parse(strings[16]);
				levels[i-2]=newLevel;
			}
		}

	}

	void LoadAchievementsFromFile(){
		TextAsset achievementData= Resources.Load("Achievements-Table 1") as TextAsset;
		var rows = achievementData.text.Split('\n');
		achievements = new Dictionary<string, achievement>();
		for(int i=1; i<rows.Length; i++){
			var strings = rows[i].Split(',');
			var newachievement = new achievement();

			newachievement.tiers = new tier[3];
			newachievement.id=int.Parse(strings[0]);
			newachievement.name=strings[1];

			newachievement.title=strings[2];
			newachievement.description=strings[3];

			tier tier1 = new tier();
			tier1.unlock = int.Parse(strings[4]);
			tier1.reward = int.Parse(strings[7]);
			newachievement.tiers[0]=tier1;

			tier tier2 = new tier();
			tier2.unlock = int.Parse(strings[5]);
			tier2.reward = int.Parse(strings[8]);
			newachievement.tiers[1]=tier2;

			tier tier3 = new tier();
			tier3.unlock = int.Parse(strings[6]);
			tier3.reward = int.Parse(strings[9]);
			newachievement.tiers[2]=tier3;

			newachievement.progress = EncryptedPlayerPrefs.GetInt("progress_"+newachievement.name,0);
			newachievement.currentTier = EncryptedPlayerPrefs.GetInt("tier_"+newachievement.name,0);

			achievements[newachievement.name]=newachievement;
			_debugAchievements.Add(newachievement);
		}

	}

	public void CalculateRewards(out int winner, out int loser){

		int totalMoves = MultiplayerGameManager.Instance.multiplayerSequence.Count;
		int variety = 0;
		int taps = 0;
		int drags = 0;
		int flicks = 0;
		int twists = 0;
		int shakes = 0;

		foreach(Move move in MultiplayerGameManager.Instance.multiplayerSequence)
		{
			switch((MoveDefinition.MoveType) move.move_id)
			{
				case MoveDefinition.MoveType.CLICK:
					if(taps==0) variety++;
					taps++;
				break;
				case MoveDefinition.MoveType.DRAG:
					if(drags==0) variety++;
					drags++;
				break;
				case MoveDefinition.MoveType.SWIP:
					if(flicks==0) variety++;
					flicks++;
				break;
				case MoveDefinition.MoveType.TWIST:
					if(twists==0) variety++;
					twists++;
				break;
				case MoveDefinition.MoveType.SHAKE:
					if(shakes==0) variety++;
					shakes++;
				break;
			}
		}

//		Debug.Log(totalMoves + " moves");
//		Debug.Log(taps + " taps");
//		Debug.Log(drags + " drags");
//		Debug.Log(flicks + " flicks");
//		Debug.Log(shakes + " shakes");
//		Debug.Log(twists + " twists");
//		Debug.Log(variety + " different types of move");

		int totalReward = Mathf.FloorToInt( 
                   			  totalMoves*MultiplayerGameManager.Instance.EndingPatternLengthCoefficient
							+ taps * MultiplayerGameManager.Instance.TapCoefficient
							+ drags * MultiplayerGameManager.Instance.DragCoefficient
							+ flicks * MultiplayerGameManager.Instance.FlickCoefficient
							+ twists * MultiplayerGameManager.Instance.RotateCoefficient
							+ shakes * MultiplayerGameManager.Instance.ColourSwapCoefficient
							+ variety * MultiplayerGameManager.Instance.VariabilityBonus
		                                   );

//		Debug.Log("total available reward: " + totalReward);

		int win = Mathf.CeilToInt(totalReward*0.7f); 
//		Debug.Log("winner reward: " + win);
		
		int lose = Mathf.FloorToInt(totalReward*0.3f); 
//		Debug.Log("loser reward: " + lose);
		
		winner = win;
		loser = lose;

	}

	/// <summary>
	/// win, for leaderboard purposes
	/// </summary>
	/// <param name="level">Level.</param>
	/// <param name="patternLength">Pattern length.</param>
	public void LogEvent(int patternLength, bool isMultiplayer, bool win = true){
		if(!isMultiplayer && !win)
			return;
		ParseObject finishedGame = new ParseObject("FinishedGame");
		if(isMultiplayer)
		{
			finishedGame["user"] = win ? ParseUser.CurrentUser : MultiplayerGameManager.Instance.currentOpponent;
			finishedGame["opponent"] = win ? ParseUser.CurrentUser : MultiplayerGameManager.Instance.currentOpponent;
		}
		else if(win)
			finishedGame["user"] = ParseUser.CurrentUser;

		finishedGame["isMultiplayer"] = isMultiplayer;

		finishedGame["patternLength"] = patternLength;

		int winReward = 0;
		int loseReward = 0;

		if(isMultiplayer)
			CalculateRewards(out winReward, out loseReward);

		finishedGame["winReward"] = winReward;
		finishedGame["loseReward"] = loseReward;

		finishedGame.SaveAsync().ContinueWith(t =>
		                                       {
			if (!(t.IsFaulted || t.IsCanceled))
			{
				ParseUser.CurrentUser.FetchAsync().ContinueWith(t2 =>
				                                                {
					if (!(t2.IsFaulted || t2.IsCanceled))
					{
						money = ParseUser.CurrentUser.Get<int>("artifacts");
					}
				});
			}
		});
	}

	public void AchievementProgress(string achievementName, int amount){
		achievements[achievementName].progress += amount;
		Debug.Log("Achievement " + achievementName + " progressed to " + achievements[achievementName].progress);

		if( achievements[achievementName].progress >= achievements[achievementName].tiers[achievements[achievementName].currentTier].unlock
	   	 && achievements[achievementName].currentTier < achievements[achievementName].tiers.Length )
		{
			achievements[achievementName].currentTier++;
			money+= achievements[achievementName].tiers[achievements[achievementName].currentTier].reward;
			Debug.Log("Achievement " + achievementName + " tier " + achievements[achievementName].currentTier + " unlocked!" );
		}

		instance.SetIntLater("progress_"+achievementName,achievements[achievementName].progress);
		instance.SetIntLater("tier_"+achievementName,achievements[achievementName].currentTier);
	}

	public void SetAchievementProgress(string achievementName, int value){
		achievements[achievementName].progress = Mathf.Max(value, achievements[achievementName].progress);
		Debug.Log("Achievement " + achievementName + " set to " + achievements[achievementName].progress);
		
		if( achievements[achievementName].progress >= achievements[achievementName].tiers[achievements[achievementName].currentTier].unlock
		   && achievements[achievementName].currentTier < achievements[achievementName].tiers.Length -1 )
		{
			achievements[achievementName].currentTier++;
			money+= achievements[achievementName].tiers[achievements[achievementName].currentTier].reward;
			Debug.Log("Achievement " + achievementName + " tier " + achievements[achievementName].currentTier + " unlocked!" );
		}
		
		SetIntLater("progress_"+achievementName,achievements[achievementName].progress);
		SetIntLater("tier_"+achievementName,achievements[achievementName].currentTier);
	}

    

	void Start(){

		instance._money = EncryptedPlayerPrefs.GetInt("money",0);
		instance._maxUnlockedLevel = EncryptedPlayerPrefs.GetInt("maxUnlockedLevel",0);

		LoadAchievementsFromFile();

	}

	// Update is called once per frame
	void Update () {
		if(thingsForTheMainThread.Count>0)
		{
			KeyValuePair<string,int> thing = thingsForTheMainThread.Dequeue();
			EncryptedPlayerPrefs.SetInt(thing.Key,thing.Value);
		}
	}

	void SetIntLater (string key, int value){
		KeyValuePair<string,int> kvp = new KeyValuePair<string, int>(key, value);
		thingsForTheMainThread.Enqueue(kvp);
	}

	//TODO Jellyfish
	public void ReturnStatus(){
		GameObject retObj = GameObject.Find("GlyphsController");
		retObj.SendMessage("UpdateLevel", level);
		print ("returned level");
	}

	public static bool SpendMoney(int howMuch){
		if(money>=howMuch){
			money-=howMuch;
			return true;
		}
		else{
			return false;
		}
	}

	[System.Serializable]
	public class levelDefinition{
		public int Min_Tap;
		public int Min_Drag;
		public int Min_Flick;
		public int Min_Rotation;
		public int Min_ColourSwap;
		public int Min_Random;
		public bool Rand_Tap;
		public bool Rand_Drag;
		public bool Rand_Flick;
		public bool Rand_Rotation;
		public bool Rand_ColourSwap;
		public int nbOfPlates;
		public int startLength=3;
		[HideInInspector]
		public int endLength = -1;
		public int replayCost;
		public int winReward;
	}

	[System.Serializable]
	public class achievement{
		public string name;
		public string title;
		public string description;
		public int id;
		public tier[] tiers;

		public int currentTier;
		public int progress;
	}

	[System.Serializable]
	public class tier{
		public int unlock;
		public int reward;
	}

}
