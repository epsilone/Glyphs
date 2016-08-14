#pragma strict
var Texture : Texture;
var GUIDEPTH = 0;
var Image = false;

var ImagePosx = 0.000;
var ImagePosy = 0.000;
var ImageWidth = 0.000;
var ImageHeight = 0.000;

private var Image_Posx = 0;
private var Image_Posy = 0;
private var Image_Width = 0;
private var Image_Height = 0;

function Start () {

}

function Update () {

Image_Posx = ImagePosx * Screen.width;
Image_Posy = ImagePosy * Screen.height;
Image_Width = ImageWidth * Screen.width;
Image_Height = ImageHeight * Screen.height;

}


function OnGUI() {
GUI.depth = GUIDEPTH;

    if (!Texture) {
        Debug.LogError("Please assign a texture on the inspector");
        return;
    }
    if(Image == true){
   GUI.DrawTexture(Rect(Image_Posx,Image_Posy,Image_Width,Image_Height),Texture); 
    }
  

}