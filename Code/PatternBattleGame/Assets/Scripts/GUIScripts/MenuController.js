#pragma strict

var LevelDataObject : GameObject;

//Keep track of the last level played, for the purpose of displaying the most relevant level information
var MinLevel =1;
var MaxLevel =100;

var LastLevelPlayed = 1;
var ComponentSpacing = 0.2;
var ScrollSpeed = 0.001;
var ScrollSensitivity = 1.00;
var ScrollDeceleration =0.00000000001;

var MenuComponentPrefab : GameObject;
var MenuComponents : GameObject[];
var CreatingComponent0 = false;
var CreatingComponentLength = false;

var CurrentTouch :Touch;
var LastTouch :Touch;
var FingerLifted = true;
var ScrollDistance = 0.0000;
var LastScrollDistance = 0.000000;
var ScrollDirection = "";

function Start () {
//Create the initial components
	CreateInitialMenuComponents();

}

function Update () {


//DestroyMenuComponents if they go too far off the screen
DestroyMenuComponent();


//Lock the Level from scrolling too far

	//if Element zero becomes == MinLevel stop scrolling if the player tries to scroll it higher up the screen than 0.182
	
	if (MenuComponents[0].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos > 0.182 && 
		MenuComponents[0].transform.Find("SignalNumber").GetComponent(SignalNumber).LevelNumber == MinLevel){
	
	}
	
	//if Element MenuComponents.Length-1 is == MaxLevel	stop scrolling if it tries to go higher than 0.818
	else if (MenuComponents[MenuComponents.Length-1].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos < 0.818 &&
			 MenuComponents[0].transform.Find("SignalNumber").GetComponent(SignalNumber).LevelNumber == MaxLevel){
	
	}
	
	//Move the MenuComponents
	else {
	MenuScrolling();
	}



}

//Set up the menu at the start
function CreateInitialMenuComponents(){

		for (var i = 0;i<MenuComponents.Length;i++){
			//Create 5 Menu Components and add them to the MenuComponents array
			var MenuComponentClone = 
			Instantiate(MenuComponentPrefab,gameObject.transform.position,gameObject.transform.rotation);
	
			MenuComponents[i] = MenuComponentClone;
			
			//Set up the Level Numbers for the start of the menu
			MenuComponents[i].transform.Find("SignalNumber").GetComponent(SignalNumber).LevelNumber = LastLevelPlayed + i;
			
			//First Position set in Prefab
			//Set the next Starting Positions based on a uniform Spacing Variable Y coordinate value (Screen Height Percentage)
			MenuComponents[i].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy =
			MenuComponents[0].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy + (ComponentSpacing*i);
			
			MenuComponents[i].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos =
			MenuComponents[0].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos + (ComponentSpacing*i);
			
			MenuComponents[i].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[0].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos + (ComponentSpacing*i);
		
			//Get the Level Number
			MenuComponents[i].transform.Find("SignalNumberTitle").GetComponent(ScalableText).str =
			MenuComponents[i].transform.Find("SignalNumberTitle").GetComponent(ScalableText).str+ ": " +
			MenuComponents[i].transform.Find("SignalNumber").GetComponent(SignalNumber).LevelNumber.ToString();
						
			MenuComponents[i].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[0].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos + (ComponentSpacing*i);
			
			MenuComponents[i].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[0].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos + (ComponentSpacing*i);
		
			MenuComponents[i].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[0].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos + (ComponentSpacing*i);
		
		
		}	




}

function MenuScrollingAftertouchHelper(){

	//KeepScrolling until the scroll decelerates to equal or less than zero 
	if(Input.touchCount==0){
			
		//Decelerate to a halt
		ScrollDistance = ScrollDistance/ScrollDeceleration;
	

	}

}


//Scroll the MenuComponents
function MenuScrolling(){

//Gives a softer feeling by keeping some movement after the player stops dragging
	MenuScrollingAftertouchHelper();

// If there is a touch,get the first touch
	if(Input.touchCount>0){
	CurrentTouch = Input.GetTouch(0);


// If there was a previous touch
	if(LastTouch.position.y != null){
	//Find out how much y movement
	ScrollDistance = CurrentTouch.position.y - LastTouch.position.y;

		//Factor in the time between frames to the magnitude of the movement over time
		ScrollDistance = ScrollDistance*Time.deltaTime*ScrollSensitivity;
		
		if (ScrollDistance >0){
		ScrollDirection = "Up";
		}

		if (ScrollDistance <0){
		ScrollDirection = "Down";
		}

			
		if (FingerLifted == true){
		ScrollDistance = 0.00;
		FingerLifted = false;
		}

	
	}
	
	}
	
	
	//Adjust the direction of the scroll up or down
	var ScrollSpeedDirected = 0.000;
	
		if(Input.GetKey("down")){
		ScrollSpeedDirected = ScrollSpeed;
		}
		if(Input.GetKey("up")){
		ScrollSpeedDirected = ScrollSpeed*-1;
		}
	
	
		for (var i = 0;i<MenuComponents.Length;i++){
		
			if (MenuComponents[i]){
			// Keyboard Control Position Update (for test purposes)
		
			MenuComponents[i].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy =
			MenuComponents[i].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy + ScrollSpeedDirected;
			
			MenuComponents[i].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos =
			MenuComponents[i].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos + ScrollSpeedDirected;
			
			MenuComponents[i].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[i].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos + ScrollSpeedDirected;
			
			MenuComponents[i].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[i].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos + ScrollSpeedDirected;
			
			MenuComponents[i].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[i].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos + ScrollSpeedDirected;
		
			MenuComponents[i].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[i].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos + ScrollSpeedDirected;
	
	
	
		// Touch Control Position Update
	
	
			MenuComponents[i].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy =
			MenuComponents[i].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy - ScrollDistance;
			
			MenuComponents[i].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos =
			MenuComponents[i].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos - ScrollDistance;
			
			MenuComponents[i].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[i].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos - ScrollDistance;
			
			MenuComponents[i].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[i].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos - ScrollDistance;
			
			MenuComponents[i].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[i].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos - ScrollDistance;
		
			MenuComponents[i].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[i].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos - ScrollDistance;
	
		
			}
		}
	

	if(Input.touchCount>0){
	//Get the last touched place
	LastTouch = Input.GetTouch(0);
	}
	
	if(Input.touchCount==0){
	FingerLifted = true;
	}

ScrollSpeedDirected = 0.000;

}

//Based on the scrolling, destroy any menu components that are too far from being used
function DestroyMenuComponent(){

		//if a MenuComponent goes too far off the bottom of the Screen	(Check Using one which is still "Alive")
		if(MenuComponents[MenuComponents.Length-1])
			if (MenuComponents[MenuComponents.Length-1].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos > 1.632){
			Destroy(MenuComponents[MenuComponents.Length-1]);

			}
	
		//if a MenuComponent goes too far off the top of the Screen	(Check Using one which is still "Alive")
		if(MenuComponents[0])
			if (MenuComponents[0].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos < -0.25){
			Destroy(MenuComponents[0]);
		
			}
	
	//If a slot becomes vacant
	if(!MenuComponents[0] || !MenuComponents[MenuComponents.Length-1]){
	
	//Call this to reshuffle all the things in the array
	RePopulatetheMenuArray();
	
	}
	
	
}

//Add a new Component if necessary when scrolling
function CreateNewMenuComponent(){

//If the first Element in the menu component array is empty then instantiate a new object for it
	if (CreatingComponent0){
			var MenuComponentClone = 
			Instantiate(MenuComponentPrefab,gameObject.transform.position,gameObject.transform.rotation);
			
			MenuComponents[0] = MenuComponentClone;
			
			MenuComponents[0].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy =
			MenuComponents[1].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy - ComponentSpacing;
			
			MenuComponents[0].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos =
			MenuComponents[1].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos - ComponentSpacing;
			
			MenuComponents[0].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[1].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos  - ComponentSpacing;
			
			MenuComponents[0].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[1].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos  - ComponentSpacing;
			
			MenuComponents[0].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[1].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos  - ComponentSpacing;
		
			MenuComponents[0].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[1].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos  - ComponentSpacing;
			
			//Set up the Level Number for the new MenuComponent
			MenuComponents[0].transform.Find("SignalNumber").GetComponent(SignalNumber).LevelNumber = 
			MenuComponents[1].transform.Find("SignalNumber").GetComponent(SignalNumber).LevelNumber-1;
			//Update the string the Level Number for the new MenuComponent
			MenuComponents[0].transform.Find("SignalNumberTitle").GetComponent(ScalableText).str =
			MenuComponents[0].transform.Find("SignalNumberTitle").GetComponent(ScalableText).str+ ": " +
			MenuComponents[0].transform.Find("SignalNumber").GetComponent(SignalNumber).LevelNumber.ToString();
			
	}

//If the Last Element in the menu component array is empty then instantiate a new object for it
	if (CreatingComponentLength){
	
	var MenuComponentCloneA = 
			Instantiate(MenuComponentPrefab,gameObject.transform.position,gameObject.transform.rotation);
			
			MenuComponents[MenuComponents.Length-1] = MenuComponentCloneA;
			
			MenuComponents[MenuComponents.Length-1].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy =
			MenuComponents[MenuComponents.Length-2].transform.Find("CurrencySymbol").GetComponent(TextureScaler).ImagePosy + ComponentSpacing;
			
			MenuComponents[MenuComponents.Length-1].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos =
			MenuComponents[MenuComponents.Length-2].transform.Find("Level").GetComponent(LoadSceneButton).BtnYPos + ComponentSpacing;
			
			MenuComponents[MenuComponents.Length-1].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[MenuComponents.Length-2].transform.Find("SignalNumberTitle").GetComponent(ScalableText).TextBoxYPos  + ComponentSpacing;
			
			MenuComponents[MenuComponents.Length-1].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[MenuComponents.Length-2].transform.Find("SignalStatus").GetComponent(ScalableText).TextBoxYPos  + ComponentSpacing;
			
			MenuComponents[MenuComponents.Length-1].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[MenuComponents.Length-2].transform.Find("UnlockCost").GetComponent(ScalableText).TextBoxYPos  + ComponentSpacing;
		
			MenuComponents[MenuComponents.Length-1].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos =
			MenuComponents[MenuComponents.Length-2].transform.Find("UnlockTitle").GetComponent(ScalableText).TextBoxYPos  + ComponentSpacing;
			
	}

}


//TODO.. nearly there, but not 

//Shuffle everything in the Menu Component array up or down
function RePopulatetheMenuArray(){
	
	
	
	//if the top component gets destroyed, shuffle everything up
	if (MenuComponents[0] == null){
	
		for (var i = 0;i<MenuComponents.Length-1;i++){
		MenuComponents[i] = MenuComponents[i+1];
		Debug.Log(i);
		}
		
		//Replace the last element
		CreatingComponent0 = false;
		CreatingComponentLength = true;
		CreateNewMenuComponent();


	}

	//if the bottom component gets destroyed, shuffle everything down
	if (MenuComponents[MenuComponents.Length -1] == null){
	
		for (var c = MenuComponents.Length-1;c>0;c--){
		MenuComponents[c] = MenuComponents[c-1];
		
		}
		//Replace the last element
		CreatingComponent0 = true;
		CreatingComponentLength = false;
		CreateNewMenuComponent();
	}

//CreateNewMenuComponent();

}


//Give the MenuComponents have the correct data
//configure the menu conponent with the appropriate data
function PopulateMenuComponent(){

}

