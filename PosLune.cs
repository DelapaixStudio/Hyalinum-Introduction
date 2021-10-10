using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosLune : MonoBehaviour
{
    [SerializeField] ListeObjets _objets;
    private Transform joueur;
    void Start()
    {
        if (_objets == null) _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();
        joueur = _objets.player.transform;
    }

    
    void Update()
    {
        if (Time.timeScale == 0f) return;
        
        Vector3 _joueurPos = joueur.position;        
        transform.position = new Vector3(0f, transform.position.y, _joueurPos.z + 1500f);
        //transform.LookAt(joueur.position);
    }
}
