using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Slate;

public class CtrlSlate : MonoBehaviour
{
    public Cutscene _cutscene;
    

    void Start()
    {
        _cutscene.Play();
        
    }

}
