using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraDebut : MonoBehaviour
{
    [SerializeField]
    private ListeObjets Objets; ///Liste d'objets communs.

    [SerializeField] private bool ScèneSommeil = false;
   
    [SerializeField] private Transform CamLuna;
    [SerializeField] private Transform LaLuna;
    [SerializeField] private Animator animator;
    private CliqueDebut ScriptLune;
    [SerializeField] private Transform Puits;
    Chemin scriptChmn;

    private bool _estFini = false;
    private bool touche = false;
    private bool WaitRaycast = true;
    private bool isOnMoon = false;
    private bool isOnMoonAndClick = false;

    // CAMERA
    public float rotSpeed = 15f;
    [Range(0f,30f)]
    public float SpeedLookAt;
    RaycastHit hit; /// Utilisé dans l'update ne pas supprimer.
    private int layerMask;
    [SerializeField] private float smoothRot;
    private Quaternion targetRot;
    private float startYaxis;

    void Start()
    {
        if (Objets == null) Objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();
       // _rb = GetComponent<Rigidbody>();
        layerMask = LayerMask.GetMask("UI"); // On enregistre un layer dans une int.
        scriptChmn = Objets.Chemin.GetComponent<Chemin>();
        ScriptLune = LaLuna.GetComponent<CliqueDebut>();
        GameObject CamPosLuna = CamLuna.gameObject;
        Objets.Chemin.SetActive(false);
        Objets.player.SetActive(false);
        StartCoroutine(WaitRay()); // Déclenche raycast plus tard sinon faut sortir col Lune et le remettre dessus pour qu'il fonctionne.
        targetRot = transform.parent.localRotation;
        startYaxis = transform.parent.rotation.y;
    }

    private IEnumerator WaitRay()
    {
        float wait = 6f;
        if (ScèneSommeil) wait = 3f;
        yield return new WaitForSeconds(wait);
        Debug.Log("Debut Raycast");
        WaitRaycast = false;
    }

    void Update()
    {
        if (_estFini) return; // Vérifie si l'interaction de la Lune est finie.
        if (WaitRaycast) return;

        //var x = CrossPlatformInputManager.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        //var y = CrossPlatformInputManager.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;
        
        Vector3 Lune = LaLuna.position;
        var x = CrossPlatformInputManager.GetAxis("Mouse X") ;
        float onMoon;
        if (isOnMoon) onMoon = 2f;
        else onMoon = 0.5f;
        if (isOnMoonAndClick) onMoon = 0f;
        
        targetRot *= Quaternion.Euler(0f, x * onMoon, 0f);
       // targetRot *= Quaternion.LookRotation(Vector3.zero, Lune);

        //Debug.Log(x);
        Transform Parent = transform.parent;
        //Parent.Rotate(Vector3.up, x); // Transmission de l'input         
        

       // if(x!=0)
        Parent.rotation = 
            Quaternion.Slerp(Parent.rotation, targetRot, smoothRot * Time.deltaTime);
        /* else
         { 

             float vitesse = (1f / SpeedLookAt) * Time.deltaTime;
             Parent.localRotation =  
                 Quaternion.Slerp(Parent.localRotation, Quaternion.LookRotation(Vector3.zero, Lune), vitesse); ///SmoothLookAt() 
         }*/



        // if(Parent.rotation.y > startYaxis) Parent.rotation *= Quaternion.AngleAxis(5f, Vector3.up); 
        // else Parent.rotation *= Quaternion.AngleAxis(-5f, Vector3.up);

        //                                *****RAYCAST***** 
        /// Trigger vers le script "Clique Debut" du Gameobject Lune (Canvas)
        #region Raycast 
        Debug.DrawRay(transform.position, transform.forward * 5000, Color.red);
       
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            ScriptLune.HitRaycast(true);     // Envoi du Raycast
            isOnMoon = true;
            if (Input.GetKey(KeyCode.Mouse0)) isOnMoonAndClick = true;
        }
        else
        {
            ScriptLune.HitRaycast(false);
            isOnMoon = false;
            isOnMoonAndClick = false;
        }
                              
        
       #endregion
    }
         

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == CamLuna.gameObject) {
            touche = true;      // Confirme l'arrivée de la caméra devant la lune.
        }
    }  


    public IEnumerator CinematiqueLune()
    {
        _estFini = true;
        // Debug.Log("<color=red>Entrée coroutine</color>");       
        //yield return new WaitForSecondsRealtime(2f);

        Vector3 NewPosLune = new Vector3(LaLuna.position.x, 150f, LaLuna.position.z);
        float yPosCam = transform.position.y;
     
        while (!touche) /// Vers le puits.
        {
            // Déplacement Caméra.
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, CamLuna.position, 1f * Time.deltaTime);
            transform.position = smoothedPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, CamLuna.rotation, Time.deltaTime);
            if (Vector3.Distance(transform.position, CamLuna.position) > 200f) 
                /// Déplace vers l'avant et le haut.
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, yPosCam + 55f, Time.deltaTime), transform.position.z); 
            ///Debug.Log(Vector3.Distance(transform.position, CamLuna.position));

            // Déplacement Verticale de la Lune.
            Vector3 descenteLune = Vector3.Lerp(LaLuna.position, NewPosLune, Time.deltaTime);
            LaLuna.position = descenteLune;
            yield return new WaitForEndOfFrame();
            
            yield return touche;
        } 
        
        if (ScèneSommeil) // On termine la cinématique du rêve ici.
        {
            Objets._Cutscenes[6].Resume();
            while (true)
            {
                if (!Objets._Cutscenes[6].isActive) break;
                yield return new WaitForEndOfFrame();
            }
            scriptChmn.FinSceneSommeil();
            yield break;
        }

        float decompte = 0f; 
        Vector3 nextPos = new Vector3(Puits.position.x, Puits.position.y + 5f, Puits.position.z);
        Quaternion nextRot = Quaternion.Euler(90f, 0f, 0f);
        
        while (true) /// Position au dessus du puits.
        {
            transform.position = Vector3.Lerp(transform.position, nextPos, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, nextRot, Time.deltaTime);
            decompte += 1f * Time.deltaTime;
            if (decompte > 1f * Time.deltaTime * 200f) break;
            yield return new WaitForEndOfFrame();
        }

        decompte = 0f;
        nextPos = new Vector3(transform.position.x, transform.position.y - 15f, Puits.position.z);

        var CutsceneIntro = Objets._Cutscenes[0];
        if (!ScèneSommeil) CutsceneIntro.Resume();

        while (true) /// Rotation et descente dans le puits
        {
            transform.position = Vector3.Lerp(transform.position, nextPos, 0.25f * Time.deltaTime);
            transform.rotation *= Quaternion.Euler(0f, 0f, 1f);
            decompte += 1f * Time.deltaTime;
            if (decompte > 1f * Time.deltaTime * 300f) break;
            yield return new WaitForEndOfFrame();
        }

        ///yield return new WaitForSecondsRealtime(1f);
        ///animator.SetBool("Fade", true);
        ///yield return new WaitForSecondsRealtime(1f);
        ///animator.SetBool("Fade", false);        
        ///gameObject.GetComponent<Camera>().enabled = false;

        while (true) /// On attend que la cut Slate se finisse.
        {
            if (!CutsceneIntro.isActive) break;
            yield return new WaitForEndOfFrame();
        }

        if (!ScèneSommeil)
        {
           SaveManager.SaveInstance.ProgressPoint = 1;
           SaveManager.SaveInstance.SaveData();
           
           Objets._gameManager.LoadAtPoint(1);  
        }
                       
    } 
}