using UnityEngine;

public class TriggerRocher : MonoBehaviour
{  
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.SendMessage("OnTriggerEnter", other);
        Destroy(this);
    }
}
