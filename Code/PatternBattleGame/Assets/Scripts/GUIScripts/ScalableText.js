public var TextBoxXPos =.9;
public var TextBoxYPos =.9;
public var TextBoxXSize =.1;
public var TextBoxYSize =.1;
public var ScaleableFontSize =.02;
//var scrollPosition : Vector2 = Vector2.zero;
var style : GUIStyle;
var font : Font;
//private var textStyle : GUIStyle = new GUIStyle();
var Depth = -1;
// Get Text Color from Inspector
var color:Color32;
//color = Color.yellow;
var str: String = "Need to Localize this text";

function Start() { 

}
function LateUpdate (){
// add localization code in place of the commented out code below
//str = "Insert x"+CurrentPrice.Price.ToString()+" Coins";
//Debug.Log(CurrentPrice.Price);
}

function OnGUI () {
    GUI.depth = Depth;
    
    //Set TextColor
    GUI.skin.label.normal.textColor = style.normal.textColor;
    
    //Get Font
    GUI.skin.font = style.font;
   
    //GUI.skin.label.fontSize = style.fontSize;  
   // GUI.skin. = style.fontSize;
//    style.fontSize = Math.Round(ScaleableFontSize*Screen.width, 0);
      style.fontSize = ScaleableFontSize*Screen.width;
/*
    //Create TextBox
    GUILayout.BeginArea(new Rect
    					(Screen.width*TextBoxXPos, 
     					Screen.height * TextBoxYPos, 
    					(Screen.width*TextBoxXSize), 
    					(Screen.height*TextBoxYSize)*
    					(Screen.width/Screen.height)));
  */
   //Create TextBox
    GUILayout.BeginArea(Rect(Screen.width*TextBoxXPos,Screen.height * TextBoxYPos
									,(Screen.width*TextBoxXSize)
									,(Screen.width*TextBoxYSize)*(Screen.height/Screen.width)));
  
    
    //Create Scroll Bar
 //   scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.Width((Screen.width*TextBoxXSize)), GUILayout.Height((Screen.height*TextBoxYSize)*
 //   (Screen.width/Screen.height)));
 //    GUI.skin.scrollView = style;
     GUI.skin.textArea.wordWrap = style.wordWrap;   
    
    //Add the text to the box TODO: Get the text from a source elsewhere
    GUILayout.Label(str,style);
               
 //   GUILayout.EndScrollView ();
    GUILayout.EndArea();
    
 

}

