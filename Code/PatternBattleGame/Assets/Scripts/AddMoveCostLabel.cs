using UnityEngine;
using System.Collections;
using com.bzr.puzzleBattle;

[RequireComponent(typeof(UILabel))]

public class AddMoveCostLabel : MonoBehaviour {

	//public bool multiplayer;

	UILabel myLabel;
	

	// Use this for initialization
	void Start () {
		myLabel = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
		if(myLabel!=null){
			int amount = 0;
			if(MultiplayerGameManager.isMultiplayer)
				amount = MultiplayerGameManager.Instance.costForSequence;
			else
				gameObject.SetActive(false);

			myLabel.text = amount.ToString();

		}
	}
}
