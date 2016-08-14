using UnityEngine;
using System.Collections;

public class SpaceScene : MonoBehaviour {

	static SpaceScene instance;

	public static SpaceScene Instance
	{
		get { return SpaceScene.instance; }
	}
	
	void Awake () {
		if( instance == null )
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if( instance != this )
		{
			Destroy( this.gameObject );
			return;
		}

	}

	// Update is called once per frame
	void Update () {

	}
}
