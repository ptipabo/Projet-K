using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier les composants des éléments de type GUI

//Cette classe permet simplement à un GameObject de possèder un prix, un nom et une quantité
public class Events : MonoBehaviour {
	
	private MasterScript masterScript;//Lie ce script au MasterScript
	private int eventId = 0;// Permet de stocker l'id du hotspot qui à été cliqué
	private int gameProgression = 0;// Permet de stocker l'étape où en est le joueur dans le jeu
	private bool triggerActivated = false;
	private string dialogueContent;// Permet de modifier le texte du personnage
	private int objectUsed = 0;
	private int eventStep = 0;// Permet de diviser chaque évènement en étapes
	private HotSpot hotSpotScript;// Permet de stocker le script du hotspot sur lequel le joueur a cliqué
	private Transform clickedObject;// Contient le dernier objet qui a été cliqué par le joueur
	private GameObject mainScreen;// Permet de stocker l'objet représentant l'écran
	private Transform inventory;// Permet de stocker un des inventaires (objets clés, objets consommables, équipements, orbes, etc...)
	private Transform keyObjectTab;
	
	// Use this for initialization
	void Start () {
		masterScript = this.transform.GetComponent<MasterScript>();
		mainScreen = GameObject.Find("MainScreen");// On stock l'objet représentant l'écran
		keyObjectTab = mainScreen.transform.GetChild(2);
	}
	
	// Update is called once per frame
	void Update () {
		
		if(triggerActivated == true){// Si un évènement est déclenché...
			
			// Evénements de la map A15
			if(eventId == 1){
				if(eventStep == 0){//Le personnage dit quelque chose
					dialogueContent = "Hey! Look what I found!";
					if(masterScript.PlayDialogue(masterScript.GetTextSpeed(), dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
						//Le personange ramasse l'objet
						clickedObject.gameObject.SetActive(false);// On désactive le mesh renderer et le mesh collider du hotspot en question
						inventory = mainScreen.transform.GetChild(2).Find("itemList");// On stock l'inventaire des objets clés
						addItem(inventory, 1, 1);// On ajoute l'os à l'inventaire des objets clés
						if(keyObjectTab.GetChild(1).GetComponent<MenuTab>().GetIsOpen() == false){
							keyObjectTab.GetChild(1).GetComponent<MenuTab>().SetTriggerAnimation(true);
						}
						eventStep++;
					}
				}
				else if(eventStep == 1){//Le personnage dit autre chose
					dialogueContent = "A real bone!_Like in the movies!";
					if(masterScript.PlayDialogue(masterScript.GetTextSpeed(), dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
						//Fin de l'évènement
						eventStep = 0;// On réinitialise le compteur d'étapes
						triggerActivated = false;// On désactive le déclencheur d'évènement
						masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
					}
				}
			}
			else if(eventId == 2){// Hotspot du petit rocher (partie 1)...
				hotSpotScript = clickedObject.transform.GetComponent<HotSpot>();
				if(masterScript.GetObjectUsed() == 1 || objectUsed == 1){// Si l'os est utilisé sur le petit rocher 
					if(eventStep == 0){// On active l'animation de Tempus qui utilise l'os sur le rocher et on déplace légèrement le rocher... 
						Animator smallRockAnim = clickedObject.transform.GetComponent<Animator>();
						smallRockAnim.SetInteger("rockMove",1);// On active la 1ère animation du rocher
						eventStep++;
						objectUsed = masterScript.GetObjectUsed();// On stock l'id de l'objet utilisé
						masterScript.SetObjectUsed(0);// On remet l'id de l'objet utilisé à zéro
					}
					else if(eventStep == 1){// Tempus dit qu'il pense que ca ira comme ça
						dialogueContent = "That should be enough...";
						
						if(masterScript.PlayDialogue(masterScript.GetTextSpeed(), dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
							eventStep++;
						}
					}
					else if(eventStep == 2){// On change l'id d'évènement et le texte du rocher pour qu'il effectue une autre action lors de sa prochaine activation
						hotSpotScript = clickedObject.transform.GetComponent<HotSpot>();
						hotSpotScript.eventId = 3;
						hotSpotScript.rightClickText = "I extracted this rock from the wall…_Don’t ask me why !";
						eventStep = 0;// On réinitialise le compteur d'étapes
						gameProgression = 1;// On enregistre la progression du joueur dans le jeu (s'exécute à la dernière frame de l'évenement)
						
						objectUsed = 0;
						triggerActivated = false;// On désactive le déclencheur d'évènement
						masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
					}
					
					//masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
				}
				else if(objectUsed != 1 && objectUsed != 0){// Si le joueur a utilisé un AUTRE objet...
					masterScript.SetObjectUsed(0);
					dialogueContent = masterScript.GetWrongObjectLine();
					
					if(masterScript.PlayDialogue(masterScript.GetTextSpeed(), dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
						objectUsed = 0;
						triggerActivated = false;// On désactive le déclencheur d'évènement
						masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
					}
				}
				else{// Si le joueur n'a utilisé AUCUN objet
					dialogueContent = "It's stuck_I can't extract it with my bare hands...";
					if(masterScript.PlayDialogue(masterScript.GetTextSpeed(), dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
						triggerActivated = false;// On désactive le déclencheur d'évènement
						masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
					}
				}
			}
			else if(eventId == 3){// Hotspot du petit rocher (partie 2)...
				if(masterScript.GetObjectUsed() != 0){
					objectUsed = masterScript.GetObjectUsed();// On stock l'id de l'objet utilisé
				}
				
				if(objectUsed == 0){
					if(gameProgression == 1){// Si le joueur a décoincé le petit rocher...
						Animator smallRockAnim = clickedObject.transform.GetComponent<Animator>();
						smallRockAnim.SetInteger("rockMove",2);// On active la 2ème animation du rocher
						clickedObject.tag = "Walkable";// On change le Tag de ce hotspot en "Walkable" afin qu'il devienne une plateforme
						gameProgression = 2;// On enregistre la progression du joueur dans le jeu
						triggerActivated = false;// On désactive le déclencheur d'évènement
						masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
					}
					else if(gameProgression == 2){// Si le joueur à déplacé le petit rocher...
						// Si le joueur a fait un clique DROIT sur le rocher...
							// Tempus dit qu'il peut atteindre la liane maintenant
					}
				}
				else{// Si le joueur a utilisé un objet...
					masterScript.SetObjectUsed(0);
					dialogueContent = masterScript.GetWrongObjectLine();
					
					if(masterScript.PlayDialogue(masterScript.GetTextSpeed(), dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
						objectUsed = 0;
						triggerActivated = false;// On désactive le déclencheur d'évènement
						masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
					}
				}
			}
			else if(eventId == 4){// Hotspot de la liane...
				if(masterScript.GetObjectUsed() != 0){
					objectUsed = masterScript.GetObjectUsed();// On stock l'id de l'objet utilisé
				}
				
				if(objectUsed == 0){
					if(gameProgression == 2){// Si le joueur a déplacé le petit rocher...
						Debug.Log("Fin de l'intro point n' click.");
						// On déclenche la cinématique de Tempus qui saute pour attraper la liane et où le mibiot vient chercher Tempus
						// On désactive le mesh renderer et le mesh collider de la liane
						// On ajoute la liane dans l'inventaire du joueur
						// On déplace le rocher bloquant la porte de la grotte
						gameProgression = 3;// On enregistre la progression du joueur dans le jeu
						// On charge la map de l'arène (où se trouve déjà le Démon nabot)
						// On déclenche quelques phrases de Tempus
						// On déclenche son animation d'entrée dans l'arène
						// On déclenche le bruit de foule
						// On déplace le rocher servant de porte à l'arène
						// On déplace la caméra autour de la tête de Tempus
						// La caméra se place directement face au Démon nabot pendant 2 secondes
						// La caméra recule jusqu'à Tempus puis elle va se placer à sa position finale, au dessus de la map
						// On réactive le mode actif du joueur
						// On active le Démon nabot
						// On cloture l'évènement
						
					}
					else if(gameProgression < 2){// Si le joueur n'a PAS ENCORE déplacé le petit rocher...
						// Tempus dit qu'il ne peut pas atteindre la liane
						dialogueContent = "It's too high, I can't reach it_If only I could climb on something...";
						if(masterScript.PlayDialogue(masterScript.GetTextSpeed(), dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
							triggerActivated = false;// On désactive le déclencheur d'évènement
							masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
						}
					}
				}
				else{
					masterScript.SetObjectUsed(0);
					dialogueContent = masterScript.GetWrongObjectLine();
					
					if(masterScript.PlayDialogue(masterScript.GetTextSpeed(), dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
						objectUsed = 0;
						triggerActivated = false;// On désactive le déclencheur d'évènement
						masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
					}
				}
			}
			
			else{// Hostpot inconnu
				Debug.Log("ERREUR : l'évènement '"+eventId+"' ne peut pas s'enclencher car cet eventId est inconnu.");
				triggerActivated = false;// On désactive le déclencheur d'évènement
				masterScript.SetActiveHotspot(false);// On indique au masterscript que l'évènement en cours est terminé (s'exécute à la dernière frame de l'évenement)
			}
		}
	}
	
	// Permet d'ajouter un objet dans un des inventaires du joueur en 1 ou plusieurs exemplaires
	private void addItem(Transform inventory, int itemId, int quantityToAdd){
		InventoryItem itemScript;// permet de stocker le script de l'objet qui sera ajouté
		int activeItems = 0;// permet de compter le nombre d'objets que le joueur possède déjà
		
		for(int i = 0;i<inventory.childCount;i++){// On cherche l'objet que l'on souhaite parmis tous les enfants de cet inventaire (actifs ET inactifs)...
			itemScript = inventory.GetChild(i).transform.GetComponent<InventoryItem>();// On stock le script de chaque enfant afin de connaitre leurs id
			
			if(itemScript.itemId == itemId){// Si l'id correspond à celui recherché...
				
				if(itemScript.gameObject.activeSelf == true){// Si l'objet existe déjà dans cet inventaire...
					itemScript.ChangeItemQuantity(quantityToAdd);// On l'incrémente de la quantité souhaitée
					Debug.Log("Quantité d'os = "+itemScript.GetItemQuantity());
				}
				else{// Si l'objet n'existe PAS dans cet inventaire...
					
					for(int k = 0;k < inventory.childCount;k++){// On cherche tous les enfants actifs de cet inventaire afin de déterminer la position du nouvel objet
						if(inventory.GetChild(k).gameObject.activeSelf == true){// Si l'enfant en cours d'analyse est actif...
							activeItems++;// On incrémente le nombre d'objets que le joueur possède (uniquement les objets actifs donc)
						}
					}
					
					itemScript.gameObject.SetActive(true);// On active le nouvel objet
					itemScript.SetItemPosition(activeItems);// On indique à l'objet qu'il devra se placer en dernière position dans cet inventaire
					itemScript.UpdatePosition();// On enclenche le changement de position
				}
			}
		}
	}
	
	public void SetEventId(int newValue){
		eventId = newValue;
	}
	
	public void SetObjectUsed(int idObjectUsed){
		objectUsed = idObjectUsed;
	}
	
	public void SetTrigger(bool triggerValue){
		triggerActivated = triggerValue;
	}
	
	public void SetClickedObject(Transform newObject){
		clickedObject = newObject;
	}
}