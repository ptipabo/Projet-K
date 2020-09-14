using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier les composants des éléments de type GUI

public class MasterScript : MonoBehaviour {
	
	// Variables
	// Informations générales
	private Events eventsScript;// Permet de stocker le script des étapes clés du jeu
	private int frameCounter = 0; // Permet d'indique le numéro de la frame actuelle
	private Camera mainCamera;
	//public Camera cameraTwo;
	//private bool fightMap = false; // Permet d'indiquer au script s'il s'agit d'une map de combat ou non
	private float gravity = 0.15f;// Permet de définir la force de gravité dans le jeu
	private int textSpeed = 35;// Permet de définir la vitesse de défilement des dialogues dans le jeu
	private int waitFrame = 0;
	private bool initDialFunction = true; // on initialise la variable qui permettra d'afficher la fenêtre de dialogue
	
	//Modification du curseur selon l'objet survolé
	/*public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;*/
	
	// Gestion de l'interface 2D
	private GameObject mainScreen;// Permet de stocker l'objet représentant l'écran
	private Transform subtitlesWindow;// Permet de stocker la fenêtre des sous-titres
	private Transform subtitlesText;// Permet de stocker le texte des sous-titres
	private Transform characterDialogueWindow;// Permet de stocker la fenêtre réservée aux dialogues
	private Transform characterText;// Permet de stocker le texte du personnage
	
	// Détection des cliques
	private Vector3 destinationPoint;// Sert à stocker le point de destination du personnage lors d'un clique gauche de la souris
	private bool clickOnGround = false;// Indique au script si le joueur à cliqué sur le sol ou non
	private bool clickOnHotSpot = false;// Indique au script si le joueur à cliqué ou non sur un objet ou une zone interactive
	private Vector3 objectPosition;// Indique au script à quel endroit se situe un objet cliqué
	private Ray mouseRay;// Permet de créer un rayon depuis la souris
	private RaycastHit mouseHitInfo;// Permet de stocker les informations concernant l'impact créé par le rayon de la souris
	private string lastActiveMouseButton = "none";// Permet au script de savoir quel bouton à été cliqué la dernière fois
	private int previousClickFrame = 0;// Permet d'enregister le numéro de la frame à laquelle le joueur à cliqué la dernière fois
	private int doubleClickDelay = 10;// Délai d'attente maximal autorisé entre 2 cliques pour être considéré comme un double-clique
	
	// Déplacements des personnages
	private GameObject defaultCharacter; // Permet de stocker le personnage controlé par défaut (Tempus en l'occurence)
	private GameObject charObject; // Permet de stocker le personnage à déplacer
	private Transform charTransform;// Permet de stocker le gizmo du personnage à déplacer
	private Vector3 charPosition;// Permet de stocker la position du personnage à déplacer
	private float characterSpeed; // Permet de stocker la vitesse du personnage controlé
	private float runningAccel = 3.0f;// Permet de définir l'accélération du personnage lorsqu'il court
	private Vector3 characterForwardAxis; // Permet de stocker la direction du personnage controlé
	private Transform obstacleDetectorLow;// Permet de stocker le détecteur d'obstacles bas du personnage controlé
	private Transform obstacleDetectorMiddle;// Permet de stocker le détecteur d'obstacles moyens du personnage controlé
	private Transform obstacleDetectorHigh;// Permet de stocker le détecteur d'obstacles hauts du personnage controlé
	private float detectorRadius = 3f;// Indique au script la longueur des rayons de détection d'obstacles
	private RaycastHit objectHitLow;// Permet de stocker les informations concernant l'objet touché par le détecteur d'obstacles bas
	private RaycastHit objectHitMiddle;// Permet de stocker les informations concernant l'objet touché par le détecteur d'obstacles middle
	private RaycastHit objectHitHigh;// Permet de stocker les informations concernant l'objet touché par le détecteur d'obstacles hauts
	private Transform directionTarget;// Permet de stocker la cible servant à diriger un personnage
	private Vector3 fixedSpace;// Permet de stocker la distance exacte entre un personnage et sa cible de direction
	private Vector3 remainingDistance;// Permet de stocker la distance restante entre le personnage et sa destination en cours
	private float objectRange;// Défini le rayon empèchant le personnage de marcher sur un objet interactif
	private float charCorpulence;// Permet de stocker la corpulence du personnage contrôlé
	private Animator characterAnim;// Permet de stocker l'animateur du personnage contrôlé
	
	//Gestion d'objets
	private Transform clickedObject;// Contient le dernier objet qui a été cliqué par le joueur
	private HotSpot hotSpotScript;// Permet de stocker le script du hotspot sur lequel le joueur a cliqué
	private int eventId = 0;
	private int itemId = 0;
	private int objectHeld = 0;
	private int objectUsed = 0;// Permet de stocker l'id de l'objet utilisé sur un hotspot
	private bool objectChange = false;// Permet de savoir s'il y a eu un changement d'objet attaché à la souris
	
	// Gestion des dialogues
	private string dialogueContent;
	private string[] textList;
	private int phraseCounter = 0;
	private int selectedLine;
	private string[] wrongObjectLines = new string[]{"Nope!", "I don't think so...", "Hmm..._Maybe another time", "Why would I do that?"};
	
	// Gestion des hotspots et menus
	private bool activeHotspot = false;
	private bool activeMenu = false;
	private Transform inventory;// Permet de stocker un des inventaires (objets clés, objets consommables, équipements, orbes, etc...)
	
	
	// Use this for initialization
	void Start () {
		eventsScript = this.transform.GetComponent<Events>();
		mainCamera = Camera.main;
		defaultCharacter = GameObject.Find("Tempus");// On stock l'objet Tempus dans la variable "defaultCharacter"
		charObject = defaultCharacter;// On indique au script qu'à l'initialisation de la map, le personnage controlé est le personnage par défaut
		charTransform = charObject.transform;// On stock le Gizmo du personnage controlé
		charPosition = charTransform.position;// On stock la position de départ du personnage controlé
		obstacleDetectorLow = charTransform.Find("ObstacleDetectorLow");// On stock le détecteur d'obstacles bas
		obstacleDetectorMiddle = charTransform.Find("ObstacleDetectorMiddle");// On stock le détecteur d'obstacles moyens
		obstacleDetectorHigh = charTransform.Find("ObstacleDetectorHigh");// On stock le détecteur d'obstacles hauts
		characterForwardAxis = charTransform.TransformDirection(Vector3.forward);// On stock la direction "devant" du personnage controlé
		destinationPoint.x = charPosition.x;// On s'assure que le personnage ne bouge pas en x à l'initialisation de la map
		destinationPoint.z = charPosition.z;// On s'assure que le personnage ne bouge pas en z à l'initialisation de la map
		directionTarget = GameObject.Find("DirectionTarget").transform;// On stock le Gizmo de la cible de direction du personnage
		mainScreen = GameObject.Find("MainScreen");// On stock l'objet représentant l'écran
		subtitlesWindow = mainScreen.transform.GetChild(0);// On stock la fenêtre des sous-titres
		subtitlesText = subtitlesWindow.Find("SubtitlesText");// On stock le texte des sous-titres
		characterDialogueWindow = mainScreen.transform.GetChild(1);// On stock la fenêtre réservée aux dialogues
		characterText = characterDialogueWindow.Find("CharacterText");// On stock le texte du personnage
		characterDialogueWindow.gameObject.SetActive(false);// On s'assure que la fenêtre réservée aux dialogues est inactive de base
		charCorpulence = charObject.GetComponent<Character>().corpulence;// On stock la tolérance de position du personnage controlé (afin d'éviter les conflits entre le character controller et le script de déplacements)
		characterAnim = defaultCharacter.GetComponent<Animator>();// On stock l'animateur du personnage contrôlé
		characterSpeed = charObject.GetComponent<Character>().speed;// A chaque frame, on stock la vitesse de base du personnage controlé
	}
	
	// Update is called once per frame
	void Update () {
		frameCounter++;// On compte sans arrêt le nombre de frames qui passent
		objectChange = false;// On réinitialise cette variable à chaque frame pour que le système de repositionnement automatique des objets s'execute correctement
		//Debug.Log("Frame "+frameCounter);
		
		charPosition = charTransform.position;// A chaque frame, on stock la position actuelle du personnage controlé
		
		//On affiche les rayons de détection (invisibles dans le jeu)
		Debug.DrawRay(obstacleDetectorLow.position, obstacleDetectorLow.TransformDirection(Vector3.forward)*detectorRadius, Color.green);
		Debug.DrawRay(obstacleDetectorLow.position, obstacleDetectorLow.TransformDirection(Vector3.left)*detectorRadius, Color.green);
		Debug.DrawRay(obstacleDetectorLow.position, obstacleDetectorLow.TransformDirection(Vector3.right)*detectorRadius, Color.green);
		Debug.DrawRay(obstacleDetectorMiddle.position, obstacleDetectorMiddle.TransformDirection(Vector3.forward) * (detectorRadius*2), Color.yellow);
		Debug.DrawRay(obstacleDetectorMiddle.position, obstacleDetectorMiddle.TransformDirection(Vector3.right) * (detectorRadius*2), Color.yellow);
		Debug.DrawRay(obstacleDetectorMiddle.position, obstacleDetectorMiddle.TransformDirection(Vector3.left) * (detectorRadius*2), Color.yellow);
		Debug.DrawRay(obstacleDetectorHigh.position, obstacleDetectorHigh.TransformDirection(Vector3.forward) * detectorRadius, Color.red);
		Debug.DrawRay(obstacleDetectorHigh.position, obstacleDetectorHigh.TransformDirection(Vector3.right) * detectorRadius, Color.red);
		Debug.DrawRay(obstacleDetectorHigh.position, obstacleDetectorHigh.TransformDirection(Vector3.left) * detectorRadius, Color.red);
		
		/*//Gestion des détecteurs d'obstacles du personnage controlé
		if(Physics.Raycast(obstacleDetectorLow.position, obstacleDetectorLow.TransformDirection(Vector3.forward), out objectHitLow, detectorRadius) == true){// Si le détecteur d'obstacles bas frontal détecte quelque chose...
			if(Physics.Raycast(obstacleDetectorMiddle.position, obstacleDetectorMiddle.TransformDirection(Vector3.forward), out objectHitMiddle, (detectorRadius*2)) == false){// Si le détecteur d'obstacles moyens frontal ne détecte rien...
				// Le personnage saute sur l'obstacle
				Debug.Log("Attention à la marche droit devant!");
			}
			else{
				// Le personnage ne peut pas aller tout droit, il regarde alors à sa droite
				if(Physics.Raycast(obstacleDetectorLow.position, obstacleDetectorMiddle.TransformDirection(Vector3.right), out objectHitLow, (detectorRadius*2)) == true){// Si le détecteur d'obstacles bas droit ne détecte rien...
					// Le personnage contourne l'obstacle par la droite
					if(Physics.Raycast(obstacleDetectorMiddle.position, obstacleDetectorMiddle.TransformDirection(Vector3.right), out objectHitMiddle, (detectorRadius*2)) == false){// Si le détecteur d'obstacles moyens droit ne détecte rien...
						// Le personnage saute sur l'obstacle à droite
						Debug.Log("Attention à la marche à droite!");
					}
					else{
						// Le personnage ne peut pas aller à droite non-plus, il regarde alors à sa gauche
						if(Physics.Raycast(obstacleDetectorLow.position, obstacleDetectorMiddle.TransformDirection(Vector3.left), out objectHitLow, (detectorRadius*2)) == true){// Si le détecteur d'obstacles bas droit ne détecte rien...
							if(Physics.Raycast(obstacleDetectorMiddle.position, obstacleDetectorMiddle.TransformDirection(Vector3.right), out objectHitMiddle, (detectorRadius*2)) == false){// Si le détecteur d'obstacles moyens droit ne détecte rien...
								// Le personnage saute sur l'obstacle à gauche
								Debug.Log("Attention à la marche à gauche!");
							}
							else{
								// Le personnage ne peut passer nul part, il fait donc demi-tour
								Debug.Log("Impossible de passer, demi-tour!!!");
							}
						}
						else{
							// Le personnage peut contourner l'obstacle par la gauche sans problème
							Debug.Log("Rien à gauche? Parfait!");
						}
					}
				}
				else{
					// Le personnage peut contourner l'obstacle par la droite sans problème
					Debug.Log("Rien à droite? Parfait!");
				}
			}
		}
		
		if(Physics.Raycast(obstacleDetectorHigh.position, obstacleDetectorHigh.TransformDirection(Vector3.forward), out objectHitHigh, detectorRadius) == true){// Si le détecteur d'obstacles hauts détecte quelque chose...
			if(Physics.Raycast(obstacleDetectorLow.position, obstacleDetectorLow.TransformDirection(Vector3.forward), out objectHitLow, detectorRadius) == false){// Si le détecteur d'obstacles bas ne détecte rien...
				Debug.Log("Attention à la tête droit devant!");
			}
		}
		else if(Physics.Raycast(obstacleDetectorHigh.position, obstacleDetectorHigh.TransformDirection(Vector3.right), out objectHitHigh, detectorRadius) == true){// Si le détecteur d'obstacles hauts détecte quelque chose...
			if(Physics.Raycast(obstacleDetectorLow.position, obstacleDetectorLow.TransformDirection(Vector3.right), out objectHitLow, detectorRadius) == false){// Si le détecteur d'obstacles bas ne détecte rien...
				Debug.Log("Attention à la tête à droite!");
			}
		}
		else if(Physics.Raycast(obstacleDetectorHigh.position, obstacleDetectorHigh.TransformDirection(Vector3.left), out objectHitHigh, detectorRadius) == true){// Si le détecteur d'obstacles hauts détecte quelque chose...
			if(Physics.Raycast(obstacleDetectorLow.position, obstacleDetectorLow.TransformDirection(Vector3.left), out objectHitLow, detectorRadius) == false){// Si le détecteur d'obstacles bas ne détecte rien...
				Debug.Log("Attention à la tête à gauche!");
			}
		}
		*/
		
		mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);// On converti la position de la souris sur l'écran en rayon perpendiculaire à celui-ci
		
		/*if(cameraTwo.enabled == true && mainCamera.enabled == false){
			mouseRay = cameraTwo.ScreenPointToRay(Input.mousePosition);// On converti la position de la souris sur l'écran en rayon perpendiculaire à celui-ci
		}
		else{
			mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);// On converti la position de la souris sur l'écran en rayon perpendiculaire à celui-ci
		}*/
		
		// A FAIRE : Si un objet d'un des inventaires est désactivé, les autres objets de cet inventaire se repositionnent automatiquement
		
		if(activeHotspot == false && activeMenu == false){// Si aucun hotspot ou menu n'est actif...
			
			//Gestion de l'affichage du nom des hotspots survolés
			if(Physics.Raycast(mouseRay, out mouseHitInfo)){// Si la souris survole un objet...
				if(mouseHitInfo.collider.tag == "HotSpot"){// Si la souris survole un hotspot...
					// On affiche le nom du hotspot en bas de l'écran
					subtitlesWindow.gameObject.SetActive(true);// On active la fenêtre de sous-titres
					subtitlesText.transform.GetComponent<Text>().text = mouseHitInfo.collider.name;// On remplace le texte du sous-titre par le nom de l'objet survolé
				}
				else{// Si la souris ne survole aucun hotspot...
					subtitlesWindow.gameObject.SetActive(false);// On désactive la fenêtre de sous-titres
					subtitlesText.transform.GetComponent<Text>().text = "Empty";// On remet le texte par défaut dans la zone des sous-titres
				}
			}
		
			//Détection des cliques et récupération de données liées à ces cliques
			if(Input.GetMouseButtonDown(0) == true){// Si un clique GAUCHE est détecté...
				lastActiveMouseButton = "left";// On stock le bouton qui a été activé
				selectedLine = Random.Range(0,4);// Pour chaque clique, on choisi un nombre au hasard qui permettra de dire une phrase si jamais un objet est utilisé là où il ne faut pas
				
				if(Physics.Raycast(mouseRay, out mouseHitInfo) && mouseHitInfo.collider != null){// Si le joueur n'a pas cliqué dans le vide...
					
					if(mouseHitInfo.collider.tag == "Walkable" || mouseHitInfo.collider.tag == "HotSpot"){// Si le joueur a cliqué sur un hotspot OU sur le sol...
						
						if(previousClickFrame != 0 && (frameCounter-previousClickFrame) <= doubleClickDelay){// Si la durée entre le clique précédent et le clique actuel est inférieure ou égale au délai autorisé pour un double-clique...
							characterAnim.SetInteger("AnimPar", 1);// On active l'animation de course
							characterSpeed = characterSpeed*runningAccel;// On double la vitesse de déplacement du personnage
						}
						else{// Si la durée entre le clique précédent et le clique actuel est supérieure au délai autorisé pour un double-clique...
							characterAnim.SetInteger("AnimPar", 2);// On active l'animation de marche
							characterSpeed = charObject.GetComponent<Character>().speed;// On remet la vitesse du personnage par défaut
						}
						
						previousClickFrame = frameCounter;
						
						if(mouseHitInfo.collider.tag == "Walkable"){//S'il s'agit d'une surface sur laquelle le personnage peut marcher (ex : sol, plateformes)...
							/*	- On enregistre le point d'impact de la souris
								- On signal à la partie "déplacements" qu'elle doit s'enclencher en mode "déplacement"*/
							destinationPoint = mouseHitInfo.point;// Le point d'impact devient la nouvelle destination du/des personnages controlés
							clickOnGround = true;// On indique au script que le joueur a cliqué sur une surface où le personnage peut marcher
							clickOnHotSpot = false;// Si le joueur a cliqué sur un Hotspot puis à re-cliqué ailleurs sur le sol avant que le personnage n'ait atteint sa destination, on indique au script que ce n'est plus le cas
						}
						else if(mouseHitInfo.collider.tag == "HotSpot"){// S'il s'agit d'un Hotspot (objet, pnj, levier, zone, etc.)...
							/*	- On enregistre l'objet qui a été cliqué
								- On enregistre la position de l'objet (info destinée uniquement à la partie "déplacements")
								- On récupère son hotspot (info destinée uniquement à la partie "hotspot")
								- On enregistre aussi l'objet utilisé sur le hotspot s'il y en a un (info destinée uniquement à la partie "hotspot")
								- On signal à la partie "déplacements" qu'elle doit s'enclencher en mode "hotspot" */
							clickedObject = mouseHitInfo.transform;// On stock l'objet qui à été cliqué
							destinationPoint = new Vector3(clickedObject.position.x, charPosition.y, clickedObject.position.z);// L'objet cliqué devient la nouvelle destination du/des personnages controlés
							hotSpotScript = clickedObject.transform.GetComponent<HotSpot>();// On récupére toutes les informations concernant le Hotspot actif
							clickOnHotSpot = true;// On indique au script que le joueur à cliqué sur un Hotspot
							clickOnGround = false;// Si le joueur a cliqué sur le sol puis à re-cliqué sur un hotspot avant que le personnage n'ait atteint sa destination, on indique au script que ce n'est plus le cas
						}
					}

					directionTarget.position = new Vector3(destinationPoint.x, directionTarget.position.y, destinationPoint.z);// On oriente le personnage vers ce point
					fixedSpace = SpaceBetweenV3(charPosition, directionTarget.position, true);// On fixe la distance entre la cible de direction et le personnage controlé
				}
				else{// Si le joueur a cliqué dans le vide
					clickOnHotSpot = false;// On indique au script que le joueur n'a pas cliqué sur un Hotspot
					clickOnGround = false;// On indique au script que le joueur n'a pas cliqué sur une surface où le personnage peut marcher
				}
			}
			else if(Input.GetMouseButtonDown(1) == true){// Si un clique DROIT est détecté...
				lastActiveMouseButton = "right";// On stock le bouton qui a été activé
				selectedLine = Random.Range(0,4);// Pour chaque clique, on choisi un nombre au hasard qui permettra de dire une phrase si jamais un objet est utilisé là où il ne faut pas
				
				objectUsed = 0;// on remet automatiquement l'objet tenu dans l'inventaire
				objectChange = true;
				
				if(Physics.Raycast(mouseRay, out mouseHitInfo) && mouseHitInfo.collider != null){// Si le joueur n'a pas cliqué dans le vide...
					
					if(mouseHitInfo.collider.tag == "HotSpot"){// S'il s'agit d'un Hotspot (objet, pnj, levier, zone, etc.)...
						/*	- On enregistre l'objet qui a été cliqué
							- On récupère son hotspot (info destinée uniquement à la partie "hotspot")
							- On enregistre la position de l'objet (info destinée uniquement à orienter le personnage)
							- On oriente le personnage vers le hotspot cliqué
							- On signal à la partie "hotspot" qu'elle doit s'enclencher */
						clickedObject = mouseHitInfo.transform;// On stock l'objet qui à été cliqué
						hotSpotScript = clickedObject.transform.GetComponent<HotSpot>();// On récupére toutes les informations concernant ce Hotspot
						objectPosition = new Vector3(clickedObject.position.x, charPosition.y, clickedObject.position.z);// Le point d'impact devient la nouvelle destination du/des personnages controlés
						directionTarget.position = new Vector3(objectPosition.x, directionTarget.position.y, objectPosition.z);// On oriente le personnage vers ce point
						activeHotspot = true;// On signal à la partie "hotspot" qu'elle doit s'enclencher
					}
				}
				else{// Si le joueur a cliqué dans le vide
					clickOnHotSpot = false;// On indique au script que le joueur n'a pas cliqué sur un Hotspot
					clickOnGround = false;// On indique au script que le joueur n'a pas cliqué sur une surface où le personnage peut marcher
				}
			}
			
			//Déplacements des personnages
			if(clickOnGround == true){// Si le joueur à cliqué sur le sol ou sur un objet où le personnage peut marcher...
				
				if(destinationPoint.x != charPosition.x && destinationPoint.z != charPosition.z){// Si le point de destination (en x et en z) est différent de l'emplacement du personnage...
					remainingDistance = SpaceBetweenV3(charPosition, destinationPoint, false);// On calcule la distance restante entre le personnage et sa destination en cours
					if(remainingDistance.x < charCorpulence && remainingDistance.z < charCorpulence){// On laisse une marge de tolérance de la position finale à cause de la corpulence des différents personnages afin qu'il ne rentre pas dans les murs
						destinationPoint = charPosition;
					}
					else{
						MoveAt(charTransform, destinationPoint, characterSpeed);// On déplace le personnage controlé précisément à l'endroit cliqué
						CopyMoves(charTransform, directionTarget, fixedSpace);// On fait en sorte que le personnage controlé regarde toujours devant lui quand il se déplace
					}
				}
				else{// Si le personnage est arrivé ou est déjà sur le point de destination
					clickOnGround = false;// On indique à la partie "déplacements" qu'elle a terminé son travail
					characterAnim.SetInteger("AnimPar", 0);// On active l'animation d'attente du personnage
					characterSpeed = charObject.GetComponent<Character>().speed;// On remet la vitesse du personnage par défaut
				}
			}
			else if(clickOnHotSpot == true){// Si le joueur à cliqué sur un hotspot...
				remainingDistance = SpaceBetweenV3(charPosition, destinationPoint, false);// On calcule la distance restante entre le personnage et sa destination en cours
				objectRange = hotSpotScript.objectRange;
				if(objectRange == 0){
					Debug.Log("ERREUR : l'objectRange de ce hotspot n'est pas défini!");
				}
				if(remainingDistance.x <= objectRange && remainingDistance.z <= objectRange){// Si le personnage est déjà juste à côté de l'objet cliqué...
					destinationPoint = charPosition;// On place le point de destination à l'emplacement actuel du personnage afin qu'il reste où il se trouve
					directionTarget.position = new Vector3(clickedObject.position.x, directionTarget.position.y, clickedObject.position.z);// On oriente le personnage vers l'objet cliqué
					characterAnim.SetInteger("AnimPar", 0);// On active l'animation d'attente du personnage
					characterSpeed = charObject.GetComponent<Character>().speed;// On remet la vitesse du personnage par défaut
					activeHotspot = true;// On signal à la partie "hotspot" qu'elle doit s'enclencher
					clickOnHotSpot = false;// On indique à la partie "déplacements" qu'elle à terminé son travail
				}
				else{// Si le personnage est éloigné de l'objet cliqué...
					// Le personnage se déplace vers l'objet cliqué
					if((remainingDistance.x-characterSpeed) >= objectRange || (remainingDistance.z-characterSpeed) >= objectRange){// Si le personnage est à plus d'une certaine distance du rayon de l'objet...
						MoveAt(charTransform, destinationPoint, characterSpeed);// Le personnage continue d'avancer à sa vitesse normale
						CopyMoves(charTransform, directionTarget, fixedSpace);// On fait en sorte que le personnage controlé regarde toujours devant lui quand il se déplace
					}
					else{// Si le personnage est à moins d'une certaine distance du rayon de l'objet...
						destinationPoint = charPosition;// On place le point de destination à l'emplacement actuel du personnage afin qu'il s'arrête avant de marcher sur l'objet cliqué
						characterAnim.SetInteger("AnimPar", 0);// On active l'animation d'attente du personnage
						characterSpeed = charObject.GetComponent<Character>().speed;// On remet la vitesse du personnage par défaut
						activeHotspot = true;// On signal à la partie "hotspot" qu'elle doit s'enclencher
						clickOnHotSpot = false;// On indique à la partie "déplacements" qu'elle a terminé son travail
					}
				}
			}
		}
		else{// Si un hotspot ou un menu est actif...
			
			if(activeHotspot == true){// Si un hotspot est actif...
				
				if(hotSpotScript.eventId != 0 && hotSpotScript.itemId == 0){// Si le hotspot est de type event...
					
					if(lastActiveMouseButton == "left"){//Si le joueur a effectué un clique GAUCHE sur le hotspot...
						eventId = hotSpotScript.eventId;// On stock l'ID d'event de ce hotpsot
						eventsScript.SetEventId(eventId);// On indique au script "Events" l'eventId du hotspot actif
						eventsScript.SetClickedObject(clickedObject);// On indique au script "Events" le gameObject qui a été cliqué
						//eventsScript.SetObjectUsed(objectUsed);// On indique au script "Events" si un objet a été utilisé sur ce hotspot et si oui, lequel a été utilisé
						eventsScript.SetTrigger(true);// On déclenche l'évènement
					}
					else if(lastActiveMouseButton == "right"){//Si le joueur a effectué un clique DROIT sur le hotspot...
						dialogueContent = clickedObject.gameObject.GetComponent<HotSpot>().rightClickText;// On récupère le texte lié au clique droit de cet objet
						if(PlayDialogue(textSpeed, dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
							activeHotspot = false;// On indique au script que le hotspot en cours est terminé (s'exécute à la dernière frame)
						}
					}
				}
				else if(hotSpotScript.eventId == 0 && hotSpotScript.itemId != 0){// Si le hotspot est de type item...
					
					objectUsed = 0;// on remet automatiquement l'objet tenu dans l'inventaire
					objectChange = true;
					if(lastActiveMouseButton == "left"){//Si le joueur a effectué un clique GAUCHE sur le hotspot...
						itemId = hotSpotScript.itemId;// On récupère l'id de l'objet à obtenir
						/*itemQty = hotSpotScript.itemQty;// On récupère le nombre d'expemplaires de cet objet à obtenir
						if(itemQty != null || itemQty == 0){
							itemQty = 1;
						}
						*/
						clickedObject.gameObject.SetActive(false);// On désactive le mesh renderer et le mesh collider du hotspot en question
						// On ajoute l'item concerné à l'inventaire du joueur
						Debug.Log("Objet obtenu : "+itemId);//+" x "+itemQty);
						
						activeHotspot = false;// On indique au script que le hotspot en cours est terminé (s'exécute à la dernière frame)
					}
					else if(lastActiveMouseButton == "right"){
						activeHotspot = false;// On indique au script que le hotspot en cours est terminé (s'exécute à la dernière frame)
					}
				}
				else if(hotSpotScript.eventId == 0 && hotSpotScript.itemId == 0){// Si le hotspot n'est ni un item, ni un event, il s'agit forcément d'un hotspot de type info...
					
					if(objectUsed == 0){
						
						if(lastActiveMouseButton == "left"){//Si le joueur a effectué un clique GAUCHE sur le hotspot...
							dialogueContent = clickedObject.gameObject.GetComponent<HotSpot>().leftClickText;// On récupère le texte lié au clique gauche de cet objet
						}
						else if(lastActiveMouseButton == "right"){//Si le joueur a effectué un clique DROIT sur le hotspot...
							dialogueContent = clickedObject.gameObject.GetComponent<HotSpot>().rightClickText;// On récupère le texte lié au clique droit de cet objet
						}
						
						if(PlayDialogue(textSpeed, dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
							activeHotspot = false;// On indique au script que le hotspot en cours est terminé (s'exécute à la dernière frame)
						}
					}
					else{
						objectUsed = 0;// on remet automatiquement l'objet tenu dans l'inventaire
						objectChange = true;
						dialogueContent = wrongObjectLines[selectedLine];// On fait dire une des 4 phrases d'erreur au personnage (choisie lors du clique)
						
						if(PlayDialogue(textSpeed, dialogueContent)){// On lance la fenêtre de dialogue, le personnage dit son texte et quand le dialogue est terminé...
							activeHotspot = false;// On indique au script que le hotspot en cours est terminé (s'exécute à la dernière frame)
						}
					}
				}
				else{// Si le hotspot cliqué est du type event ET item...
					Debug.Log("ERREUR : le hotspot nommé \""+mouseHitInfo.collider.name+"\" est de 2 types différents, veuillez n'en choisir qu'un seul");// Un message d'erreur apparaît car cela crée un conflit
				}
			}
			else if(activeMenu == true){// Si un menu est actif...
				Debug.Log("Menu ouvert");
				activeMenu = false;
			}
		}
	}
	
	// Functions
    /*void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }*/
	
	// Permet d'obtenir une des 4 phrases (choisie lors d'un clique) quand un objet est mal utilisé
	public string GetWrongObjectLine(){
		return wrongObjectLines[selectedLine];
	}
	
	// Permet d'obtenir le numéro de frame actuel
	public int GetFrameNumber(){
		return frameCounter;
	}
	
	// Permet de savoir s'il y a eu un changement d'objet attaché à la souris
	public bool GetObjectChange(){
		return objectChange;
	}
	
	// Permet aux autres scripts de connaître l'id de l'event lié au hotspot sur lequel le joueur vient de cliquer
	public int GetEventId(){
		return eventId;
	}
	
	// Permet d'indiquer au script quel objet est actuellement en cours d'utilisation
	public void SetObjectUsed(int idObjectUsed){
		objectChange = true;
		objectUsed = idObjectUsed;
	}
	
	public int GetObjectUsed(){
		return objectUsed;
	}
	
	//Permet de faire attendre un script pendant le nombre de frames souhaité avant d'effectuer une action
	public bool waitFor(int framesToWait){
		if(frameCounter < framesToWait){
			frameCounter++;
			return false;
		}
		else{
			frameCounter = 0;
			return true;
		}
		
	}
	
	//Permet de redimensionner un Transform à la vitesse souhaitée et sur l'axe souhaité
	public void Scaling(Transform objectToScale, float speed, float newX, float newY = 0, float newZ = 0){
		objectToScale.localScale += new Vector3(newX, newY, newZ)*speed*Time.deltaTime;
		return;
	}
	
	//Permet à un objet d'effectuer exactement les mêmes déplacements qu'un autre objet
	public void CopyMoves(Transform originalObject, Transform copyObject, Vector3 displacement){
		Vector3 originalPos;
		Vector3 copyNewPos;
		
		originalPos = originalObject.position;// On stock la position de l'objet original

		copyNewPos.x = originalPos.x+displacement.x;
		copyNewPos.y = originalPos.y+displacement.y;
		copyNewPos.z = originalPos.z+displacement.z;

		copyObject.position = copyNewPos;// On applique la nouvelle position à l'objet copieur

		return;
	}
	
	//Permet de déplacer de manière progressive un Transform jusqu'à un point bien précis
	public bool MoveAt(Transform objectToMove, Vector3 finalPosition, float speed, Vector3 axisToUse = default(Vector3)){
		float totalFrames = 0;// On crée la variable qui contiendra le nombre de frames total du déplacement en cours
		Vector3 pos;
		Vector3 remainingDist;
		
		pos = objectToMove.position;// On stock la position actuelle de l'objet
		remainingDist = SpaceBetweenV3(pos, finalPosition, false);// On calcule la distance restante entre la position actuelle de l'objet et sa position finale
		
		if(remainingDist.x >= remainingDist.y && remainingDist.x >= remainingDist.z){// Si l'axe des X est le plus éloigné de sa destination...
			totalFrames = remainingDist.x / speed;// Le nombre total de frames est calculé d'après le déplacement en X
		}
		else if(remainingDist.y >= remainingDist.x && remainingDist.y >= remainingDist.z){// Si l'axe des Y est le plus éloigné de sa destination...
			totalFrames = remainingDist.y / speed;// Le nombre total de frames est calculé d'après le déplacement en Y
		}
		else if(remainingDist.z >= remainingDist.x && remainingDist.z >= remainingDist.y){// Si l'axe des Z est le plus éloigné de sa destination...
			totalFrames = remainingDist.z / speed;// Le nombre total de frames est calculé d'après le déplacement en Z
		}
		
		if(axisToUse != Vector3.zero){// Si l'objet doit se déplacer uniquement sur un seul axe
			if((axisToUse == Vector3.right || axisToUse == Vector3.left) && remainingDist.x < speed && pos.x != finalPosition.x){// Si l'axe est gauche ou droite ET si la distance restante en x est plus petite que la vitesse de l'objet ET si la position en x de l'objet est différente de la nouvelle position en x...
				objectToMove.Translate(axisToUse * remainingDist.x, Space.Self);// On termine le déplacement en adaptant la vitesse pour que l'objet s'arrête exactement au bon endroit en x
			}
			else if((axisToUse == Vector3.up || axisToUse == Vector3.down) && remainingDist.y < speed && pos.y != finalPosition.y){// Si l'axe est haut ou bas ET si la distance restante en y est plus petite que la vitesse de l'objet ET si la position en y de l'objet est différente de la nouvelle position en y...
				objectToMove.Translate(axisToUse * remainingDist.y, Space.Self);// On termine le déplacement en adaptant la vitesse pour que l'objet s'arrête exactement au bon endroit en y
			}
			else if((axisToUse == Vector3.forward || axisToUse == Vector3.back) && remainingDist.z < speed && pos.z != finalPosition.z){// Si l'axe est avant ou arrière ET si la distance restante en z est plus petite que la vitesse de l'objet ET si la position en z de l'objet est différente de la nouvelle position en z...
				objectToMove.Translate(axisToUse * remainingDist.z, Space.Self);// On termine le déplacement en adaptant la vitesse pour que l'objet s'arrête exactement au bon endroit en z
			}
			else{//Si la vitesse est inférieure à la distance restante...
				objectToMove.Translate(axisToUse * speed, Space.Self); // L'objet avance à sa vitesse normale sur l'axe choisi
			}
		}
		else{// Si l'objet doit se déplacer sur tous les axes...
			
			Vector3 adaptedSpeed;
			
			adaptedSpeed.x = remainingDist.x / totalFrames;// On calcule la vitesse de déplacement en X pour que celui-ci arrive en même temps qu'Y et Z à destination
			adaptedSpeed.y = remainingDist.y / totalFrames;// On calcule la vitesse de déplacement en Y pour que celui-ci arrive en même temps qu'X et Z à destination
			adaptedSpeed.z = remainingDist.z / totalFrames;// On calcule la vitesse de déplacement en Z pour que celui-ci arrive en même temps qu'X et Y à destination
			
			if(remainingDist.x < adaptedSpeed.x && remainingDist.x > 0 && pos.x != finalPosition.x){//Si la distance restante de X est inférieure à sa vitesse ET supérieure à zéro...
				if(finalPosition.x > pos.x){
					objectToMove.Translate(Vector3.right * remainingDist.x, Space.World);// On déplace l'objet vers la droite
				}
				else{
					objectToMove.Translate(Vector3.left * remainingDist.x, Space.World);// On déplace l'objet vers la gauche
				}
			}
			
			if(remainingDist.y < adaptedSpeed.y && remainingDist.y > 0 && pos.y != finalPosition.y){//Si la distance restante de Y est inférieure à sa vitesse ET supérieure à zéro...
				if(finalPosition.y > pos.y){
					objectToMove.Translate(Vector3.up * remainingDist.y, Space.World);// On déplace l'objet vers le haut
				}
				else{
					objectToMove.Translate(Vector3.down * remainingDist.y, Space.World);// On déplace l'objet vers le bas
				}
			}
			
			if(remainingDist.z < adaptedSpeed.z && remainingDist.z > 0 && pos.z != finalPosition.z){//Si la distance restante de Z est inférieure à sa vitesse ET supérieure à zéro...
				if(finalPosition.z > pos.z){
					objectToMove.Translate(Vector3.forward * remainingDist.z, Space.World);// On déplace l'objet vers l'avant
				}
				else{
					objectToMove.Translate(Vector3.back * remainingDist.z, Space.World);// On déplace l'objet vers l'arrière
				}
			}
			
			if(remainingDist.x >= adaptedSpeed.x || remainingDist.y >= adaptedSpeed.y || remainingDist.z >= adaptedSpeed.z){// Si la vitesse de l'objet est inférieure à la distance restante...
				if(pos.x < finalPosition.x){
					objectToMove.Translate(Vector3.right * adaptedSpeed.x, Space.World);
				}
				else if(pos.x > finalPosition.x){
					objectToMove.Translate(Vector3.left * adaptedSpeed.x, Space.World);
				}
				
				if(pos.y < finalPosition.y){
					objectToMove.Translate(Vector3.up * adaptedSpeed.y, Space.World);
				}
				else if(pos.y > finalPosition.y){
					objectToMove.Translate(Vector3.down * adaptedSpeed.y, Space.World);
				}
				
				if(pos.z < finalPosition.z){
					objectToMove.Translate(Vector3.forward * adaptedSpeed.z, Space.World);
				}
				else if(pos.z > finalPosition.z){
					objectToMove.Translate(Vector3.back * adaptedSpeed.z, Space.World);
				}
			}
		}
		
		if(remainingDist.x < 0.02f && remainingDist.x > -0.02f && remainingDist.y < 0.02 && remainingDist.y > -0.02 && remainingDist.z < 0.02 && remainingDist.z > -0.02){
			return true;
		}
		else{
			return false;
		}
	}
	
	//Calcule la distance en x, y et z entre 2 points dans l'espace 3D (si les nombres négatifs sont activés, la distance est donnée par rapport au premier point)
	public Vector3 SpaceBetweenV3(Vector3 firstPoint, Vector3 secondPoint, bool signedOrNot){
		Vector3 distance;
		
		// On calcule la différence entre le x du premier point et le x du second point
		if(firstPoint.x < secondPoint.x){// Si le X du premier point est EN DESSOUS de celui du second point, le résultat en X sera obligatoirement positif...
			if(firstPoint.x < 0){// Si le x du premier point est en dessous de zéro...
				distance.x = secondPoint.x + (firstPoint.x*-1);// On le rend positif afin d'additionner les 2 valeurs par rapport à zéro
			}
			else{// Si le x du premier point est au dessus de zéro...
				distance.x = secondPoint.x - firstPoint.x;
			}
		}
		else{// Si le X du premier point est PLUS GRAND ou égal à celui du second point, le résultat sera obligatoirement négatif (si les nombres négatifs sont activés)...
			// On retire le x du second point au x du premier point, ce qui nous donne obligatoirement un résultat positif => du coup on rend ce résultat négatif pour spécifier que le second point est en dessous du premier
			
			if(secondPoint.x < 0){// Si le x du second point est négatif...
				distance.x = firstPoint.x + (secondPoint.x*-1);// On le rend positif afin de ne pas fausser le calcule
			}
			else{// Si le x du second point est positif, le premier point l'est forcément aussi...
				distance.x = firstPoint.x - secondPoint.x;// Donc on retire le x du second point au x du premier point, ce qui nous donne obligatoirement un résultat positif
			}
			
			if(signedOrNot == true){// Si les nombres négatifs sont activés...
				distance.x = distance.x*-1;// On rend le résultat négatif pour spécifier que le second point est en dessous du premier sur l'axe X
			}
		}
		
		// On calcule la différence entre le y du premier point et le y du second point
		if(firstPoint.y < secondPoint.y){
			if(firstPoint.y < 0){
				distance.y = secondPoint.y + (firstPoint.y*-1);
			}
			else{
				distance.y = secondPoint.y - firstPoint.y;
			}
		}
		else{
			if(secondPoint.y < 0){
				distance.y = firstPoint.y + (secondPoint.y*-1);
			}
			else{
				distance.y = firstPoint.y - secondPoint.y;
			}
			
			if(signedOrNot == true){// Si les nombres négatifs sont activés...
				distance.y = distance.y*-1;// On rend le résultat négatif pour spécifier que le second point est en dessous du premier sur l'axe Y
			}
		}
		
		// On calcule la différence entre le z du premier point et le z du second point
		if(firstPoint.z < secondPoint.z){
			if(firstPoint.z < 0){
				distance.z = secondPoint.z + (firstPoint.z*-1);
			}
			else{
				distance.z = secondPoint.z - firstPoint.z;
			}
		}
		else{
			if(secondPoint.z < 0){
				distance.z = firstPoint.z + (secondPoint.z*-1);
			}
			else{
				distance.z = firstPoint.z - secondPoint.z;
			}
			
			if(signedOrNot == true){// Si les nombres négatifs sont activés...
				distance.z = distance.z*-1;// On rend le résultat négatif pour spécifier que le second point est en dessous du premier sur l'axe Z
			}
		}
		
		return distance;
	}
	
	//Calcule automatiquement la différence entre 2 nombres de type float
	public float SpaceBetweenF(float valueA, float valueB){
		float distance;// On crée la variable de retour
		
		if(valueA < valueB){
			if(valueA < 0){// Si la valeur A est négative...
				distance = valueB + (valueA*-1);// On le rend positif afin d'additionner les 2 valeurs par rapport à zéro
			}
			else{
				distance = valueB - valueA;
			}
		}
		else{
			if(valueB < 0){// Si la valeur B est négative...
				distance = valueA + (valueB*-1);// On le rend positif afin d'additionner les 2 valeurs par rapport à zéro
			}
			else{
				distance = valueA - valueB;
			}
		}
		
		return distance;
	}
	
	// Permet de récupérer la valeur de la gravité
	public float GetGravity(){
		return gravity;
	}
	
	// Permet de récupérer la vitesse de défilement des dialogues
	public int GetTextSpeed(){
		return textSpeed;
	}
	
	// Convertit une variable de type Vector3 en variable de type Float
	public float Vector3ToFloat(Vector3 valueToConvert){// ATTENTION : PAS SUR QUE LE CALCUL SOIT CORRECT
		float result = Mathf.Sqrt((valueToConvert.x * valueToConvert.x) + (valueToConvert.y * valueToConvert.y) + (valueToConvert.z * valueToConvert.z));// On calcule simplement l'hypoténuse des 3 axes
		return result;
	}

	//Permet de modifier l'id de l'event en cours
	public void SetEventId(int newEventId){
		hotSpotScript.eventId = newEventId;
		return;
	}
	
	//Permet de modifier le contenu de la fenêtre de dialogue
	public void SetDialogueContent(string textValue){
		dialogueContent = textValue;
		return;
	}

	//Permet de connaître le contenu de la variable ActiveMenu
	public bool GetActiveMenu(){
		return activeMenu;
	}
	
	//Permet de connaître le contenu de la variable ActiveHotspot
	public bool GetActiveHotspot(){
		return activeHotspot;
	}
	
	//Permet de modifier le contenu de la variable ActiveHotspot
	public void SetActiveHotspot(bool newValue){
		activeHotspot = newValue;
		return;
	}
		
	//Permet d'afficher une série de phrases dans la fenêtre de dialogue (Important : les variables "initDialFunction", "phraseCounter" et "textList" doivent être déclarées en dehors de cette fonction)
	public bool PlayDialogue(int timing, string rawText){
		if(rawText != null){// S'il y a au moins 1 phrase à dire...
			//Debug.Log("lastActiveMouseButton = "+lastActiveMouseButton);
			if(initDialFunction == true){// On initialise la fonction (première frame uniquement)
				Cursor.lockState = CursorLockMode.Locked;
				characterDialogueWindow.gameObject.SetActive(true);// On active la fenêtre réservée aux dialogues	
				textList = rawText.Split('_');// On sépare toutes les phrases que va dire le personnage
				initDialFunction = false;// On désactive l'initialisation de la fonction
				phraseCounter = 0;
			}
			
			if(phraseCounter < textList.Length){// Si le personnage à encore des phrases à dire... (de la première jusqu'à l'avant-dernière)
				
				if(waitFrame < timing){// Si le nombre de frame à patienter n'est pas encore atteint... (groupes de x frames)
					characterText.transform.GetComponent<Text>().text = textList[phraseCounter];// On affiche la phrase en cours
					waitFrame++;// Puis on incrémente le nombre de frames écoulées
				}
				else{// Si le nombre de frames à patienter est dépassé... (après chaque groupe de x frames)
					waitFrame = 0;// On remet le compteur de frames à zéro pour la prochaine fois qu'il sera utilisé
					phraseCounter++;// On passe à la phrase suivante
				}
				return false;
			}
			else{// Si le personnage n'a plus rien à dire... (dernière frame uniquement)
				Cursor.lockState = CursorLockMode.None;
				characterText.transform.GetComponent<Text>().text = "New text"; // On remet le texte par défaut
				characterDialogueWindow.gameObject.SetActive(false);// On désactive la fenêtre réservée aux dialogues
				phraseCounter = 0;// On remet le compteur de phrases à zéro
				initDialFunction = true;// On réinitialise la fonction (pour une prochaine utilisation)
				return true;
			}
		}
		else{
			return true;
		}
	}
}