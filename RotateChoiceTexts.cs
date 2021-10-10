using UnityEngine;

public class RotateChoiceTexts : MonoBehaviour
{
    public Vector3 rotateTexts;
    private float m_LastRealTime;

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        transform.Rotate(rotateTexts * deltaTime, Space.World);
    }
}
