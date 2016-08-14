using UnityEngine;
using System.Collections;

public class AndroidBackButtonController : MonoBehaviour {

	public GameObject[] GUIs;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			if(Application.loadedLevelName == "CommonFlow"){
				for(int i = 0;i<GUIs.Length;i++){
					
					if (GUIs[i].transform.root.gameObject.activeSelf){
						
						switch(GUIs[i].name) {
						case "TitleScreen":
							//Application.Quit();
							break;
							
						case "back_btn":
							GUIs[i].GetComponent<UIButton>().OnClick();
							break;
							
						default://
							break;
							
						}
						
					}
				}

			}
			else if(Application.loadedLevelName == "GamePlayFlow"){
				for(int i = 0;i<GUIs.Length;i++){
					if (GUIs[i].transform.parent.gameObject.activeSelf){
						
						switch(GUIs[i].name) {
							
						case "back_btn":
							GUIs[i].GetComponent<UIButton>().OnClick();
							break;
							
						default://
							break;
							
						}
						
					}
				
				}
			}

		}
	}
}