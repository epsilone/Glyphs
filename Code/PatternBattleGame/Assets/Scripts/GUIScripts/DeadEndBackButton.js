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
    //This is loads the scene from which this scene was loaded
   		LoadThisScene = ScenetoLoad.LastScene;
       	
    	
    	if(LoadThisScene != "null"){
    	
     		ScenetoLoad.ScenetoLoad = LoadThisScene;
 
   	 		Application.LoadLevel("LoadingScreen"); 
   		}
    }
     
   
}


