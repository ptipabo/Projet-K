using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier les composants des éléments de type GUI
using UnityEngine.EventSystems;//Permet d'utiliser le système d'évènements de Unity

public class InventoryItem : MonoBehaviour, IPointerDownHandler {
	
	private MasterScript masterScript;//Lie ce script au MasterScript
	public int itemId;// Contient l'id de l'objet
	public string itemDescription;// Contient la description de l'objet
	public GameObject initialSlot;// permet de stocker le 1er slot du menu contenant cet objet
	private Vector3 initialSlotPosition;// permet de stocker la position du 1er slot du menu contenant cet objet
	private int itemPosition = 0;// Contient la position de l'objet dans l'inventaire
	private int itemQuantity = 1;// Contient le nombre d'exemplaires de cet objet possédé par le joueur
	private float itemSpace = 80f;// Distance entre chaque item dans l'inventaire
	private Vector3 mousePos;
	private GameObject mainScreen;// Permet de stocker l'objet représentant l'écran
	private bool followMouse = false;// Permet de savoir si l'objet doit accompagner la souris ou non
	
	// Use this for initialization
	void Start () {
		masterScript = GameObject.Find("GameManager").GetComponent<MasterScript>();
		mainScreen = GameObject.Find("MainScreen");// On stock l'objet représentant l'écran
	}
	
	// Update is called once per frame
	void Update () {
		
		if(followMouse == true){// Si l'objet doit suivre la souris...
			mousePos = Input.mousePosition;// On enregistre la position de la souris à chaque frame
			transform.position = new Vector3(mousePos.x+20, mousePos.y-20, transform.position.z);// Puis on applique les mouvements de la souris sur l'objet en le décalant légèrement
		}
		
		if(masterScript.GetObjectChange() == true){
			if(masterScript.GetObjectUsed() != itemId){// Si l'objet en cours d'utilisation n'est pas celui-ci...
				followMouse = false;// On signal à l'objet qu'il ne doit pas/plus suivre la souris
				
				if(initialSlot != null){// Si l'emplacement du slot initial est bien défini...
					initialSlotPosition = initialSlot.transform.position;// On stock la position actuelle du slot initial
					transform.position = new Vector3(initialSlotPosition.x+(itemSpace*itemPosition),initialSlotPosition.y,initialSlotPosition.z);// On replace l'objet à l'endroit où il était dans l'inventaire quand il à été cliqué
				}
				// Si l'objet est censé disparaitre au lieu de revenir dans l'inventaire...
					// On désactive cet objet
					// On déplace d'une case vers la gauche tous les autres objets qui le suivaient dans l'inventaire
			}
		}
	}
	
	public void OnPointerDown(PointerEventData eventData){// Ne doit PAS être appelée dans Update(), Permet de détecter un clique (gauche ou droit) de la souris
		if(Input.GetMouseButtonDown(0)){// S'il s'agit d'un clique gauche...
			masterScript.SetObjectUsed(itemId);
			followMouse = true;
		}
		else if(Input.GetMouseButtonDown(1)){
			Debug.Log(itemDescription);
		}
	}
	
	/*public void OnPointerUp(PointerEventData eventData){
		leftClickActive = false;// On signale au script que le joueur a cliqué sur cet objet
	}*/
	
	public int GetItemPosition(){
		return itemPosition;
	}
		
	public void SetItemPosition(int newPosition){
		itemPosition = newPosition;
	}
	
	//Permet de mettre à jour la position de cet objet
	public void UpdatePosition(){
		transform.position = new Vector3(transform.position.x+(itemPosition*itemSpace),transform.position.y, transform.position.z);
	}
	
	// Permet de connaitre le nombre d'exmplaire de cet objet possédé par le joueur
	public int GetItemQuantity(){
		return itemQuantity;
	}
	
	// Permet d'augmenter le nombre d'exemplaires de cet objet
	public void ChangeItemQuantity(int quantityToAddRemove){
		itemQuantity = itemQuantity+quantityToAddRemove;
	}
}