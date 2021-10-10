using UnityEngine;
using UnityEngine.UI;
using Slate;

public class MenuPause : Menu {
    
    [SerializeField] ListeObjets _objets;
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    
    
    protected override void Start()
    {
        base.Start();
        if(_objets == null) _objets = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ListeObjets>();
        OngletLangues.value = SaveManager.SaveInstance.currentlanguage;
        Resume();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                if (startLanguage != OngletLangues.value) SaveManager.SaveInstance.SaveData();
                Resume();
            }                
            else
            {
                startLanguage = OngletLangues.value;
                Pause();     
            }                     
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;   
        AudioListener.pause = false;
        Cursor.visible = false;
       // _objets.CamJoueur.TriggerFinal = false; // Relance Update()
        pauseMenuUI.SetActive(false);             
        GameIsPaused = false;
    }

    void Pause ()
    {
        AudioListener.pause = true;
        Cursor.visible = true;
      //  _objets.CamJoueur.TriggerFinal = true; // Pause Update()
        pauseMenuUI.SetActive(true);        
        GameIsPaused = true;  
        Time.timeScale = 0f;    
    }

    public void Langue()
    {
        _objets._gameManager.ChangeLanguage();
    }

    public void LoadLastSave()
    {
        Debug.Log("Chargement du menu. . .");
        foreach(Cutscene cut in _objets._Cutscenes)
        {
            if (cut.isActive) cut.Stop();
        }
        Load("Desert");
    }

    public void GoBackToMainMenu()
    {
        Load("MenuPrincipal");
    } 

}

