using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Slate;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;


public class JoueurColere : MonoBehaviour
{
    [SerializeField] private ListeObjets _objets;
    [SerializeField] [HideInInspector] private Rigidbody _rb;
    private bool stopTrigger = false;

    [SerializeField] private GameObject StatueGeantPrefab;
    [SerializeField] private Transform firstTransStatue;
    [SerializeField] private Vector3 PosStatueGeant;
    [SerializeField] Transform[] trsfrmStatue;
    [SerializeField] Transform targetCenterPos;
    [SerializeField] Transform centerExplosion;
    [SerializeField] GameObject ParentPetitesStatues;

    [SerializeField] private float speedBuste;
    private bool bustMoving = false;
    [HideInInspector]
    [SerializeField] Transform BusteTransform;

    [SerializeField] GameObject TerrainTremblant;
    [HideInInspector]
    [SerializeField] Renderer rend;
    private Vector3 ScaleTerrain;
    private float startStrength;

    private bool EscapeChallenge = false;
    [SerializeField] float TimeToNextPart;
    private float startTimeToNextPart;
    private float TimeInCounter;
    private bool firstDeath = false;
    private bool end = false;
    private float _playNextBreath = 0f;
    private bool wasEnferSoundPlayed = false;
    private string lastBreathName;
    public GameObject InstanceGrandeStatue;
    private bool StopBreathing = false;

    [SerializeField] private FPS_Controller controllerJoueur;
    private float startSpeed;
        

    private void Start()
    {
        if(controllerJoueur == null) controllerJoueur = GetComponent<FPS_Controller>();
        startSpeed = controllerJoueur.m_WalkSpeed;
        controllerJoueur.JoueurColere = false;
        SoundManager.Instance.Play("Enfer");

        _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();        
        _rb = GetComponent<Rigidbody>();
        PosStatueGeant = firstTransStatue.position;

        rend = TerrainTremblant.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Custom/ShakingTerrain");
        //rend.material.SetFloat("_Strength", 2f);     
        ScaleTerrain = TerrainTremblant.transform.localScale;
        startStrength = rend.material.GetFloat("_Strength");
        
        startTimeToNextPart = TimeToNextPart;
        StartCoroutine(ShakeTerrain());
        
        // Time.timeScale = 0.2f;
    }

   
    private void Update()
    {
        if (Time.timeScale == 0f || end) return;
        if(bustMoving) BusteVersJoueur();
        if(EscapeChallenge && firstDeath) CounterToNextPart();
        
        
        float h = CrossPlatformInputManager.GetAxis("Vertical");
        float v = CrossPlatformInputManager.GetAxis("Horizontal");
        if (h > 0.1f || h < -0.1f || v > 0.1f || v < -0.1f) /// En mouvement.
        {
            float strength = 3f;
            bool check = false;
            if (lastBreathName != null) SoundManager.Instance.CheckSound(lastBreathName, out check);
            if (check) strength += 0.5f;
            rend.material.SetFloat("_Strength", strength);
        }
            

        bool vMotionLess = false;
        bool hMotionLess = false;
        if (h < 0.1f && h > -0.1f) /// Immobile.
            hMotionLess = true;            
        if (v < 0.1f && v > -0.1f)
            vMotionLess = true;
        if(vMotionLess && hMotionLess)
        {            
            bool check = false;
            if (lastBreathName != null) SoundManager.Instance.CheckSound(lastBreathName, out check);
            if(check) rend.material.SetFloat("_Strength", 3F);
            else rend.material.SetFloat("_Strength", 0f);
        }
            

        _playNextBreath += Time.deltaTime;
        if(_playNextBreath > 8f && !StopBreathing)
        {
            int chooseBreath = Random.Range(1, 4);
            lastBreathName = "RespiColere" + chooseBreath.ToString();
            SoundManager.Instance.Play(lastBreathName, 1f, Random.Range(0.3f, 0.6f));
            _playNextBreath = 0f;
        }
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (end) return;

        if (stopTrigger) 
        {            
            if(other.CompareTag("Finish")) /// Mort par Buste Grande Statue.
            {
                GameObject Statue = other.transform.parent.gameObject;
                if (Statue != InstanceGrandeStatue) Destroy(Statue);
                MortStatueGeante();
            }
            return; 
        }

        
        if (other.CompareTag("Mur")) return;

        if (other.CompareTag("Portail")) /// Col Pied Grande Statue.
        {
            if (ParentPetitesStatues != null) Destroy(ParentPetitesStatues);
            if (EscapeChallenge == false) GrandeStatuePart();
            controllerJoueur.m_WalkSpeed = 2f;
            controllerJoueur.JoueurColere = true;

            SoundManager.Instance.Play("ExplosionVerre", 1f, Random.Range(0.1f, 0.2f));

            Rigidbody[] rbs = other.transform.parent.gameObject.GetComponentsInChildren<Rigidbody>();    
            foreach (Rigidbody rb in rbs)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.useGravity = true;
                Transform busteTransform = rb.transform;
                if (rb.name == "Buste") BusteTransform = rb.transform;                
            }

            Vector3 explosionPos = centerExplosion.position;
            Vector3 explosionPos2 = explosionPos + new Vector3(0f, 40f, 0f);
            float radius = 25f;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null && rb.name != "Buste")
                {
                    rb.AddExplosionForce(10000f, explosionPos, radius, 3f);
                    rb.AddExplosionForce(10000f, explosionPos2, radius, 3f);
                    rb.AddExplosionForce(1000f, explosionPos2, 5f, 2f);
                }
            }

            SoundManager.Instance.Stop("Enfer");
            
            bustMoving = true;
            _rb.isKinematic = true; /// Sera désactivé dans tpJoueur()
            stopTrigger = true; /// Bloque ce if
            return;
        }      
    }

    private void MortStatueGeante()
    {
        bustMoving = false;
        BusteTransform = null;

        if(InstanceGrandeStatue != null)
        {
            Destroy(InstanceGrandeStatue);
        }
            
        InstanceGrandeStatue = Instantiate(StatueGeantPrefab, PosStatueGeant, StatueGeantPrefab.transform.rotation) as GameObject;
        Debug.Log(InstanceGrandeStatue);
        TpJoueurColere();

        rend.material.SetFloat("_Strength", 2f);
        Transform terrain = TerrainTremblant.transform;
        terrain.localScale = ScaleTerrain;

        if (!firstDeath)
        {
            firstDeath = true;
        }
        TimeInCounter = 0f;
        stopTrigger = false;

        if (!wasEnferSoundPlayed)
        {    
           // SoundManager.Instance.Play("Enfer");
            wasEnferSoundPlayed = true;
        }

        controllerJoueur.JoueurColere = false;

        SoundManager.Instance.Stop("ExplosionVerre");
        SoundManager.Instance.Play("Enfer", 0.8f, 1f);
    }

    private void BusteVersJoueur()
    {
        //Debug.Log("BusteVersJoueur()");        
        
        Transform busteTrsfrm = BusteTransform;
        //Vector3 targetPos = new Vector3(transform.position.x, busteTrsfrm.position.y, transform.position.z);
        float step = speedBuste * Time.deltaTime;
        Vector3 obj = transform.position;
        Vector3 dirNormalized = (transform.position - busteTrsfrm.position).normalized;
        busteTrsfrm.position = busteTrsfrm.position + dirNormalized * step;
       
        Quaternion targetRot = Quaternion.Euler(180f, 0f, 0f);
        BusteTransform.rotation = Quaternion.Slerp(BusteTransform.rotation, targetRot, 0.5f * Time.deltaTime);      
    }   
 
    private void TpJoueurColere()
    {
        var camComponent = transform.gameObject.GetComponentInChildren<Camera>();
        camComponent.enabled = false;

        transform.gameObject.GetComponent<CharacterController>().enabled = false; // Empeche de changer la position du transform.

        transform.position = targetCenterPos.position;

        FPS_Controller scriptJoueur = transform.gameObject.GetComponent<FPS_Controller>();

        scriptJoueur.m_MouseLook.m_CharacterTargetRot = targetCenterPos.rotation;
        scriptJoueur.m_MouseLook.m_CameraTargetRot = Quaternion.identity;

        transform.gameObject.GetComponent<CharacterController>().enabled = true;

        camComponent.enabled = true; // Camera

        controllerJoueur.m_WalkSpeed = startSpeed;
        TimeToNextPart = startTimeToNextPart;
    }

    private void CounterToNextPart() // Compteur vers la fin du chapitre
    {
        if (end) return;
       // TimeToNextPart -= 1f * Time.deltaTime;
        TimeInCounter += Time.deltaTime;


        if(TimeInCounter <= TimeToNextPart)
        {
            float smooth = TimeInCounter / TimeToNextPart;
            Transform terrain = TerrainTremblant.transform;
            Vector3 ReduceScale = new Vector3(terrain.localScale.x, 0f / smooth, terrain.localScale.z);
            terrain.localScale = Vector3.Lerp(ScaleTerrain, ReduceScale, smooth);
           // float strengthterrain = rend.material.GetFloat("_Strength");
           // rend.material.SetFloat("_Strength", Mathf.Lerp(strengthterrain, 0f, smooth));        
        }
        else
        { 
            Time.timeScale = 1f;
            SoundManager.Instance.Stop("Enfer");

            Cutscene colere = _objets.colereCutscene;            
            colere.SkipCurrentSection();
           /* if(colere.isPaused)
            colere.Resume();*/
                        
           /* if(InstanceGrandeStatue != null)
            {
                Destroy(InstanceGrandeStatue);
            }*/
           
            end = true;
        }

        if (TimeInCounter > TimeToNextPart / 0.7f) StopBreathing = true;
        else StopBreathing = false;
        

        //Debug.Log(TimeInCounter);
       
    }

    private void GrandeStatuePart()
    {
        TimeToNextPart = startTimeToNextPart;
        EscapeChallenge = true;
        Time.timeScale = 1f;
    }

    private IEnumerator ShakeTerrain()
    {        
        Transform terrain = TerrainTremblant.transform;
        float yScale = terrain.localScale.y;
        while (true)
        {
            if (TerrainTremblant.activeSelf)
            { 
                Vector3 scaleChange = new Vector3(terrain.localScale.x, yScale + Random.Range(0f, 25f), terrain.localScale.z);
                terrain.localScale = scaleChange;                           
            }
            yield return new WaitForEndOfFrame(); 
        }
    }

    
}