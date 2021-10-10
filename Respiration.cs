using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;


public class Respiration : MonoBehaviour
{

    public string _state;
    [SerializeField] float NextRespi;
    private float startNextRespi;
    private float startSpeed;
    private float startSpeedRun;
    private bool BreathQte = false;
    private bool wasInspiring = false;
    private bool qte = false;

    public Text ClicDroitTexte;
    private ListeObjets _objets;
    [SerializeField] FPS_Controller controllerScript;

    [Header("VisualEffects")]
    [HideInInspector] PostProcessVolume postProcess;
    [SerializeField] PostProcessProfile ppProfile;
    [SerializeField] Renderer rend;
    private Malaise malaise;
    [SerializeField] AnimationCurve curveColorGrad;

    [Header("UI")]
    [SerializeField] GameObject goRespiUI;
    [SerializeField] Animator _anim;
    [SerializeField] Graphic spriteRespi;
    private Color colorSpriteRespi = new Color();
    private Color noAlphaSprite;



    private void Start()
    {
        if (_objets == null) _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();
        if (controllerScript == null) controllerScript = transform.parent.GetComponent<FPS_Controller>();

        startSpeed = controllerScript.m_WalkSpeed;
        startSpeedRun = controllerScript.m_RunSpeed;
        startNextRespi = NextRespi;

        /// PostProcess ColorGrad.
        postProcess = transform.parent.gameObject.AddComponent<PostProcessVolume>();
        postProcess.profile = ppProfile;
        postProcess.isGlobal = true;
        postProcess.weight = 0f;

        /// Shader Malaise avec un mesh devant le joueur.
        rend.material.shader = Shader.Find("Custom/Chaleur");
        if (spriteRespi == null) spriteRespi = goRespiUI.GetComponentInChildren<Image>();
        colorSpriteRespi = spriteRespi.color;
        noAlphaSprite = colorSpriteRespi - new Color(0f, 0f, 0f, colorSpriteRespi.a);
        spriteRespi.color = noAlphaSprite;
    }

    private void OnEnable()
    {
        if (_objets == null) _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();

        goRespiUI.SetActive(false);
        rend.gameObject.SetActive(false);
        rend.material.SetFloat("_Size", 0f);
        rend.material.SetFloat("_Speed", 0f);
        rend.material.SetFloat("_Strenght", 0f);

        if (spriteRespi != null) spriteRespi.color = noAlphaSprite;

        StartCoroutine(Respi());

    }

    private void OnDisable()
    {
        goRespiUI.SetActive(false);
        StopAllCoroutines();
    }

    private void Update()
    {
        if (Time.timeScale == 0f || BreathQte) return;

        if (!qte && Input.GetKeyDown(KeyCode.Mouse1))
        {
            int chanceToChoke = 0;
            chanceToChoke = Random.Range(0, 7);
            if (Random.Range(0, 7) == 7)
                PlayTouxSound();
        }

    }

    private IEnumerator Respi()
    {
        // Debug.Log("Coroutine Respi");


        float startTimeToChoke = 2f;
        float timeToChoke = startTimeToChoke; // Temps avant échec
        qte = false;
        int fail = 0;
        _anim.SetBool("ClicDroit", false);
        goRespiUI.SetActive(false);

        while (true)
        {
            BreathQte = false;
            yield return new WaitForSeconds(1f);
            StartCoroutine(ColorGradLerp(true)); /// N&B
            yield return new WaitForSeconds(NextRespi);

            SoundManager.Instance.Play("Suffocation", Random.Range(0.4f, 1f), Random.Range(0.9f, 1.1f));
            goRespiUI.SetActive(true);
            qte = true;
            BreathQte = true;

            while (qte == true)
            {
                _anim.SetBool("ClicDroit", true);

                if (Input.GetKeyDown(KeyCode.Mouse1)) //Réussite = Son
                {

                    SoundManager.Instance.Stop("Suffocation");
                    string expi = "Expi";
                    string inspi = "Inspi";

                    if (_objets.CamJoueur.isSceneMots)
                    {
                        expi = "ExpiMots";
                        inspi = "InspiMots";
                    }
                   
                    if (wasInspiring)
                    {
                        PlayRespiSound(expi);
                        wasInspiring = false;
                    }
                    else
                    {
                        PlayRespiSound(inspi);
                        wasInspiring = true;
                    }
                 
                 
                    fail = 0;
                    controllerScript.m_WalkSpeed = startSpeed;
                    controllerScript.m_RunSpeed = startSpeedRun;
                    StartCoroutine(ColorGradLerp(false));
                    StartCoroutine(lerpMalaise(false, 1f));
                    // StartCoroutine(SpriteLerp(false));
                    NextRespi = startNextRespi;
                    timeToChoke = startTimeToChoke;
                    qte = false;
                    yield return qte;
                }

                timeToChoke -= Time.deltaTime;
                if (timeToChoke <= 0f && qte) // Echec
                {
                    fail++;
                    if (fail < 3) SoundManager.Instance.Play("Verre", 0.8f, 1f);

                    if (fail == 1)
                    {
                        // Bruit d'étouffement
                        controllerScript.m_WalkSpeed = 0.5f;
                        controllerScript.m_RunSpeed = 0.5f;
                        StartCoroutine(lerpMalaise(true, 2f));
                    }

                    if (fail == 2)
                    {
                        controllerScript.m_WalkSpeed = 0.1f;
                        controllerScript.m_RunSpeed = 0.1f;
                        StartCoroutine(lerpMalaise(true, 1f));
                        Debug.Log("Fail2");
                    }

                    //StartCoroutine(SpriteLerp(false));
                    PlayTouxSound();
                    NextRespi = 3f;
                    timeToChoke = 5f;
                    qte = false;
                }

                yield return new WaitForEndOfFrame();
                yield return qte;
            }

            _anim.SetBool("ClicDroit", false);
            goRespiUI.SetActive(false);

        }


    }

    public void SetBreathingState(string s)
    {
        _state = s;
    }
    
    private void PlayRespiSound(string nom)
    {
        int chooseSound = Random.Range(1, 3);
        float ptch = Random.Range(0.9f, 1.1f);
   
        if (chooseSound == 1) nom += "1";
        if (chooseSound == 2) nom += "2";
        SoundManager.Instance.Play(nom, 1, ptch);
    }    

    private void PlayTouxSound()
    {
        int ran = Random.Range(1, 4);
        float vol = 1f;
        float pit = Random.Range(0.9f, 1.1f);
        string name = "Toux" + ran.ToString();
        SoundManager.Instance.Play(name, vol, pit); return;
    }

    private IEnumerator lerpMalaise(bool Activate = false, float Divide = 1f, float desiredSize = 10f, float desiredSpeed = 1f, float desiredStrength = 10f)
    {
        float tempsT = 0f;
        float timeToLerp = new float();

        float startSize =  rend.material.GetFloat("_Size");
        float startSpeed = rend.material.GetFloat("_Speed");
        float startStrength = rend.material.GetFloat("_Strength");

        if (Activate)
        {
            rend.gameObject.SetActive(true);

            timeToLerp = 2f;
            desiredSize = desiredSize / Divide;
            desiredSpeed = desiredSpeed / Divide;
            desiredStrength = desiredStrength / Divide;
            //if (rend.material.GetFloat("_Size") == desiredSize) yield break;
        }
        else
        {
            timeToLerp = 1f;
            desiredSize = 0f;
            desiredSpeed = 0f;
            desiredStrength = 0f;
        }

        while (true)
        {
             if(tempsT <= timeToLerp)
             {
                 tempsT += Time.deltaTime; 
                 float actualTime = tempsT / timeToLerp;                    
                 rend.material.SetFloat("_Size", Mathf.Lerp(startSize, desiredSize, actualTime));
                 rend.material.SetFloat("_Speed", Mathf.Lerp(startSpeed, desiredSpeed, actualTime));
                 rend.material.SetFloat("_Strenght", Mathf.Lerp(startStrength, desiredStrength, actualTime));                    
                 yield return new WaitForEndOfFrame();
                 yield return null;
             }
             else
             {
                 if (!Activate) rend.gameObject.SetActive(false);
                 Debug.Log("EndMalaise");
                 yield break;
             }       
                             
        }

    }
    
    private IEnumerator ColorGradLerp(bool Activate)
    {
        //if (colorGrad.temperature.Equals(desiredTemperature)) yield break;

        float tempsC = 0f;
        float timeToLerp;
        float desiredWeight;

        if (Activate)
        {
            desiredWeight = 1f; /// N&B
            timeToLerp = NextRespi - 0.1f;
        }
        else
        {
            desiredWeight = 0f;
            timeToLerp = 1f;
        }

        float startWeight = postProcess.weight;

        while (true)
        {

            if (tempsC <= timeToLerp)
            {
                float currentTime = tempsC / timeToLerp;
                if(Activate)
                    postProcess.weight = Mathf.Lerp(startWeight, desiredWeight, curveColorGrad.Evaluate(currentTime));
                if(!Activate)
                    postProcess.weight = Mathf.Lerp(startWeight, desiredWeight, currentTime);
                yield return new WaitForEndOfFrame();
                tempsC += Time.deltaTime;   

            }
            else
            {
                Debug.Log("EndColorGradLerp" + Activate);
                yield break;
            }

        }
       

    }

    private IEnumerator SpriteLerp(bool Activate = false)
    {
        float desiredLerp = 0.3f * Time.deltaTime;

        if (Activate)
        {
            if (spriteRespi.color == colorSpriteRespi) yield break;
            float tempsS = 0f;

            while (true)
            {
                spriteRespi.color = Color.Lerp(spriteRespi.color, colorSpriteRespi, desiredLerp);
                tempsS += Time.deltaTime;
                if (tempsS >= 2f) yield break;
            }            
        }
        else
        {
            if (spriteRespi.color == noAlphaSprite) yield break;
            float tempsS2 = 0f;
            
            while (true)
            {
                spriteRespi.color = Color.Lerp(spriteRespi.color, noAlphaSprite, desiredLerp);
                tempsS2 += Time.deltaTime;
                if (tempsS2 >= 2f) yield break;
            }
        }
    }

    private IEnumerator postProcessLerp(float desiredWeight)
    {
        if (postProcess.weight == desiredWeight) yield break;

        while (true)
        {
            postProcess.weight = Mathf.Lerp(postProcess.weight, desiredWeight, Time.deltaTime);
            if(postProcess.weight == desiredWeight) yield return false;
            yield return new WaitForEndOfFrame();            
        }        
    }

}
