using UnityEngine;
using System.Collections;

public class LevelList : MonoBehaviour {

	public GameObject listItemPrefab;
	public UITable table;
	public LoadLevelOnClick toGameplayFlow;
	public GameListManager gameListManager;
	public TransitionGUIonClick toDummy;
	public GameObject endLevel;
    public GameObject allLevelsCompleted;
    
    void Start()
    {
        for (int i = 0; i < SinglePlayerProgression.Instance.levels.Length - 1; i++)
        {
            GameObject newListItem = (GameObject)Instantiate(listItemPrefab);
            newListItem.GetComponent<LevelListItem>().number = i;
            newListItem.GetComponent<LevelListItem>().Init();
            newListItem.transform.SetParent(table.transform, false);

        }

        bool hasCompletedAllLevels = SinglePlayerProgression.maxUnlockedLevel == SinglePlayerProgression.Instance.levels.Length - 1;

        allLevelsCompleted.SetActive(hasCompletedAllLevels);
        if (hasCompletedAllLevels)
        {
            allLevelsCompleted.transform.SetParent(table.transform, false);
        }

        endLevel.transform.SetParent(table.transform, false);
        table.repositionNow = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public void toGameplay(){
		gameListManager.DoSinglePlayer();
		toDummy.OnClick();
		toGameplayFlow.OnClick();
	}
}
