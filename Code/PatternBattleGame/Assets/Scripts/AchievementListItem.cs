using UnityEngine;
using System.Collections;

public class AchievementListItem : MonoBehaviour {
	
	public string achievementName;
	
	public UILabel titleLabel;
	public UILabel descriptionLabel;
	public UISprite progressBar;
	public UISprite tier1star;
	public UISprite tier2star;
	public UISprite tier3star;
	public UILabel currentProgress;
	public UILabel nextLevelAmount;
	public UILabel reward;
	public UILabel tierText;
	
	// Update is called once per frame
	void Update () {

		if(SinglePlayerProgression.Instance.achievements.ContainsKey(achievementName)){

		if(SinglePlayerProgression.Instance.achievements[achievementName].currentTier>=3)
			tier3star.enabled=true;
		if(SinglePlayerProgression.Instance.achievements[achievementName].currentTier>=2)
			tier2star.enabled=true;
		if(SinglePlayerProgression.Instance.achievements[achievementName].currentTier>=1)
			tier1star.enabled=true;

		currentProgress.text = SinglePlayerProgression.Instance.achievements[achievementName].progress.ToString();
		nextLevelAmount.text = SinglePlayerProgression.Instance.achievements[achievementName].tiers[SinglePlayerProgression.Instance.achievements[achievementName].currentTier].unlock.ToString();
		reward.text = "x " + SinglePlayerProgression.Instance.achievements[achievementName].tiers[SinglePlayerProgression.Instance.achievements[achievementName].currentTier].reward.ToString();
		tierText.text = "Tier " + (SinglePlayerProgression.Instance.achievements[achievementName].currentTier+1).ToString();
		titleLabel.text = SinglePlayerProgression.Instance.achievements[achievementName].title;
		descriptionLabel.text = string.Format( SinglePlayerProgression.Instance.achievements[achievementName].description , SinglePlayerProgression.Instance.achievements[achievementName].tiers[SinglePlayerProgression.Instance.achievements[achievementName].currentTier].unlock );

		float percentage = ( (float) SinglePlayerProgression.Instance.achievements[achievementName].progress / (float) SinglePlayerProgression.Instance.achievements[achievementName].tiers[SinglePlayerProgression.Instance.achievements[achievementName].currentTier].unlock ) ;
		progressBar.fillAmount = percentage;
		}
	}


}