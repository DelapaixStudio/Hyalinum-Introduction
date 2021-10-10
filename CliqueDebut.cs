using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class CliqueDebut : MonoBehaviour
{
    [SerializeField]
    private ListeObjets _objets;   

    private Image chargement;
    private Image _this;
    private Color CouleurSprite;
    [SerializeField] private float _speedMouseOn;

    private bool _estFini = false;

    private Animator _Animator;

    private float _currentAmount;
    private float _mouseOnMoon;
    [SerializeField]  private float _pitch;
    private Vector3 startScaleLune;


    [SerializeField] private float _speed;
    private Color StartLuneColor;
    private Color transitionColor;
    [SerializeField] private Color LuneEclairee;
    [SerializeField] private GameObject Cam;


    public void Start()
    {
        Cursor.visible = false;
        if (_objets == null) _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();
        _Animator = GetComponent<Animator>();        
        _this = GetComponent<Image>();
        StartLuneColor = _this.color;        
        CouleurSprite = _this.color;
        _currentAmount = 0;
        startScaleLune = transform.localScale;

        SoundManager.Instance.Play("LuneProche");
        SoundManager.Instance.Set("LuneProche", 0f, 0f);
    }

    private void Update()
    {
        transform.LookAt(Cam.transform);
    }

    public void HitRaycast(bool ray) // Appel depuis Update CameraDebut.cs
    {
        if (_estFini) return;        

        if (ray)  // RAYCAST ON
        {         
            CouleurSprite = Color.Lerp(CouleurSprite, LuneEclairee, 1 / _speedMouseOn);
            _this.color = CouleurSprite;
                        
            OnMoon();            
        }
        else  // RAYCAST OFF
        {  
            CouleurSprite = Color.Lerp(CouleurSprite, StartLuneColor, 1 / _speedMouseOn);
            _this.color = CouleurSprite;
            SoundManager.Instance.Set("LuneProche", 0f, 0f);

            if (_currentAmount < 100 && _currentAmount > 0) // Si l'action n'est pas complète.
            {     _currentAmount -= _speed * 5f * Time.deltaTime;    }
        }
    }

    public void  Stop()
    {

        SoundManager.Instance.Stop("LuneProche");
        SoundManager.Instance.Play("LuneOnMouse1");
        StartCoroutine(Cam.GetComponent<CameraDebut>().CinematiqueLune());
        _estFini = true;
        //SpriteChargement.layer = Objets.layerMaskUI;
        gameObject.GetComponent<BoxCollider>().enabled = false; // Box collider de la Lune
        enabled = false;
        
          //  Destroy(this);      
    }

    private void OnMoon() // Appelé si le Raycast est sur la Lune.
    {

        if (_currentAmount >= 100)
        {
           Stop();
           return;
        }
       
       
        if ( _currentAmount < 100 ) // Le chargement inferieure à 100 donc non fini
        {
            if ( Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1) ||
                  Input.GetKey(KeyCode.Mouse2) )
            {
              // Stop(); //   <==== PENSER A LE DESACTIVER *!*!*!*!*!*!*!*!!!!!!!
               _currentAmount += _speed * Time.deltaTime;
               float pitchOnMouse = _pitch + (_currentAmount / 100) / 3;
               SoundManager.Instance.Set("LuneProche", pitchOnMouse,  _pitch);

               float desiredScale = (_currentAmount / 100);
                transform.localScale = startScaleLune + new Vector3(desiredScale, desiredScale, 0f);
            }
            else if(_currentAmount >= 0)
            {                    
                _currentAmount -= _speed * 5f * Time.deltaTime;
                float pitchOnMouse = _pitch + ((_currentAmount / 100) / 2);
                SoundManager.Instance.Set("LuneProche", pitchOnMouse, _pitch);

                float desiredScale = (_currentAmount / 100);
                transform.localScale = startScaleLune + new Vector3(desiredScale, desiredScale, 0f);
            }
            else
            {
                SoundManager.Instance.Set("LuneProche", 0.1f, _pitch);
                transform.localScale = startScaleLune;
            }
           
        }
    }

}
