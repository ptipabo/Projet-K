using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier le champ "Text" d'un composant Text

public class ScreenManager : MonoBehaviour {
	
	private MasterScriptOld functions;//Lie ce script au MasterScriptOld
	private float menuSpeed = 20.0f;//Vitesse d'apparition et de disparition du menu
	private Transform mainMenu;//Objet présent dans le jeu
	private Transform underMenuImage;//Image d'arrière-plan du sous-menu
	private float BGrotation = 0;//Angle de rotation actuel de l'arrière plan du sous-menu
	private float rotationSpeed = 2;//Vitesse de rotation de l'arrière plan du sous-menu
	private Transform inventory;//Lie ce script à l'inventaire
	private Transform dialogue;//Lie ce script à l'objet "Dialogue"
	private bool menuOpened = false;//Indique au script si le menu principal est ouvert on non
	private int idUnderMenu = 0;//id du sous menu en cours d'utilisation (0 = menu principal)
	private int idMenuAnimation = 0;//id de l'animation de menu à effectuer
	private Vector3 menuStartPosition;//Enregistre la position de départ du menu principal afin qu'il y retourne lorsqu'il est fermé
	private Vector3 menuCurrentPosition;//Enregistre la position actuelle du menu principal (à chaque frame)
	private Vector3 underMenuPosition;//Position exacte du sous-menu une fois ouvert
	private Vector3 mainMenuPosition;//Position exacte du menu principal une fois ouvert
	private Vector3 screenCenter;//Permet de baser les déplacement du menu d'après le centre de l'écran
	private Vector3 menuStartSize;//Enrigistre la taille de départ du menu principal afin qu'il la reprenne lorsque les sous-menus sont fermés
	private Vector3 menuCurrentSize;//Enregistre la taille actuelle du menu principal (à chaque frame)
	private float zoomMenu = 2.452f;//Intensité du zoom/dézoom sur le menu principal lorsqu'un sous-menu s'ouvre ou se ferme
	private float zoomSpeed = 6.0f;//Vitesse du zoom/dézoom lorsqu'un sous-menu s'ouvre ou se ferme
	private int itemNumber = 0;//Nombre d'objet possédés par le joueur
	private int activeDialogue = 0;//Signal au script s'il doit ouvrir la fenêtre de dialogue et si oui, laquelle (0 = fermée, 1 = fenêtre avec visage, 2 = fenêtre sans visage)
	private int answerWindow = 0;
	private int optionsNumber = 0;//Nombre d'options de réponses possible dans une fenêtre de dialogue dynamique(expérimental)
	private bool windowTopLeft = false;//Indique au script si la fenêtre de dialogue haut gauche est ouverte ou non
	private Transform centralWindow;//Lie ce script à la fenêtre de dialogue centrale
	
	// Use this for initialization
	void Start () {
		functions = GameObject.Find("GameManager").GetComponent<MasterScriptOld>();
		mainMenu = transform.GetChild(0);
		inventory = transform.GetChild(0).Find("UnderMenu").GetChild(1);
		dialogue = transform.GetChild(1);
		centralWindow = dialogue.GetChild(4);
		menuStartSize = mainMenu.localScale;
		underMenuImage = mainMenu.GetChild(0).GetChild(0);
		menuStartPosition = mainMenu.position;
	}
	
	// Update is called once per frame
	void Update () {
		menuCurrentPosition = mainMenu.position;
		menuCurrentSize = mainMenu.localScale;
		underMenuImage.rotation = Quaternion.Euler(0, 0, BGrotation);
		
		//Fait simplement tourner l'arrière plan du sous-menu
		if(BGrotation >= 360){
			BGrotation = 0;
		}
		else{
			BGrotation = BGrotation+rotationSpeed;
		}
		
		if(functions.GetDialogueMode() == false){
			//Grace a la touche Escape, on peut ouvrir et fermer le menu principal et également fermer les sous-menus
			if(Input.GetKeyDown(KeyCode.Escape)){
				//On vérifie tout d'abord si un menu est ouvert ou non
				if(menuOpened == true){//si oui, s'agit-il du menu principal ou non
					if(idUnderMenu == 0){//S'il s'agit du menu principal, on le ferme tout simplement
						menuOpened = false;
						functions.SetPauseMode(false);
						idMenuAnimation = 2;
					}
					else{//S'il s'agit d'un sous-menu, on le ferme et on revient au menu principal
						idMenuAnimation = 4;
						idUnderMenu = 0;
					}
				}
				else{//Sinon on ouvre simplement le menu principal
					menuOpened = true;
					mainMenu.gameObject.SetActive(menuOpened);
					functions.SetPauseMode(true);
					idMenuAnimation = 1;
				}
			}
		}
		
		//Animations d'ouverture et de fermeture du menu principal et des sous-menus (4 animations en tout)
		if(idMenuAnimation == 1){//Ouverture du menu principal
			
			BGrotation = -90;
			screenCenter = new Vector3(Screen.width/2,Screen.height/2,0);
			mainMenuPosition = new Vector3(menuStartPosition.x, screenCenter.y, menuStartPosition.z);//Position exacte du menu principal calculée automatiquement d'après la taille de l'écran
			
			functions.MoveAt(mainMenu, mainMenuPosition, menuSpeed);
			
			if(menuCurrentPosition.y == mainMenuPosition.y && menuCurrentPosition.x == mainMenuPosition.x){
				idMenuAnimation = 0;
			}
		}
		else if(idMenuAnimation == 2){//Fermeture du menu principal
			
			functions.MoveAt(mainMenu, menuStartPosition, menuSpeed);
			
			if(menuCurrentPosition.y == menuStartPosition.y && menuCurrentPosition.x == menuStartPosition.x){
				menuOpened = false;
				mainMenu.gameObject.SetActive(menuOpened);
				idMenuAnimation = 0;
			}
		}
		else if(idMenuAnimation == 3){//Ouverture d'un sous-menu
			underMenuPosition = new Vector3(mainMenuPosition.x+(Screen.width/5.0f), mainMenuPosition.y, mainMenuPosition.z);//Position exacte du sous-menu calculée automatiquement d'après la taille de l'écran
			if(underMenuPosition.x != menuCurrentPosition.x || underMenuPosition.y != menuCurrentPosition.y || menuCurrentSize.x < menuStartSize.x+zoomMenu){//Si les positions X et Y actuelles du menu ne sont pas les mêmes que les positions X et Y du sous-menu
				functions.MoveAt(mainMenu, underMenuPosition, menuSpeed);
				functions.Scaling(mainMenu, zoomSpeed, zoomMenu, zoomMenu);
			}
			else{
				idMenuAnimation = 0;
			}
		}
		else if(idMenuAnimation == 4){//Fermeture d'un sous-menu
			float dezoomMenu = zoomMenu*-1;
			if(menuCurrentPosition.x != mainMenuPosition.x || menuCurrentPosition.y != mainMenuPosition.y || menuCurrentSize.x > menuStartSize.x){
				functions.MoveAt(mainMenu, mainMenuPosition, menuSpeed);
				functions.Scaling(mainMenu, zoomSpeed, dezoomMenu, dezoomMenu);
			}
			else{
				idMenuAnimation = 0;
			}
		}
		
		if(idUnderMenu == 1){
			//Debug.Log("Magic menu activated!");
		}
		else if(idUnderMenu == 2){
			inventory.gameObject.SetActive(true);
			//Debug.Log("Inventory menu activated!");
		}
		else if(idUnderMenu == 3){
			//Debug.Log("Equipment menu activated!");
		}
		else if(idUnderMenu == 4){
			//Debug.Log("Tactics menu activated!");
		}
		else if(idUnderMenu == 5){
			//Debug.Log("Save menu activated!");
		}
		else if(idUnderMenu == 6){
			//Debug.Log("Options menu activated!");
		}
		else if(idUnderMenu == 7){
			//Debug.Log("Status menu activated!");
		}
		else{
			inventory.gameObject.SetActive(false);
		}
		
		//Active ou désactive la fenêtre de dialogue
		if(activeDialogue == 1){
			dialogue.GetChild(0).gameObject.SetActive(true);
		}
		else if(activeDialogue == 2){
			dialogue.GetChild(1).gameObject.SetActive(true);
		}
		else{
			dialogue.GetChild(0).gameObject.SetActive(false);
			dialogue.GetChild(1).gameObject.SetActive(false);
		}
		
		//Active ou désactive la fenêtre de réponse du personnage
		if(answerWindow == 1){
			dialogue.GetChild(2).gameObject.SetActive(true);
		}
		else if(answerWindow == 2){
			dialogue.GetChild(3).gameObject.SetActive(true);
		}
		else{
			dialogue.GetChild(2).gameObject.SetActive(false);
			dialogue.GetChild(3).gameObject.SetActive(false);
		}
	}
	
	
	//Fonctions du ScriptManager :
	
	public void SetIdUnderMenu(int newValue){
		idUnderMenu = newValue;
		return;
	}
	
	public int GetIdUnderMenu(){
		return idUnderMenu;
	}
	
	public void SetIdMenuAnimation(int newValue){
		idMenuAnimation = newValue;
		return;
	}
	
	public void SetItemNumber(int newValue){
		itemNumber = newValue;
		return;
	}
	
	public int CountItems(){
		return itemNumber;
	}
	
	public void SetActiveDialogue(int newValue){
		activeDialogue = newValue;
		return;
	}
	
	public void SetAnswerWindow(int newValue){
		answerWindow = newValue;
		return;
	}
	
	public void SetWindowTopLeft(bool isOpened, int linesNumber){
		windowTopLeft = isOpened;
		optionsNumber = linesNumber;
		return;
	}
}
