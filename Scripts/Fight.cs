using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fight : MonoBehaviour {
	
	private bool startBattle = false;
	private bool battleFirstFrame = true;
	public Camera fightCam;
	public Camera mainCam;
	
	// Use this for initialization
	void Start(){
		mainCam.enabled = true;
		fightCam.enabled = false;
	}
	
	// Update is called once per frame
	void Update(){
		if(startBattle == true){
			if(battleFirstFrame == true){
				mainCam.enabled = false;
				fightCam.enabled = true;
				battleFirstFrame = false;
			}
			
			if (Input.GetKeyDown(KeyCode.C)) {
				mainCam.enabled = !mainCam.enabled;
				fightCam.enabled = !fightCam.enabled;
			}
		}
	}
	
	public void SetStartBattle(bool newValue){
		startBattle = newValue;
		return;
	}
	
	public bool GetStartBattle(){
		return startBattle;
	}
}
