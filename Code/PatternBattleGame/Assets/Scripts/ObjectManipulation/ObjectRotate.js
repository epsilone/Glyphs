var RotationObject : GameObject;

//starting rotation
var RotationX = 0.000;
var RotationY = 0.000;
var RotationZ = 0.000;

var SpeedX = 0.000;
var SpeedY = 0.000;
var SpeedZ = 0.000;

@HideInInspector
var last_turn:boolean = false;
//@HideInInspector
var rotation_disable:boolean = true;

var plateHandler : GameObject;

function Awake(){
	plateHandler = GameObject.Find("Playfield");
}

function start_rotating() {
	last_turn = false;
	rotation_disable = false;	
}

function FixedUpdate () {
	if (rotation_disable)
		return;

	var turn_completed = false;
	if (RotationX > 360) {
		RotationX = 0;
		turn_completed = true;
	}
	if (RotationX < 0) {
		RotationX = 360;
		turn_completed = true;
	}
		

	if (RotationY > 360) {
		RotationY = 0;
		turn_completed = true;
	}
	if (RotationY < 0) {
		turn_completed = true;
		RotationY = 360;
	}

	if (RotationZ > 360) {
		RotationZ = 0;
		turn_completed = true;
	}
	if (RotationZ < 0) {
		RotationZ = 360;
		turn_completed = true;
	}

	if (last_turn && turn_completed) {
		rotation_disable = true;
	//	Debug.Log("done rotating");
		if(plateHandler!=null&&this.GetComponent(GamePlayGUIControl)!=null)
			plateHandler.SendMessage("doneRotating");
	} else {

		RotationX = RotationX+SpeedX;
		//RotationObject.transform.localEulerAngles.x = RotationX;
		RotationY = RotationY+SpeedY;
		//RotationObject.transform.localEulerAngles.y = RotationY;
		RotationZ = RotationZ+SpeedZ;
		//RotationObject.transform.localEulerAngles.z = RotationZ;
	}
	RotationObject.transform.localEulerAngles = Vector3 (RotationX,RotationY,RotationZ);
}
