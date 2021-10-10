using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;
using TMPro;
using UnityEngine.UI;


[ExecuteInEditMode]
public class MotsVolants : MonoBehaviour
{
    [SerializeField] private AutoMoveAndRotate[] MoveMotsVolants;
    [SerializeField] private ListeObjets _objets;
    [SerializeField] BoxCollider[] boxCols;
    void Start()
    {
        MoveMotsVolants = GetComponentsInChildren<AutoMoveAndRotate>();
        if(_objets == null) _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();

      
        // boxCols = GetComponentsInChildren<BoxCollider>();
        foreach(BoxCollider col in boxCols)
        {
            foreach(BoxCollider box in boxCols)
            {
                Physics.IgnoreCollision(col, box);
            }          
        }

    }

  
    public void SetTextWords()
    {
        TextMeshProUGUI[] txts = GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI t in txts)
        {
            // string code = "MER" + Random.Range(1,4).ToString();  

            string word = "MOTS0" + Random.Range(1, 4).ToString();
            DialogueManager.Instance.GetText(word, out string getTxt);
            t.text = getTxt;

        }
    }


    public void SetWordsPos()
    {
        Debug.Log("SetWordsPos");

        Transform[] trans = GetComponentsInChildren<Transform>();
        foreach(Transform t in trans)
        {
            t.position -= new Vector3(0f, t.position.y, 0f);
        }

        foreach (BoxCollider box in boxCols)
        {
            
            Transform Mot = box.transform;
            box.enabled = false;

           // Mot.position -= new Vector3(0f, Mot.position.y, 0f);



            Mot.position += (Vector3.up * 100f); /// On met le GO en hauteur pour calculer sa distance par rapport au sol.

            RaycastHit hit;
            int _layerMask = LayerMask.GetMask("Ground");
            if (Physics.Raycast(origin: Mot.position, -Mot.up, out hit, Mathf.Infinity, _layerMask))
            { 
                Mot.position -= new Vector3(0f, hit.distance - 1f, 0f);  /// On soustrait la distance qui le separe du sol.  
                // Mot.localEulerAngles = new Vector3(hit.normal.x, 0f, hit.normal.z);   
                // Mot.transform.rotation = Quaternion.Euler(hit.normal.x, Mot.transform.rotation.y, hit.normal.z);
                Mot.LookAt(_objets.player.transform);                
            }

            // child.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);          
            box.enabled = true;
        }       

    

    }

    public void FinSceneMots()
    {
      //  Destroy(gameObject);
    }


  

}
