using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using com.bzr.puzzleBattle;
using System.IO;

public class GameListManager : MonoBehaviour {

	public UITable myTurnTable;
	public UITable theirTurnTable;

	public GameObject noGamesMyTurnLabel;
	public GameObject noGamesTheirTurnLabel;

	public GameObject gameInProgressPrefab;

	public Challenge challenge;

	private Queue<ParseObject> gamesToAdd;

	public bool doClearGames = false;

	// Use this for initialization
	void Awake () {

		challenge = GameObject.FindObjectOfType<Challenge>();

	}

	void OnEnable(){
//		clearGames();
		challenge.GetChallengeWaitingOnMe();
		challenge.GetSavedChallenge();
	}
	
	// Update is called once per frame
	void Start () {
	
		gamesToAdd = new Queue<ParseObject>();

	//	challenge.saveChallenge("1234123",ParseUser.CurrentUser,"4");
		
		//challenge.GetSavedChallenge();
		//challenge.GetChallengeWaitingOnMe();
	}
	/*
	void OnEnable(){
		challenge.GetSavedChallenge();
		challenge.GetChallengeWaitingOnMe();
	}*/

	private void addGame(bool myTurn, ParseObject gameInfo){

		GameObject newTableRow = (GameObject) Instantiate(gameInProgressPrefab);
		newTableRow.GetComponent<MultiplayerGameListItem>().myTurn=myTurn;
		newTableRow.GetComponent<MultiplayerGameListItem>().populateFields(gameInfo);
		if(myTurn){
			myTurnTable.GetComponent<UIScrollView>().ResetPosition();
			newTableRow.transform.parent=myTurnTable.transform;
			newTableRow.transform.localScale=Vector3.one;
			newTableRow.transform.localPosition=Vector3.zero;
			myTurnTable.repositionNow=true;
			myTurnTable.GetComponent<UIScrollView>().ResetPosition();
			noGamesMyTurnLabel.SetActive(false);
		}
		else{
			theirTurnTable.GetComponent<UIScrollView>().ResetPosition();
			newTableRow.transform.parent=theirTurnTable.transform;
			newTableRow.transform.localScale=Vector3.one;
			newTableRow.transform.localPosition=Vector3.zero;
			//newTableRow.GetComponent<BoxCollider>().enabled=false;
			//newTableRow.GetComponent<UIButton>().isEnabled=false;
			theirTurnTable.repositionNow=true;
			theirTurnTable.GetComponent<UIScrollView>().ResetPosition();
			noGamesTheirTurnLabel.SetActive(false);
		}

	}

	public void clearGames(){
		foreach (Transform g in myTurnTable.children){
			Destroy(g.gameObject);
		}
		foreach (Transform g in theirTurnTable.children){
			Destroy(g.gameObject);
		}
		noGamesMyTurnLabel.SetActive(true);
		noGamesTheirTurnLabel.SetActive(true);
	}

	public void repositionTables(){
		myTurnTable.Reposition();
		theirTurnTable.Reposition();
		myTurnTable.GetComponent<UIScrollView>().ResetPosition();
		theirTurnTable.GetComponent<UIScrollView>().ResetPosition();
		//todo: custom sort by updatedAt

	}

	public void QueueAddGame(bool myTurn, ParseObject gameInfo){

		gameInfo["myTurn"]=myTurn;

		gamesToAdd.Enqueue (gameInfo);

	}

	void Update(){

		if(gamesToAdd.Count>0){

			ParseObject newGame = gamesToAdd.Dequeue();

			addGame((bool)newGame["myTurn"],newGame);
		
		}
		if(doClearGames)
		{
			doClearGames = false;
			clearGames();
		}

	}
	
	public void DoSinglePlayer(){
		
		MultiplayerGameManager.isMultiplayer=false;
		MultiplayerGameManager.Instance.multiplayerSequence=new List<Move>();
		MultiplayerGameManager.Instance.addedMoves = new List<Move>();
		
	}

}
