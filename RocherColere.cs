using UnityEngine;
using System.Collections;


public class RocherColere : MonoBehaviour
{
    private bool stopTrig = false;
    [HideInInspector]
    [SerializeField] Rigidbody[] rbs;
    [SerializeField] Collider[] cols;
    [SerializeField] Collider playerCol;


    [SerializeField] ListeObjets _objets;
    

    private void Start()
    {
        _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();
        if (playerCol) return;
        playerCol = _objets.JoueurCinematiqueColere.GetComponent<Collider>();           
    }

    private void OnTriggerEnter(Collider other) // Appelle depuis TriggerRocher.cs
    {      

        if (stopTrig) return;
        if (other.CompareTag("Mur")) return;

        SoundManager.Instance.Play("Verre", 1f, Random.Range(0.5f, 0.9f));
        
        /*
        foreach(Transform child in transform)
        {
            Rigidbody rb = child.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rb.mass = .1f;
        }*/

        rbs = new Rigidbody[transform.childCount];        
        for (int i = 0; i < transform.childCount; i++)
        {
            Rigidbody rb = transform.GetChild(i).gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rb.mass = 0.1f;
            rbs[i] = rb;
        }

        StartCoroutine(ManageCol());
        stopTrig = true;        
    } 

    private IEnumerator ManageCol()
    {
        Debug.Log("ManageCol");
        Time.timeScale = 0.5f;
        bool slowMo = true;
        int compteur = 0;
        float time = 0f;

        while (true)
        {

            if (compteur > 1) /// On laisse une frame de collision pour donner une direction à la chute des meshs.
            {
                foreach (Collider col in cols) Physics.IgnoreCollision(playerCol, col);
                Time.timeScale = 1f;
            }
            /*
            if (time > 0.1f && slowMo)
            {
                Time.timeScale = 1f;
                slowMo = false;
            }
            else Time.timeScale = 0.5f;*/

            if(time > 5f)
            { 
                Time.timeScale = 1f;          

                foreach(Rigidbody rb in rbs)
                {
                    Destroy(rb);
                }
               
                yield break;
            }

            yield return new WaitForEndOfFrame();
            compteur++;
            time += 1f * Time.deltaTime;
        }
        
    }

}
