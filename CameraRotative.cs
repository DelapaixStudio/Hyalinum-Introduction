using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

public class CameraRotative : MonoBehaviour
{
    [SerializeField]
    private MouseLook _mouseLook;

    void Start()
    {
        _mouseLook.Init(transform, transform);
    }
    private void Update()
    {
        _mouseLook.LookRotation(transform, transform);
    }

}
