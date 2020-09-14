using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier le champ "Text" d'un composant Text

//Cette classe permet simplement à un GameObject de possèder un prix, un nom et une quantité
public class HotSpot : MonoBehaviour {
	
	//private MasterScript masterScript;//Lie ce script au MasterScript
	public int eventId;// S'il s'agit d'un hotspot de type event, définit l'ID de l'event qui est lié à ce hotspot
	public int itemId;// S'il s'agit d'un hotspot de type item, définit l'ID de l'objet à ajouter à l'inventaire - Vaut zero si pas utilisé
	public bool animate;// Permet d'indiquer au script que ce hotSpot doit s'animer lorsqu'il est cliqué (exemple : coffre qui s'ouvre)
	public string leftClickText;// Permet de stocker le texte que doit dire le personnage si le joueur fait un clique gauche sur ce hotspot
	public string rightClickText;// Permet de stocker le texte que doit dire le personnage si le joueur fait un clique droit sur ce hotspot
	public float objectRange;
	
	// Use this for initialization
	void Start () {
		//masterScript = GameObject.Find("GameManager").GetComponent<MasterScript>();
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}