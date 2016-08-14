using UnityEngine;
using System.Collections;

public class TransitionGUIonClick : MonoBehaviour {

	public bool Clicked = false;
	public GameObject  NewGUI;

	public void OnClick (){
		Clicked = true;
//		Debug.Log ("clicked");
		NewGUI.SetActive(true);
	}
}
