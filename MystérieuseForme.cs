using UnityEngine;

public class MystérieuseForme : MonoBehaviour
{
    private ListeObjets _objets;
    private GameManager gManager;
    [SerializeField]
    private float yPosGround;
    [SerializeField]
    private Vector3 moveUnitsPerSecond;
    [SerializeField]
    private Vector3 rotateDegreesPerSecond;

    void Start()
    {
        _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();
        gManager = _objets._gameManager;
    }


    void Update()
    {
        if(transform.position.y > yPosGround)
        {
            transform.Translate(moveUnitsPerSecond * Time.deltaTime, Space.World);
            transform.Rotate(rotateDegreesPerSecond * Time.deltaTime, Space.World);
        }
    }
}
