using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {
	[SerializeField]
	private GameObject[] tutorialPages;

	[SerializeField]
	private UIPanel gameSetup;

	[SerializeField]
	private GameObject animControllerObj;

	private int currTut = -1;


	[SerializeField]
	private Collider[] glyphs;

    public bool FailedTutorial { get; set; }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartTutorial (int tutNum) {
		tutorialPages[tutNum].SetActive(true);
		gameSetup.enabled = false;

		currTut = tutNum;

		if(tutNum == 0){
			return;
		} else {
			DisableGlyphs();
			return;
		}
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

	public void EndTutorial (){
		//tutorialPages[currTut].GetComponent<FlyMenu>().FlyOut();
		if(currTut > 0){
			EnableGlyphs();
		}
		gameSetup.enabled = true;

		//		tutorialPages[currTut].SetActive(false);
	}

}
