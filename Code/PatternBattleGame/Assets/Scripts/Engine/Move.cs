//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1022
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.bzr.puzzleBattle;

namespace com.bzr.puzzleBattle {
	[Serializable]
	public class Move {
		
		public int move_id;
		public int src_plate_id;
		public SimonPlate src_plate;

		public int twistAngle = 0;
		public Transform field = null;

		public int target_plate_id = 0;
		public SimonPlate target_plate = null;

		public Move(int move_id, int plate_id, int target_plate_id, int twistAngle=0) {
			this.move_id = move_id;
			this.src_plate_id = plate_id;
			this.target_plate_id = target_plate_id;
			this.twistAngle = twistAngle;
		}

		public Move(int move_id, int plate_id) {
			this.move_id = move_id;
			this.src_plate_id = plate_id;
			this.target_plate_id = -1;
			this.twistAngle = 0;
		}

		public void DisplayMove(){

			if (src_plate != null) {
				src_plate.DisplayMove (this);
			}
			else if(move_id==MoveDefinition.SHAKE){
				Debug.Log ("shake");
				iTween.ShakePosition(field.gameObject,iTween.Hash("amount",Vector3.down*0.2f,"time",0.5f,"space",Space.World));
			}
			else if(move_id==MoveDefinition.TWIST){
				Debug.Log ("twist "+ twistAngle.ToString());
				iTween.RotateBy(field.gameObject,new Vector3(0,0,-twistAngle/360f),1f);
			} 
		}

		public void EndMove(){
			if (src_plate != null) {
				src_plate.EndMove (this);
			}
		}

		public string toString(){
			return "{'move_id':"+move_id+",'src_plate_id':"+src_plate_id+",'twistAngle':"+twistAngle+",'target_plate_id':"+target_plate_id+"}";
		}

		public IDictionary toDict(){
			IDictionary tempDict = new Dictionary<string,int>();
			tempDict.Add("move_id",move_id);
			tempDict.Add("src_plate_id",src_plate_id);
			tempDict.Add("twistAngle",twistAngle);
			tempDict.Add("target_plate_id",target_plate_id);
			return tempDict;

		}

	}
}

