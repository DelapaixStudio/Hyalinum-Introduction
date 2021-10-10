using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Slate;

//CHARGE la scène avec les paramètres prédéfinis de la SAUVEGARDE.
//           ------------------------------

public class GameManager : MonoBehaviour    
{
   
    [SerializeField]
    private ListeObjets _objets;
    
    [HideInInspector][SerializeField]
    private float walkSpeed; 

    
    [SerializeField] Dropdown _dropdown; /// Onglet Menu : Langues.

    public bool PlayTest;
    [SerializeField]
    private float StartCutsceneAt = 0f;

    private bool isPlaying = false;

    private void Awake()
    {               
        if (_objets == null)
            _objets = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ListeObjets>();

        walkSpeed = _objets.CamJoueur.m_WalkSpeed;

        if (PlayTest)
        {
            LoadAtPoint(SaveManager.SaveInstance.ProgressPoint);
            Debug.Log("PlayTestLoad");
            return;
        }
       
        _dropdown.value = SaveManager.SaveInstance.currentlanguage;
        LoadAtPoint(SaveManager.SaveInstance.ProgressPoint);

        Debug.Log("Progress Point Form SaveManager.cs = " + SaveManager.SaveInstance.ProgressPoint);
        Debug.Log("Langue = " + SaveManager.SaveInstance.currentlanguage); /// Numéro langue
    }

    private void Start()
    {
        SetAllTexts();
        isPlaying = true;  /// Nécessaire pour charger scène Colère.
    }

  
    public void ChangeLanguage()
    {
        SaveManager.SaveInstance.currentlanguage = _dropdown.value;
        DialogueManager.Instance.Reader();

        SetAllTexts();
    }

    private void SetAllTexts()
    {                
        foreach(TextData txt in _objets.TextsToSet)
        {
            if (txt != null)
            {
                var textComponent = txt.GetComponent<Text>();
                var TmpComponent = txt.GetComponent<TextMeshProUGUI>();
                string code = txt.lineCode;
                DialogueManager.Instance.GetText(code, out string getTxt);
                if (textComponent != null) textComponent.text = getTxt;
                if (TmpComponent != null) TmpComponent.text = getTxt;
            }
            else Debug.Log("TextData.cs MANQUANT!");
        }
                
        _objets.motsVolants.SetTextWords();
    }

    public void LoadAtPoint(int point)
    {
        /// La position du joueur est réglée dans Slate
        Debug.Log("LoadAtPoint" + point);
        GameObject joueur = _objets.player;
        GameObject terrain = _objets.TerrainPrincipal;
        GameObject chemin = _objets.Chemin;


        //Intro
        if(point == 0) 
        {            
            joueur.SetActive(false);
            chemin.SetActive(false);
            _objets.TerrainPrincipal.SetActive(false);
            SetScenesState("Intro");            

            _objets._Cutscenes[0].Play();
        }

        /// Marche 1.
        if(point == 1) 
        {
            foreach(Transform child in _objets._ParentScenes.transform)
            {
                child.gameObject.SetActive(false);
            }
            if (!terrain.activeSelf) terrain.SetActive(true);
            if (!joueur.activeSelf) joueur.SetActive(true);
            commonLoadingData(point);
        }

        /// Marche 2.
        if (point == 2) 
        {
            foreach (Transform child in _objets._ParentScenes.transform)
            {
                child.gameObject.SetActive(false);
            }
            if(!terrain.activeSelf) terrain.SetActive(true);
            if(!joueur.activeSelf) joueur.SetActive(true);            
            commonLoadingData(point);
        }

        /// Colère.
        if (point == 3) 
        {            
            if (!isPlaying)
            {
                SetScenesState("Colere");
                commonLoadingData(point);  
            }    
        }

        /// Marche 3 :Mer de maux 
        if (point == 4) 
        {
            SetScenesState("Marche3");
            if (!terrain.activeSelf) terrain.SetActive(true);
            if (!joueur.activeSelf) joueur.SetActive(true);
            if (!chemin.activeSelf) chemin.SetActive(true);

            commonLoadingData(point);                   
        }
        
        /// Marche 4 + Course Finale
        if(point == 5)
        {
            if (!isPlaying)
                chemin.GetComponent<Chemin>().SetStormPos();
            if (!terrain.activeSelf) 
                terrain.SetActive(true);
            if (!joueur.activeSelf)
                joueur.SetActive(true);
            Debug.Log("Mots Volants Détruits");
            commonLoadingData(point);
        }

    }

    public void commonLoadingData(int p)
    {                       
        _objets.CamJoueur.m_WalkSpeed = walkSpeed;  
        Transform _transformPlayer = _objets.player.transform;
        GameObject _chemin = _objets.Chemin;
        if(!_chemin.activeSelf) _chemin.SetActive(true);

        GameObject tempete = _objets._Tempete.gameObject;
        if (!tempete.activeSelf) tempete.SetActive(true);
        if(p != 4 && p != 5)
            _chemin.GetComponent<Chemin>().SetStormPos();

        if (p == 5)
        {
            _objets._Cutscenes[4].Play(35.9f);
            return;
        }

        if (!PlayTest)
        _objets._Cutscenes[p].Play();
        else
            _objets._Cutscenes[p].Play(StartCutsceneAt);

        _chemin.GetComponent<Chemin>().enabled = true;
    }
 
    public void ResumeCurrentScene()
    {
        int state = SaveManager.SaveInstance.ProgressPoint;
        Cutscene currentCutscene = _objets._Cutscenes[state];
        currentCutscene.Resume();
    }

    private void SetScenesState(string nom, bool destroy = false)
    {
        var objetScene = Array.Find(_objets._Scenes, findScene => findScene._name == nom);
        GameObject go = objetScene.scene;
        if(destroy)
        {
            // ATTENTION ERROR SLATE QUAND IL ARRETE LA CINEMATIQUE.
            Destroy(go);
            return;
        }

        foreach(Transform child in go.transform.parent)
        {
            child.gameObject.SetActive(false);
        }
        go.SetActive(true);
    }

}


