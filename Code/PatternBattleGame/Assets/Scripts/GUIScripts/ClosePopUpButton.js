public var BtnXPos =.8;
public var BtnYPos =.9;
public var BtnXSize =.1;
public var BtnYSize =.1;

var PopUp : GameObject;

//var CameraState = 1;

var Depth = -1;

var CustomButton : GUIStyle;
var BTNText = "null";

// Main Map Button Texture
var btnTexture : Texture;
// Performs GUI actions


function Update(){


	
}

function OnGUI() {
GUI.depth = Depth;

var ClosePopUpButton = null;

    
    ClosePopUpButton = GUI.Button(Rect(Screen.width*BtnXPos,Screen.height * BtnYPos
									,(Screen.width*BtnXSize)
									,(Screen.width*BtnYSize)*(Screen.height/Screen.width)),BTNText,CustomButton);
									
 
    
  	if (ClosePopUpButton)
    {
    	PopUp.SetActive(false);
    }
     
   
}


