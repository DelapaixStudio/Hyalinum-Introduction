using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections.Generic;
using System;
using Slate;


public class ListeObjets : MonoBehaviour
{
    public GameObject player;
    public FPS_Controller CamJoueur;
    PostProcessVolume _PostProcessJoueur;
    public GameObject Chemin;
    public Transform _Tempete;
    public GameObject TerrainPrincipal;
    public GameObject _ParentScenes;

    public int layerUI = 5, layerMaskUI = 11;  // Attention ne fonctionne pas avec les raycast.

    public GameObject _startCam;
    public GameObject SavePos_Parent;
    public Transform[] SavePos;
    public GameObject DialoguesChoix;  // GO Parent des Textes 
    public Dictionary<string, GameObject> TriggerMessage;
    public GameObject _TriggerMessage4;
    public GameObject _FeuPS; 
    public Transform _LuneIntro;
    public Transform _LuneDesert;
   
    public GameManager _gameManager;

    public GameObject JoueurCinematiqueColere;
    
    public Cutscene[] _Cutscenes;
    public Cutscene[] _endCutscenes;
    public Cutscene[] _FinLuneCutscenes;
    public Cutscene colereCutscene;
    public SceneObject[] _Scenes;
    public SceneObject[] _Triggers;

    public GameObject ChapitreTxtGo;
    public Text NumTxt;
    public Text NomTxt;

    // Textes 
    public MotsVolants motsVolants;
    public TextData[] TextsToSet;
        
    private void Awake()
    {
        if(_gameManager == null)  _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if (player == null) player = GameObject.FindWithTag("Player"); // GO parent Joueur.
        if (CamJoueur == null) CamJoueur = player.GetComponent<FPS_Controller>();
        if(_PostProcessJoueur == null) _PostProcessJoueur = player.GetComponentInChildren<PostProcessVolume>();
        if (_Tempete == null) _Tempete = GameObject.FindGameObjectWithTag("Tempete").transform;      
        if(SavePos == null)
        {
           SavePos = new Transform[SavePos_Parent.transform.childCount];
           for (int i = 0; i < SavePos_Parent.transform.childCount; i++)        
           SavePos[i] = SavePos_Parent.transform.GetChild(i);
        }             
    }

    [Serializable]
    public class SceneObject
    {
        public string _name;
        public GameObject scene;
    }

}