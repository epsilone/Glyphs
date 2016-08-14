using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Examples/Load Level On Click")]
public class LoadLevelOnClick : MonoBehaviour
{

	int numberofplayers = 0;
	public string levelName;
	public string test = "unchanged";
	public int UI_ID = 0;

	private UIManager jsScript;


	void Awake()
	{
		//Get the JavaScript component
		jsScript = this.GetComponent<UIManager>();//Don't forget to place the 'JS1' file inside the 'Standard Assets' folder

	}

	[ContextMenu("OnClick")]
	public void OnClick ()
	{

		if(gameObject.tag != "transition"){

			// Check to see if this will be setting the 1 or 2 player flow
			if(numberofplayers == 0){
			//if not a load button on the titlescreen do nothing
					
			}
			else{
				if (levelName == "SinglePlayerSetup")
					numberofplayers = 1;
				if (levelName == "MultiPlayerLogin")
					numberofplayers = 2;

			// if a loadbutton on the title screen selecting player mode
			//it should have the number of players set to either 1 or 2 
				HowManyPlayerSetup.NumberofPlayers = numberofplayers;
		}


			if (!string.IsNullOrEmpty(levelName))
			{
			
					//This captures the name of the scene in which this load button is used for use with DeadEndBackButton
					ScenetoLoad.LastScene = Application.loadedLevelName;
					ScenetoLoad.ScenetoLoad = levelName;
					Application.LoadLevelAdditive("LoadingScreen"); 

	
			}

		}






	}


}