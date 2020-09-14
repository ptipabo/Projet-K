using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
	
	private Image activeImg;
	private ScreenManager screenManager;
	public int idUnderMenu = 0;

	// Use this for initialization
	void Start () {
		activeImg = GetComponent<Image>();
		screenManager = GameObject.Find("Screen").GetComponent<ScreenManager>();
	}
	
	public void OnPointerEnter(PointerEventData eventData){
		activeImg.enabled = true;
		//Debug.Log("Cercle actif");
	}
   
	public void OnPointerExit(PointerEventData eventData){
		activeImg.enabled = false;
		//Debug.Log("Cercle inactif");
	}
   
	public void OnPointerClick(PointerEventData eventData){
	   //on agrandit la fenêtre de gauche et on affiche le contenu détaillé
	   screenManager.SetIdUnderMenu(idUnderMenu);
	   screenManager.SetIdMenuAnimation(3);
	   //Debug.Log("Sous-menu ouvert");
	}
}
