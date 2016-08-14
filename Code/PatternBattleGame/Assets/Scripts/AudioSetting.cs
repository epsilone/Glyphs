using UnityEngine;
using System.Collections;

public class AudioSetting : MonoBehaviour {
	public string settingName = "sound";
	// Use this for initialization
	void Start () {
		updateVolume();
	}

	public void updateVolume(){
		if(GetComponent<AudioSource>()!=null)
			GetComponent<AudioSource>().volume = PlayerPrefs.GetInt(settingName,1);	
		if(GetComponent<UIPlaySound>()){
			GetComponent<UIPlaySound>().volume=PlayerPrefs.GetInt(settingName,1);
		}
	}

}
