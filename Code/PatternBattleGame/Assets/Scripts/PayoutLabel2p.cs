using UnityEngine;
using System.Collections;
using com.bzr.puzzleBattle;

[RequireComponent(typeof(UILabel))]

public class PayoutLabel2p : MonoBehaviour {

	//public bool multiplayer;

	int amount;

	private bool didCalculate = false;

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
		if (didCalculate) return;

		if(myLabel!=null){
			int throwaway;
//				gameObject.SetActive(false);
//			else
			if(MultiplayerGameManager.isMultiplayer)
				SinglePlayerProgression.Instance.CalculateRewards(out throwaway, out amount);

			myLabel.text = string.Format(localizedString, amount.ToString());

			if(amount>0) didCalculate = true;

		}
	}
}
