using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using Slate;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextAsset[] dictionary;
    [SerializeField]
    Dictionary<string, string> obj = new Dictionary<string, string>();
        
    public int _currentStoryPart;
    private string _choosenPath;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }       

    public void GetText(string LineCode, out string txt, string type = "simple", int id = 0)
    {
        string result = null;                 

        if (type == "simple")
            obj.TryGetValue(LineCode, out result); 
        
        if (type == "simplewithpath")
            obj.TryGetValue(LineCode + _choosenPath + id, out result);  
            
        txt = result;
    }

    public void Reader()
    {
        // ATTENTION : SI ID MANQUANT ERROR DANS LE DICO OBJ CAR IL NE PEUT PAS AJOUTER DEUX FOIS LA MEME CLEF
        // --> Et le fichier xml ne sera pas lu jusqu'au bout.


        Debug.Log("Reader()");
        TextAsset dico = dictionary[SaveManager.SaveInstance.currentlanguage];
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(dico.text);
        XmlNodeList txtList = xmlDoc.SelectSingleNode("dialogs").ChildNodes; /// On récupere tous les nodes à partir du root <dialogs></dialogs>
        obj.Clear(); /// On vide le dico sinon ERROR si il y a déjà des données sérialisés.

        foreach (XmlNode nodeContent in txtList) /// NodeContent = SCENE.
        {
            XmlNodeList txtContent = nodeContent.ChildNodes;           

            //                                     TEXTE
            XmlNodeList txtNodes = xmlDoc.SelectNodes("/dialogs/" + nodeContent.Name + "//txt");
            foreach(XmlNode txtCont in txtNodes)
            {
                obj.Add(nodeContent.Name + txtCont.Attributes["id"].Value, txtCont.InnerText);
                ///Debug.Log(nodeContent.Name + txtCont.Attributes["id"].Value);
            }  

        }
    }

   
}
