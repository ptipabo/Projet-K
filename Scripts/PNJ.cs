using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier le champ "Text" d'un composant Text

public class PNJ : MonoBehaviour {

	private MasterScriptOld functions;//Lie ce script au MasterScriptOld
	public int characterType;//0 ou autre = basique, 1 = cadeau, 2 = échange
	public string[] dialogList = new string[1];//Liste des phrases et questions que devra dire ce personnage
	public int[] partitions = new int[1];//Nombre de phrases à dire dans chaque partie de la conversation
	public int hitBoxDistance;//La distance à laquelle le script est pris en compte par rapport au personnage principal
	public int faceId;//Id du personnage permettant d'afficher sa photo lors des conversations (s'il s'agit d'un personnage lambda, la valeur est égale à zéro)
	private Vector3 pnjPosition;//Position actuelle du PNJ
	private Vector3 playerPosition;//Position actuelle du joueur
	private Vector3 distancePnjPlayer;//Distance actuelle entre le joueur et le PNJ
	private int dialogProgression = 0;//Permet au script de savoir à quel moment de la conversation il se trouve
	private bool sendDetection = true;//Permet de prévenir le MasterScriptOld que le joueur à été détecté près du PNJ
	private bool itemTrigger = false;//Empèche le script de continuer tant qu'une autre action n'est pas terminée
	private bool itemGiven = false;//Signal au script que l'objet du PNJ à été donné ou non
	private bool eventTriggered = false;//Signal au script s'il à déjà été activé une fois ou pas
	private GameObject screenManager;//Lie ce script à l'objet "Screen"
	private Transform noFaceWindow;//Lie ce script à la fenêtre de dialogue sans visage
	private Transform faceWindow;//Lie ce script à la fenêtre de dialogue avec visage
	private bool mouseTrigger = false;//Permet de déclencher un dialogue via la souris
	private bool activateDialogue = false;//Permet de rassembler tous les moyens de déclencher/continuer un dialogue dans 1 seule variable
	
	// Use this for initialization
	void Start () {
		functions = GameObject.Find("GameManager").GetComponent<MasterScriptOld>();
		screenManager = GameObject.Find("Screen");
		noFaceWindow = screenManager.transform.GetChild(1).GetChild(1);
		faceWindow = screenManager.transform.GetChild(1).GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {
		if(functions.GetPauseMode() == false){
			pnjPosition = this.transform.position;
			playerPosition = GameObject.Find("Tempus").transform.position;
			distancePnjPlayer = functions.DistanceBetween(playerPosition,pnjPosition);
			
			//Si le joueur est proche du PNJ
			if(distancePnjPlayer.x < hitBoxDistance && distancePnjPlayer.y < hitBoxDistance && distancePnjPlayer.z < hitBoxDistance){
				if(sendDetection == true){
					functions.playerDetection(this.name);
					sendDetection = false;
				}
				if(functions.CheckIfNearest(this.name) == true){
					this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
					//Debug.Log(functions.GetDialogueMode());
					
					if(Input.GetKeyDown(KeyCode.Return) || mouseTrigger == true){
						activateDialogue = true;
					}
					else if(functions.GetDialogueMode() == true && Input.GetMouseButtonDown(0) == true){
						activateDialogue = true;
					}
					
					if(activateDialogue == true && itemTrigger == false){
						activateDialogue = false;
						mouseTrigger = false;
						//Debug.Log(functions.GetFrame());
						if(functions.GetDialogueMode() == false){
							functions.SetDialogueMode(true);
						}
						
						if(characterType == 1){//Si le type de ce personnage est 1, le personnage dit entre 1 et plusieurs phrases puis donne quelque chose au héro et enfin dit entre 1 et plusieurs phrases pour conclure la conversation
							if(eventTriggered == false){
								if(dialogProgression < partitions[0]+partitions[1]){
									if(dialogProgression < partitions[0]){//Le PNJ dit quelque chose
										if(faceId == 0){
											screenManager.GetComponent<ScreenManager>().SetActiveDialogue(2);
											noFaceWindow.gameObject.SetActive(true);
											noFaceWindow.GetChild(0).GetComponent<Text>().text = this.name;
											noFaceWindow.GetChild(1).GetComponent<Text>().text = dialogList[dialogProgression];
										}
										else{
											screenManager.GetComponent<ScreenManager>().SetActiveDialogue(1);
											faceWindow.gameObject.SetActive(true);
											faceWindow.GetChild(0).GetComponent<Text>().text = this.name;
											faceWindow.GetChild(1).GetComponent<Text>().text = dialogList[dialogProgression];
										}
										dialogProgression++;
									}
									else{//Le PNJ donne un objet
										if(itemGiven == false){
											itemTrigger = true;
											screenManager.GetComponent<ScreenManager>().SetActiveDialogue(0);
										}
										else{//Le PNJ dit quelque chose
											if(dialogProgression < partitions[0]+partitions[1]){
												if(faceId == 0){
													screenManager.GetComponent<ScreenManager>().SetActiveDialogue(2);
													noFaceWindow.GetChild(1).GetComponent<Text>().text = dialogList[dialogProgression];
												}
												else{
													screenManager.GetComponent<ScreenManager>().SetActiveDialogue(1);
													faceWindow.GetChild(1).GetComponent<Text>().text = dialogList[dialogProgression];
												}
												dialogProgression++;
											}
										}
									}
								}
								else{//Fin de conversation
									screenManager.GetComponent<ScreenManager>().SetActiveDialogue(0);
									if(faceId == 0){
										noFaceWindow.GetChild(0).GetComponent<Text>().text = "Name";
										noFaceWindow.GetChild(1).GetComponent<Text>().text = "Text";
									}
									else{
										faceWindow.GetChild(0).GetComponent<Text>().text = "Name";
										faceWindow.GetChild(1).GetComponent<Text>().text = "Text";
									}
									functions.SetNoLoop(true);
									functions.SetDialogueMode(false);
									eventTriggered = true;
								}
							}
							else{//Le PNJ dit quelque chose
								if(dialogProgression < partitions[0]+partitions[1]+partitions[2]){
									if(faceId == 0){
										screenManager.GetComponent<ScreenManager>().SetActiveDialogue(2);
										noFaceWindow.gameObject.SetActive(true);
										noFaceWindow.GetChild(0).GetComponent<Text>().text = this.name;
										noFaceWindow.GetChild(1).GetComponent<Text>().text = dialogList[dialogProgression];
									}
									else{
										screenManager.GetComponent<ScreenManager>().SetActiveDialogue(1);
										faceWindow.gameObject.SetActive(true);
										faceWindow.GetChild(0).GetComponent<Text>().text = this.name;
										faceWindow.GetChild(1).GetComponent<Text>().text = dialogList[dialogProgression];
									}
									dialogProgression++;
								}
								else{//Fin de conversation
									screenManager.GetComponent<ScreenManager>().SetActiveDialogue(0);
									if(faceId == 0){
										noFaceWindow.GetChild(0).GetComponent<Text>().text = "Name";
										noFaceWindow.GetChild(1).GetComponent<Text>().text = "Text";
									}
									else{
										faceWindow.GetChild(0).GetComponent<Text>().text = "Name";
										faceWindow.GetChild(1).GetComponent<Text>().text = "Text";
									}
									functions.SetNoLoop(true);
									functions.SetDialogueMode(false);
									dialogProgression = partitions[0]+partitions[1];
								}
							}
						}
						else if(characterType == 2){//Si le type de ce personnage est 2, le personnage pose une question (en 1 ou plusieurs phrases) puis attend la réponse du joueur. Enfin, selon la réponse, il dit quelques phrases et ne fait plus rien.
							
						}
						else{//Si le type de ce personnage est 0 ou autre, le personnage dit entre 1 et plusieurs phrases puis ne fait plus rien
							if(dialogProgression < partitions[0]){
								
								if(faceId == 0){
									screenManager.GetComponent<ScreenManager>().SetActiveDialogue(2);
									noFaceWindow.GetChild(0).GetComponent<Text>().text = this.name;
									noFaceWindow.GetChild(1).GetComponent<Text>().text = dialogList[dialogProgression];
								}
								else{
									screenManager.GetComponent<ScreenManager>().SetActiveDialogue(1);
									faceWindow.GetChild(0).GetComponent<Text>().text = this.name;
									faceWindow.GetChild(1).GetComponent<Text>().text = dialogList[dialogProgression];
								}
								dialogProgression++;
							}
							else{//Fin de conversation
								screenManager.GetComponent<ScreenManager>().SetActiveDialogue(0);
								if(faceId == 0){
									noFaceWindow.GetChild(0).GetComponent<Text>().text = "Name";
									noFaceWindow.GetChild(1).GetComponent<Text>().text = "Text";
								}
								else{
									faceWindow.GetChild(0).GetComponent<Text>().text = "Name";
									faceWindow.GetChild(1).GetComponent<Text>().text = "Text";
								}
								functions.SetNoLoop(true);
								functions.SetDialogueMode(false);
								dialogProgression = 0;
							}
						}
					}
				}
				else{
					this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
				}
			}
			else{
				this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
				sendDetection = true;
			}
		}
	}
	
	//Permet au MasterScriptOld de savoir si le joueur est toujours proche de cet objet
	public bool isPlayerAround(){
		playerPosition = GameObject.Find("Tempus").transform.position;
		distancePnjPlayer = functions.DistanceBetween(playerPosition,pnjPosition);
		if(distancePnjPlayer.x < hitBoxDistance && distancePnjPlayer.y < hitBoxDistance && distancePnjPlayer.z < hitBoxDistance){
			return true;
		}
		else{
			return false;
		}
	}
	
	//Permet d'activer ou désactiver la variable "itemTrigger" depuis un autre script
	public void SetItemTrigger(bool newValue){
		itemTrigger = newValue;
		return;
	}
	
	//Permet de récupérer la valeur de la variable "itemTrigger"
	public bool GetItemTrigger(){
		return itemTrigger;
	}
	
	//Permet d'activer ou désactiver la variable "itemGiven" depuis un autre script
	public void SetItemGiven(bool newValue){
		itemGiven = newValue;
		return;
	}
	
	//Permet d'activer ce script via la souris plutôt qu'au clavier
	public void SetMouseTrigger(bool newValue){
		if(sendDetection == false){
			mouseTrigger = newValue;
		}
		return;
	}
}