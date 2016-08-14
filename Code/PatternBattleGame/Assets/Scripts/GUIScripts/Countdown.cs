using com.bzr.puzzleBattle;
using UnityEngine;

public class Countdown : MonoBehaviour
{
	[SerializeField]
	private string goString = "GO!";
	[SerializeField]
	private float countdownAmount = 3f;
	[SerializeField]
	private float endBuffer = 0.5f;

	[SerializeField]
	private UILabel title;
	[SerializeField]
	private UISprite commence;

	[SerializeField]
	private UILabel getReady;

	[SerializeField]
	private UILabel countdownLabel;


	[SerializeField]
	private TransitionGUIonClick mainGame;
	[SerializeField]
	private GameObject glyphsController;
	[SerializeField]
	private PlateHandler playField;
	[SerializeField]
	private MultiplayerGUIManager multiplayerGUIManager;


	private bool isCountingDown = false;
	private float startTime;
	private float currTime;

	[SerializeField]
	private bool isMultiplayer = false;

	[SerializeField]
	private bool isSequence = true;

	[SerializeField]
	private Collider[] glyphs;
	
	void Update ()
    {
		if(isCountingDown)
        {
			if(Time.time < (startTime+countdownAmount))
            {
                int currentValue = Mathf.CeilToInt(currTime);
				countdownLabel.text =  currentValue.ToString();

                currTime -= Time.deltaTime;
            }
            else if (Time.time - (startTime + countdownAmount + endBuffer) <= 0.1f)
            {
				// end stuff here
				countdownLabel.text = goString;
			}
            else
            {
				//turn things off
				EndCountdown();
			}
		}
	}

	public void StartCountdown()
    {
		startTime = Time.time;
		currTime = countdownAmount;
		isCountingDown = true;

        if (isSequence)
        {
            DisableGlyphs();
        }
        else
        {
			title.gameObject.SetActive(false);
			title.enabled = false;
			commence.gameObject.SetActive(false);
		}

		getReady.gameObject.SetActive(true);
		countdownLabel.gameObject.SetActive(true);
	    countdownLabel.text = countdownAmount.ToString();
    }

	private void EndCountdown()
    {
		gameObject.GetComponent<FlyMenu>().FlyOut();
		isCountingDown = false;
		if(isMultiplayer == false) {
			
			if(isSequence == true){
				EnableGlyphs();
			} else {
				CommenceSingle();
			}
		} else { 
			CommenceMulti();
		}
	}

	public void Reset(){
		if(isSequence == true){
			title.gameObject.SetActive(true);
			commence.gameObject.SetActive(true);
			EnableGlyphs();
		}

		getReady.gameObject.SetActive(false);
		countdownLabel.gameObject.SetActive(false);
	}

	private void CommenceSingle(){
		mainGame.OnClick();
		glyphsController.SendMessage("TurnGlyphsAnimationControllerOff");
		playField.ChangeToWaitMode();
		playField.newGame();
	}
	private void CommenceMulti(){
		playField.newMultiplayerGame();
		multiplayerGUIManager.ChangeMode();
	}

	private void DisableGlyphs(){
		for(int i = 0; i < glyphs.Length; i++){
			glyphs[i].enabled = false;
		}
	}
	
	private void EnableGlyphs(){
		for(int i = 0; i < glyphs.Length; i++){
			glyphs[i].enabled = true;
		}
	}
}
