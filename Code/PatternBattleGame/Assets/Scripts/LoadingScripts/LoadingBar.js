#pragma strict
var LoadingBar : GameObject;
var LoadBarPercentage = 0.0000;
var LoadingBarFill : GameObject;
var FillWidth = 0.000;
var LoadingBarSpeed = 0.01;

function Start () {
	FillWidth = LoadingBarFill.GetComponent(TextureScaler).ImageWidth;
}

function FixedUpdate () {
	LoadBarPercentage = LoadBarPercentage + LoadingBarSpeed;
	if (LoadBarPercentage >= 1)
		LoadBarPercentage = 1;

	LoadingBarFill.GetComponent(TextureScaler).ImageWidth = LoadBarPercentage*FillWidth;
}