using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using Slate;

public class Ellipsoide : MonoBehaviour
{
    // APPEL COLLISIONS scripts FPS Controller et Chemin.cs 

    [SerializeField]
    private ListeObjets _objets;

    public GameObject _cam;
    PostProcessVolume _PostProcess;
    ColorGrading _colorGrad = null;
    Chemin ScriptChemin;
    public GameObject InteractionChemin;
    public GameObject Audio;
    private SoundManager _audio;
    UnityStandardAssets.Characters.FirstPerson.FPS_Controller CamJoueur;
    [SerializeField] CharacterController charaController;

    private float vitesse;
    private bool ZoneMort = false;
    private int PortailCount = 0;
    private float isInStorm = 0f;

    private void Start()
    {
        if (_objets == null) _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();
        if (charaController == null) charaController = GetComponent<CharacterController>();
        ScriptChemin = _objets.Chemin.GetComponent<Chemin>();
        
        CamJoueur = _objets.CamJoueur;
        vitesse = CamJoueur.m_WalkSpeed; // On stocke vitesse de départ pour pouvoir la réattribuer.

        PortailCount = 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!this.enabled) return;
        if (!charaController.enabled) return;   

        if (other.CompareTag("Mur"))
        {
          //  ZoneMort = true;
          //  StartCoroutine(GameOver());
        }

        if (other.CompareTag("Chemin"))
        {
            Debug.Log("TriggerChemin");
            string nom = other.name;

            if(nom == "CourseFinale")
            {
                Debug.Log("CourseFinale");
                ScriptChemin.StartScriptCourse();             
                
            }

           // if (nom == "Chapitre4") ScriptChemin.JustSavePoint(); 
            /// Pas de return car on reste dans la même cutscene
            /// ProgressPoint ++ et Sauvegarde

            NextSectionCustscene();
            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Portail"))
        {
            Debug.Log("TriggerPortal");

            // Portails Scène Rêve
            //---------------------

            PortailCount++;

            if(PortailCount != 4) other.gameObject.SetActive(false);

            if (PortailCount == 3)
                ResumeCurrentScene();
            
            if(PortailCount == 4)
            {
                ResumeCurrentScene();
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;                
            }
        }

        if (other.CompareTag("Tempete"))
        {
          if (ScriptChemin.Course)    
                  NextSectionCustscene(); /// Fin du jeu          
        }

    }

 

    private IEnumerator GameOver()
    {
        CamJoueur.m_WalkSpeed = 1.5f;
        // Insérer effet shader
        yield return new WaitForSecondsRealtime(3.236f);
        if (!ZoneMort)
        {
            CamJoueur.m_WalkSpeed = vitesse;
            yield break;
        }
        // Fondu + bruits de pas.
        CamJoueur.enabled = false;
        yield return new WaitForSeconds(1);
        _audio.Play("Mort");
        yield return new WaitForSeconds(18f);

        

        // Rechargement de la scène.
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);               
    }

    public void FinMarche() // Stop la cinématique et lance la cinématique "Sommeil"
    {
        //SaveManager.SaveInstance.ProgressPoint++; // Progress Point += 1;
        int state = SaveManager.SaveInstance.ProgressPoint;
        Cutscene currentCutscene = _objets._Cutscenes[state]; 
        currentCutscene.Stop(Cutscene.StopMode.Skip);
        Cutscene fin = _objets._endCutscenes[0]; // [0] car c'est toujours la même
        fin.Play();
    }    

    private void ResumeCurrentScene()
    {
        int state = SaveManager.SaveInstance.ProgressPoint;
        Cutscene currentCutscene = _objets._Cutscenes[state];
        currentCutscene.Resume();
    }

    private void NextSectionCustscene()
    {
        Cutscene currentCutscene;
        if (ScriptChemin.Course || SaveManager.SaveInstance.ProgressPoint == 5) currentCutscene = _objets._Cutscenes[4];
        else
        {
            int state = SaveManager.SaveInstance.ProgressPoint;
            currentCutscene = _objets._Cutscenes[state];
        }        

        currentCutscene.SkipCurrentSection();
        if(currentCutscene.isPaused) currentCutscene.Resume();

        if (ScriptChemin.Course) this.enabled = false;

    }
  
}
