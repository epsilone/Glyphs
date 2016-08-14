using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System;
using Facebook;
using Facebook.MiniJSON;
using System.Linq;
using System.Threading.Tasks;
using Analytics;

public class CustomLogin : MonoBehaviour {

	public TransitionGUIonClick gameListGUI;
	public TransitionGUIonClick loginButtonGUI;

	public GameObject errorPopup;

	private string[] SENDER_IDS = {"677079040683"};

	public void toSocialConnect(){

		if (FB.IsLoggedIn) {
			// Check if we're logged in to Parse
			if (ParseUser.CurrentUser == null) {
				// If not, log in with Parse
				StartCoroutine("ParseLogin");
			} else if(ParseUser.CurrentUser.ContainsKey("facebookId")){
				loginSuccessful();
			}
			else {
				//ParseUser.LogOut();
				Debug.Log("already have a parse user, but not connected to facebook");
				StartCoroutine("ParseLogin");
			}
		} else {
			loginButtonGUI.OnClick();
		}
	}

	public void FBLogin() {
		// Logging in with Facebook
		Analytics.Flurry.Instance.LogEvent("tryConnectFacebook");
		FB.Login("email, user_friends, public_profile", FBLoginCallback);
	}

	private void FBLoginCallback(FBResult result) {
		// Login callback
		if(FB.IsLoggedIn) {
			Debug.Log("Connected to facebook. Now logging in to Parse...");
			StartCoroutine("ParseLogin");
		} else {
			Debug.Log ("FBLoginCallback: User canceled login");
			Analytics.Flurry.Instance.LogEvent("facebookLoginCanceled");
			loginFail();
		}
	}

	private IEnumerator ParseLogin() {
		if (FB.IsLoggedIn) {

//			Analytics.Flurry.Instance.LogEvent("facebookConnected");


			//if(ParseUser.CurrentUser == null)
			//	logInTask = ParseFacebookUtils.LogInAsync(FB.UserId, FB.AccessToken, DateTime.Now);
			//else



			var existingUser = ParseUser.Query
				.WhereEqualTo("facebookId",FB.UserId)
					.FirstAsync();



//			.ContinueWith(t =>
//					                          {
//						if (!(t.IsFaulted || t.IsCanceled) && t.Result !=null)
//						{
////							ParseUser.LogOut();
//							newUser = t.Result;
////							ParseUser oldUser = ParseUser.CurrentUser;
//						}
//						else{
//							Debug.Log("couldn't find existing user with this facebook id");
//						}
//					});
//
			while(!existingUser.IsCompleted) yield return 1;

			if(!existingUser.IsCanceled && !existingUser.IsFaulted)
			{
				if(existingUser.Result!=null)
				{
					Debug.Log("found parse user with this facebook id");
					ParseUser newUser = existingUser.Result;
					var oldUser = ParseUser.CurrentUser;
					var loginTask = ParseUser.LogInAsync(newUser.Username, FB.UserId).ContinueWith(t3=>{
						if (!(t3.IsFaulted || t3.IsCanceled)) {
							Debug.Log("successfully logged in to existing parse account with facebook id");
	//						FB.API("/me?fields=name,gender,email,location", HttpMethod.GET, FBAPICallback);
							SinglePlayerProgression.money =  Mathf.Max (SinglePlayerProgression.money, newUser.Get<int>("artifacts"));
							SinglePlayerProgression.maxUnlockedLevel = Mathf.Max (SinglePlayerProgression.maxUnlockedLevel, newUser.Get<int>("maxUnlockedLevel"));
							SinglePlayerProgression.Instance.SetAchievementProgress("days", newUser.Get<int>("consecutiveLogins"));
						}
						else
						{
							Debug.LogError("unable to log in to existing parse account");
	//						loginFail();
						}
					});

					while(!loginTask.IsCompleted) yield return 1;
					if(!loginTask.IsFaulted && !loginTask.IsCanceled)
					{
						Debug.Log("Deleting guest account");
						var deleteTask = oldUser.DeleteAsync();
					}
				}
				else
				{
				Debug.Log("couldn't find existing user with this facebook id");
				}
			}


//			Task logInTask = ParseFacebookUtils.LinkAsync(ParseUser.CurrentUser, FB.UserId, FB.AccessToken, DateTime.Now);

			
//			if(!ParseFacebookUtils.IsLinked(ParseUser.CurrentUser))
//			{
//					Debug.LogError("unable to link facebook user parse account");
//					Debug.LogError(logInTask.Exception);
//										Debug.LogError(error.Code);
//					Analytics.Flurry.Instance.LogError(logInTask.Exception.Message);
//					loginFail();
//					ParseFBLogout();
										
										// error.Message will contain an error message
										// error.Code will return "OtherCause"
//			}
								
//			else {
					// Handle success
//					Debug.Log("logged in to both Facebook and Parse. Now querying user info from facebook...");
					ParseUser.CurrentUser["facebookId"] = FB.UserId;
					ParseUser.CurrentUser["password"] = FB.UserId;
					var saveTask = ParseUser.CurrentUser.SaveAsync();
					while(!saveTask.IsCompleted) yield return 1;

					FB.API("/me?fields=name,gender,email,location", HttpMethod.GET, FBAPICallback);
//			}


		}
		else
		{
			loginFail();
		}
	}

	private void ParseFBLogout() {
		FB.Logout();
		ParseUser.LogOut();
//		toSocialConnect();
	}

	private void FBAPICallback(FBResult result)
	{
		if (!String.IsNullOrEmpty(result.Error)) {
			Debug.Log ("FBAPICallback: Error getting user info: + "+ result.Error);
			Analytics.Flurry.Instance.LogError("FBAPICallback",result.Error);
			// Log the user out, the error could be due to an OAuth exception
			loginFail();
			//ParseFBLogout();
		} else {
			Debug.Log(result.Text);
			// Got user profile info
			var resultObject = Json.Deserialize(result.Text) as Dictionary<string, object>;
			var userProfile = new Dictionary<string, string>();
			
			userProfile["facebookId"] = getDataValueForKey(resultObject, "id");
			userProfile["name"] = getDataValueForKey(resultObject, "name");
			userProfile["email"] = getDataValueForKey(resultObject, "email");
			userProfile["gender"] = getDataValueForKey(resultObject, "gender");
			userProfile["location"] = getDataValueForKey(resultObject, "location");
			//userProfile["birthday"] = getDataValueForKey(resultObject, "birthday");
			//userProfile["relationship"] = getDataValueForKey(resultObject, "relationship_status");
			if (userProfile["facebookId"] != "") {
				userProfile.Add("pictureURL","https://graph.facebook.com/" + userProfile["facebookId"] + "/picture?type=large&return_ssl_resources=1");
			}
			/*
			var emptyValueKeys = userProfile
				.Where(pair => String.IsNullOrEmpty(pair.Value))
					.Select(pair => pair.Key).ToList();
			foreach (var key in emptyValueKeys) {
				userProfile.Remove(key);
			}*/
#if UNITY_ANDROID
			GCM.ShowToast ("Facebook login success");
#endif
#if UNITY_IPHONE
			UnityEngine.iOS.LocalNotification notif = new UnityEngine.iOS.LocalNotification();
			notif.fireDate = DateTime.Now;
			notif.alertBody = "Facebook login success";
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
#endif
			
			StartCoroutine("saveUserProfile", userProfile);

			loginSuccessful();
		}
	}

	private IEnumerator saveUserProfile(Dictionary<string, string> profile) {
		var user = ParseUser.CurrentUser;

//		user["facebookId"] = profile["facebookId"];
//		user["profile"] = profile;
		user["name"] = profile["name"];
		user["email"] = profile["email"];
		user["gender"] = profile["gender"];
		user["location"] = profile["location"];
//		user["password"] = profile["facebookId"];
#if UNITY_ANDROID
		if(GCM.IsRegisteredOnServer ()){
			user["registration_id"]=GCM.GetRegistrationId();
			Analytics.Flurry.Instance.LogEvent("pushNotificationsRegistered");
		}
#endif
		if(profile["gender"]!=null)
			Analytics.Flurry.Instance.LogUserGender((Analytics.UserGender) System.Enum.Parse( typeof(Analytics.UserGender), profile["gender"], true ) );

		var saveTask = user.SaveAsync();
		while (!saveTask.IsCompleted) yield return null;
		if (saveTask.IsFaulted || saveTask.IsCanceled)
			Debug.LogError(saveTask.Exception);
		/*
		// Save if there have been any updates
		if (user.IsKeyDirty("profile")) {
			var saveTask = 
			while (!saveTask.IsCompleted) yield return null;
			//loginSucessful();
		}*/
	}

	private string getDataValueForKey(Dictionary<string, object> dict, string key) {
		object objectForKey;
		if (dict.TryGetValue(key, out objectForKey)) {
			return (string)objectForKey;
		} else {
			return "";
		}
	}

	private void SetInit()
	{
//		FB.GetDeepLink(DeepLinkCallback);
		//enabled = true;
	}
	
	private void OnHideUnity(bool isGameShown) {
		if (!isGameShown)
		{
			// pause the game - we will need to hide
			Time.timeScale = 0;
		}
		else
		{
			// start the game back up - we're getting focus again
			Time.timeScale = 1;
		}
	}

	/* ############################ */
	/* #######Unity Lifecyle####### */
	/* ############################ */

	void Awake(){
		FB.Init(SetInit, OnHideUnity);
#if UNITY_ANDROID
		GCM.Initialize();
		GCM.Register (SENDER_IDS);
		GCM.SetRegisteredCallback ((string registrationId) => {
//			Debug.Log ("Registered!!! " + registrationId);
		//	GCM.ShowToast ("Registered!!!");
			if(ParseUser.CurrentUser!=null){
				ParseUser.CurrentUser["registration_id"]=registrationId;
				ParseUser.CurrentUser.SaveAsync();
			}
		});
		GCM.SetMessageCallback ((Dictionary<string, object> table) => {
			//Debug.Log ("Message!!!");
			//GCM.ShowToast ("Message!!!");
			//_text = "Message: " + System.Environment.NewLine;
			foreach (var key in  table.Keys) {
				Debug.Log(key + "=" + table[key]);
			}
			loginSuccessful();
			Analytics.Flurry.Instance.LogEvent("OpenedAppFromGCMPushNotification");
			//GCM.ShowToast (table["data"].ToString());
		});



#endif

#if UNITY_IOS
		UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
		if(ParseUser.CurrentUser!=null){
			ParseUser.CurrentUser["deviceToken"]=UnityEngine.iOS.NotificationServices.deviceToken;
			ParseUser.CurrentUser.SaveAsync();
		}
#endif

		if(ParseUser.CurrentUser==null){
			string username = System.Guid.NewGuid().ToString();
			string password = System.Guid.NewGuid().ToString();
			
			ParseUser parseUser = new ParseUser()
			{
				Username = username,
				Password = password,
			};
			
			parseUser["name"] = "Guest #" + username;
			
			try
			{
				parseUser.SignUpAsync().ContinueWith(t =>
				                                     {
					if (t.IsFaulted || t.IsCanceled)
					{
						// The login failed. Check t.Exception to see why.
						Debug.Log("Parse Registration failed!");
//						Flurry.Instance.LogError("parseUser.SignUpAsync",t.Exception.ToString());
//						LogParseException(t.Exception);
					}
					else
					{
						// Login was successful.
						Debug.Log("Parse Registration was successful!");
						parseUser.SaveAsync();
//						Flurry.Instance.LogUserID(parseUser.ObjectId);
					}
				});
			}
			catch (System.Exception e)
			{
				Debug.Log("Failed to sign up Parse User. Reason: " + e.Message);
				Flurry.Instance.LogError("parseUser.SignUpAsync",e.Message);
				throw;
			}
		}
		else{
			Flurry.Instance.LogUserID(ParseUser.CurrentUser.ObjectId);
		}
		
		Flurry.Instance.LogEvent("startedApp");
	}

	
	void DeepLinkCallback(FBResult response) {
		if(response.Text != null) {
			var index = (new Uri(response.Text)).Query.IndexOf("request_ids");
			if(index != -1) {
				Analytics.Flurry.Instance.LogEvent("OpenedAppFromFacebookPushNotification");
				loginSuccessful();
			}
		}
	}
	
	private void loginSuccessful()
	{
		Debug.Log ("login successful");

		gameListGUI.OnClick();

//		SinglePlayerProgression.Instance.SetAchievementProgress("days", ParseUser.CurrentUser.Get<int>("consecutiveLogins"));

		//MultiplayerGameManager.Instance.challengeManager.GetChallengeWaitingOnMe();
		//MultiplayerGameManager.Instance.challengeManager.GetSavedChallenge();
	
	}
	
	private void loginFail()
	{
		Debug.Log ("login failed");
#if UNITY_ANDROID
		GCM.ShowToast ("Facebook login failed");
#endif
#if UNITY_IOS
		UnityEngine.iOS.LocalNotification notif = new UnityEngine.iOS.LocalNotification();
		notif.fireDate = DateTime.Now;
		notif.alertBody = "Facebook login failed";
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
#endif
		errorPopup.SetActive(true);
		//ParseFBLogout();
	}
}
