using UnityEngine;

public class SetPosToGround 
{
    public void SetPos(Transform t, string layer)
    {
       // t.position = new Vector3(t.position.x, 0f, t.position.z);
        t.position += (Vector3.up * 1000f);

        RaycastHit hit;
        int _layerMask = LayerMask.GetMask(layer);
        if (Physics.Raycast(origin: t.position, -t.up, out hit, Mathf.Infinity, _layerMask))
        {
            t.position -= new Vector3(0f, hit.distance, 0f);  /// On soustrait la distance qui le separe du sol.                  
        }
    }
}
