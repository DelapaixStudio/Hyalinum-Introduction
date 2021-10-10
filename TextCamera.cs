using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class TextCamera : MonoBehaviour
{
    public ListeObjets _objets;

    [SerializeField] private MouseLook m_MouseLook;
    RaycastHit hit;
    int _layerMask;

    private Camera _CamComponent;
    private DialogueManager _DialogueManager;
    [SerializeField] [HideInInspector]
    private RotateChoiceTexts scriptRotation;

    private bool _Wait = true;
    private string _colliderConfirm = null;
    private string _Collider;
    private float TimeToConfirm = 3;
    private float ReachConfirm = 0;
    private float startRotSpeedTexts;
    private float _startFOV; // FielOfView = Focale.


    void Start()
    {
        _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();
        m_MouseLook.Init(transform, transform);
        _layerMask = LayerMask.GetMask("UI");
        _CamComponent = GetComponent<Camera>();
        if (scriptRotation == null) scriptRotation = _objets.DialoguesChoix.GetComponent<RotateChoiceTexts>();
        startRotSpeedTexts = scriptRotation.rotateTexts.y;
        _startFOV = _CamComponent.fieldOfView;
    }

    void Update()
    {
        if (_objets.DialoguesChoix.activeSelf)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
               Cursor.visible = true;
               _CamComponent.enabled = true;
               m_MouseLook.LookRotation(transform, transform);
             
               Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);
               if (Physics.Raycast(origin: transform.position, transform.forward, out hit, 10f, _layerMask))
               {
                    _CamComponent.fieldOfView = 50f;
                   scriptRotation.rotateTexts.y = -10f;
                   _Collider = hit.collider.gameObject.name;
                   if (_Collider == "Select Dial 1"){ checkConfirm(_Collider, 0); return;} 
                   if (_Collider == "Select Dial 2"){ checkConfirm(_Collider, 1); return;}
                   if (_Collider == "Select Dial 3"){ checkConfirm(_Collider, 2); return;}
                   if (_Collider == "Select Dial 4"){ checkConfirm(_Collider, 3); return;}                    
               }
                else
                {
                    scriptRotation.rotateTexts.y = startRotSpeedTexts;
                    _CamComponent.fieldOfView = _startFOV;
                }
            }
            else 
            {
                Cursor.visible = false;
                _CamComponent.enabled = false;
                transform.localRotation = _objets.CamJoueur.gameObject.transform.GetChild(0).transform.localRotation;
                scriptRotation.rotateTexts.y = startRotSpeedTexts;
                _CamComponent.fieldOfView = _startFOV;

            }
        }     
        else
            _CamComponent.enabled = false;


    }


    private void checkConfirm(string colName, int answer)
    {
        if(colName == _colliderConfirm)
        {
            ReachConfirm += Time.deltaTime;
        }
        else
        {
            ReachConfirm = 0f;
        }
        
        if (ReachConfirm > TimeToConfirm)  Stop(answer);
        _colliderConfirm = colName;
    }
    
    private void Stop(int answer)
    {
        //_DialogueManager.Answer(answer);
        ReachConfirm = 0f;
        enabled = false;
    }
}
