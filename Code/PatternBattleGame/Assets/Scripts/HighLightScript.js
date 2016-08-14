#pragma strict

// this is the plate the gameplay engine will give to you.
var plateid = -1;
var GamePlayControl : GameObject;
var PlateLightStates : GameObject[];

function Start () {

}

function Update () {

}

function highlight(plate_id, light) {


	// Now you know which plate id you are.
	plateid = plate_id;
	
	// Set the highlight on the plate
	if (light) {
		// turn highlight on
		PlateLightStates[plateid].SetActive(true);
	//	Debug.Log("turning on highlighting");
		
	} else {
	
		for (var i = 0; i < PlateLightStates.Length; i++){
		PlateLightStates[i].SetActive(false);
		}
	
		// turn highlight off
	//	Debug.Log("turning off highlighting");
		
	}
}

// call this function when the button is clicked.
function onclick() {
	var gameplay = GamePlayControl.GetComponent(gameplay);
	// give the plate id to the gameplay so it can validate.
	gameplay.OnTap(plateid);
}
