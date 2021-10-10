using UnityEngine;

[ExecuteInEditMode]
public class CloudsVisibility : MonoBehaviour
{
    public Material _clouds;
    public float _visibility = -0.2f;

    private void Start()
    {
        _clouds.SetFloat("_Coverage", _visibility);
    }
    private void Update()
    {
        _clouds.SetFloat("_Coverage", _visibility);
    }
}
