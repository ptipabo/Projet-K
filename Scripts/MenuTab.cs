using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Permet de modifier les composants des éléments de type GUI
using UnityEngine.EventSystems;//Permet d'utiliser le système d'évènements de Unity

public class MenuTab : MonoBehaviour, IPointerDownHandler {
	
	private MasterScript masterScript;//Lie ce script au MasterScript
	public GameObject tabToAnimate;// Contient l'onglet qui sera animé lors du clique gauche du joueur
	private Animator tabAnim;// Permet de stocker le gestionnaire d'animation de l'onglet à animer
	private bool triggerAnimation = false;// Permet d'indiquer au script si le joueur à effectué un clique gauche ou non
	private bool isOpen = false;// Permet de vérifier si le menu est ouvert ou fermé
	public Sprite openArrow, closeArrow;

	// Use this for initialization
	void Start () {
		masterScript = GameObject.Find("GameManager").GetComponent<MasterScript>();// On lie ce script au MasterScript
		tabAnim = tabToAnimate.GetComponent<Animator>();// On stock le composant "Animation" de l'onglet des objets clés
	}
	
	// Update is called once per frame
	void Update () {
		isOpen = tabAnim.GetBool("tabOpen");// On vérifie à chaque frame si l'onglet est ouvert ou fermé
		if(masterScript.GetActiveHotspot() == false || masterScript.GetActiveMenu() == false){
			if(triggerAnimation == true){// Si le joueur a cliqué sur cet objet...
				if(tabToAnimate != null){// Si un onglet à animer est bien indiqué au script...
					if(tabAnim != null){// Si cet onglet possède bien une animation d'ouverture/fermeture...
						tabAnim.SetBool("tabOpen", !isOpen);// Puis on inverse son état (si ouvert -> on le ferme, si fermé -> on l'ouvre)
						if(isOpen == true){
							GetComponent<Image>().sprite = openArrow;
						}
						else{
							GetComponent<Image>().sprite = closeArrow;
						}
						triggerAnimation = false;// On signale au script que l'action liée au clique est terminée
					}
				}
			}
		}
	}
	
	public void OnPointerDown(PointerEventData eventData){// Ne doit PAS être appelée dans Update(), Permet de détecter un clique (gauche ou droit) de la souris
		if(Input.GetMouseButtonDown(0)){// S'il s'agit d'un clique gauche...
			triggerAnimation = true;// On signale au script que le joueur a cliqué sur cet objet
		}
	}
	
	// Permet à un autre script de savoir si ce menu est ouvert ou fermé
	public bool GetIsOpen(){
		return isOpen;
	}
	
	// Permet de déclencher l'ouverture ou la fermeture du menu depuis un autre script
	public void SetTriggerAnimation(bool newValue){
		triggerAnimation = newValue;
	}
}