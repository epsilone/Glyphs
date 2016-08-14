#pragma strict

enum StateEnum {
    WAIT,
    INIT,
    DISPLAY,
    USER_INPUT,
    FEEDBACK_FAIL,
    FEEDBACK_WIN
};

var GUITracker:GameObject[];

var GamePlayGuiController : GameObject;

var Plates : GameObject[];

var SequenceLength:int = 7;
var sequence:int[];

var mode:int = StateEnum.INIT;
var currentMoveId:int = -1;

var Counter:float = 0f;

var feedback:GameObject;

var GameComplete = false;


function HighLight(plate_id, highlight) {
	var plate = Plates[plate_id];
	var h:HighLightScript = plate.GetComponent(HighLightScript);
	h.highlight(plate_id, highlight);
}

function ResetPlates() {
	// turn off all the plate so we can light only one.
	for (var i = 0 ; i < Plates.Length; i++) {
		HighLight(i, false);
	}
	//Turn off the actual plate state trackers
	HighLight(0, false);
}

function Start () {
	//var generator = GetComponent(SequenceGenerator);
	//sequence = generator.generateSequence(SequenceLength, Plates.Length);
	//Debug.Log(sequence);
}

function WaitForInit() {
	// Could use a rule to make this an event as it is inefficiently being called every frame whilst in Mode 1
	ResetPlates();
	
	//Scale Glyphs Small Again
	GamePlayGuiController.GetComponent(GamePlayGUIControl).Shrink = true;
	
	mode = 1;
	currentMoveId = -1;
	GameComplete = false;

	// do something cool for init.
	
}

function displayMode() {
// TODO: have a timer between moves, in case the same Glyph is pressed twice in a row

	if (Counter > 1) {
	
	if(Counter>1.5f){
		Counter = 0;

		if (currentMoveId >= 0) {
			ResetPlates();
		}

		currentMoveId += 1;

		if (currentMoveId < sequence.Length) {
				var NextPlate = sequence[currentMoveId];
				HighLight(NextPlate, true);
		} else {
				mode = StateEnum.USER_INPUT;
				ResetPlates();
				currentMoveId = 0;
		}
	
		}
		else{
			ResetPlates();
		}
	}
}

function win(){
	if (GameComplete == true){
		return;
	}
	//display Win GUI
	GUITracker[0].SetActive(true);
	//Scale Glyphs Small Again
	GamePlayGuiController.GetComponent(GamePlayGUIControl).Shrink = true;
	GameComplete = true;
	HighLight(0, false);

}

function fail(){
	if (GameComplete == true){
		return;
	}
	//display Fail GUI
	GUITracker[1].SetActive(true);
	//Scale Glyphs Small Again
	GamePlayGuiController.GetComponent(GamePlayGUIControl).Shrink = true;
	GameComplete = true;
	HighLight(0, false);
	
}

function Update () {
	Counter += Time.deltaTime;
	switch (mode) {
		case StateEnum.INIT:
	//		WaitForInit();
			break;
		case StateEnum.DISPLAY:
	//		displayMode();
			break;
		case StateEnum.USER_INPUT:
			break;
		case StateEnum.FEEDBACK_WIN:
			win();
			break;
		case StateEnum.FEEDBACK_FAIL:
			fail();
			break;
		case StateEnum.WAIT:
			break;
	}
}


// this plate id had been tapped.
function OnTap( plateid )
{
	// ignore input when the mode is not correct.
	if (mode == StateEnum.USER_INPUT) {
		ResetPlates();
		HighLight(plateid, true);

		if (currentMoveId >= 0 && currentMoveId < sequence.Length) {
			if (sequence[currentMoveId] == plateid) {
			
				Debug.Log("good job, correct move.");
				currentMoveId ++;
				if (currentMoveId >= sequence.Length) {
					mode = StateEnum.FEEDBACK_WIN;
				}
			} else {
				mode = StateEnum.FEEDBACK_FAIL;
			}
		} else {
			Debug.LogError("you shouldn't be there.");
		}
	} else {
		Debug.Log("Not in user input mode.");
	}
}

function StartDisplaySequence() {
		//Make Sure the Glyphs Scale Big
		GamePlayGuiController.GetComponent(GamePlayGUIControl).Zoom = true;
		
	if ( mode != StateEnum.DISPLAY) {
		Debug.Log("starting the display mode");
		Counter = 0;
		mode = StateEnum.DISPLAY;
	} else {
		Debug.Log("mode is already display");
	}
}
