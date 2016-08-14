using UnityEngine;
using System.Collections;

public class FlyMenu : MonoBehaviour {
	[SerializeField]
	private TutorialManager tutMan;


	public float fromDistance = 10f;
	public float fromAlpha = 0f;
	public float inTime = 1f;
	public iTween.EaseType inEaseType = iTween.EaseType.easeOutCubic;

	public float toDistance = -5f;
	public float toAlpha = 0f;
	public float outTime = 3f;
	public iTween.EaseType outEaseType = iTween.EaseType.linear;
	

	public UIPanel uiPanel;	

	// Use this for initialization
	void Start () {
		uiPanel.alpha=fromAlpha;
		FlyIn();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FlyOut(){

		iTween.Stop(this.gameObject, false);
		//transform.position.Set(transform.position.x,transform.position.y,0);
		iTween.MoveTo(this.gameObject,iTween.Hash("z",toDistance,"time",outTime,"easetype",outEaseType));
		iTween.ValueTo(this.gameObject,iTween.Hash("from",uiPanel.alpha,"to",toAlpha,
		                                           "onupdate","updateAlpha","onupdatetarget",this.gameObject,
		                                           "oncomplete","TurnMeOff","oncompletetarget",this.gameObject));
	}

	public void updateAlpha(float val){
		uiPanel.alpha=val;
	}

	public void FlyIn(){
		iTween.Stop(this.gameObject, false);
		Vector3 newPos = Vector3.zero;
		newPos.z = fromDistance;

		transform.position = newPos;
		uiPanel.alpha=fromAlpha;

		iTween.MoveTo(this.gameObject,iTween.Hash("z",0,"time",inTime,"easetype",inEaseType));
		iTween.ValueTo(this.gameObject,iTween.Hash("from",fromAlpha,"to",1f,"onupdate","updateAlpha","onupdatetarget",this.gameObject));
		
	}

	public void TurnMeOff(){
		if(tutMan != null){
			tutMan.EndTutorial();
		}
		this.gameObject.SetActive(false);
	}
}
