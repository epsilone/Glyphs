
var GUIModes : GameObject[];
var GUIModesClickTracker : GameObject[];
var GUIDummy :GameObject;

function Update(){
CheckForClicks();
}


function CheckForClicks(){

	for(var i = 0; i < GUIModesClickTracker.Length;i++){
	
	
		if(GUIModesClickTracker[i].activeSelf == true){
			SetNewGUIActive(i);
			SetOldGUItoOutro(i);
			GUIModesClickTracker[i].SetActive(false);
	
		}
	
	}

}


public function SetNewGUIActive(New_GUI_ID){
//Turn on the GUI
GUIModes[New_GUI_ID].SetActive(true);

GUIModes[New_GUI_ID].SendMessage("FlyIn",SendMessageOptions.DontRequireReceiver);

	// Turn off the GUI ClickerTrackers for everything else
	for(var i = 0; i < GUIModesClickTracker.Length;i++){
		if (GUIModesClickTracker[i]!= New_GUI_ID){
		GUIModesClickTracker[i].SetActive(false);
	}

}


}


public function SetOldGUItoOutro(New_GUI_ID){


//Cycle through all GUI modes
	for(var i = 0; i < GUIModesClickTracker.Length;i++){
	
		//If a Gui mode is active and it's not the new GUI mode	
		if(GUIModes[i].activeSelf == true && i != New_GUI_ID){
			
			//if it's not moving away transitioning out, set the GUI mode to false
			if(!GUIModes[i].GetComponent("FlyMenu")){
			GUIModes[i].SetActive(false);
			}
			else{
				GUIModes[i].SendMessage("FlyOut",SendMessageOptions.DontRequireReceiver);
			}
		
		}
	

	}
}


//Use this at the end of an exit transition
public function SetOldGUIActiveFalse(){

		for(var i = 0; i < GUIModesClickTracker.Length;i++){
		
		}

}

