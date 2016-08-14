#pragma strict

var GamePlayObject : GameObject;
var FingerGestures : GameObject;
//var MainCamera : GameObject;

function Start () {

	//Add the controls to the scene
	var FingerGesturesClone = 
	Instantiate(FingerGestures,FingerGestures.transform.position,FingerGestures.transform.rotation);

	//Add the Engine Code to the scene
	var	GameplayObjectClone = 
	Instantiate(GamePlayObject,GamePlayObject.transform.position, GamePlayObject.transform.rotation);
}

function Update () {

}