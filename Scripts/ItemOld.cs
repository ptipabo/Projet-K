using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier le champ "Text" d'un composant Text

//Cette classe permet simplement à un GameObject de possèder un prix, un nom et une quantité
public class ItemOld : MonoBehaviour {
	
	//private MasterScript functions;//Lie ce script au MasterScript
	public int elementId;//Définit l'ID de ce type d'objet pour le lier à des event et pour indiquer au script ce qu'il doit faire lorsque cet objet est utilisé
	public int price;//Définit le prix de ce type d'objet
	public int quantity;//La quantité actuelle de cet objet que possède le joueur
	private string elementName;//Nom de cet objet tel qu'il apparaît dans le jeu
	private int ranking = 0;//Emplacement actuel de cet objet dans l'inventaire
	private Vector3 itemPosition;//Emplacement de cet objet à l'écran d'après son ranking
	private bool recalculatePosition = false;

	// Use this for initialization
	void Start () {
		//functions = GameObject.Find("GameManager").GetComponent<MasterScript>();
		elementName = this.name;
	}
	
	// Update is called once per frame
	void Update () {
		
		//De base tous les objets de l'inventaire sont placés en première position, il faut donc que lorsqu'un objet est ajouté, qu'il calcule sa véritable position afin qu'il se place à la fin de la liste
		if(recalculatePosition == true){
			itemPosition = new Vector3(this.transform.position.x, this.transform.position.y-(ranking*30), this.transform.position.z);
			transform.position = itemPosition;
			recalculatePosition = false;
		}
		
		//Si la quantité de l'objet est inférieure à zéro, on désactive cet objet
		if(quantity < 1){
			//Debug.Log(this);
			transform.gameObject.SetActive(false);
			ranking = 0;
			if(transform.position != itemPosition){
				recalculatePosition = true;
			}
		}
		else{//Sinon on met sa quantité à jour et on active l'item
			transform.Find("Quantity").GetComponent<Text>().text = quantity+" ";
		}
		
		//Si cet objet porte l'id 0, il s'agit d'une boîte de lait
		if(elementId == 0){
			//Si cet objet est utilisé, il se passe...
		}
	}
	
	
	//Fonctions du script Item :
	
	//Permet de savoir la quantité de cet objet que possède le joueur
	public int GetQuantity(){
		return quantity;
	}
	
	//Permet d'ajouter ou retirer une valeur souhaitée au nombre d'objets de ce type
	public void SetQuantity(int value){
		quantity = quantity+value;
		return;
	}
	
	//Permet de récupèrer l'id de cet objet afin de savoir de quel objet il s'agit
	public int GetElementId(){
		return elementId;
	}
	
	//Permet de récupèrer le nom de l'objet
	public string GetName(){
		return elementName;
	}
	
	//Permet de connaitre le ranking de cet objet, c'est à dire l'ordre dans lequel il apparaît dans le jeu
	public int GetRanking(){
		return ranking;
	}
	
	//Permet de modifier le ranking de cet objet, c'est à dire l'ordre dans lequel il apparaît dans le jeu
	public void SetRanking(int newValue){
		ranking = newValue;
		return;
	}
	
	public void SetRecalculatePosition(bool newValue){
		recalculatePosition = newValue;
		return;
	}
}
