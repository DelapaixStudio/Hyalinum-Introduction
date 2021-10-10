using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class Menu : MonoBehaviour
{

    public int startLanguage;
    public Dropdown OngletLangues;

    protected virtual void Start()
    {
        if(OngletLangues == null) OngletLangues = GetComponentInChildren<Dropdown>();
        OngletLangues.value = SaveManager.SaveInstance.currentlanguage;
        startLanguage = OngletLangues.value;

        DialogueManager.Instance.Reader();
    }

    public void Load(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        Debug.Log("Vous quittez la partie. . . :'( ");
        if (OngletLangues.value != startLanguage) SaveManager.SaveInstance.SaveData();
        Application.Quit();
    }

}
