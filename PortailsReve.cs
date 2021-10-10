using UnityEngine;

public class PortailsReve : MonoBehaviour
{
    private GameObject[] Portails;
    private SetPosToGround PosToGround = new SetPosToGround();
    [SerializeField]
    private Transform MysteriousShape;

    void Start()
    {
        Portails = GameObject.FindGameObjectsWithTag("Portail");
        foreach(GameObject _p in Portails)
        {
            PosToGround.SetPos(_p.transform, "Ground");
        }
    }

    


}
