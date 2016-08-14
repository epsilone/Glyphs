using UnityEngine;
using System.Collections;
using com.bzr.puzzleBattle;

public class KeyShopBackButton : MonoBehaviour {

	public PlateHandler plateHandler;
	public TransitionGUIonClick backToFail;
	public TransitionGUIonClick backToStart;
	public TransitionGUIonClick backToTitleScreen;
	public TransitionGUIonClick backTo2pStart;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick(){

		if(plateHandler!=null){

			if(MultiplayerGameManager.isMultiplayer){
				backTo2pStart.OnClick();
			}

			else {
				if(plateHandler.mode==com.bzr.puzzleBattle.PlayMode.FEEDBACK_FAIL){
					backToFail.OnClick();
				}
				else{
					backToStart.OnClick();
				}
			}
		}
		else{
			backToTitleScreen.OnClick();
		}
	}
}
