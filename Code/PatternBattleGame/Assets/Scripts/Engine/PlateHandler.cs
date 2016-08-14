using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.bzr.puzzleBattle;

namespace com.bzr.puzzleBattle {

	public enum PlayMode {WAIT, INIT, DISPLAY, USER_INPUT, FEEDBACK_FAIL, FEEDBACK_WIN, ADD_MOVES};

	public class PlateHandler : MonoBehaviour {
		public GameObject[] plates;

		public GameObject[] GUITracker;
		
		public GameObject GamePlayGuiController;

		public GameObject GlyphsController;

		//public int SequenceLength = 1;
		[SerializeField]
		public Sequence sequence = null;

		public PlayMode mode = PlayMode.WAIT;
		public PlayMode nextMode = PlayMode.DISPLAY;
		private int currentMoveId = -1;

		public float Counter = 0f;

		bool GameComplete = false;

		public GameObject feedback;
		private Vector3[] initialPlates;

		//public int costToRestart = 2;

		private AudioSource myAudioSource;

		public AudioClip winSound;
		public AudioClip loseSound;

		public GameObject incomingLabel;

		public Move[] moves;

	//	public GameObject continueButton;
	//	public GameObject nextLevelButton;

		public GameObject payoutLabel;

	    private bool tutorialFailed;

		
		[SerializeField]
		private SequenceLengthLabel singlePlayerSequenceLabel;
		
		[SerializeField]
		private SequenceLengthLabel multiPlayerSequenceLabel;

		[SerializeField]
		private Countdown singleCountdown;
		[SerializeField]
		private Countdown multiCountdown;

		// Use this for initialization
		void Start () {

			var numPlates = plates.Length;
			
			if (!MultiplayerGameManager.isMultiplayer)
				numPlates = SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].nbOfPlates;

			GlyphsController.SendMessage("setNumGlyphs", numPlates);

//			if (!MultiplayerGameManager.isMultiplayer)
//				GlyphsController.SendMessage("doFlyIn");

			Analytics.Flurry.Instance.BeginLogEvent("PlaySession");

			Counter = 0f;

			Vector3 v = field.eulerAngles;
			v.x = Mathf.Round(field.eulerAngles.x / 90) * 90;
			v.y = Mathf.Round(field.eulerAngles.y / 90) * 90;
			v.z = Mathf.Round(field.eulerAngles.z / 90) * 90;

			field.eulerAngles = v;
			initialOrientation = Mathf.Round(v.x + v.y + v.z);

		//	initialPlates = new Vector3[plates.Length];

			//newGame();

			/*
			Move[] moves = MoveDefinition.GetPossibleMoves(plates, field);
			sequence = new Sequence (null, SequenceLength, moves);
			*/

			//PlaySequence();

			//moves = MoveDefinition.GetPossibleMoves(plates, field,SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level]);

			myAudioSource = (AudioSource) gameObject.AddComponent(typeof(AudioSource));



		}

		void Awake () {
			GlyphsController = GameObject.Find("GlyphsController");
		}
		/*
		void takeInitialPlates(){
			for (int i = 0; i < plates.Length; i++) {
				Vector3 p = plates[i].transform.position;
				initialPlates[i] = new Vector3(p.x, p.y, p.z);
			}
		}



*/

		void ResetField(bool immediate = false){
			iTween.Stop(field.gameObject,"RotateTo");
			iTween.RotateTo(field.gameObject,Vector3.zero,immediate? 0f : 1f);
		}

		void ResetPlates() {
/*			Vector3 v = field.eulerAngles;
			v.x = Mathf.Round(field.eulerAngles.x / 90) * 90;
			v.y = Mathf.Round(field.eulerAngles.y / 90) * 90;
			v.z = Mathf.Round(field.eulerAngles.z / 90) * 90;

			initialOrientation = Mathf.Round(v.x + v.y + v.z);
*/
			//ResetField();

			for (int i = 0; i < plates.Length; i++) {

				//plates[i].transform.position = initialPlates[i];


				SimonPlate plate = (SimonPlate)plates[i].GetComponent ("SimonPlate");
				//plate.initialTransform = initialPlates[i];
				//plate.takeInitialTransform();
				//plate.ResetState(immediate);
				iTween.Stop(plate.gameObject,"MoveTo");
				plate.ReallyResetState();

			}
		}

		
		void ResetColours() {
			
			for (int i = 0; i < plates.Length; i++) {

				SimonPlate plate = (SimonPlate)plates[i].GetComponent ("SimonPlate");
				//plate.initialTransform = initialPlates[i];
				//plate.takeInitialTransform();
				plate.altColour=false;
				plate.ChangeColour();
			}

		}

		void ChangeColours() {
			
			for (int i = 0; i < plates.Length; i++) {
				
				SimonPlate plate = (SimonPlate)plates[i].GetComponent ("SimonPlate");
				//plate.initialTransform = initialPlates[i];
				//plate.takeInitialTransform();
				plate.altColour=!plate.altColour;
				plate.ChangeColour();
			}
			
		}

		void InstanciatePlates() {
		}

		/*void PlaySequence() {
			for (int i = 0; i < sequence.moves.Length; i++) {
				Move currentMove = ((Move)sequence.moves[i]);
				SimonPlate plate = (SimonPlate) plates[currentMove.src_plate_id].GetComponent("SimonPlate");
				plate.DisplayMove(currentMove);
			}
		}*/

		public void doneRotating(){
			Debug.Log("done rotating");
			if(mode==PlayMode.WAIT){
				if(nextMode==PlayMode.DISPLAY)
					changeToDisplayMode();
				if(nextMode==PlayMode.ADD_MOVES)
					mode=nextMode;
			}
		}

		public void ChangeToWaitMode(){
			/*for (var i=0;i < plates.Length; i++) {
				plates[i].GetComponent<SimonPlate>().fadeOut(0f);
			}*/


			var numPlates = plates.Length;

			if (!MultiplayerGameManager.isMultiplayer)
				numPlates = SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].nbOfPlates;

			for (int i=0;i < numPlates; i++) {
				plates[i].gameObject.SetActive(true);
				plates[i].GetComponent<SimonPlate>().fadeIn();
				//	plates[i].GetComponent<SimonPlate>().rotateToZero();
			}
			for(int j=numPlates; j<plates.Length; j++){
				plates[j].gameObject.SetActive(false);
			}
			
			//ResetField();

			GamePlayGuiController.SendMessage("HideButtons");
			GamePlayGuiController.SendMessage("_doZoom");

			Counter = 0;
			mode = PlayMode.WAIT;

		}

		public void ChangeToInitMode(){
			mode = PlayMode.INIT;
			Counter = 0;

		}

		public void DoFlyin(){
			GlyphsController.SendMessage("doFlyIn");
			GlyphsController.SendMessage("TurnGlyphsAnimationControllerOn",0f);
			fadeOut();
//			GamePlayGuiController.SendMessage("doShrink",0.5f);
		}

		public void ChangeToAddMode(){
			nextMode = PlayMode.ADD_MOVES;
			GlyphsController.SendMessage("TurnGlyphsAnimationControllerOff",0f);
			ChangeToWaitMode();
		}

		public void doneAdding(){
			GamePlayGuiController.SendMessage("doShrink",1f);
			GamePlayGuiController.SendMessage("TurnGlyphsAnimationControllerOn",1f);
			for (var i=0;i < plates.Length; i++) {
				plates[i].GetComponent<SimonPlate>().fadeOut();
			}
			ChangeToInitMode();

		}

		void wait(){
			/*if (Counter > 3f) {
				if(nextMode==PlayMode.DISPLAY)
					changeToDisplayMode();
				if(nextMode==PlayMode.ADD_MOVES && Counter>5f){
					mode=nextMode;
				}
			}*/
		}

		void WaitForInit() {
		/*	if (Counter > 30f) {
				ChangeToWaitMode();
			} else{
					int i = (int)(Counter*10f % plates.Length);
					
					SimonPlate plate = (SimonPlate) plates[i].GetComponent("SimonPlate");
					plate.Highlight(true);
					if (i==0) i=plates.Length;
					Debug.Log (i-1);
					SimonPlate prevPlate = (SimonPlate) plates[i-1].GetComponent("SimonPlate");
					prevPlate.unHighlight(true);

				//Counter = ((Counter / 10) + 1) * 10;
			}*/
		}

		// Update is called once per frame
		void Update () {
			Counter += Time.deltaTime;
			switch (mode) {
				case PlayMode.INIT:
				//	WaitForInit();
					break;
				case PlayMode.DISPLAY:
					displayMode();
					break;
				case PlayMode.USER_INPUT:
					break;
				case PlayMode.FEEDBACK_WIN:
					win();
					break;
				case PlayMode.FEEDBACK_FAIL:
					fail();
					break;
				case PlayMode.WAIT:
					wait();
					break;
				default:
					break;
			}
		}
#if UNITY_EDITOR
		void OnGUI(){
			if(mode==PlayMode.USER_INPUT || mode==PlayMode.ADD_MOVES)
				if(GUI.Button(new Rect(50,50,100,30),"Shake"))
					OnShake();
		}
#endif
		public void changeToDisplayMode(){
			incomingLabel.SetActive(true);	
			//takeInitialPlates();


			//ResetPlates();
			//ResetColours();

			Counter = 0;
			mode=PlayMode.DISPLAY;
			GameComplete=false;

		}

		public void setLevelToOne(){
			var dict = new Dictionary<string, string>();
			dict.Add("level",SinglePlayerProgression.level.ToString());
			dict.Add("patternLength",SinglePlayerProgression.patternLength.ToString());
			dict.Add("money",SinglePlayerProgression.money.ToString());
			Analytics.Flurry.Instance.LogEvent("abandonedGame",dict);
			SinglePlayerProgression.progressInCurrentLevel=0;
			EncryptedPlayerPrefs.DeleteKey("sequence");
			//GameObject.Find("GlyphsController").SendMessage("doFlyOut");
		}

		public void changeToFailMode(){
			mode=PlayMode.FEEDBACK_FAIL;

			//Analytics.Flurry.Instance.EndLogEvent("PlayingGame");

			var dict = new Dictionary<string, string>();
			dict.Add("level",SinglePlayerProgression.level.ToString());
			dict.Add("patternLength",SinglePlayerProgression.patternLength.ToString());

			Analytics.Flurry.Instance.LogEvent("fail",dict);

			//GameComplete=true;
			Counter=0;
			currentMoveId=-1;

			ResetField(false);
			//ResetPlates();
			ResetColours();

			for (var i=0;i < plates.Length; i++) {
				plates[i].GetComponent<SimonPlate>().fadeOut();
			}

		    if (SinglePlayerProgression.level == 0)
		    {
		        tutorialFailed = true;
                GamePlayGuiController.SendMessage("TutorialFail");
            }

			GamePlayGuiController.SendMessage("doShrink",1f);
			GamePlayGuiController.SendMessage("TurnGlyphsAnimationControllerOn",1f);

			SinglePlayerProgression.Instance.LogEvent(sequence.moves.Length, MultiplayerGameManager.isMultiplayer, false);
			if(MultiplayerGameManager.isMultiplayer)
				MultiplayerGameManager.Instance.EndGame();

			//SinglePlayerProgression.level=1;
			//newGame();

			//ResetPlates();
			//ResetColours();
		}

		public void fadeOutCompletely(){
			for (var i=0;i < plates.Length; i++) {
				plates[i].GetComponent<SimonPlate>().fadeOutCompletely();
			}
		}
		public void fadeOut(){
			for (var i=0;i < plates.Length; i++) {
				plates[i].GetComponent<SimonPlate>().fadeOut();
			}
		}
		
		public void changeToWinMode(){
			//ResetField(true);
			//ResetPlates();

			//Analytics.Flurry.Instance.EndLogEvent("PlayingGame");

			var dict = new Dictionary<string, string>();
			dict.Add("level",SinglePlayerProgression.level.ToString());
			dict.Add("patternLength",SinglePlayerProgression.patternLength.ToString());
			
			Analytics.Flurry.Instance.LogEvent("win",dict);

			ResetColours();
			
			for (var i=0;i < plates.Length; i++) {
				plates[i].GetComponent<SimonPlate>().fadeOut();
			}

			GamePlayGuiController.SendMessage("TurnGlyphsAnimationControllerOn",1f);
			GamePlayGuiController.SendMessage("doShrink",1f);


			//SinglePlayerProgression.money++;
			//GameComplete=true;
			if(!MultiplayerGameManager.isMultiplayer)
			{
				if(sequence.moves.Length==SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].endLength)
				{
					singlePlayerSequenceLabel.transform.parent.gameObject.SetActive(true);
					singlePlayerSequenceLabel.UpdateLabel(SinglePlayerProgression.patternLength);

					GlyphsController.SendMessage("doFlyOutDelay", 2f);
					if(SinglePlayerProgression.level==SinglePlayerProgression.maxUnlockedLevel)
					{
						SinglePlayerProgression.money+=SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].winReward;
						payoutLabel.SetActive(true);
					}
					levelUp();
				}

			}
			else if(moves.Length==0)
			{
				SinglePlayerProgression.Instance.SetAchievementProgress("patternlengthmulti", sequence.moves.Length);
				SinglePlayerProgression.Instance.LogEvent(sequence.moves.Length, MultiplayerGameManager.isMultiplayer);
			}

			mode=PlayMode.FEEDBACK_WIN;
			Counter=0;

			//ResetPlates();
		}

		public void payMoneyToRestart(){
		
			IDictionary<string,string> dict = new Dictionary<string, string>();
			dict.Add("level",SinglePlayerProgression.level.ToString());
			dict.Add("patternLength",SinglePlayerProgression.patternLength.ToString());
			dict.Add("money",SinglePlayerProgression.money.ToString());

			if(SinglePlayerProgression.SpendMoney(SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].replayCost)){

				Analytics.Flurry.Instance.LogEvent("payMoneyToRestart",dict);
				SinglePlayerProgression.Instance.AchievementProgress("replays",1);
				GamePlayGuiController.SendMessage("TurnGlyphsAnimationControllerOff");
				restart();
				nextMode=PlayMode.DISPLAY;
				ChangeToWaitMode();
				GUITracker[3].SetActive(true);
				//newGame();
			}
			else{
				Analytics.Flurry.Instance.LogEvent("notEnoughMoneyToRestart",dict);
				GUITracker[2].SetActive(true);
			}

		}

		public void restart(){
			//mode=PlayMode.INIT;
			//EncryptedPlayerPrefs.DeleteKey("sequence");
			GameComplete=false;
			Counter=0;
			currentMoveId=-1;
			currentDraggedPlate = null;
			//GamePlayGuiController.SendMessage("doZoom");
			//ResetField();
			//ResetPlates();
			//ResetColours();
			Rotating=false;
			//ChangeToWaitMode();
			//GamePlayGuiController.SendMessage("TurnGlyphsAnimationControllerOn");
			//GamePlayGuiController.SendMessage("doShrink",0f);
			/*for (var i=0;i < plates.Length; i++) {
				plates[i].GetComponent<SimonPlate>().fadeIn();
			}

			Move[] moves = MoveDefinition.GetPossibleMoves(plates, field);
			sequence = new Sequence (null, SequenceLength, moves);
*/
		}

		public void levelUp(){
			Debug.Log("no more moves! level complete");
			SinglePlayerProgression.level++;
			SinglePlayerProgression.progressInCurrentLevel=0;
			EncryptedPlayerPrefs.DeleteKey("sequence");
			sequence = null;

		}

		public void newGame(){

			//Move[] moves = MoveDefinition.GetPossibleMoves(plates, field);
			//Move[] moves = MoveDefinition.GetPossibleMoves(plates, field,SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level]);

			if(mode==PlayMode.FEEDBACK_WIN || mode == PlayMode.USER_INPUT){
				if(moves.Length==0){
//					//moves = MoveDefinition.GetPossibleMoves(plates, field, SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level]);
					EncryptedPlayerPrefs.DeleteKey("sequence");
					Application.LoadLevel(Application.loadedLevel);
//					//GamePlayGuiController.SendMessage("TurnGlyphsAnimationControllerOff");
//					//GameObject.Find("GlyphsController").SendMessage("doFlyOut");
				}
				else{
					SinglePlayerProgression.progressInCurrentLevel++;
					sequence = new Sequence(sequence, Math.Min (SinglePlayerProgression.patternLength,SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].endLength), moves, out moves);
					EncryptedPlayerPrefs.SetString("sequence",sequence.toString());
				}


			}
			else{

				string oldSeqStr = EncryptedPlayerPrefs.GetString("sequence","");
				Sequence oldSequence = null;
				//moves = null;

				if(oldSeqStr!=""){

					Debug.Log("loading previous sequence from playerprefs");

					//IList listOfObjects = (IList) MiniJSON.Json.Deserialize(oldSeqStr);
					var newObj = (IDictionary) MiniJSON.Json.Deserialize(oldSeqStr);
					//Debug.Log((IList) newObj["moves"]);

					Move[] oldMoves = new Move[((IList) newObj["moves"]).Count];

					int i=0;

					foreach(IDictionary move in (IList) newObj["moves"]){
						var plate_id = move["src_plate_id"];
						var plate_id_int = Convert.ToInt32(plate_id);
						var newMove = new Move(Convert.ToInt32(move["move_id"]), Convert.ToInt32(move["src_plate_id"]) , Convert.ToInt32(move["target_plate_id"]), Convert.ToInt32(move["twistAngle"]) );
						//Debug.Log(newMove.toString());
						oldMoves[i]=newMove;
						if(newMove.src_plate_id>-1)
							newMove.src_plate=(SimonPlate)plates[newMove.src_plate_id].GetComponent ("SimonPlate");
						else
							newMove.field=field;
						if(newMove.target_plate_id>-1)
							newMove.target_plate=(SimonPlate)plates[newMove.target_plate_id].GetComponent ("SimonPlate");
						i++;
					}

					Debug.Log(oldMoves.Length + " possible moves");
					Debug.Log("pattern length: " + SinglePlayerProgression.patternLength);

					oldSequence = new Sequence(oldMoves);

					moves=null;
					Move[] newMoves = MoveDefinition.GetPossibleMoves(plates, field, SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level]);
					sequence = new Sequence(oldSequence, SinglePlayerProgression.patternLength, newMoves, out moves, SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].endLength);

				}
				else{
					Debug.Log("creating new sequence from level settings for level " + (SinglePlayerProgression.level+1).ToString());
					//GameObject.Find("GlyphsController").SendMessage("doFlyIn");
					moves = MoveDefinition.GetPossibleMoves(plates, field, SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level]);
					sequence = new Sequence (moves, Math.Min (SinglePlayerProgression.patternLength,SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].endLength), out moves);
				}
//				Debug.Log(sequence.toString());

				EncryptedPlayerPrefs.SetString("sequence",sequence.toString());
			}

		

			//IDictionary<string,string> dict = new Dictionary<string,string>();
			//dict.Add("level",SinglePlayerProgression.patternLength.ToString());

			//Analytics.Flurry.Instance.BeginLogEvent("PlayingGame",dict);

		}

		public void newMultiplayerGame(){

			MultiplayerGameManager.isMultiplayer = true;
			sequence=new Sequence(null,0,null);
			if(MultiplayerGameManager.Instance.multiplayerSequence.Count>0){
				sequence.moves = MultiplayerGameManager.Instance.multiplayerSequence.ToArray();
				foreach (Move move in sequence.moves){
					if(move.src_plate_id>-1)
						move.src_plate=(SimonPlate)plates[move.src_plate_id].GetComponent ("SimonPlate");
					else
						move.field=field;
					if(move.target_plate_id>-1)
						move.target_plate=(SimonPlate)plates[move.target_plate_id].GetComponent ("SimonPlate");
				}
			}
			else{
//				ChangeToAddMode();
			}

			DoFlyin();
				//sequence= new Sequence(sequence,MultiplayerGameManager.Instance.startingPatternLength,MoveDefinition.GetPossibleMoves(plates,field));

			//MultiplayerGameManager.Instance.SaveGame(sequence.moves);
		}


		private void displayMode() {

			if (Counter > 1.1f) {
				Counter=0f;
				currentMoveId += 1;
				
				if (currentMoveId < sequence.moves.Length) {


					Move NextMove = sequence.moves [currentMoveId];

					if (NextMove.move_id == MoveDefinition.TWIST)
					{

						for (int i = 0; i < plates.Length; i++) {
							SimonPlate p = (SimonPlate)plates [i].GetComponent ("SimonPlate");
							p.Highlight(false);
						}

					}
					else if (NextMove.move_id==MoveDefinition.SHAKE){
						ChangeColours();
					}

					//if(NextMove.src_plate_id>-1)
					NextMove.DisplayMove ();

				} else {
					if(MultiplayerGameManager.isMultiplayer == false){
						if(SinglePlayerProgression.level == 0)
                        {
                            if (tutorialFailed)
                            {
                                tutorialFailed = false;
                            }
                            else
                            {
                                //trigger tutorial part 2 here
                                GameObject.Find("TutorialManager").GetComponent<TutorialManager>().StartTutorial(1);
                            }
						}
                        else
                        {
							if(singleCountdown != null)
                            {
								if(singleCountdown.gameObject.activeSelf == false){
									singleCountdown.gameObject.SetActive(true);
								}
							}
							//print ("triggering sequence countdown");
							//singleCountdown.StartCountdown();
						
						}					
					} else {
						multiCountdown.StartCountdown();
					}

					mode = PlayMode.USER_INPUT;
					incomingLabel.SetActive(false);	
					ResetField();
					currentMoveId = -1;
					Counter=0;
					ResetColours();	
					applyNextMove ();
					for (int i = 0; i < plates.Length; i++) {
						SimonPlate p = (SimonPlate)plates [i].GetComponent ("SimonPlate");
						//p.takeInitialTransform();
						p.unHighlight();
					}
				}


			}
			else if(Counter>0.5f){

				for (int i = 0; i < plates.Length; i++) {
					SimonPlate p = (SimonPlate)plates [i].GetComponent ("SimonPlate");
					//p.takeInitialTransform();
					p.unHighlight();
				}
			}

		}

		private void userInputMode() {
			// TODO timer
		}

		private void applyNextMove() {

			//ResetPlates();
			currentMoveId ++;
			if (currentMoveId >= SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].endLength) {
				changeToWinMode();
			}
			else if (currentMoveId >= sequence.moves.Length) {
				restart ();
//				SinglePlayerProgression.progressInCurrentLevel++;
				newGame ();
				ChangeToWaitMode ();
				doneRotating ();
			}
			else{
				Move currentMove = sequence.moves[currentMoveId];
				for (int i = 0; i < plates.Length; i++) {
					SimonPlate plate = (SimonPlate)plates[i].GetComponent ("SimonPlate");
					if ( i == currentMove.src_plate_id) {
						plate.currentMove = currentMove;
					} else {
						plate.currentMove = null;
					}
				}

			}
		}

		void OnFingerDown(FingerDownEvent e) {
		
			if(mode==PlayMode.USER_INPUT){

				GameObject selection = e.Selection;

				if (selection == null) return;


				SimonPlate plate = (SimonPlate)selection.GetComponent ("SimonPlate");
				if (plate == null) return;

				plate.Highlight(true);
//				Debug.Log (plate);
			
			}
		
		}

	//	void OnFingerUp(FingerUpEvent e) { /* your code here */ }

		void OnFingerUp(FingerUpEvent e) {
			
			if(mode==PlayMode.USER_INPUT){
				
				GameObject selection = e.Selection;
				
				if (selection == null) return;
				
				
				SimonPlate plate = (SimonPlate)selection.GetComponent ("SimonPlate");
				if (plate == null) return;
				
				plate.unHighlight(true);
				//				Debug.Log (plate);
				
			}
			
		}

		public void OnTap( TapGesture gesture )
		{
			//Debug.Log(gesture);
			if(mode==PlayMode.USER_INPUT || mode==PlayMode.ADD_MOVES){
			// first finger
		//	FingerGestures.Finger finger = gesture.Fingers[0];
			GameObject selection = gesture.Selection;
			if (selection == null) {
				return;
			}
			SimonPlate plate = (SimonPlate)selection.GetComponent ("SimonPlate");
			if (plate == null) return; // trying to drag not a plate

			if(gesture.Fingers[0].Index!=0) return;

				if(mode == PlayMode.ADD_MOVES){
					plate.Highlight();
					Debug.Log ("added tap move");
					MultiplayerGameManager.Instance.AddMove (new Move(MoveDefinition.CLICK,plate.plate_id));
					if(plate.altColour)
						plate.GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().Play();
					else
						plate.GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().PlayJustOne();
					plate.StartCoroutine(plate.DelayedUnHighlight(0.25f));
					currentDraggedPlate = null;
				}
				else{
				bool valid = plate.ValidateMove (gesture);

				Debug.Log("tap " + gesture.Selection);

				

				Move move = sequence.moves[currentMoveId];

				//if(move.move_id==MoveDefinition.CLICK)
				//	move.DisplayMove();

				//plate.unHighlight(false);
				plate.StartCoroutine(plate.DelayedUnHighlight(0.25f));

				if (valid) {
					SinglePlayerProgression.Instance.AchievementProgress("tap",1);
					applyNextMove ();
				} else {
					//mode = PlayMode.FEEDBACK_FAIL;
					changeToFailMode();
				}
				}
			}
		}

		public Transform field;
		public float initialOrientation;
		private bool Rotating = false;
		void OnTwist( TwistGesture gesture )
		{
		if(currentDraggedPlate==null && ((mode==PlayMode.USER_INPUT && (MultiplayerGameManager.isMultiplayer | SinglePlayerProgression.isTwistLeftUnlocked | SinglePlayerProgression.isTwistRightUnlocked)) || mode == PlayMode.ADD_MOVES)){

			if( gesture.Phase == ContinuousGesturePhase.Started && !Rotating && gesture.PreviousState==GestureRecognitionState.Ready)
			{

				Debug.Log("STARTED TWIST");
				Debug.Log("previous state: "+gesture.PreviousState);

				initialOrientation = field.eulerAngles.z;
				Rotating = true;
				for (int i = 0; i < plates.Length; i++) {
					SimonPlate p = (SimonPlate)plates [i].GetComponent ("SimonPlate");
					if(p.previousTransform!=p.transform.position)
						p.takePreviousTransform();
					iTween.Stop(p.gameObject,"MoveTo");
					p.ResetState(true);
					p.takePreviousTransform();
					p.Highlight(false);
				}
			}
			else if( gesture.Phase == ContinuousGesturePhase.Updated )
			{
				if( Rotating )
				{
						currentDraggedPlate=null;
					// apply a rotation around the Z axis by rotationAngleDelta degrees on our target object
						//Debug.Log (gesture.TotalRotation);
						field.transform.eulerAngles = new Vector3 ( 0, 0, initialOrientation + Mathf.Clamp(gesture.TotalRotation,-90f,90f));
						//Debug.Log( Mathf.Clamp(gesture.TotalRotation,-90f,90f));
				}
			}
			else if( gesture.Phase == ContinuousGesturePhase.Ended)
			{
				if( Rotating )
				{
					Debug.Log("ENDED TWIST");

					for (int i = 0; i < plates.Length; i++) {
						SimonPlate p = (SimonPlate)plates [i].GetComponent ("SimonPlate");
						p.takePreviousTransform();
						p.unHighlight(false);
					}

					Vector3 vec = field.eulerAngles;
					vec.x = Mathf.Round(vec.x / 90) * 90;
					vec.x = vec.x % 360;
					vec.y = Mathf.Round(vec.y / 90) * 90;
					vec.y = vec.y % 360;
					vec.z = Mathf.Round(vec.z / 90) * 90;
					vec.y = vec.y % 360;
			//		Debug.Log(vec);
					iTween.RotateTo(field.gameObject,vec,0.5f);

					int turn = (int) (initialOrientation - vec.x - vec.y - vec.z);


					/*
					if(gesture.TotalRotation<15){
						Debug.Log("twist smaller than 15 degrees, giving user some lenience");
						return;
					}
					*/
					
					//field.eulerAngles = vec;

					if(mode==PlayMode.ADD_MOVES){
							Rotating = false;
							if (Mathf.Abs(Mathf.DeltaAngle(0,turn)) < 15) return;
							Debug.Log ("added twist move");
							MultiplayerGameManager.Instance.AddMove(new Move(MoveDefinition.TWIST,-1,-1,Mathf.RoundToInt(Mathf.DeltaAngle(0,turn))));

						}
					else{

				    if (currentMoveId >= 0 && currentMoveId < sequence.moves.Length)
					{
						Move move = sequence.moves[currentMoveId];

						//if (turn % 360 == 0) return;

						//if (Mathf.Abs(turn % 360) < 15) return;
//						if ((Mathf.Abs((turn%360)-360)) < 15) return;
						if (Mathf.Abs(Mathf.DeltaAngle(0,turn)) < 15) return;
					

						if (move.move_id != MoveDefinition.TWIST)
						{
							Debug.Log ("twist when shouldn't have");
							initialOrientation = field.eulerAngles.z;
							//mode = PlayMode.FEEDBACK_FAIL;
							Rotating = false;
							changeToFailMode();
							return;
						}

						Debug.Log("rotated " + turn);
					/*	Debug.Log("mod 360 = " + (turn%360));
						Debug.Log("(angle % 360) +360 = " + ((turn%360)+360));
						Debug.Log("target is " + move.twistAngle);
*/
						//if (turn % 360 == 0) return;

						//if (Mathf.Abs((turn % 360) - move.twistAngle)<=1)
						if (Mathf.Abs((Mathf.DeltaAngle(0,turn) - move.twistAngle))<=2)
						{
							SinglePlayerProgression.Instance.AchievementProgress("rotate",1);
							initialOrientation = field.eulerAngles.z;
							//takeInitialPlates();
							applyNextMove ();
							Rotating = false;
						}
						/*else if (Mathf.Abs(((turn%360)+360)-move.twistAngle)<=1)
						{
							initialOrientation = field.eulerAngles.z;
							//takeInitialPlates();
							applyNextMove ();
						}
						else if (Mathf.Abs(((turn%360)-360)-move.twistAngle)<=1)
						{
							initialOrientation = field.eulerAngles.z;
							//takeInitialPlates();
							applyNextMove ();
						}*/
						else{
						
							Debug.Log ("twist wrong direction");
							initialOrientation = field.eulerAngles.z;
							//mode = PlayMode.FEEDBACK_FAIL;
							//takeInitialPlates();
							changeToFailMode();
							Rotating = false;
							return;

						}
						
						}

					}
				}
			}
			}
		}

		private SimonPlate currentDraggedPlate = null;
		private int dragFingerIndex = -1;
		public void OnDrag( DragGesture gesture )
		{
			if (mode == PlayMode.USER_INPUT || mode==PlayMode.ADD_MOVES)
			{    //&&(SinglePlayerProgression.isDragUnlocked|SinglePlayerProgression.isSwipeUnlocked)){
			// first finger
			FingerGestures.Finger finger = gesture.Fingers[0];
			GameObject selection = gesture.Selection;
			if (selection == null) {
				return;
			}

			if(finger.Index>0)
				return;

			SimonPlate plate = (SimonPlate)selection.GetComponent ("SimonPlate");

			if (plate == null) return; // trying to drag not a plate

			if(Rotating)
				return;

			

			if( gesture.Phase == ContinuousGesturePhase.Started && !Rotating && gesture.PreviousState==GestureRecognitionState.Ready )
			{

				Debug.Log("STARTED DRAG");
				Debug.Log("previous state: "+gesture.PreviousState);

				if (currentDraggedPlate == null) {
					currentDraggedPlate = plate;
						for (int i = 0; i < plates.Length; i++) {
							plates[i].GetComponent<SimonPlate>().takeInitialTransform();
						}
					plate.Highlight ();
					plate.GetComponent<UISprite>().depth=2;
					dragFingerIndex = finger.Index;
					return;
				}
				else{
					plate.ResetState();
					currentDraggedPlate.ResetState();
					currentDraggedPlate=null;
					return;
				}
			}
			else if( finger.Index == 0)//dragFingerIndex)  // gesture in progress, make sure that this event comes from the finger that is dragging our dragObject
			{
				float dragDistance = Vector2.Distance(gesture.StartPosition,gesture.Position);
			//	Debug.Log ("drag distance: "+ dragDistance);

				if (currentDraggedPlate == null || currentDraggedPlate != plate ) {
					//currentDraggedPlate.ResetState();
					plate.ResetState();
					return;
				}
				if( gesture.Phase == ContinuousGesturePhase.Updated && !Rotating)
				{
					// update the position by converting the current screen position of the finger to a world position on the Z = 0 plane
					//Debug.Log(GetWorldPos( gesture.Position ));
					Camera raycastCam = GetComponent<ScreenRaycaster>().Cameras[0];
					plate.transform.position = GetWorldPos( gesture.Position , raycastCam);
					Vector3 p = plate.transform.position;
					p.z = -0.01f;
					plate.transform.position = p;

						for (int i = 0; i < plates.Length; i++) {
							SimonPlate otherPlate = (SimonPlate)plates[i].GetComponent ("SimonPlate");

							float distance = Vector2.Distance((Vector2)p,(Vector2)plates[i].transform.position);
							//Debug.DrawRay(currentMove.target_plate.transform.position,mousePosition,Color.white,30f);
							//Debug.Log(PlateHandler.GetWorldPos( gesture.Position ));
			//				Debug.Log(distance);
							if (distance < 0.1f ) {
								otherPlate.Highlight();
							} else {
								otherPlate.unHighlight();
							}
						}

					//Debug.Log (gesture.Selection);
				}
				else if(dragDistance<25f && gesture.Phase == ContinuousGesturePhase.Ended)
				{

					Debug.Log("ENDED DRAG");

					if(mode == PlayMode.ADD_MOVES){
							MultiplayerGameManager.Instance.AddMove (new Move(MoveDefinition.CLICK,currentDraggedPlate.plate_id));
							if(currentDraggedPlate.altColour)
								currentDraggedPlate.GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().Play();
							else
								currentDraggedPlate.GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().PlayJustOne();
							Debug.Log("tap move caught by drag handler");
							Debug.Log ("Added tap move");
							currentDraggedPlate.ResetState();
							currentDraggedPlate = null;
							//plate.StartCoroutine(plate.DelayedUnHighlight(0.5f));
						}
					else{
					Move move = sequence.moves[currentMoveId];

						if(move.move_id == MoveDefinition.CLICK){
							Debug.Log("tap move caught by drag handler");
							bool valid = currentDraggedPlate.currentMove.move_id==MoveDefinition.CLICK;
								if (valid) {
									SinglePlayerProgression.Instance.AchievementProgress("tap",1);
									currentDraggedPlate.ResetState();
									currentDraggedPlate = null;
									applyNextMove ();
								} else {
									currentDraggedPlate.ResetState();
									Invoke("changeToFailMode",0.5f);
									currentDraggedPlate = null;
								}
							return;
						}

					Debug.Log("drag distance smaller than 25, ignoring");
					currentDraggedPlate.ResetState();
					}
				}
				else if (gesture.Phase == ContinuousGesturePhase.Ended){
					
					Debug.Log("ENDED DRAG");

					// reset our drag finger index
					dragFingerIndex = -1;

					if(mode == PlayMode.ADD_MOVES){

						int target_plate_id = -1;

						for (int i = 0; i < plates.Length; i++) {
							SimonPlate otherPlate = (SimonPlate)plates[i].GetComponent ("SimonPlate");
							
							float distance = Vector2.Distance((Vector2)plate.transform.position,(Vector2)otherPlate.transform.position);
							//Debug.DrawRay(currentMove.target_plate.transform.position,mousePosition,Color.white,30f);
							//Debug.Log(PlateHandler.GetWorldPos( gesture.Position ));
							//				Debug.Log(distance);
							if (distance < 0.2f ) {
								target_plate_id = otherPlate.plate_id;
							} 
							otherPlate.unHighlight(true);
						}

						if(currentDraggedPlate.plate_id!=target_plate_id){
								Debug.Log ("Added drag move");
								MultiplayerGameManager.Instance.AddMove (new Move(MoveDefinition.DRAG,currentDraggedPlate.plate_id,target_plate_id,0));

							}
						else{
							Debug.Log("swipe move caught by drag handler");
							Debug.Log ("magnitude" + gesture.TotalMove.magnitude);
							
							float angle = Vector2.Angle((PlateHandler.GetWorldPos(gesture.Position)-transform.parent.position),PlateHandler.GetWorldPos(gesture.StartPosition));
							//Debug.Log(angle);
							
							if(angle<45f && gesture.TotalMove.magnitude>1f){
								Debug.Log("Added swipe move");
								MultiplayerGameManager.Instance.AddMove (new Move(MoveDefinition.SWIP,currentDraggedPlate.plate_id,-1,0));
								
							}
						}

						if(currentDraggedPlate.altColour)
							currentDraggedPlate.GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().Play();
						else
							currentDraggedPlate.GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().PlayJustOne();
						
						currentDraggedPlate.ResetState();
						
						currentDraggedPlate = null;
					}
					else{

						bool valid = currentDraggedPlate.ValidateMove (gesture);
							for (int i = 0; i < plates.Length; i++) {
								SimonPlate otherPlate = (SimonPlate)plates[i].GetComponent ("SimonPlate");
								otherPlate.unHighlight();
							}

						if (valid) {
							currentDraggedPlate.ResetState();
							currentDraggedPlate = null;
							applyNextMove ();
						} else {
							currentDraggedPlate.ResetState();
							Invoke("changeToFailMode",0.5f);
							currentDraggedPlate = null;
							//mode = PlayMode.FEEDBACK_FAIL;
						}
					}
				}
			}
				}
		}

		/*
		void OnSwipe(SwipeGesture gesture) { 
			//Debug.Log (gesture.StartSelection);
			SimonPlate plate = currentDraggedPlate;
			if(!plate){
				GameObject selection = gesture.StartSelection;
				if(!selection) return;
				plate = selection.GetComponent<SimonPlate>();
				if(!plate) return;
			}

			if(mode==PlayMode.USER_INPUT&&plate!=null){//&&(SinglePlayerProgression.isSwipeUnlocked|SinglePlayerProgression.isDragUnlocked)){
				//Debug.Log("swipe " + gesture.Direction);
				bool valid = plate.ValidateMove (gesture);
				if (valid) {
					plate.ResetState();
					currentDraggedPlate = null;
					applyNextMove ();
				} else {
					plate.ResetState();
					currentDraggedPlate = null;
					changeToFailMode();
				}

			}
		}
*/
		void OnShake(){
			if((mode==PlayMode.USER_INPUT && (MultiplayerGameManager.isMultiplayer | SinglePlayerProgression.isShakeUnlocked)) || mode == PlayMode.ADD_MOVES){

				if(mode == PlayMode.ADD_MOVES){
					Debug.Log("added shake move");
					MultiplayerGameManager.Instance.AddMove(new Move(MoveDefinition.SHAKE,-1));
					iTween.ShakePosition(field.gameObject,iTween.Hash("amount",Vector3.one*0.2f,"time",0.5f,"space",Space.World));
					ChangeColours();

				}
				else{

				if (currentMoveId >= 0 && currentMoveId < sequence.moves.Length)
				{
					Move move = sequence.moves[currentMoveId];
					if (move.move_id != MoveDefinition.SHAKE)
					{
						Debug.Log ("shake when shouldn't have");
						iTween.ShakePosition(field.gameObject,iTween.Hash("amount",Vector3.one*0.2f,"time",0.5f,"space",Space.World,"oncomplete","changeToFailMode","oncompletetarget",this.gameObject));
						//mode = PlayMode.FEEDBACK_FAIL;
						changeToFailMode();
						return;
					}
					else{
						SinglePlayerProgression.Instance.AchievementProgress("shake",1);
						ChangeColours();
						move.DisplayMove();
						applyNextMove();

					}
				}
				}

			}
		}
		
		private void fail(){
			if (GameComplete == true){
				return;
			}
			//display Fail GUI
			//Debug.Log ("fail");
			if(MultiplayerGameManager.isMultiplayer)
			{
				GUITracker[5].SetActive(true);
				GlyphsController.SendMessage("doFlyOutDelay", 2f);
			}
			else
				GUITracker[1].SetActive(true);
			//Scale Glyphs Small Again

			GameComplete = true;
			//ResetPlates();
			//HighLight(0, false);
			
		}

		private void win () {
			if (GameComplete == true){
				return;
			}

			if(MultiplayerGameManager.isMultiplayer)
				GUITracker[4].SetActive(true);
			else
				GUITracker[0].SetActive(true);

			//MultiplayerGameManager.Instance.challengeManager.saveChallenge(sequence.moves,MultiplayerGameManager.Instance.currentOpponent,null);

			//Scale Glyphs Small Again
			//GamePlayGuiController.SendMessage("doShrink");
			GameComplete = true;
			//ResetField(false);
			//ResetPlates();
			//HighLight(0, false);
		}

		public static Vector3 GetWorldPos( Vector2 screenPos){
			//Camera raycastCam = gameObject.GetComponent<ScreenRaycaster>().Cameras[0];

			Camera uiCam = FindObjectOfType<UICamera>().GetComponent<Camera>();

			return GetWorldPos(screenPos,uiCam);
		}

		// Convert from screen-space coordinates to world-space coordinates on the Z = 0 plane
		public static Vector3 GetWorldPos( Vector2 screenPos, Camera theCam )
		{

			Ray ray = theCam.ScreenPointToRay( screenPos );
			
			// we solve for intersection with z = 0 plane
			float t = -ray.origin.z / ray.direction.z;
			Vector3 p = ray.GetPoint( t );
			return p;
		}

		void OnApplicationQuit(){
			if(mode==PlayMode.FEEDBACK_FAIL)
				setLevelToOne();
			//Analytics.Flurry.Instance.EndLogEvent("PlaySession");
		}

		void OnDisable(){
			//Analytics.Flurry.Instance.EndLogEvent("PlaySession");
		}


	}

	[Serializable]
	public class Sequence {
		[SerializeField]
		public Move[]  moves;

		public Sequence(Sequence s, int length, Move[] availableMoves) {
			ArrayList _moves = new ArrayList();
			int previousSeq = s == null?0: s.moves.Length;
			int max = Mathf.Min (length, previousSeq);

			Debug.Log ("adding " + (length-max).ToString() + "moves to sequence of length "+previousSeq);

			for (int i = 0; i < max; i++) {
				_moves.Add(s.moves[i]);
			}
			for (int i = max; i < length; i ++) {
				int r = UnityEngine.Random.Range(0, availableMoves.Length);
				Move m = availableMoves[r];
				_moves.Add(m);
			}
			this.moves = new Move[_moves.Count];
			_moves.CopyTo (this.moves);
		}

		public Sequence(Sequence s, int length, Move[] availableMoves, out Move[] remainingMoves, int maxLength=-1) {

			ArrayList _availableMoves = new ArrayList();
			for (int i = 0; i < availableMoves.Length; i++) {
				Move m = availableMoves[i];
				_availableMoves.Add(m);
			}

			ArrayList _moves = new ArrayList();
			int previousSeq = s == null?0: s.moves.Length;
			int max = Mathf.Min (length, previousSeq);
			
			Debug.Log ("adding " + (length-max).ToString() + "moves to sequence of length "+previousSeq);
			
			for (int i = 0; i < length; i++) {
				if(i<max)
					_moves.Add(s.moves[i]);
				else
				{
				if(availableMoves.Length>0)
					{
					int r = UnityEngine.Random.Range(0, availableMoves.Length-1);
					Move m = availableMoves[r];
					_moves.Add(m);
					_availableMoves.Remove(m);
					}
					availableMoves = new Move[_availableMoves.Count];
					_availableMoves.CopyTo(availableMoves);
				}
			}

			if(maxLength!=-1){
				while(_availableMoves.Count > maxLength-length){
					int r = UnityEngine.Random.Range(0, availableMoves.Length);
					Move m = availableMoves[r];
					_availableMoves.Remove(m);
				}
			}

			remainingMoves = new Move[_availableMoves.Count];
			_availableMoves.CopyTo(remainingMoves);

			
			this.moves = new Move[_moves.Count];
			_moves.CopyTo (this.moves);
		}

		public Sequence(Move[] oldMoves) {
			ArrayList _moves = new ArrayList();
			for (int i = 0; i < oldMoves.Length; i++) {
				_moves.Add(oldMoves[i]);
			}
			this.moves = new Move[_moves.Count];
			_moves.CopyTo (this.moves);

		}

		public Sequence(Move[] availableMoves, int length, out Move[] remainingMoves) {

			ArrayList _availableMoves = new ArrayList();
			for (int i = 0; i < availableMoves.Length; i++) {
				Move m = availableMoves[i];
				_availableMoves.Add(m);
			}

			ArrayList _moves = new ArrayList();
			for (int i = 0; i < length; i++) {
				int r = UnityEngine.Random.Range(0, availableMoves.Length);
				Move m = availableMoves[r];
				_moves.Add(m);
				_availableMoves.Remove(m);
				availableMoves = new Move[_availableMoves.Count];
				_availableMoves.CopyTo(availableMoves);
			}

			this.moves = new Move[_moves.Count];
			_moves.CopyTo (this.moves);

			remainingMoves = new Move[_availableMoves.Count];
			_availableMoves.CopyTo(remainingMoves);
		}


		public string toString(){
			IList tempList = new List<IDictionary<string,int>>();

			for(int i=0;i<moves.Length;i++)
				tempList.Add(moves[i].toDict());

			IDictionary tempDict = new Dictionary<string,List<IDictionary<string,int>>>();

			tempDict.Add ("moves",tempList);

      		return MiniJSON.Json.Serialize(tempDict);
		
		}
	}



	public class MoveDefinition {
		public static Move[] PossibleMoves = null;

		public const int CLICK = 0;
		public const int DRAG = 1;
		public const int TWIST = 2;
		public const int SWIP = 3;
		public const int SHAKE = 4;

		public enum MoveType {CLICK, DRAG, TWIST, SWIP, SHAKE};

		private static int[] SrcOnlyMoves = {CLICK, SWIP,SHAKE};
		private static int[] SrcTargetMoves ={DRAG};// {DRAG};

		public static Move[] GetPossibleMoves(GameObject[] plates, Transform field) {
			int nbOfPlates = plates.Length;

			ArrayList rtrn = new ArrayList();
			if(SinglePlayerProgression.isTwistLeftUnlocked){
				// add twist left
				Move twistMove_l = new Move(TWIST, -1, -1);
				twistMove_l.twistAngle = 90;
				twistMove_l.field = field;
				rtrn.Add (twistMove_l);
			}
			if(SinglePlayerProgression.isTwistRightUnlocked){
				// add twist right
				Move twistMove_r = new Move(TWIST, -1, -1);
				twistMove_r.twistAngle = -90;
				twistMove_r.field = field;
				rtrn.Add (twistMove_r);
			}

			if(SinglePlayerProgression.isShakeUnlocked){
				Move shakeMove = new Move(SHAKE, -1, -1);
				shakeMove.field = field;
				rtrn.Add(shakeMove);
				
			}

			// add src only moves

			for (int j = 0 ; j < nbOfPlates ; j++) {
				if(!SinglePlayerProgression.isSwipeUnlocked){
					Move m = new Move(CLICK, j, -1);
					m.src_plate = (SimonPlate)plates[j].GetComponent ("SimonPlate");
					rtrn.Add(m);
				}
				else {
					
					Move swipeMove = new Move(SWIP, j, -1);
					swipeMove.src_plate = (SimonPlate)plates[j].GetComponent ("SimonPlate");
					rtrn.Add(swipeMove);
						
				}

			}

			/*for (int i = 0; i < SrcOnlyMoves.Length; i++) {
				for (int j = 0 ; j < nbOfPlates ; j++) {
					Move m = new Move(SrcOnlyMoves[i], j, -1);
					m.src_plate = (SimonPlate)plates[j].GetComponent ("SimonPlate");
					if(SrcOnlyMoves[i]==SHAKE)
						m.field = field;
					rtrn.Add(m);
				}
			}*/

			// add src + target moves
			//for (int i = 0; i < SrcTargetMoves.Length; i++) {

			if(SinglePlayerProgression.isDragUnlocked || MultiplayerGameManager.isMultiplayer){

				for (int j = 0 ; j < nbOfPlates ; j++) {
					for (int jj  = 0; jj < nbOfPlates ; jj++) {
						if (j != jj) {
							Move dragMove = new Move(DRAG, j, jj);
							dragMove.src_plate = (SimonPlate)plates[j].GetComponent ("SimonPlate");
							dragMove.target_plate = (SimonPlate)plates[jj].GetComponent ("SimonPlate");
							rtrn.Add(dragMove);
						}
					}
				}

			}
			PossibleMoves = new Move [rtrn.Count];
			rtrn.CopyTo(PossibleMoves);
			return PossibleMoves;
		}

		public static Move[] GetPossibleMoves(GameObject[] plates, Transform field,
		                                      SinglePlayerProgression.levelDefinition levelDefinition){
			return GetPossibleMoves(plates,field,
			                 levelDefinition.Min_Tap,
			                 levelDefinition.Min_Drag,
			                 levelDefinition.Min_Flick,
			                 levelDefinition.Min_Rotation,
			                 levelDefinition.Min_ColourSwap,
			                 levelDefinition.Min_Random,
			                 levelDefinition.Rand_Tap,
			                 levelDefinition.Rand_Drag,
			                 levelDefinition.Rand_Flick,
			                 levelDefinition.Rand_Rotation,
			                 levelDefinition.Rand_ColourSwap,
			                 levelDefinition.nbOfPlates);
		}

		public static Move[] GetPossibleMoves(GameObject[] plates, Transform field,
		                                      int Min_Tap,
		                                      int Min_Drag,
		                                      int Min_Flick,
		                                      int Min_Rotation,
		                                      int Min_ColourSwap,
		                                      int Min_Random,
		                                      bool Rand_Tap,
		                                      bool Rand_Drag,
		                                      bool Rand_Flick,
		                                      bool Rand_Rotation,
		                                      bool Rand_ColourSwap,
		                                      int nbOfPlates=5) {

			//int nbOfPlates = plates.Length;
			
			ArrayList rtrn = new ArrayList();

			ArrayList randMoves = new ArrayList();

			ArrayList tapMoves = new ArrayList();
			ArrayList dragMoves = new ArrayList();
			ArrayList flickMoves = new ArrayList();
			ArrayList twistMoves = new ArrayList();
			ArrayList shakeMoves = new ArrayList();

			// add twist left
			Move twistMove_l = new Move(TWIST, -1, -1);
			twistMove_l.twistAngle = 90;
			twistMove_l.field = field;
			twistMoves.Add (twistMove_l);
		
			// add twist right
			Move twistMove_r = new Move(TWIST, -1, -1);
			twistMove_r.twistAngle = -90;
			twistMove_r.field = field;
			twistMoves.Add (twistMove_r);
			if(Rand_Rotation)
				randMoves.Add (twistMove_r);
			
			Move shakeMove = new Move(SHAKE, -1, -1);
			shakeMove.field = field;
			shakeMoves.Add(shakeMove);
			if(Rand_ColourSwap)
				randMoves.Add (shakeMove);
			
			// add src only moves
			
			for (int j = 0 ; j < nbOfPlates ; j++) {
				Move m = new Move(CLICK, j, -1);
				m.src_plate = (SimonPlate)plates[j].GetComponent ("SimonPlate");
				tapMoves.Add(m);
				if(Rand_Tap)
					randMoves.Add (m);
				
				Move swipeMove = new Move(SWIP, j, -1);
				swipeMove.src_plate = (SimonPlate)plates[j].GetComponent ("SimonPlate");
				flickMoves.Add(swipeMove);
				if(Rand_Flick)
					randMoves.Add (swipeMove);

			}

			for (int j = 0 ; j < nbOfPlates ; j++) {
				for (int jj  = 0; jj < nbOfPlates ; jj++) {
					if (j != jj) {
						Move dragMove = new Move(DRAG, j, jj);
						dragMove.src_plate = (SimonPlate)plates[j].GetComponent ("SimonPlate");
						dragMove.target_plate = (SimonPlate)plates[jj].GetComponent ("SimonPlate");
						dragMoves.Add(dragMove);
						if(Rand_Drag)
							randMoves.Add (dragMove);

					}
				}
			}

			for(int k=0; k<Min_Tap; k++)
				rtrn.Add (tapMoves[UnityEngine.Random.Range(0,tapMoves.Count)]);
			for(int k=0; k<Min_Drag; k++)
				rtrn.Add (dragMoves[UnityEngine.Random.Range(0,dragMoves.Count)]);
			for(int k=0; k<Min_Flick; k++)
				rtrn.Add (flickMoves[UnityEngine.Random.Range(0,flickMoves.Count)]);
			for(int k=0; k<Min_Rotation; k++)
				rtrn.Add (twistMoves[UnityEngine.Random.Range(0,twistMoves.Count)]);
			for(int k=0; k<Min_ColourSwap; k++)
				rtrn.Add (shakeMoves[UnityEngine.Random.Range(0,shakeMoves.Count)]);

			for(int k=0; k<Min_Random; k++)
				rtrn.Add (randMoves[UnityEngine.Random.Range(0,randMoves.Count)]);


			PossibleMoves = new Move [rtrn.Count];
				rtrn.CopyTo(PossibleMoves);
			return PossibleMoves;
		}
	}


}