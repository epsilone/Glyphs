#pragma strict
var Player1Setup : GameObject;
var Player2Setup : GameObject;

function Start () {
if(HowManyPlayerSetup.NumberofPlayers == 1){
Player1Setup.SetActive(true);
Player2Setup.SetActive(false);
}

if(HowManyPlayerSetup.NumberofPlayers == 2){
Player1Setup.SetActive(false);
Player2Setup.SetActive(true);
}

}



function Update () {

}