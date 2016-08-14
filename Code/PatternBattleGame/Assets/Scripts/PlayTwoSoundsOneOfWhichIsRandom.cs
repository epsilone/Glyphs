using UnityEngine;
using System.Collections;

public class PlayTwoSoundsOneOfWhichIsRandom : MonoBehaviour {

	public AudioClip mySound;
	public AudioClip[] randomSounds;

	private AudioSource myAudioSource;
	private AudioSource randomAudioSource;

	public void Play(){

		myAudioSource.volume = PlayerPrefs.GetInt("sound",1);	
		randomAudioSource.volume = PlayerPrefs.GetInt("sound",1);	
	
		myAudioSource.PlayOneShot(mySound);
		randomAudioSource.PlayOneShot(randomSounds[Random.Range(0,randomSounds.Length)]);
	
	}

	public void PlayJustOne(){
	
		myAudioSource.volume = PlayerPrefs.GetInt("sound",1);	
		randomAudioSource.volume = PlayerPrefs.GetInt("sound",1);	

		myAudioSource.PlayOneShot(mySound);
	
	}

	// Use this for initialization
	void Start () {
		myAudioSource = gameObject.AddComponent<AudioSource>();
		randomAudioSource = gameObject.AddComponent<AudioSource>();
		myAudioSource.volume = PlayerPrefs.GetInt("sound",1);	
		randomAudioSource.volume = PlayerPrefs.GetInt("sound",1);	
	}

	void Awake(){

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
