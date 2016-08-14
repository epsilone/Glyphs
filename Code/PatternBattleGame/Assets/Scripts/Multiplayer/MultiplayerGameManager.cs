using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using com.bzr.puzzleBattle;
using System.Threading;

public class MultiplayerGameManager : MonoBehaviour {

	[SerializeField]
	public List<Move> multiplayerSequence;

	[SerializeField]
	public List<Move> addedMoves;

	public Challenge challengeManager;
	public PlateHandler plateHandler;
	//public ParseObject currentChallenge;

	static MultiplayerGameManager instance;

	public ParseUser currentOpponent;

	public string currentGameObjectId;

	public static bool isMultiplayer = false;

	public LoadLevelOnClick sceneLoader;
	public TransitionGUIonClick dummyClick;

	public int costForSequence;

	private bool loadScene = false;

	public float EndingPatternLengthCoefficient = 0.2f;
	public float TapCoefficient = 0.2f;
	public float FlickCoefficient = 0.25f;
	public float DragCoefficient = 0.3f;
	public float RotateCoefficient = 0.3f;
	public float ColourSwapCoefficient = 0.3f;
	public float VariabilityBonus = 0.4f;

	//public int startingPatternLength = 3;
	
	public static MultiplayerGameManager Instance
	{
		get { return MultiplayerGameManager.instance; }
	}

	void Awake () {
		if( instance == null )
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
		else if( instance != this )
		{
			Destroy( this);
			return;
		}
		if(challengeManager==null)
			challengeManager = GameObject.FindObjectOfType<Challenge>();
		if(plateHandler==null)
			plateHandler=GameObject.FindObjectOfType<PlateHandler>();
		if(sceneLoader==null){
			GameObject go = GameObject.Find("toGamePlayFlow");
			if(go!=null)
				sceneLoader=go.GetComponent<LoadLevelOnClick>();
		}
		
		if(dummyClick==null){
			GameObject go = GameObject.Find("toDummy");
			if(go!=null)
				dummyClick=go.GetComponent<TransitionGUIonClick>();
		}
	}

	void Update()
	{
		if(loadScene)
		{	
			loadScene=false;
			if(dummyClick!=null)
				dummyClick.OnClick();
			if(sceneLoader!=null)
				sceneLoader.OnClick();
		}

	}

	void OnLevelWasLoaded(int lvl){

		if(Application.loadedLevelName=="LoadingScreen")
			return;

		if(challengeManager==null)
			challengeManager = GameObject.FindObjectOfType<Challenge>();
		if(plateHandler==null)
			plateHandler=GameObject.FindObjectOfType<PlateHandler>();//.GetComponent<PlateHandler>();

		if(sceneLoader==null){
			GameObject go = GameObject.Find("toGamePlayFlow");
			if(go!=null)
				sceneLoader=go.GetComponent<LoadLevelOnClick>();
		}

		if(dummyClick==null){
			GameObject go = GameObject.Find("toDummy");
			if(go!=null)
				dummyClick=go.GetComponent<TransitionGUIonClick>();
		}
	
	}
	//TODO Jellyfish
	public void ReturnStatus(){
		GameObject retObj = GameObject.Find("GlyphsController");
		retObj.SendMessage("UpdateMulti", isMultiplayer);
		print ("returned multi");
	}

	public void ContinueGame(List<Move> sequence){
	
		multiplayerSequence = sequence;
		addedMoves = new List<Move>();
		costForSequence = 0;
		isMultiplayer = true;

		SinglePlayerProgression.Instance.AchievementProgress("playmulti", 1);

		loadScene=true;

	}

	public void ContinueGame(object sequence){

		List<Move> tempSequence = sequence as List<Move>;
		ContinueGame(tempSequence);

	}

	public void SaveGame(){

		SinglePlayerProgression.SpendMoney(costForSequence);
		challengeManager.saveChallenge(multiplayerSequence.ToArray(),currentOpponent,addedMoves.ToArray(),currentGameObjectId);
		if(currentOpponent!=null){
			FB.AppRequest("It's your turn!",new string[]{currentOpponent.Get<string>("facebookId")});
#if UNITY_ANDROID
			GCM.ShowToast ("Sent challenge to " + currentOpponent.Get<string>("name"));
#endif
		}
		costForSequence=0;
	}

	[ContextMenu("fake multiplayer")]
	public void fakeMultiplayer(){
		isMultiplayer = true;
	}

	public void NewRandomGame(){
		Debug.Log("random button pressed");
		StartCoroutine(_NewRandomGame());
	}

	private IEnumerator _NewRandomGame(){
		currentOpponent=null;
		currentGameObjectId=null;
		isMultiplayer = true;

		Debug.Log("attempting to start a random game...");
#if UNITY_ANDROID
		GCM.ShowToast(Localization.Get("randomLoading"));
#endif
		
		var query = ParseObject.GetQuery("MultiplayerSequence")
			.WhereDoesNotExist("playing_user")
			.WhereNotEqualTo("waiting_user", ParseUser.CurrentUser)
			.OrderBy("createdAt")
			.Include("playing_user")
			.Include("waiting_user");

		var queryTask = query.FirstAsync();

		while(!queryTask.IsCompleted) yield return 1;

		if(queryTask.IsFaulted || queryTask.IsCanceled)
		{
			Debug.LogError("error finding open games, making a new one...");
//			if(challengeManager!=null)
			   challengeManager.newChallenge(null);
		}
		else if(queryTask.Result!=null)
		{
			Debug.Log("got open game from Parse");
			ParseObject result = queryTask.Result;
			currentGameObjectId = result.ObjectId;
			currentOpponent = result.Get<ParseUser>("waiting_user");
			var sequence = new List<Move>();
			
			List<object> moves = result.Get<List<object>>("sequence");
			
			foreach (var move in moves){
				var dictionary = move as Dictionary<string,object>;
				sequence.Add (new Move(int.Parse(dictionary["move_id"].ToString()),int.Parse(dictionary["src_plate_id"].ToString()),int.Parse(dictionary["target_plate_id"].ToString()),int.Parse(dictionary["twistAngle"].ToString())));
			}
			
			ContinueGame(sequence);
		}
		else
		{
			Debug.Log("no open games found, making a new one...");
          challengeManager.newChallenge(null);
		}

	}

	public void AddMove(Move newMove){
	
		int costForMove=0;

		if(newMove.move_id==MoveDefinition.DRAG || newMove.move_id == MoveDefinition.SWIP){
			costForMove+=1;
		}
		else if(newMove.move_id==MoveDefinition.TWIST){
			costForMove+=2;
		}
		else if(newMove.move_id==MoveDefinition.SHAKE){
			costForMove+=3;
		}

		switch(addedMoves.Count){
		
			case (3):
				costForMove+=3;
			break;
			case (4):
				costForMove+=5;
			break;
			case (5):
				costForMove+=8;
			break;
			case (6):
				costForMove+=12;
			break;

		}

		costForSequence+=costForMove;

		if(addedMoves.Count<7 && SinglePlayerProgression.money>=costForSequence){
			//SinglePlayerProgression.SpendMoney(costForMove);
			//GCM.ShowToast("Spent " + costForMove + " artifact(s) to add move");
			addedMoves.Add(newMove);
		}
		else{
			Debug.Log("sequence too long or not enough money");
#if UNITY_ANDROID
			GCM.ShowToast ("Sequence too long or not enough artifacts");
#endif
			plateHandler.doneAdding();
			//SaveGame();
			//sceneLoader.OnClick();
		}
	
	}

	public void EndGame(){

		Analytics.Flurry.Instance.LogEvent("multiplayerEndGame");

		var query = new ParseQuery<ParseObject>("MultiplayerSequence");
		query.GetAsync(currentGameObjectId).ContinueWith(t =>
		                                      {
			ParseObject result = t.Result;

			result.DeleteAsync();
	
		});

		FB.AppRequest("You won!",new string[]{currentOpponent.Get<string>("facebookId")});
#if UNITY_ANDROID
//		GCM.ShowToast ("You just lost to " + currentOpponent.Get<string>("name"));
#endif
		SinglePlayerProgression.Instance.AchievementProgress("losemulti", 1);
	}

}
