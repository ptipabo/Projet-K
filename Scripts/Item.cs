using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier le champ "Text" d'un composant Text

//Cette classe permet simplement à un GameObject de possèder un prix, un nom et une quantité
public class Item : MonoBehaviour {
	
	//private MasterScript functions;//Lie ce script au MasterScript
	public int elementId;//Définit l'ID de ce type d'objet pour le lier à des event et pour indiquer au script ce qu'il doit faire lorsque cet objet est utilisé
	public bool keyItem;
	public bool usableItem;
	public string description;
	public string charText;

	// Use this for initialization
	void Start () {
		//functions = GameObject.Find("GameManager").GetComponent<MasterScript>();
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}