using UnityEngine;
using System.Collections;
using com.bzr.puzzleBattle;

[RequireComponent(typeof(UILabel))]

public class LevelLabel : MonoBehaviour {

	//public bool multiplayer;

	private string localizedString;
	public string stringToLocalize;

	public bool addOne = true;

	UILabel myLabel;

	// Use this for initialization
	void Start () {
		myLabel = GetComponent<UILabel>();
		stringToLocalize = myLabel.text;
		localizedString = Localization.Get(stringToLocalize);
	}
	
	// Update is called once per frame
	void Update () {
		if(myLabel!=null){
			int level;
			if(MultiplayerGameManager.isMultiplayer){
				level=0;
				gameObject.SetActive(false);
			}
			else
				level = SinglePlayerProgression.level + (addOne?1:0);

			myLabel.text = string.Format(localizedString, level.ToString());

		}
	}
}
