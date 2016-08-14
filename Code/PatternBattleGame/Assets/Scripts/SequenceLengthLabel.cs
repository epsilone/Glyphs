using UnityEngine;
using System.Collections;
using com.bzr.puzzleBattle;

[RequireComponent(typeof(UILabel))]

public class SequenceLengthLabel : MonoBehaviour {

	//public bool multiplayer;

	UILabel myLabel;

	private string localizedString;
	public string stringToLocalize;

		

	// Use this for initialization
	void Start () {
		Initialize();
	}
	void Initialize() {
		myLabel = GetComponent<UILabel>();
		stringToLocalize = myLabel.text;
		localizedString = Localization.Get(stringToLocalize);
	}
	// Update is called once per frame
	void Update () {

	}

	public void UpdateLabel (int length) {

		if(myLabel==null){
			Initialize();
		}

		//int length;
		if(MultiplayerGameManager.isMultiplayer)
			length = MultiplayerGameManager.Instance.multiplayerSequence.Count;
		//else
		//	length = SinglePlayerProgression.patternLength;
		myLabel.text = string.Format(localizedString, length.ToString());

	}
}
