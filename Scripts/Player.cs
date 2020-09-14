using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private MasterScriptOld functions;
	private Animator anim;//composant "Animator" du personnage
	private CharacterController controller;//composant "CharacterController" du personnage
	private float speed = 0.7f;//vitesse de déplacement du personnage par frame
	//private float turnSpeed = 160.0f;//vitesse de rotation du personnage
	private float rotationY;//Valeur de la rotation du personnage
	private float turnSpeed = 3.0f;//vitesse de rotation du personnage
	private Vector3 moveDirection = Vector3.zero;//position où doit se rendre le personnage (se basant sur la position actuelle du personnage)
	private float gravity = 0.3f;//force de la gravité s'exercant sur ce personnage
	private float jumpHeight = 4.0f;//hauteur du saut du personnage
	private int animId;//l'id de l'animation à effectuer
	private float verticalAxis = 0.0f;
	private float horizontalAxis = 0.0f;
	private int frameNumber = 0;//numéro de la frame en cours
	private float locationA;//position du personnage en Y à une frame donnée (fonctionne en alternance avec la variable "locationB")
	private float locationB;//position du personnage en Y à une frame donnée (fonctionne en alternance avec la variable "locationA")
	private float characterFall;//distance entre les positions A et B des 2 variables précédentes (locationA et locationB)
	private bool poSwitch = true;//permet d'alterner l'utilisation des variables locationA et locationB
	private int frameAtJump = 0;//si le personnage saute, le numéro de la frame où le saut commence est enregistré et indique au script si le personnage à sauté avant de tomber ou s'il est tombé directement sans sauter
	private Fight fightScript;
	private bool isFalling = false;
	
	// Use this for initialization
	void Start() {
		functions = GameObject.Find("GameManager").GetComponent<MasterScriptOld>();
		//on place le composant "Animator" dans la variable "anim"
		anim = GetComponent<Animator>();
		//on place le composant "CharacterController" dans la variable "controller"
		controller = GetComponent<CharacterController>();
		//on enregistre la position Y du personnage (le "transform") dans la variable locationA
		locationA = moveDirection.y;
		fightScript = GameObject.Find("GameManager").GetComponent<Fight>();
	}
	
	// Update is called once per frame
	void Update() {
		if(functions.GetPauseMode() == false && functions.GetDialogueMode() == false){
			enableControls(true, true);
			//Debug.Log("Frame "+frameNumber+" : "+transform.position.y);
			//Debug.Log("Gravity : "+gravity);
			//Toutes les 3 frames, on vérifie la position en Y du personnage afin de savoir s'il est en train de tomber ou non
			if(functions.IsMultiple(frameNumber, 3)){
				if(poSwitch==true){
					locationA = transform.position.y;
					poSwitch = false;
				}
				else{
					locationB = transform.position.y;
					poSwitch = true;
				}
			}
			//On vérifie la différence entre l'ancienne position du personnage et la nouvelle
			if((poSwitch == true && locationB < locationA) || (poSwitch == false && locationA < locationB)){
				isFalling = true;
			}
			else{
				isFalling = false;
			}
			
			characterFall = locationA-locationB;
			
			//si la valeur ci-dessus est négative, on la change en valeur positive pour la suite du script
			if(characterFall<0.0f){
				characterFall = characterFall*-1;
			}
			
			//Le personnage ne touche pas le sol
			//lors d'un saut, permet au personnage de rester en position de saut pendant le nombre de frames désiré avant de passer en position de chute
			if(frameAtJump!=0){
				if((frameAtJump+25) == frameNumber){
					animId = 4;
					frameAtJump = 0;
				}
				else{
					animId = 3;
				}
				anim.SetInteger("AnimPar", animId);
			}//Le personnage est en train de tomber
			else if(isFalling == true && IsFalling(characterFall, gravity)){
				animId = 4;
				anim.SetInteger("AnimPar", animId);
			}
			else{//Le personnage touche le sol
				frameAtJump = 0;
				//Le personnage avance en marchant ou en courant selon si la touche shift (gauche ou droite) est activée
				if(Input.GetKey("d")){
					horizontalAxis = 1.0f;
				}
				else if(Input.GetKey("q")){
					horizontalAxis = -1.0f;
				}
				else{//Le personnage ne bouge pas
					horizontalAxis = 0.0f;
				}
					
				if(Input.GetKey("z") || Input.GetKey("up")){
					verticalAxis = 1.0f;
					speed=0.7f;
					if(Input.GetKey("right shift") || Input.GetKey("left shift")){
						speed = 0.2f;
						animId = 2;
					}
					else{
						animId = 1;
					}
					anim.SetInteger("AnimPar", animId);
				}//Le personnage recule (toujours en marchant)
				else if(Input.GetKey("s") || Input.GetKey("down")){
					verticalAxis = -1.0f;
					speed=0.3f;
					animId = 2;
				}
				else{//Le personnage ne bouge pas
					verticalAxis = 0.0f;
					animId = 0;
				}
				
				anim.SetInteger("AnimPar", animId);
				moveDirection = transform.forward*verticalAxis*speed;
			}
			
			//Le personnage saute
			if(Input.GetKeyDown(KeyCode.Space)){
				//le personnage ne peut sauter que s'il n'est pas en train de tomber
				if(IsFalling(characterFall, gravity) == false){
					moveDirection.y += jumpHeight;
					animId = 3;
					anim.SetInteger("AnimPar", animId);
					frameAtJump = frameNumber;
				}
			}
			
			if(Input.GetKeyDown(KeyCode.F)){
				if(fightScript.GetStartBattle() == false){
					fightScript.SetStartBattle(true);
				}
				else{
					fightScript.SetStartBattle(false);
				}
			}
			
			//le personnage ne peut pivoter que s'il n'est pas en train de tomber
			if(IsFalling(characterFall, gravity) == false){
				transform.Rotate(0, horizontalAxis*turnSpeed,0);
			}
			
			//"controller.Move" permet d'enclencher le déplacement du personnage, "Time.deltaTime" fait en sorte qu'il y ait 60 frames par seconde
			
			moveDirection.y -= gravity;
			
			controller.Move(moveDirection);
			
			//Les 2 lignes suivantes donnent le contrôle de la rotation de la camera (et du personnage) à la souris
			//rotationY += Input.GetAxis("Mouse X") * turnSpeed;
			//transform.rotation = Quaternion.Euler(0, rotationY, 0);
			
			frameNumber++;
		}
		else if(functions.GetDialogueMode() == true){
			enableControls(false, true);
		}
		else{
			enableControls(false, false);
		}
	}
	
	public void SetTurnSpeed(float newValue){
		turnSpeed = newValue;
		return;
	}
	
	public void SetGravity(float newValue){
		gravity = newValue;
		return;
	}
	
	public bool IsFalling(float displacement, float fallSpeed){
		if(displacement<fallSpeed){
			return false;
		}
		else{
			return true;
		}
	}
	
	public void enableControls(bool newValueA, bool newValueB){
		controller.enabled = newValueA;
		anim.enabled = newValueB;
	}
}
