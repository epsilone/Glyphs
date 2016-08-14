using UnityEngine;
using System.Collections;
using Parse;
using System;
using UnityEngine.UI;
using System.IO;

public class FriendListItem : MonoBehaviour {

	public UILabel nameLabel;
	public Image profilePicture;
	
	public ParseUser friendUser;

	public string friendName;
	public string facebookId;
	public string pictureURL;

	public void newChallenge(){
	if(MultiplayerGameManager.Instance!=null)
		{
		if(friendUser!=null)
		{
			if(MultiplayerGameManager.Instance.challengeManager!=null)
				MultiplayerGameManager.Instance.challengeManager.newChallenge(friendUser);
			else
				Debug.LogError("friendlistitem missing challengeManager");
		}
		else
			Debug.LogError("friendlistitem missing frienduser");
		}
	else
		Debug.LogError("no multiplayergamemanager instance");

	}

	void SaveTextureToFile(Texture2D texture, string fileName)
	{
		if(File.Exists(Application.persistentDataPath + "/"+fileName))
			return;
		var bytes=texture.EncodeToPNG();
		var file = File.Open(Application.persistentDataPath + "/"+fileName,FileMode.Create);
		var binary= new BinaryWriter(file);
		binary.Write(bytes);
		file.Close();
	}

	public IEnumerator updateLabels(){
		string url;
		nameLabel.text = friendName;
		nameLabel.MarkAsChanged();
		//Debug.Log(pictureURL);
		if(File.Exists(Application.persistentDataPath + "/" + facebookId + ".png"))
			url = "file://" + Application.persistentDataPath +  "/" + facebookId + ".png";
		else
			url = pictureURL;// + "&access_token=" + FB.AccessToken;
		WWW www = new WWW(url);
		yield return www;
		if(www.error==null){
			Sprite sprite = new Sprite();
			sprite = Sprite.Create(www.texture, new Rect(0, 0, 50, 50),new Vector2(0, 0),100.0f);
			//Debug.Log(profilePicture);
			profilePicture.sprite = sprite;
			profilePicture.color = Color.white;
			SaveTextureToFile(www.texture,facebookId+".png");
			//TweenAlpha.Begin(profilePicture.gameObject, 1f, 1f);
		}
		else{
			Debug.Log(www.error);
		}
		yield return 1;

	}

	public void populateFields(string first_name, string id, ParseUser friend){

		friendUser = friend;
		friendName = first_name;
		facebookId = id;
		pictureURL = "http://graph.facebook.com/" + facebookId + "/picture?type=small&return_ssl_resources=0";

		StartCoroutine(updateLabels());
	}
}
