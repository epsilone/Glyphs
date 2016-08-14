//this script turns things on and off.. it actually deactivates them, it's not just stopping the from rendering

var targetObjectOn : boolean;
var targetObject :GameObject;
var flashframes = 0;
var counter = 0;

function FixedUpdate () {
counter++;

if (targetObjectOn == true){
//Target Object ON
if (counter < flashframes){
targetObject.SetActive(true);
}
//Target Object OFF
if (counter > flashframes){
targetObject.SetActive(false);
}
}

if (counter > flashframes*2){
counter = 0;
}

}