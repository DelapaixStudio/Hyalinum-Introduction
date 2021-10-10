using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class IgnoreCol : MonoBehaviour
{  
    [SerializeField] private bool getAllColsAndMeshCol = false;
    
    [Header("Target")]
    [SerializeField] Collider WantedCol;
    [SerializeField] MeshCollider WantedMeshCol;
    
    [Header("Colliders To Ignore")]
    [SerializeField] MeshCollider[] IgnoreMeshCols;
    [SerializeField] Collider[] IgnoreCols;

    

    void Start()
    {
        if (getAllColsAndMeshCol)
        {
           IgnoreCols = GetComponentsInChildren<Collider>();
           IgnoreMeshCols = GetComponentsInChildren<MeshCollider>();
        }


        if (WantedCol)
        {
            foreach(Collider col in IgnoreCols)
                Physics.IgnoreCollision(col, WantedCol);

            foreach (MeshCollider meshCol in IgnoreMeshCols)
                Physics.IgnoreCollision(meshCol, WantedCol);            
        }

        if (WantedMeshCol)
        {
            foreach (Collider col in IgnoreCols)
                Physics.IgnoreCollision(col, WantedMeshCol);

            foreach (MeshCollider meshCol in IgnoreMeshCols)
                Physics.IgnoreCollision(meshCol, WantedMeshCol);
        }

    }

    
}
