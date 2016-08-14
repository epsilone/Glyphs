#pragma strict

var multiplayerGM : GameObject;
var singlePlayerP : GameObject;
var isMultiplayer : boolean;
var level : int = -1;
var tutorialObj : GameObject;
var inTutorial : boolean = false;

var GlyphObjects : GameObject[];
var GlyphPositions : GameObject[];

var GlyphMoveFlag : boolean[];

var GlyphMoveFlagStarted : boolean[];
var FlyINComplete : boolean[];

var FlyOUTComplete : boolean[];
var CircleRadius = 1.0000;
var StartScaleSize = 1.000;
@HideInInspector
var EndScaleSize = 1.000;

var ScaleRatiotoDistance = 0.1;
var MoveXSpeed = 10.000;


var FlyInFinished:boolean = true;

var FlyOutFinished:boolean = true;
var CommenceTransButtons : GameObject[];

var numGlyphs : int = 5;

var plateHandler : GameObject;

var offsetAngle : int;

var singlePlayerCountdown: GameObject;
var multiplayerCountdown: GameObject;

var tutorialFailed: boolean = false;


function Start () {
	//Set the FinalScale Based on the Size Displayed in the final circle
	EndScaleSize = GlyphObjects[0].transform.localScale.x;
	
	//We want to stop scaling when we get to CircleRadius and we want the scale to be 1
	//We want the Scale to be uniform and to be StartScale size at the Begining 
	var StartDistance = Vector3.Distance(GlyphObjects[0].transform.position,
			 					gameObject.GetComponent(ObjectRotate).RotationObject.transform.position);
			 					
	var DistanceTravelled = StartDistance-CircleRadius;

	//Scale speed
	ScaleRatiotoDistance = EndScaleSize/CircleRadius;

	//Initialise Start Position
	for (var i=0;i < GlyphObjects.Length;i++){
		GlyphObjects[i].transform.position = GlyphPositions[i].transform.position;
	}
	
//	FlyInFinished=true;
//	FlyOutFinished=true;
	
	//doFlyIn();
	
	//Initialise Start Scale
	
	multiplayerGM = GameObject.Find("MultiplayerChallengeManager");
	singlePlayerP = GameObject.Find("SinglePlayerProgression");
}

function setNumGlyphs(num){
	numGlyphs = num;
}

function FixedUpdate () {
	//Fly In
	FlyIN();
	FlyOUT();
}

function FlyIN() 
{
    if (FlyInFinished) 
    {
		// so that we don't evaluate them all every frame.
		return;
	}
	//Fly in
    for (var i=0;i < numGlyphs; i++) 
    {
		//Initiate startPosition
        if(GlyphMoveFlag[i] == false && GlyphMoveFlagStarted[i] == false)
        {
			GlyphObjects[i].transform.position = GlyphPositions[i].transform.position;
		}

        var spacingAngle : float = 360.00/numGlyphs;

        // 5 glyphs 90 degrees
        // 4        75?
        // 3 glyphs 45 degrees

        //Debug.Log("starting flyin with " + numGlyphs + " glyphs");

        if(numGlyphs == 4)
	        offsetAngle = 75;
        else if(numGlyphs == 3)
	        offsetAngle = 45;
        else if (numGlyphs == 5)
	        offsetAngle = 90;

		//Tell them to start moving equally spaced
		if(Mathf.Abs( gameObject.GetComponent(ObjectRotate).RotationZ - ((i*spacingAngle)+offsetAngle) % 360)  < 2f)
		{
			GlyphMoveFlagStarted[i]= true;
			GlyphMoveFlag[i] = true;
		}
	
		//Move all objects in who didn't reach their end position yet and scale them
		if(GlyphMoveFlag[i] == true && FlyINComplete[i] == false) {
			
			GlyphObjects[i].transform.localPosition = 
				Vector3.MoveTowards(GlyphObjects[i].transform.localPosition,
								gameObject.GetComponent(ObjectRotate).RotationObject.transform.position,MoveXSpeed*Time.deltaTime);
			//Scale Them according to distance
			var CurrentDistance = Vector3.Distance(GlyphObjects[i].transform.position,
			 										gameObject.GetComponent(ObjectRotate).RotationObject.transform.position);
			 										
			GlyphObjects[i].transform.localScale.x = ScaleRatiotoDistance*CurrentDistance;
			GlyphObjects[i].transform.localScale.y = ScaleRatiotoDistance*CurrentDistance;
		
		}
		
		if(Vector3.Distance(GlyphObjects[i].transform.position,
		 					gameObject.GetComponent(ObjectRotate).RotationObject.transform.position) < CircleRadius) {
			//TheFlying for the object is complete
			FlyINComplete[i] = true;
			//Debug.Log(Vector3.Distance(GlyphObjects[i].transform.position,
		 	//				gameObject.GetComponent(ObjectRotate).RotationObject.transform.position));
		 }
	}

	var numComplete : int = 0;
	for(var j=0; j< FlyINComplete.Length; j++)
		if(FlyINComplete[j]) numComplete++;
		
//	Debug.Log(numComplete);

	if (numComplete == numGlyphs) 
	{
		FlyInFinished = true;
		// Check if the last Fly in is complete:
		for (var f = 0; f < numGlyphs ; f++) {
			FlyInFinished = FlyInFinished && FlyINComplete[f];
		}

		if (FlyInFinished) {
			EndOfFlyIn();
		}
	}
}

function FlyOUT() 
{
    if (FlyOutFinished) 
    {
		// so that we don't evaluate them all every frame.
		return;
	}

    //Fly out
    for (var i=0;i < numGlyphs; i++) 
    {
		var spacingAngle : float = 360.00/numGlyphs;

		if(Mathf.Abs( gameObject.GetComponent(ObjectRotate).RotationZ - (((i*spacingAngle)+offsetAngle/* + 180*/) % 360)) < 2f)
		{
			GlyphMoveFlagStarted[i]= true;
			GlyphMoveFlag[i] = true;
		}
	
		//Move all objects in who didn't reach their end position yet and scale them
		if(GlyphMoveFlag[i] == true && FlyOUTComplete[i] == false) 
		{
			
			GlyphObjects[i].transform.position = 
				Vector3.MoveTowards(GlyphObjects[i].transform.position,gameObject.GetComponent(ObjectRotate).RotationObject.transform.position + GlyphObjects[i].transform.position*4f,Time.deltaTime);
			//Scale Them according to distance
			var CurrentDistance = Vector3.Distance(GlyphObjects[i].transform.position,
			 										gameObject.GetComponent(ObjectRotate).RotationObject.transform.position);
			 										
			GlyphObjects[i].transform.localScale.x = ScaleRatiotoDistance*CurrentDistance;
			GlyphObjects[i].transform.localScale.y = ScaleRatiotoDistance*CurrentDistance;
		
		var targetDistance = Vector3.Distance(GlyphPositions[i].transform.position, gameObject.GetComponent(ObjectRotate).RotationObject.transform.position);
			if(CurrentDistance > targetDistance) {
				FlyOUTComplete[i] = true;
			}
		}
		
		
		var numComplete : int = 0;
		for(var j=0; j< FlyOUTComplete.Length; j++)
			if(FlyOUTComplete[j]) numComplete++;
		
		if (numComplete == numGlyphs) {
			FlyOutFinished = true;
			// Check if the last Fly in is complete:
			for (var f = 0; f < numGlyphs ; f++) {
				FlyOutFinished = FlyOutFinished && FlyOUTComplete[f];
			}
			
		}
		
		if(FlyOutFinished){
//			for (var h=0;h < numGlyphs;h++){
//				GlyphObjects[h].transform.position = GlyphPositions[h].transform.position;
//			}
		//	doFlyIn();
			Debug.Log("flyout complete");
//			SendMessage("TurnGlyphsAnimationControllerOff",0f);
//			SendMessage("ForceStopRotating");
			plateHandler.SendMessage("fadeOutCompletely");
			ShowButtons();
		}
	}
	
}

function EndOfFlyIn() {
	Debug.Log("The fly in is complete ");
	
	//TODO Jellyfish
	multiplayerGM.SendMessage("ReturnStatus");
	singlePlayerP.SendMessage("ReturnStatus");
	print("sent messages");
	yield WaitForSeconds(0.1);
	if(isMultiplayer == false && level == 0)
	{
	    if (tutorialFailed) 
	    {
	        for(var i=0;i<CommenceTransButtons.Length;i++)
	        {
	            if(CommenceTransButtons[i]){
	                CommenceTransButtons[i].SetActive(true);
	            }
	        }
	        tutorialFailed = false;
	        return;
	    }

		//trigger tutorial part 1 here
		tutorialObj.SendMessage("StartTutorial", 0);
		inTutorial = true;
						
		while(inTutorial == true)
		{
			//waiting for the tutorial to end because i have to send messages
			yield WaitForSeconds(0.1);
		}

        //Wait for fly out to complete before countdown
        yield WaitForSeconds(0.75f);
						
		print ("tutorial 1 complete");
	}
	
	//var guicontrol = this.GetComponent(GamePlayGUIControl);
	//guicontrol.TurnGlyphsAnimationControllerOff();
	
    //Turn on the commence tranmission button
	for(var j=0;j<CommenceTransButtons.Length;j++)
	{
	    if(CommenceTransButtons[j]){
	        CommenceTransButtons[j].SetActive(true);
	    }
	}

	if (isMultiplayer && multiplayerCountdown != null) 
	{
	    multiplayerCountdown.SendMessage("StartCountdown", 0);
	} 
	else if(singlePlayerCountdown != null)
	{
	    singlePlayerCountdown.SendMessage("StartCountdown", 0);
	}
	
	//doFlyOut();
}

function TutorialFail() 
{
    tutorialFailed = true;
}

function TutorialComplete() {
	inTutorial = false;
}
function UpdateLevel(lev : int){
	level = lev;
}
function UpdateMulti(mult : boolean){
	isMultiplayer = mult;
}
function HideButtons(){
	for(var i=0;i<CommenceTransButtons.Length;i++){
		if(CommenceTransButtons[i]){
			CommenceTransButtons[i].SetActive(false);
		}
	}
}

function ShowButtons(){
	for(var i=0;i<CommenceTransButtons.Length;i++){
		if(CommenceTransButtons[i]){
			CommenceTransButtons[i].SetActive(true);
		}
	}
}

@ContextMenu("flyin")
function doFlyIn(){
	for (var i=0;i < numGlyphs; i++) {
		GlyphMoveFlagStarted[i]= false;
		GlyphMoveFlag[i] = false;
		FlyINComplete[i]=false;
	}
	FlyInFinished=false;
	FlyOutFinished=true;
	plateHandler.SendMessage("fadeOut");
}

@ContextMenu("flyout")
function doFlyOut(){
	FlyInFinished=true;
	for (var i=0;i < GlyphObjects.Length; i++) {
		GlyphMoveFlagStarted[i]= false;
		GlyphMoveFlag[i] = false;
		FlyOUTComplete[i]=false;
		FlyINComplete[i]=true;
	}
	FlyOutFinished=false;
	FlyInFinished=true;
}

function doFlyOutDelay(delay){
	HideButtons();
	Invoke("doFlyOut", delay);
}