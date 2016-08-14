using UnityEngine;
using System.Collections;

public class LevelListItem : MonoBehaviour {

	public LevelList levelList;

	public enum levelStatus {LOCKED, UNLOCKED, PLAYED};
	public levelStatus status;
	public int number;
	
	public UILabel titleLabel;
	public UILabel statusLabel;
	public UILabel rewardLabel;
	//public GameObject[] costToUnlockObjects;
	public UISprite bigSprite;
	public UISprite connectorSprite;

	public Color lockedColour;
	public Color unlockedColour;
	public Color playedColour;

	private UIButton meButton;

	void Awake(){
		levelList = GameObject.Find("1p_LvlSelect").GetComponent<LevelList>();
		meButton = GetComponent<UIButton>();
	}

	// Use this for initialization
	void Start () {
		titleLabel.text=string.Format(Localization.Get ("200.level_title"), (number+1).ToString());
		rewardLabel.text = SinglePlayerProgression.Instance.levels[number].winReward.ToString()+"x";
	}

	public void Init () {
		if(number<SinglePlayerProgression.maxUnlockedLevel)
			status=levelStatus.PLAYED;
		else if(number==SinglePlayerProgression.maxUnlockedLevel)
			status=levelStatus.UNLOCKED;
        else
			status=levelStatus.LOCKED;

		switch(status){
			case levelStatus.LOCKED:
				meButton.normalSprite = meButton.disabledSprite = meButton.pressedSprite = meButton.hoverSprite = bigSprite.spriteName="signal_anomaly";
				connectorSprite.spriteName="connector-locked";
				statusLabel.color=lockedColour;
				statusLabel.text=Localization.Get("200.level_locked");
			//	for(int i=0; i<costToUnlockObjects.Length; i++)
			//	rewardLabel.gameObject.SetActive(true);
			break;
			case levelStatus.UNLOCKED:
				meButton.normalSprite = meButton.disabledSprite = meButton.hoverSprite = bigSprite.spriteName="signal_incoming";
				meButton.pressedSprite = "signal_aquired";
				connectorSprite.spriteName="connector-active";
				statusLabel.color=unlockedColour;
				statusLabel.text=Localization.Get("200.level_unlocked");
			//	for(int i=0; i<costToUnlockObjects.Length; i++)
			//	rewardLabel.gameObject.SetActive(true);
			break;
			case levelStatus.PLAYED:
				meButton.normalSprite = meButton.disabledSprite = meButton.pressedSprite = meButton.hoverSprite = bigSprite.spriteName="signal_aquired";
				connectorSprite.spriteName="connector-complete";
				if(number<SinglePlayerProgression.maxUnlockedLevel-1)
					connectorSprite.spriteName="green_connector";
				statusLabel.color=playedColour;
				statusLabel.text=Localization.Get("200.level_complete");
			//	for(int i=0; i<costToUnlockObjects.Length; i++)
			//	rewardLabel.gameObject.SetActive(false);
			break;
		}

		bigSprite.MarkAsChanged();
		connectorSprite.MarkAsChanged();
		statusLabel.MarkAsChanged();
	}

	public void goToGame(){
		if(SinglePlayerProgression.maxUnlockedLevel<number){
		//	if(!SinglePlayerProgression.SpendMoney(SinglePlayerProgression.Instance.levels[number].unlockCost))
#if UNITY_ANDROID
			GCM.ShowToast("Level is locked");
#endif
			return;
		}
		if(SinglePlayerProgression.level!=number){
			EncryptedPlayerPrefs.DeleteKey("sequence");
			SinglePlayerProgression.level=number;
		}
		SinglePlayerProgression.progressInCurrentLevel=0;
		levelList.toGameplay();
	}
}
