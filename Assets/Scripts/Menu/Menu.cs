using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] public Canvas menu = null;

    private void Start() {
        // Canvas menu = GetComponent<Canvas>();
        // menu.gameObject.SetActive(false);
    }

    public void Resume(){
        // menu.enabled=true;
        GameObject.Find("Menu").GetComponent<Canvas>().enabled=false;
    }
    
    public void OpenSettingMenu(){

    }

    public void Quit(){
        Application.Quit();
        EditorApplication.isPlaying = false;
    }
}
