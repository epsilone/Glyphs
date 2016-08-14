using UnityEngine;
using System.Collections;


public class HighlightScript : MonoBehaviour {

	public string deactivated;
	public string activated;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void highlight(bool light) {
		UISprite s = GetComponent<UISprite> ();
		Debug.Log (s);
		if (light) {
			s.spriteName = this.activated;
		} else {
			s.spriteName = this.deactivated;
		}
	}
}
