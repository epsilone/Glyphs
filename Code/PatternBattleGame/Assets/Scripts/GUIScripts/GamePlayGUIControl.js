#pragma strict

//Glyph Flyin Tracker
var GlyphIn : GameObject;
var MainGameGUI :GameObject;
var Symbols : GameObject;
var GlyphZoomSpeed = 1.0000;
@HideInInspector
var ScaleSmall = 1.00;
var ScaleLarge = 1.50;
@HideInInspector
var Zoom = false;
@HideInInspector
var Shrink = false;

@HideInInspector
var state = "";

var zoomSound : AudioClip;
var shrinkSound : AudioClip;

private var myAudioSource : AudioSource;


function Start () {
//	TurnGlyphsAnimationControllerOn(0f);
//	ZoomSmall();
//	Shrink = true;
	myAudioSource = gameObject.AddComponent.<AudioSource>();
	myAudioSource.volume = PlayerPrefs.GetInt("sound",1);
}


function FixedUpdate () {

ZoomLarge();
ZoomSmall();

}


function ZoomLarge(){

	if (!Zoom) {
		// so that we don't evaluate them all every frame.
		return;
	}

	if(Symbols.transform.localScale.x < ScaleLarge){
		Symbols.transform.localScale.x = Symbols.transform.localScale.x+(GlyphZoomSpeed*Time.deltaTime);
		Symbols.transform.localScale.y = Symbols.transform.localScale.y+(GlyphZoomSpeed*Time.deltaTime);
		
		if(Symbols.transform.localScale.x >= ScaleLarge){
			Zoom = false;
			doneZooming();
		}
	}
}


function ZoomSmall(){

	if (!Shrink) {
		// so that we don't evaluate them all every frame.
		return;
	}

	if(Symbols.transform.localScale.x > ScaleSmall){
		Symbols.transform.localScale.x = Symbols.transform.localScale.x-(GlyphZoomSpeed*Time.deltaTime);
		Symbols.transform.localScale.y = Symbols.transform.localScale.y-(GlyphZoomSpeed*Time.deltaTime);
		
		if(Symbols.transform.localScale.x <= ScaleSmall){
			Shrink = false;
			doneShrinking();
		}
	}
}

function doShrink(delay:float){
	Invoke("_doShrink",delay);
	//Debug.Log("doshrink with delay");
}

function doZoom(delay:float){
	Invoke("_doZoom",delay);
}

function _doZoom(){
	Zoom=true;
	Shrink=false;
	myAudioSource.PlayOneShot(zoomSound);
}

function _doShrink(){
	Shrink=true;
	Zoom=false;
	myAudioSource.PlayOneShot(shrinkSound);
	//Debug.Log("doshrink");
}


function doneZooming(){
	Debug.Log("done zooming");
}

function doneShrinking(){
	Debug.Log("done shrinking");
	gameObject.SendMessage("EndOfFlyIn");
}



function TurnGlyphsAnimationControllerOff(){
	if(this.GetComponent(ObjectRotate)!=0)
		this.GetComponent(ObjectRotate).last_turn = true;
	var rotateScripts = Symbols.GetComponentsInChildren(ObjectRotate);
	for (var r = 0 ; r < rotateScripts.Length ; r++) {
		var o :ObjectRotate = rotateScripts[r];
		if(o.RotationZ!=0)
			o.last_turn = true;
	}
}


function ForceStopRotating(){
	this.GetComponent(ObjectRotate).rotation_disable = true;
	var rotateScripts = Symbols.GetComponentsInChildren(ObjectRotate);
	for (var r = 0 ; r < rotateScripts.Length ; r++) {
		var o :ObjectRotate = rotateScripts[r];
		o.rotation_disable = true;
	}
}

function _TurnGlyphsAnimationControllerOn(){
	GlyphIn.GetComponent(ObjectRotate).start_rotating();
	var rotateScripts = Symbols.GetComponentsInChildren(ObjectRotate);
	for (var r = 0 ; r < rotateScripts.Length ; r++) {
		var o :ObjectRotate = rotateScripts[r];
		o.start_rotating();
	}
}

function TurnGlyphsAnimationControllerOn(delay:float){
	Invoke("_TurnGlyphsAnimationControllerOn",delay);
}
