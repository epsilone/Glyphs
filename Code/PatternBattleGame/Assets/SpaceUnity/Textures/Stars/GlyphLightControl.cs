using UnityEngine;
using System.Collections;

public class GlyphLightControl : MonoBehaviour {

	public GameObject[] Glyphs;
	public GameObject[] GlyphLights;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		//Only want to run these functions when in Display Mode

	//	TurnGlyphsOff();
	//	TurnHilightOn();
	
	}

	void TurnGlyphsOff(){

		for (int i = 0; i < GlyphLights.Length; i++)
		{
			if(GlyphLights[i].activeSelf == false)
				Glyphs[i].GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, true);
		}
		
	}



	void TurnHilightOn(){
		
		for (int i = 0; i < GlyphLights.Length; i++)
		{
			if(GlyphLights[i].activeSelf == true)
				Glyphs[i].GetComponent<UIButton>().SetState(UIButtonColor.State.Hover, true);
		}
		
	}

}
