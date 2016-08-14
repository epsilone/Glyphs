public var BtnXPos =.8;
public var BtnYPos =.9;
public var BtnXSize =.1;
public var BtnYSize =.1;

var LoadThisScene = "null";

//var CameraState = 1;

var Depth = -1;

var CustomButton : GUIStyle;
var BTNText = "null";

// Main Map Button Texture
var btnTexture : Texture;
// Performs GUI actions

var numberofplayers = 0;

function Update(){


	
}

function OnGUI() {
GUI.depth = Depth;

var LoadSceneButton = null;

    
    
  	LoadSceneButton = GUI.Button(Rect(Screen.width*BtnXPos,Screen.height * BtnYPos
									,(Screen.width*BtnXSize)
									,(Screen.width*BtnYSize)*(Screen.height/Screen.width)),BTNText,CustomButton);
									
 
    
  	if (LoadSceneButton)
    {
    	if(numberofplayers == 0){
    	//if not a load button on the titlescreen do nothing

    	
    	}
    	else{
    	// if a loadbutton on the title screen selecting player mode
    	//it should have the number of players set to either 1 or 2
    	HowManyPlayerSetup.NumberofPlayers = numberofplayers;
    	}
    	
    	
    	if(LoadThisScene != "null"){
    	    if (LoadThisScene == "reload") {
    	    	Application.LoadLevel(Application.loadedLevel);
    	    } else {
    	//This captures the name of the scene in which this load button is used for use with DeadEndBackButton
    		ScenetoLoad.LastScene = Application.loadedLevelName;
     		ScenetoLoad.ScenetoLoad = LoadThisScene;
   	 		Application.LoadLevel("LoadingScreen"); 
   	 		}
   		}
    }
     
   
}


