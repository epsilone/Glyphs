﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]

public class MoneyLabel : MonoBehaviour {

	UILabel myLabel;

	// Use this for initialization
	void Start () {
		myLabel = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
		if(myLabel!=null)
			myLabel.text=SinglePlayerProgression.money.ToString();
	}
}
