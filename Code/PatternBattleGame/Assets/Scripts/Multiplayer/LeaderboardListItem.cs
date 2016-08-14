using UnityEngine;
using System.Collections;
using Parse;
using System;
using UnityEngine.UI;
using System.IO;

public class LeaderboardListItem : MonoBehaviour {

	public UILabel nameLabel;
	public Image profilePicture;
	public UILabel valueLabel;

	[SerializeField]
	public ParseUser friendUser;

	public string friendName;
	public string facebookId;
	public string pictureURL;
	public int score;

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
		valueLabel.text = score.ToString();
		valueLabel.MarkAsChanged();
		//Debug.Log(pictureURL);


		if(pictureURL!=null){
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
				SaveTextureToFile(www.texture, facebookId+".png");
				profilePicture.sprite = sprite;
				profilePicture.color = Color.white;
				//TweenAlpha.Begin(profilePicture.gameObject, 1f, 1f);
			}
			else{
				Debug.Log(url);
				Debug.LogError(www.error);
			}
		}
		yield return 1;

	}

	public void populateFields(string first_name, string id, int val){

		//friendUser = friend;
		friendName = first_name;
		score=val;

		if(id!=""){
			facebookId = id;
			pictureURL = "http://graph.facebook.com/" + facebookId + "/picture?type=small&return_ssl_resources=0";
		}

		StartCoroutine(updateLabels());
	}
}
