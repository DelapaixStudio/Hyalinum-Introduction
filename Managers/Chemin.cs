using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Slate;
using UnityEngine.UI;

public class Chemin : MonoBehaviour
{
    // Script cinématique du parcours
    // Exception pour le GameOver() dans Ellipsoide.cs sur le go joueur enfant
    // ------------------------------
    [SerializeField] ListeObjets _objets;   

   
    //                       Scène Sommeil
    [SerializeField] GameObject SceneSommeil;
    GameObject instantSceneSommeil;

    //                           Joueur
    [SerializeField] GameObject player;
    [SerializeField] CharacterController charaControlPlayer;
    [SerializeField] UnityStandardAssets.Characters.FirstPerson.FPS_Controller CamJoueur;    
    [SerializeField]  PostProcessVolume _PostProcess;

    //                        Course Finale
    public bool Course;
    public Transform Tempete;
    [SerializeField] Transform LookStorm;
    [SerializeField] GameObject AmbiantDust;
    private Vector3 TempeteDistance = new Vector3(0, 0, 0);
    public float SpeedTempete;
    [Range(3, 5)]
    public float VitesseCourseFinale;
    private bool SonRespiCourse = false;
    private Vector3 startScaleLune;
    private float TimeInCourse = 0f;

    void Awake()
    {
        if (_objets == null) _objets = GameObject.FindGameObjectWithTag("ObjectManager").GetComponent<ListeObjets>();
        startScaleLune = _objets._LuneDesert.localScale;      
                                
        SoundManager.Instance.Play("Ambiance1");
    }


    private void Update()
    {
        if (!this.enabled) return;

        if (Course)
        {
            _objets._Tempete.Translate(Vector3.forward * SpeedTempete * Time.deltaTime, Space.World);
            
            /// Jouer son respi si le joueur se déplace.
            if (charaControlPlayer.velocity.magnitude > 0)
            {
                if (!SonRespiCourse)
                {
                    SoundManager.Instance.Set("RespiRapide", 1.5f, 1f);
                    SonRespiCourse = true;
                }                
            }
            else
            {
                if (SonRespiCourse)
                {
                    SoundManager.Instance.Set("RespiRapide", 0f, 1f);
                    SonRespiCourse = false;
                }
            }

            TimeInCourse += Time.deltaTime;
            float desiredScale = TimeInCourse / 100f;
            _objets._LuneDesert.localScale = startScaleLune + new Vector3(desiredScale, desiredScale, 0f);

        }       

    }    
  
 
    private IEnumerator CinematiqueFinale()
    {
        //  CamJoueur.TriggerFinal = true;
        Debug.Log("Cinematique Finale");
        var RespiScript = _objets.player.GetComponentInChildren<Respiration>();
        RespiScript.enabled = false;
        SoundManager.Instance.Play("Tempete", 1.2f); /// Il faudrait que le joueur soit bousculer par le vent.
        SoundManager.Instance.Stop("Ambiance1");
        yield return new WaitForSecondsRealtime(3f);
        CamJoueur.enabled = false;

        Transform tempete = _objets._Tempete;
        BoxCollider colTempete = tempete.GetComponent<BoxCollider>();
        colTempete.isTrigger = true;
        colTempete.size = new Vector3(colTempete.size.x, colTempete.size.y, 12f);
        SetStormPosFinal(true, 100f);
        player.transform.LookAt(LookStorm);
        Instantiate(AmbiantDust, player.transform.position, Quaternion.identity); /// Sable ambiant.


        //            **  ZOOM RAPIDE.  **
        Camera cam = player.transform.GetChild(0).gameObject.GetComponent<Camera>();
        float startFov = cam.fieldOfView;
        float fov = startFov, t = 0f;
        
        CamJoueur.JoueurColere = true;
        CamJoueur.shakeCamMagnitude = 0.2f;

        yield return new WaitForEndOfFrame();
        while (t < 2f)
        {
            fov = Mathf.Lerp(fov, 20f, Time.deltaTime * 1.5f);
           // fov -= 15f * Time.deltaTime;
            cam.fieldOfView = fov;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //Textes[3].layer = _objets.layerUI; // Texte FUIS
        yield return new WaitForSecondsRealtime(1f);
        cam.fieldOfView = startFov;
        //StartCoroutine(Interaction()); // Ajouter tremblements ?        
        yield return new WaitForSecondsRealtime(2f);



        //    ********************   DEBUT COURSE  ***********************
        //Tempete.position = player.transform.position - new Vector3(0, 0, 40);
        CamJoueur.enabled = true;        
        SonRespiCourse = true;
        CamJoueur.m_WalkSpeed *= VitesseCourseFinale;
        CamJoueur.m_RunSpeed = CamJoueur.m_WalkSpeed;
        //StartCoroutine(Fuis());
        SoundManager.Instance.Play("RespiRapide", 1f, 1f);
        Course = true;  /// Début Fonction déplacement Tempête dans Update()
        /*
        PostProcessVolume[] postPro = _objets.player.GetComponentsInChildren<PostProcessVolume>();
        foreach(PostProcessVolume p in postPro)
        {
           MotionBlur mBlur = p.profile.GetSetting<MotionBlur>();
            if(mBlur != null)
                mBlur.enabled.value = true; 
        }
        */
        //_PostProcess.profile.GetSetting<Vignette>().enabled.value = true;
               
        yield return new WaitForSecondsRealtime(1f);
       // gameObject.SetActive(false);
    }

    public void StopCinematiqueFinale()
    {
        StopAllCoroutines();
        Course = false;
        _objets.TerrainPrincipal.SetActive(false);
        gameObject.SetActive(false);
        player.SetActive(false);
    }

    public void StartScriptCourse()
    {
        StartCoroutine(CinematiqueFinale());
    }

    public void EndCutscene() //Lancé depuis Slate ou Ellipsoide.cs. Lance Interaction Lune.
    {
        Debug.Log("EndCutscene");

        SoundManager.Instance.Play("LuneProcheBis"); /// Bass.
        SoundManager.Instance.Stop("Ambiance1"); /// Musique de fond.        
        if(instantSceneSommeil == null)
        {
            GameObject prefab = Instantiate(SceneSommeil) as GameObject;
            instantSceneSommeil = prefab;
        }        

        if(_objets._Cutscenes[6].isPaused || !_objets._Cutscenes[6].isActive)
            _objets._Cutscenes[6].Play();        

        _objets.TerrainPrincipal.SetActive(false);
        _objets.player.SetActive(false);        
    }

    public void FinSceneColere()
    {
        Debug.Log("FinScèneColère()");
        var scriptColère = _objets.JoueurCinematiqueColere.GetComponent<JoueurColere>();
        if(scriptColère.InstanceGrandeStatue != null)
        {
            GameObject statue = scriptColère.InstanceGrandeStatue;
            Destroy(statue);
        }
        SoundManager.Instance.CheckSound("Enfer", out bool play);
        if (play) SoundManager.Instance.Stop("Enfer");
        SoundManager.Instance.Stop("Enfer");


        EndCutscene();
    }

    public void FinSceneSommeil()
    {
        if(instantSceneSommeil != null)
        {
           Destroy(instantSceneSommeil);
        }
        SoundManager.Instance.Stop("LuneProche");
        SoundManager.Instance.Stop("LuneProcheBis");        
        SoundManager.Instance.Play("Ambiance1");
        gameObject.SetActive(true);
        StartCoroutine(EndLune());
    }

    public void NextCutscene()
    {
        SaveManager.SaveInstance.ProgressPoint++; // Progress Point += 1;
        SaveManager.SaveInstance.SaveData();
        _objets._gameManager.LoadAtPoint(SaveManager.SaveInstance.ProgressPoint);
        Debug.Log("Progress Point = " + SaveManager.SaveInstance.ProgressPoint + " Progress Saved and NextSceneLoaded");
    }

    public void JustSavePoint()
    {
        SaveManager.SaveInstance.ProgressPoint++; // Progress Point += 1;
        SaveManager.SaveInstance.SaveData();
    }

    private IEnumerator EndLune() /// Fin Scène du Rêve.
    {
        int progress = SaveManager.SaveInstance.ProgressPoint;
        Debug.Log("FinSommeil au point de sauvegarde " + progress);

        if (progress == 1 || progress == 3)
        {
            if (progress == 3) progress--;
            _objets._FinLuneCutscenes[progress - 1].Play(); // ProgressPoint = 2 pour Marche1 car incrémenté au début cinématique sommeil
            while (true)
            {                
                /// Attend que les textes à la fin de la cinématique du Rêve soient finis.
                if(!_objets._FinLuneCutscenes[progress - 1].isActive)
                {
                    NextCutscene();
                    yield break;
 
                }  
                yield return new WaitForEndOfFrame();          
            }

        }
        else
        {
            NextCutscene();
            yield break;
        }              
    }

    private void SetStormPosFinal(bool isFinal = false, float dist = 0f)
    {
        Vector3 posTempete = _objets._Tempete.position;
        Transform tempete = _objets._Tempete;
        ParticleSystem psTempete = tempete.gameObject.GetComponent<ParticleSystem>();
        tempete.position = new Vector3(posTempete.x, posTempete.y, _objets.player.transform.position.z - 100f - dist);
        psTempete.Clear();
        var no = psTempete.noise;
        no.positionAmount = 8; 
        var emi = psTempete.emission;
        emi.rateOverTime = 200f;
        psTempete.Simulate(1f);
        psTempete.Play();
        Debug.Log("SetStromPosFinal");
    }

    public void SetStormPos()
    {
        Vector3 posTempete = _objets._Tempete.position;
        Transform tempete = _objets._Tempete;
        ParticleSystem psTempete = tempete.gameObject.GetComponent<ParticleSystem>();
        tempete.position = new Vector3(posTempete.x, posTempete.y, _objets.player.transform.position.z - 150f);
        psTempete.Clear();
        var no = psTempete.noise;
        no.positionAmount = 1;
       
        psTempete.Simulate(1f);
        
        Debug.Log("SetStromPos");
    }

    public void SetFirePos()
    {
        Transform Feu = _objets._FeuPS.transform;
        Vector3 desiredPos = _objets._LuneDesert.position;
        Vector3 SetPos = player.transform.position;
        Feu.position = SetPos;
        
        while (true)
        {
            Feu.position = Vector3.Lerp(Feu.position, desiredPos, 1f * Time.deltaTime);
            
            if (Vector3.Distance(Feu.position, SetPos) > 100f) /// Au dela de 200 le PS(avec sprites) n'est plus visible.
            {               
                Feu.position += (Vector3.up * 100f); /// On met le GO en hauteur pour calculer sa distance par rapport au sol.
                RaycastHit hit;
                int _layerMask = LayerMask.GetMask("Ground");
                if (Physics.Raycast(origin: Feu.position, -transform.up, out hit, Mathf.Infinity, _layerMask))
                {
                    Feu.position -= new Vector3(0f, hit.distance, 0f);  /// On soustrait la distance qui le separe du sol.                  
                }
                break;
            }           
        }    
    }   

}
