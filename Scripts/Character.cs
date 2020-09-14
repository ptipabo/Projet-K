using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier les composants des éléments de type GUI

public class Character : MonoBehaviour {
	
	// Variables
	private MasterScript masterScript;// Permet de stocker et d'utiliser toutes les fonctions du MasterScript
	private CharacterController controller;//composant "CharacterController" du personnage
	public float speed;// Vitesse de base du personnage
	public float agility;// Agilité de base du personnage
	public float characterSize;// Taille du personnage en cm pour pouvoir positionner sa fenêtre de dialogue
	public float corpulence;// Corpulence du personnage (sert à corriger les conflits entre le character controller de ce personnage et le script de déplacements)
	private Transform charTransform;// Permet de stocker les contrôles du personnage
	private Vector3 moveDirection = Vector3.zero;// Contient la direction dans laquelle le personnage doit se diriger
	
	// Use this for initialization
	void Start () {
		charTransform = this.transform;
		controller = GetComponent<CharacterController>();//on place le composant "CharacterController" dans la variable "controller"
		masterScript = GameObject.Find("GameManager").GetComponent<MasterScript>();// On stock toutes les fonctions du MasterScript
	}
	
	// Update is called once per frame
	void Update () {
		charTransform.LookAt(GameObject.Find("DirectionTarget").transform);// On oblige le personnage à regarder sa cible de direction
		charTransform.eulerAngles = new Vector3(0f, charTransform.eulerAngles.y, 0f);// On corrige la rotation du personnage car les axes Y et Z sont inversés entre Blender et Unity
		
		moveDirection.y -= masterScript.GetGravity();// On indique que la gravité s'applique sur l'axe Y
		controller.Move(moveDirection);// On applique la gravité sur le controleur du personnage
	}
	
	// Functions

}