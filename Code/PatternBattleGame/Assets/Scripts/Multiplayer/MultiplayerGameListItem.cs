using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System;
using com.bzr.puzzleBattle;
using UnityEngine.UI;
using System.IO;

public class MultiplayerGameListItem : MonoBehaviour {

	public UILabel opponentNameLabel;
	public ParseUser waiting_user;
	public ParseUser playing_user;
	public List<Move> sequence;
	public DateTime updatedAt;
	public bool myTurn;
	public string parseObjectId;
	public UILabel turnNumberLabel;
	public int turnNumber;
	public Image profilePicture;
	public string pictureURL;

	//public UILabel turnNumberLabel;

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

	public void ContinueGame(){
		if(myTurn){
			SinglePlayerProgression.Instance.SetAchievementProgress("roundsmulti",turnNumber);
			MultiplayerGameManager.Instance.currentGameObjectId=parseObjectId;
			MultiplayerGameManager.Instance.currentOpponent=waiting_user;
			MultiplayerGameManager.Instance.ContinueGame(sequence);
		}
	}

	IEnumerator updateLabels(){
	
		string url;
		string facebookId;

		if(myTurn){
			opponentNameLabel.text = waiting_user.Get<string>("name");
			facebookId = waiting_user.Get<string>("facebookId");
		}
		else{
			opponentNameLabel.text = playing_user.Get<string>("name");
			facebookId = playing_user.Get<string>("facebookId");
		}

		pictureURL = "http://graph.facebook.com/" + facebookId + "/picture?type=small&return_ssl_resources=0";
		opponentNameLabel.MarkAsChanged();

		turnNumberLabel.text="TURN " + turnNumber.ToString();
		turnNumberLabel.MarkAsChanged();

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
			//profilePicture.sprite = sprite;
			profilePicture.sprite = sprite;
			profilePicture.color = Color.white;
			//TweenAlpha.Begin(profilePicture.gameObject, 1f, 1f);
			//profilePicture.MarkAsChanged();
		}

		yield return 1;

	}

	public void populateFields(ParseObject gameInfo){
		waiting_user = gameInfo.Get<ParseUser>("waiting_user");
		playing_user = gameInfo.Get<ParseUser>("playing_user");

		parseObjectId = gameInfo.ObjectId;

		turnNumber=gameInfo.Get<int>("turn_number");

		//IList<object> list = gameInfo.Get<List<object>>("sequence");

		sequence = new List<Move>();

		List<object> moves = gameInfo.Get<List<object>>("sequence");

		foreach (var move in moves){
			//Debug.Log (move.ToString());
			//if(move.ContainsKey("target_plate_id"))
			   //{"move_id",move.move_id},{"src_plate_id",move.src_plate_id},{"target_plate_id",move.target_plate_id},{"twistAngle",move.twistAngle}
			/*
			Debug.Log (move["move_id"]);
			Debug.Log (move["src_plate_id"]);
			Debug.Log (move["target_plate_id"]);
			Debug.Log (move["twistAngle"]);
*/
			var dictionary = move as Dictionary<string,object>;

		//	Debug.Log(dictionary.ToString());
			sequence.Add (new Move(int.Parse(dictionary["move_id"].ToString()),int.Parse(dictionary["src_plate_id"].ToString()),int.Parse(dictionary["target_plate_id"].ToString()),int.Parse(dictionary["twistAngle"].ToString())));

		}



		StartCoroutine(updateLabels());
	}
}
