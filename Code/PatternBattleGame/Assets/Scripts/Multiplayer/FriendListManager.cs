using UnityEngine;
using System.Collections;
using Parse;
using Facebook.MiniJSON;
using Facebook;
using System.Collections.Generic;

public class FriendListManager : MonoBehaviour {

	public UITable friendListTable;

	public GameObject friendPrefab;

	private Queue<friendInfo> friendsToAdd;

	public struct friendInfo
	{
		public string name;
		public string fbid;
		public ParseUser friend;
	}

	// Use this for initialization
	void Awake () {

	}

	void OnEnable(){
		//reset scroll
		friendListTable.GetComponent<UIPanel>().clipOffset=Vector2.zero;
		//listFriends();
	}

	// Update is called once per frame
	void Update()
	{
		if(friendsToAdd.Count>0){
			
			friendInfo newFriend = friendsToAdd.Dequeue();
			makeFriend(newFriend);
			
		}
	}

	void Start () {
		friendsToAdd = new Queue<friendInfo>();
		
		listFriends();
	}

	public void RandomGame(){
		MultiplayerGameManager.Instance.NewRandomGame();
	}

	public void inviteNewFriend()                                                                                              
	{ 
		Analytics.Flurry.Instance.LogEvent("inviteFBFriendsClicked");
		FB.AppRequest("Play Glyphs with me!",null,null,null,null,null,"Invite a friend",appRequestCallback);	

	/*	FB.AppRequest(
			message: "It's Glyphtastic!",
			title: "Play Glyphs with me!",
			callback:appRequestCallback
			);    */                                                                                                          
		
	}   
	

	public void listFriends(){
                                                                        
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

		SinglePlayerProgression.Instance.SetAchievementProgress("friends", friends.Count);

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
			//FB.API("/me?fields=friends.fields(name,id)", Facebook.HttpMethod.GET, APICallback);  
			return;                                                                                                                
		}                                                                                                                         

		Debug.Log (result.Text);

		var friends = DeserializeJSONFriends(result.Text);

		foreach (Dictionary<string,object> friend in friends){
			addFriend(getDataValueForKey(friend,"name"),getDataValueForKey(friend,"id"));
		}


	}   
	
	private void appRequestCallback (FBResult result)                                                                              
	{                                                                                                                              
//		Util.Log("appRequestCallback");                                                                                         
		if (result != null)                                                                                                        
		{                                                                                                                          
			var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj))                                                                 
			{                                                                                                                      
				Debug.Log("Request cancelled");              
				Analytics.Flurry.Instance.LogEvent("inviteFBFriendsCanceled");
			}                                                                                                                      
			else if (responseObject.TryGetValue ("request", out obj))                                                              
			{                
				Debug.Log("Request sent");      
				Analytics.Flurry.Instance.LogEvent("invitedFBFriends");
			}                                                                                                                      
		}                                                                                                                          
	}   

	public void makeFriend(friendInfo newFriend)
	{
		GameObject newTableRow = (GameObject) Instantiate(friendPrefab);
		FriendListItem friendListItem = newTableRow.GetComponent<FriendListItem>();
		newTableRow.transform.parent=friendListTable.transform;
		newTableRow.transform.localScale=Vector3.one;
		newTableRow.transform.localPosition=Vector3.zero;
		friendListTable.repositionNow=true;
		friendListItem.populateFields(newFriend.name,newFriend.fbid,newFriend.friend);
	}

	public void addFriend(string name, string facebookId){

		var query = ParseUser.Query
			.WhereEqualTo("facebookId", facebookId)
				.FirstAsync().ContinueWith(t =>
                  {

					if (t.IsFaulted || t.IsCanceled)
					{
						Debug.LogError("friendlistmanager couldn't find user");
					}
					else{
						ParseUser user = t.Result;
						friendInfo tempFriend = new friendInfo();
						tempFriend.name = name;
						tempFriend.fbid = facebookId;
						tempFriend.friend = user;
						friendsToAdd.Enqueue(tempFriend);
					}
				});


	}
	
	
}
