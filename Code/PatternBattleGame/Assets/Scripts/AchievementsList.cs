using UnityEngine;
using System.Collections;

public class AchievementsList : MonoBehaviour {
	public GameObject listItemPrefab;
	public UITable table;
	
	// Use this for initialization
	void Awake () {
		foreach(SinglePlayerProgression.achievement ach in SinglePlayerProgression.Instance.achievements.Values){
			GameObject newListItem = (GameObject) Instantiate(listItemPrefab);
			newListItem.GetComponent<AchievementListItem>().achievementName=ach.name;
			newListItem.transform.SetParent(table.transform,false);
		}
		table.repositionNow=true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
