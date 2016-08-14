using UnityEngine;
using System.Collections;
using com.bzr.puzzleBattle;
using Parse;
using UnityEngine.UI;
using System.IO;

public class MultiplayerGUIManager : MonoBehaviour {

	public GameObject[] clickTrackers;

	public PlateHandler plateHandler;

	public UILabel[] myNameLabels;
	public UILabel[] myLocationLabels;
	public Image[] myProfilePictures;

	public UILabel[] opponentNameLabels;
	public UILabel[] opponentLocationLabels;
	public Image[] opponentProfilePictures;
	
	[SerializeField]
	public ParseUser opponent;
	
	public string opponentName;
	public string opponentLocation;
	public string opponentFacebookId;
	public string opponentPictureURL;

	public string myName;
	public string myLocation;
	public string myFacebookId;
	public string myPictureURL;

	public GameObject sendButton;

	// Use this for initialization
	void Start () {
		if(MultiplayerGameManager.isMultiplayer){
//			clickTrackers[0].SetActive(false);
			clickTrackers[1].SetActive(true);

//			plateHandler.newMultiplayerGame();

//			if(MultiplayerGameManager.Instance.multiplayerSequence.Count==0)
//			{
////				clickTrackers[4].SetActive(true);
//			}
//			else
//			{
//
//			}

		}
		else
		{
			clickTrackers[0].SetActive(true);
			plateHandler.DoFlyin();
		}
		PopulateFields();
		StartCoroutine(updateLabels());


	}

	public void ChangeMode(){
		if(MultiplayerGameManager.Instance.multiplayerSequence.Count==0)
		{
			clickTrackers[4].SetActive(true);
		}
		else
		{
			clickTrackers[0].SetActive(true);
		}
	}

	void Update()
	{
		if(plateHandler!=null)
			if(plateHandler.mode==com.bzr.puzzleBattle.PlayMode.ADD_MOVES)
				if(MultiplayerGameManager.Instance.addedMoves.Count>0)
					sendButton.SetActive(true);
	}

	public void BackButton(){
	
		if(MultiplayerGameManager.isMultiplayer)
			clickTrackers[1].SetActive(true);
		else
			clickTrackers[0].SetActive(true);

	}


	public void SaveGame(){
		MultiplayerGameManager.Instance.SaveGame();
	}

	public void EndGame(){
		MultiplayerGameManager.Instance.EndGame();
	}

	public void NewGame(){
		MultiplayerGameManager.Instance.challengeManager.newChallenge(MultiplayerGameManager.Instance.currentOpponent);
	}

	public void PopulateFields(){
	

		if(MultiplayerGameManager.Instance.currentOpponent!=null){
			opponent = MultiplayerGameManager.Instance.currentOpponent;
			opponentName = opponent.Get<string>("name");
			opponentFacebookId = opponent.Get<string>("facebookId");
			opponentLocation = opponent.Get<string>("location");
			opponentPictureURL = "http://graph.facebook.com/" + opponentFacebookId + "/picture?type=small&return_ssl_resources=0";
		}

		if(ParseUser.CurrentUser!=null)
		{
			ParseUser me = ParseUser.CurrentUser;
			myName = me.Get<string>("name");
			myFacebookId = me.Get<string>("facebookId");
			myLocation = me.Get<string>("location");
			myPictureURL = "http://graph.facebook.com/" + myFacebookId + "/picture?type=small&return_ssl_resources=0";
		}
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
		WWW www;
		Sprite sprite;

		foreach(UILabel myNameLabel in myNameLabels){
			myNameLabel.text = myName;
			myNameLabel.MarkAsChanged();
		}
		foreach(UILabel myLocationLabel in myLocationLabels){
			myLocationLabel.text = myLocation;
			myLocationLabel.MarkAsChanged();
		}

		if(MultiplayerGameManager.Instance.currentOpponent!=null){
			
			foreach(UILabel opponentNameLabel in opponentNameLabels){
				opponentNameLabel.text = opponentName;
				opponentNameLabel.MarkAsChanged();
			}
			foreach(UILabel opponentLocationLabel in opponentLocationLabels){
				opponentLocationLabel.text = opponentLocation;
				opponentLocationLabel.MarkAsChanged();
			}

			//Debug.Log(pictureURL);

			sprite = new Sprite();

			if(File.Exists(Application.persistentDataPath + "/" + opponentFacebookId + ".png"))
				url = "file://" + Application.persistentDataPath +  "/" + opponentFacebookId + ".png";
			else
				url = opponentPictureURL;// + "&access_token=" + FB.AccessToken;
			www = new WWW(url);
			yield return www;
			if(www.error==null){
				sprite = Sprite.Create(www.texture, new Rect(0, 0, 50, 50),new Vector2(0, 0),100.0f);
				//Debug.Log(profilePicture);
				foreach(Image opponentProfilePicture in opponentProfilePictures){
					opponentProfilePicture.sprite = sprite;
					opponentProfilePicture.color = Color.white;
				}
				SaveTextureToFile(www.texture, opponentFacebookId+".png");
				//TweenAlpha.Begin(profilePicture.gameObject, 1f, 1f);
			}
			else{
				Debug.Log(url);
				Debug.LogError(www.error);
			}

		}

		sprite = new Sprite();

		if(File.Exists(Application.persistentDataPath + "/" + myFacebookId + ".png"))
			url = "file://" + Application.persistentDataPath +  "/" + myFacebookId + ".png";
		else
			url = myPictureURL;// + "&access_token=" + FB.AccessToken;
		www = new WWW(url);
		yield return www;
		if(www.error==null){
			sprite = Sprite.Create(www.texture, new Rect(0, 0, 50, 50),new Vector2(0, 0),100.0f);
			//Debug.Log(profilePicture);
			foreach(Image myProfilePicture in myProfilePictures){
				myProfilePicture.sprite = sprite;
				myProfilePicture.color = Color.white;
			}
			SaveTextureToFile(www.texture, myFacebookId+".png");
			//TweenAlpha.Begin(profilePicture.gameObject, 1f, 1f);
		}
		else{
			Debug.Log(url);
			Debug.LogError(www.error);
		}

		yield return 1;
		
	}
}
