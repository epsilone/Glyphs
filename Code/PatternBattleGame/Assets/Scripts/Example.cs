using UnityEngine;
using System.Collections;
using Analytics;
using Parse;

public class Example : MonoBehaviour 
{

	void Awake()
	{

		//PlayerPrefs.DeleteAll();
		//ParseUser.LogOut();

	//	IAnalytics analytics = Flurry.Instance;

	//	analytics.Start();

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
						Flurry.Instance.LogError("parseUser.SignUpAsync",t.Exception.ToString());
						//LogParseException(t.Exception);
					}
					else
					{
						// Login was successful.
						Debug.Log("Parse Registration was successful!");
						parseUser.SaveAsync();
						Flurry.Instance.LogUserID(parseUser.ObjectId);
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

}
