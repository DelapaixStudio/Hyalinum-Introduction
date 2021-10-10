using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMouse : MonoBehaviour
{

    [SerializeField] Text _text;
    [SerializeField] Color startColor;
    [SerializeField] SoundManager _audio;

    void Start()
    {
        _text = GetComponent<Text>();
        startColor = _text.color;
    }

    private void OnMouseOver()
    {
        
    }

    private void OnMouseExit()
    {
        _audio.Play("Menu1");
    }
}
