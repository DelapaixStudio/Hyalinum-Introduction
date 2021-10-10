using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

namespace Slate.ActionClips
{
    [Category("Utility")]
    public class TeleportPlayer : DirectorActionClip
    {
        [SerializeField]
        private Transform targetTransform;
        private ListeObjets _objets;
        [SerializeField]
        private bool joueurColere = false;
        
    
        protected override bool OnInitialize()
        {
            _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();                   
              
            return true;
        }
        

        protected override void OnEnter()
        {
            if (!Application.isPlaying) return;

            Transform _transform = null;
            if (joueurColere) _transform = _objets.JoueurCinematiqueColere.transform;
            if(!joueurColere) _transform = _objets.player.transform;


            var camComponent = _transform.gameObject.GetComponentInChildren<Camera>();
            camComponent.enabled = false;
            
            _transform.gameObject.GetComponent<CharacterController>().enabled = false; // Empeche de changer la position du transform.
            
            _transform.position = targetTransform.position;

            FPS_Controller scriptJoueur = _transform.gameObject.GetComponent<FPS_Controller>();
                       
            scriptJoueur.m_MouseLook.m_CharacterTargetRot = targetTransform.rotation;
            scriptJoueur.m_MouseLook.m_CameraTargetRot = Quaternion.identity;
               
            _transform.gameObject.GetComponent<CharacterController>().enabled = true;
    
            camComponent.enabled = true; // Camera
        }
    }
}


