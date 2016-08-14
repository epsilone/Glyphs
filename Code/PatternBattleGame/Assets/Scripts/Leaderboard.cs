using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using Facebook.MiniJSON;
using Facebook;
using System.Linq;
using System;

public class Leaderboard : MonoBehaviour {

	public UITable worldTable;
	public UITable friendsTable;

	public GameObject worldWinsTitle;
	public GameObject friendsWinsTitle;

	public GameObject worldLengthTitle;
	public GameObject friendsLengthTitle;

	public GameObject listItemPrefab;

	public enum table_name{
		most_wins,
		longest_pattern,
		most_wins_friends,
		longest_pattern_friends
	}

	private UITable[] tables;

	public List<string> friendIds;

	private Queue<QueuedItem> itemsToAdd;

	public class QueuedItem{
		public string name;
		public string facebookId;
		public int val;
		public table_name whichTable;
	}

	// Use this for initialization
	void Start () {
//		if(FB.IsLoggedIn)
			listFriends();

		tables = new UITable[4];

		tables[(int)table_name.most_wins] = worldTable;
		tables[(int)table_name.longest_pattern] = worldTable;
		tables[(int)table_name.most_wins_friends] = friendsTable;
		tables[(int)table_name.longest_pattern_friends] = friendsTable;

		itemsToAdd = new Queue<QueuedItem>();

		PopulateLeaderboards();
	}
	
	// Update is called once per frame
	void Update () {
		if(itemsToAdd.Count>0){
			
			QueuedItem tempUser = itemsToAdd.Dequeue();
			
			addItem(tempUser);

		}
	}

	void PopulateLeaderboards() {

		var aWeekAgo = DateTime.Now.AddDays(-7);

		var winsQuery = ParseUser.Query
			.WhereExists("wins")
			.OrderByDescending("winsThisWeek")
			.Limit(10)
			.WhereGreaterThanOrEqualTo("updatedAt", aWeekAgo)
			.FindAsync().ContinueWith(t =>
		     {
			if (!(t.IsFaulted || t.IsCanceled))
			{
				IEnumerable<ParseUser> results = t.Result;
				foreach (ParseUser user in results){
					if(!user.ContainsKey("facebookId"))
						{
						user["facebookId"]="";
						user["name"] = "Guest";
						}
					queueAddItem (user.Get<string> ("name"),user.Get<string> ("facebookId"),user.Get<int>("winsThisWeek"),table_name.most_wins);
				}
			}
		});


		var patternQuery = ParseUser.Query
			.WhereExists("longestPattern")
			.OrderByDescending("longestPatternThisWeek")
			.Limit(10)
			.WhereGreaterThanOrEqualTo("updatedAt", aWeekAgo)
			.FindAsync().ContinueWith(t =>
			                          {
				if (!(t.IsFaulted || t.IsCanceled))
				{
					IEnumerable<ParseUser> results = t.Result;
					foreach (ParseUser user in results){
						if(!user.ContainsKey("facebookId"))
							{
							user["facebookId"]="";
							user["name"] = "Guest";
							}
						queueAddItem (user.Get<string> ("name"),user.Get<string> ("facebookId"),user.Get<int>("longestPatternThisWeek"),table_name.longest_pattern);
					}
				}
			});
	
	}

	public void PopulateFriendsLeaderboards(){

		var aWeekAgo = DateTime.Now.AddDays(-7);

		var winsQuery2 = ParseUser.Query
			.WhereExists("wins")
			.OrderByDescending("wins")
			.Limit(10)
			.WhereGreaterThanOrEqualTo("updatedAt", aWeekAgo)
			//.WhereEqualTo("facebookId",FB.UserId);
			.WhereContainedIn("facebookId", friendIds);

		winsQuery2.FindAsync().ContinueWith(t =>
		        {
				if (!(t.IsFaulted || t.IsCanceled))
					{
						IEnumerable<ParseUser> results = t.Result;
						foreach (ParseUser user in results){
							if(string.IsNullOrEmpty(user.Get<string> ("facebookId")))
								Debug.LogError("no facebook id for leaderboard user");
							queueAddItem (user.Get<string> ("name"),user.Get<string> ("facebookId"),user.Get<int>("winsThisWeek"),table_name.most_wins_friends);
						}
					}
					else{
						Debug.LogException(t.Exception);
					}
				});



		var patternQuery2 = ParseUser.Query
			.WhereExists("longestPattern")
			.OrderByDescending("longestPattern")
			.Limit(10)
			//.WhereEqualTo("facebookId",FB.UserId);
			.WhereGreaterThanOrEqualTo("updatedAt", aWeekAgo)
			.WhereContainedIn("facebookId", friendIds);

		patternQuery2.FindAsync().ContinueWith(t =>
				{
					if (!(t.IsFaulted || t.IsCanceled))
					{
						IEnumerable<ParseUser> results = t.Result;
						foreach (ParseUser user in results){
						if(string.IsNullOrEmpty(user.Get<string> ("facebookId")))
							Debug.LogError("no facebook id for leaderboard user");
						queueAddItem (user.Get<string> ("name"),user.Get<string> ("facebookId"),user.Get<int>("longestPatternThisWeek"),table_name.longest_pattern_friends);
						}
					}
					else{
						Debug.LogException(t.Exception);
					}
				});
	}

	
	public void queueAddItem(string name, string fbid, int val, table_name whichTable){
	
		var tempItem = new QueuedItem();
		tempItem.name=name;
		tempItem.facebookId=fbid;
		tempItem.val=val;
		tempItem.whichTable=whichTable;

		itemsToAdd.Enqueue(tempItem);
	}

	public void addItem(QueuedItem queuedItem){
	
		addItem(queuedItem.name,queuedItem.facebookId,queuedItem.val,queuedItem.whichTable);

	}

	public void addItem(string name, string fbid, int val, table_name whichTable){

	//	Debug.Log(name);

		GameObject newTableRow = (GameObject) Instantiate(listItemPrefab);
		
		LeaderboardListItem leaderboardListItem = newTableRow.GetComponent<LeaderboardListItem>();
		
		leaderboardListItem.populateFields(name,fbid, val);

		tables[(int)whichTable].GetComponent<UIScrollView>().ResetPosition();

		newTableRow.transform.parent=tables[(int)whichTable].transform;

		if(whichTable==table_name.most_wins)
		{
			newTableRow.transform.SetSiblingIndex(worldLengthTitle.transform.GetSiblingIndex());
			worldWinsTitle.SetActive(true);
		}
		else if(whichTable==table_name.most_wins_friends)
		{
			newTableRow.transform.SetSiblingIndex(friendsLengthTitle.transform.GetSiblingIndex());
			friendsWinsTitle.SetActive(true);
		}
		else if(whichTable==table_name.longest_pattern)
		{
			newTableRow.transform.SetAsLastSibling();
			worldLengthTitle.SetActive(true);
		}
		else if(whichTable==table_name.longest_pattern_friends)
		{
			newTableRow.transform.SetAsLastSibling();
			friendsLengthTitle.SetActive(true);
		}
		newTableRow.transform.localScale=Vector3.one;
		newTableRow.transform.localPosition=Vector3.zero;
		tables[(int)whichTable].repositionNow=true;
		tables[(int)whichTable].GetComponent<UIScrollView>().ResetPosition();
		
	}

	void listFriends(){
		
		FB.API("/me?fields=friends.fields(name,id)", Facebook.HttpMethod.GET, APICallback);  
		
	}
	
	public List<object> DeserializeJSONFriends(string response)
	{
		var responseObject = Json.Deserialize(response) as Dictionary<string, object>;
		object friendsH;
		var friends = new List<object>();
		if (responseObject.TryGetValue("invitable_friends", out friendsH))
		{
			friends = (List<object>)(((Dictionary<string, object>)friendsH)["data"]);
		}
		if (responseObject.TryGetValue("friends", out friendsH))
		{
			friends.AddRange((List<object>)(((Dictionary<string, object>)friendsH)["data"]));
		}
		return friends;
	}
	
	private string getDataValueForKey(Dictionary<string, object> dict, string key) {
		object objectForKey;
		if (dict.TryGetValue(key, out objectForKey)) {
			return (string)objectForKey;
		} else {
			return "";
		}
	}
	
	void APICallback(FBResult result)                                                                                              
	{                                                                                                                              
		
		if (result.Error != null)                                                                                                  
		{                                                                                                                          
			Debug.LogError(result.Error);                                                        
			// Let's just try again                                                                                                
//			FB.API("/me?fields=friends.fields(name,id)", Facebook.HttpMethod.GET, APICallback);  
			return;                                                                                                                
		}                                                                                                                         
		
		//Debug.Log (result.Text);
		
		var friends = DeserializeJSONFriends(result.Text);

		//var temp_array = new string[friends.Count+1];
		friendIds = new List<string> ();
		
		foreach (Dictionary<string,object> friend in friends){
			//Debug.Log(getDataValueForKey(friend,"id"));
			friendIds.Add(getDataValueForKey(friend,"id"));
		}

		friendIds.Add(FB.UserId);

		//temp_array.CopyTo(friendIds,0);

		PopulateFriendsLeaderboards();
		
	}  

}
