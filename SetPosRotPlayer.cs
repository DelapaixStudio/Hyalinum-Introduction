using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class SetPosRotPlayer : MonoBehaviour
{
    
    public void SetPosAndRot(Transform _transformPlayer, Transform targetTransform)
    {
        var camComponent = _transformPlayer.gameObject.GetComponentInChildren<Camera>();
        camComponent.enabled = false;

        _transformPlayer.gameObject.GetComponent<CharacterController>().enabled = false; // Empeche de changer la position du transform.

        _transformPlayer.position = targetTransform.position;

        FPS_Controller scriptJoueur = _transformPlayer.gameObject.GetComponent<FPS_Controller>();

        scriptJoueur.m_MouseLook.m_CharacterTargetRot = targetTransform.rotation;
        scriptJoueur.m_MouseLook.m_CameraTargetRot = Quaternion.identity;

        _transformPlayer.gameObject.GetComponent<CharacterController>().enabled = true;

        camComponent.enabled = true; // Camera
    }
}
