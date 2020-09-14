using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeItem : MonoBehaviour {
	
	private MasterScriptOld functions;//Lie ce script au MasterScriptOld
	public int objectType;//0 ou autre = objet à ramasser, 1 = cadeau d'un PNJ, 2 = coffre
	public GameObject inventory;//Lie ce script à l'inventaire
	public int idElement;//Indique au script à quel objet il correspond dans l'inventaire
	public int quantity;//Le nombre d'exemplaire de cet objet que reçoit le joueur si ce script est activé
	public int hitBoxDistance;//La distance à laquelle le script est pris en compte par rapport au personnage principal
	private GameObject screenCanvas;//Lie ce script à l'objet Screen
	private ScreenManager screenManager;//Lie cet objet au ScreenManager
	private Transform linkedItem;//Lie ce script à l'objet qu'il lui correspond dans l'inventaire
	private Vector3 itemPosition;//Position actuelle de l'objet (uniquement pour types 0 et 2)
	private Vector3 playerPosition;//Position actuelle du joueur (uniquement pour types 0 et 2)
	private Vector3 distanceItemPlayer;//Distance actuelle entre le joueur et l'objet (uniquement pour types 0 et 2)
	private PNJ pnjScript;//Lie cet objet au PNJ qui le possède (uniquement pour type 1)
	private int objectsPossessed;//Permet de connaitre le nombre d'objet que possède actuellement le joueur
	private int currentQuantity;//Permet de savoir combien d'exemplaires de cet objet le joueur possède actuellement
	private bool mouseTrigger = false;//Permet de déclencher le script via la souris
	private bool playerDetected = false;//Permet de savoir si le joueur est proche de cet objet ou non
	
	// Use this for initialization
	void Start () {
		functions = GameObject.Find("GameManager").GetComponent<MasterScriptOld>();
		screenCanvas = GameObject.Find("Screen");
		
		if(objectType == 1){
			pnjScript = this.GetComponent<PNJ>();
		}
		else{
			itemPosition = this.transform.position;
		}
		screenManager = screenCanvas.GetComponent<ScreenManager>();
		linkedItem = screenCanvas.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(functions.GetChildId(idElement));
	}
	
	// Update is called once per frame
	void Update () {
		if(objectType == 1){
			if(pnjScript.GetItemTrigger() == true){
				functions.AddItem(idElement,quantity);

				pnjScript.SetItemTrigger(false);
				pnjScript.SetItemGiven(true);
			}
		}
		else{
			//Si l'objet est visible
			if(this.GetComponent<MeshRenderer>().enabled == true){
				playerPosition = GameObject.Find("Tempus").transform.position;
				distanceItemPlayer = functions.DistanceBetween(playerPosition,itemPosition);
				//Si le joueur est proche de l'objet
				if(distanceItemPlayer.x < hitBoxDistance && distanceItemPlayer.y < hitBoxDistance && distanceItemPlayer.z < hitBoxDistance){
					playerDetected = true;
					this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
					if(Input.GetKeyDown(KeyCode.Return) || mouseTrigger == true){
						mouseTrigger = false;
						//Le Mesh renderer de l’objet 3d est désactivé
						this.GetComponent<MeshRenderer>().enabled = false;
						this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
						//On ajoute l'objet à l'invenaire du joueur et on le lui signal dans la fen^tre de dialogue centrale
						functions.AddItem(idElement,quantity);
						functions.SetCentralWindow(true, "Received "+quantity+" "+linkedItem.GetComponent<ItemOld>().GetName());
						functions.SetDialogueMode(true);
					}
				}
				else{
					this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
				}
			}//Si le mode dialogue est activé ici, cela signifie que le joueur vient de recevoir un objet et le jeu attend simplement que le joueur ferme la fenêtre centrale
			else if(functions.GetDialogueMode() == true){
				if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0) == true){
					functions.SetCentralWindow(false);
					functions.SetDialogueMode(false);
				}
			}
		}
	}
	
	
	//Fonctions du script FreeItem :
		
	//Permet d'activer ce script via la souris plutôt qu'au clavier
	public void SetMouseTrigger(bool newValue){
		if(playerDetected == true){
			mouseTrigger = newValue;
		}
		return;
	}
}
