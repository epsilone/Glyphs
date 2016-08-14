using UnityEngine;
using System.Collections;
using com.bzr.puzzleBattle;

[RequireComponent(typeof(UILabel))]

public class PayoutLabel : MonoBehaviour {

	//public bool multiplayer;

	UILabel myLabel;

	private string localizedString;
	public string stringToLocalize;

	// Use this for initialization
	void Start () {
		myLabel = GetComponent<UILabel>();
		stringToLocalize = myLabel.text;
		localizedString = Localization.Get(stringToLocalize);
	}
	
	// Update is called once per frame
	void Update () {
		if(myLabel!=null){
			int amount = 0;
			if(MultiplayerGameManager.isMultiplayer)
				gameObject.SetActive(false);
			else 
			{
			    if (SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level] != null)
			    {
			        amount = SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level].winReward;
			    }
			    else
			    {
			        amount = SinglePlayerProgression.Instance.levels[SinglePlayerProgression.level-1].winReward;
			    }
			}
				

			myLabel.text = string.Format(localizedString, amount.ToString());

		}
	}
}
