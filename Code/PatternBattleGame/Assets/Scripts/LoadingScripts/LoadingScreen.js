#pragma strict
var mincutscenelength = 60;
var waiting = true;
var counter = 0.00000;

function Start () {
	counter = 0.0000;
}

function Update () {
	if (waiting) {
		counter = counter + Time.deltaTime;

		if(counter >= mincutscenelength) {
			waiting = false;
			LoadScene();
		}
	}
}

function LoadScene(){
	if(ScenetoLoad.ScenetoLoad != "null" && ScenetoLoad.ScenetoLoad != null) {
		Application.LoadLevelAsync(ScenetoLoad.ScenetoLoad);
//		Application.LoadLevelAdditive(ScenetoLoad.ScenetoLoad);
	}
}