using UnityEngine;
using System.Collections;
using Parse;
using System;
using System.Collections.Generic;
using System.Threading;
using com.bzr.puzzleBattle;

public class Challenge : MonoBehaviour
{
	private String ChallengeObjectClass = "MultiplayerSequence";
	public String method = "";
	public GameListManager gameListManager;

	// Use this for initialization
	void Awake ()
	{
		if(GameObject.FindObjectsOfType<Challenge>().Length>1)
			Destroy(gameObject);

		if(!gameListManager)
			gameListManager = GameObject.FindObjectOfType<GameListManager>().GetComponent<GameListManager>();
	}

	void Start(){
		DontDestroyOnLoad(this.gameObject);
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void newChallenge(ParseUser friend){
//		if(friend!=null)
//			Debug.Log("attempting to start new game against " + friend.Get<string>("name"));


		MultiplayerGameManager.Instance.currentGameObjectId=null;
		MultiplayerGameManager.Instance.currentOpponent=friend;
//		if(friend==null)
//			Analytics.Flurry.Instance.LogEvent("multiplayerNewRandomChallenge");
//		else

		MultiplayerGameManager.Instance.ContinueGame(new List<Move>());

		Analytics.Flurry.Instance.LogEvent("multiplayerNewChallenge");

		//do flurry stuff THEN load the new scene


	}

	public void saveChallenge(Move[] sequence, ParseUser user, Move[] addedMoves, string objectId=null ) {

	//	ParseObject m_sequence = new ParseObject("sequence");

		IList<Dictionary<string,int>> m_sequence = new List<Dictionary<string,int>>();
		
		foreach (Move move in sequence){
			Dictionary<string, int> dictionary = new Dictionary<string, int>
			{
				{"move_id",move.move_id},{"src_plate_id",move.src_plate_id},{"target_plate_id",move.target_plate_id},{"twistAngle",move.twistAngle}
			};
			m_sequence.Add(dictionary);
		}
		
		if(addedMoves!=null){
			foreach (Move move in addedMoves){
				Dictionary<string, int> dictionary = new Dictionary<string, int>
				{
					{"move_id",move.move_id},{"src_plate_id",move.src_plate_id},{"target_plate_id",move.target_plate_id},{"twistAngle",move.twistAngle}
				};
				m_sequence.Add(dictionary);
			}
		}

		ParseObject challenge = new ParseObject(ChallengeObjectClass);
		if(objectId==null){
			challenge["playing_user"] = user;
			challenge ["waiting_user"] = ParseUser.CurrentUser;
			challenge ["sequence"] = m_sequence;
			challenge["turn_number"]=1;
			challenge.SaveAsync();
		}
		else{
			var query = new ParseQuery<ParseObject>(ChallengeObjectClass);
			query.GetAsync(objectId).ContinueWith(t =>
			{
				ParseObject result = t.Result;
				challenge = result;
				challenge["playing_user"] = user;
				challenge ["waiting_user"] = ParseUser.CurrentUser;
				challenge ["sequence"] = m_sequence;
				challenge.Increment("turn_number");
				challenge.SaveAsync();
			});
		}
	}

	public void GetSavedChallenge() {
		if(!gameListManager)
			gameListManager = GameObject.FindObjectOfType<GameListManager>().GetComponent<GameListManager>();

		var query = ParseObject.GetQuery(ChallengeObjectClass)
			.WhereEqualTo("waiting_user", ParseUser.CurrentUser)
				.Include("playing_user").Include("waiting_user");
		query.FindAsync().ContinueWith(t =>
		                               {
			IEnumerable<ParseObject> results = t.Result;

			foreach (ParseObject obj in results){
//				Debug.Log (obj.ToString());
				if(obj.ContainsKey("playing_user"))
					gameListManager.QueueAddGame(false,obj);
			}
			
			
		});
		//gameListManager.repositionTables();
	}

	public void GetChallengeWaitingOnMe() {
		if(!gameListManager)
			gameListManager = GameObject.FindObjectOfType<GameListManager>().GetComponent<GameListManager>();


		var query = ParseObject.GetQuery(ChallengeObjectClass)
			.WhereEqualTo("playing_user", ParseUser.CurrentUser)
			.Include("playing_user").Include("waiting_user");
		query.FindAsync().ContinueWith(t =>
		                               {

			IEnumerable<ParseObject> results = t.Result;
			gameListManager.doClearGames=true;
			foreach (ParseObject obj in results){
//				Debug.Log (obj.ToString());
				gameListManager.QueueAddGame(true,obj);
			}


		});
		//gameListManager.repositionTables();
	}
}

