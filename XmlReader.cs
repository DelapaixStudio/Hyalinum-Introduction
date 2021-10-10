using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class XmlReader : MonoBehaviour
{
    public TextAsset dictionary;  // C'est le fichier xml.

    public string languageName;
    public int currentLanguage;

    public GameObject textesPensees;
    private Text[] _pensees;
    string[] _textepensee;

    List<Dictionary<string, string>> languages = new List<Dictionary<string, string>>();
    Dictionary<string, string> obj;

    void Start()
    {
        Reader();

        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
            _pensees = GetComponentsInChildren<Text>();
        //   _pensees[i] = transform.GetChild(i).GetComponent<Text>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0)) Cursor.visible = true; else Cursor.visible = false;
        //languages[currentLanguage].TryGetValue("Name", out languageName);
        languages[1].TryGetValue("txt1", out _textepensee[0]);
        /*languages[currentLanguage].TryGetValue("txt2", out _textepensee[1]);
        languages[currentLanguage].TryGetValue("txt3", out _textepensee[2]);
        languages[currentLanguage].TryGetValue("txt4", out _textepensee[3]);*/

        for (int i = 0; i < _pensees.Length; ++i)
            _pensees[i].text = _textepensee[i];
    }

    void Reader()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(dictionary.text);
        XmlNodeList languageList = xmlDoc.GetElementsByTagName("language");

        foreach (XmlNode languageValue in languageList)
        {
            XmlNodeList languageContent = languageValue.ChildNodes;
            obj = new Dictionary<string, string>();

            foreach (XmlNode value in languageContent)
            {
                if (value.Name == "Name")
                {
                    obj.Add(value.Name, value.InnerText);
                }
                if (value.Name == "P1")
                {
                    XmlNodeList txtcontent = value.ChildNodes;
                    for (int i = 0; i < txtcontent.Count; ++i)
                        obj.Add(txtcontent[i].Name, txtcontent[i].InnerText);
                }

            }
        }

        languages.Add(obj);

    }
}
