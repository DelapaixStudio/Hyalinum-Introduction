using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class MenuPrincipal : Menu
{
    [SerializeField] GameObject ContinuerUI;
    [SerializeField] Text TexteNouvellePartie;
    [SerializeField] InputField _inputField;
    [SerializeField] Transform FondAmovible;
    private Vector3 startPosFondAmovible;

    private bool hasClicked = false;

    protected override void Start()
    {        
        startPosFondAmovible = FondAmovible.position;
        ///transform.GetChild(0).gameObject.SetActive(false);
        Cursor.visible = true;

        base.Start();
       
        SetTexts();      

        if (SaveManager.SaveInstance.ProgressPoint > 0) ContinuerUI.SetActive(true);
        else ContinuerUI.SetActive(false);
        ///transform.GetChild(0).gameObject.SetActive(true);     
    }


    private void Update()
    {
        var x = CrossPlatformInputManager.GetAxis("Mouse X");
        var y = CrossPlatformInputManager.GetAxis("Mouse Y");

        Vector3 pos = FondAmovible.position;
        FondAmovible.position = new Vector3(pos.x - x, pos.y - y, pos.z);
    }


    public void Langue()
    {
        if (hasClicked) return;

        SaveManager.SaveInstance.currentlanguage = OngletLangues.value;
        DialogueManager.Instance.Reader();
        SetTexts();
    }

    public void InputChapter(string chapter)
    {
        SaveManager.SaveInstance.ProgressPoint = int.Parse(_inputField.text);
    }

    public void Continuer()
    {
        if (hasClicked) return;

        Debug.Log("Continuer");
        Text TexteContinuer = ContinuerUI.GetComponentInChildren<Text>(); 
        StartCoroutine(LoadAsync(TexteContinuer)); /// On a besoin du texte du boutton pour y ajouter le pourcentage.
        Common();
    }

    public void NouvellePartie()
    {
        if (hasClicked) return;

        Debug.Log("Nouvelle Partie");
        SaveManager.SaveInstance.ProgressPoint = 0;
        Common();

        StartCoroutine(LoadAsync(TexteNouvellePartie)); /// On a besoin du texte du boutton pour y ajouter le pourcentage.
        
    }

    private void Common()
    {
        SoundManager.Instance.Play("Menu1");
        hasClicked = true;
        SaveManager.SaveInstance.SaveData();
    }

    public void LienItch()
    {
        Application.OpenURL("https://delapaix.itch.io");
    }

    public void Quitter()
    {
        if (hasClicked) return;

        Debug.Log("Quitter");

        QuitGame(); /// Vérifie si la langue est la même que au chargement, sauvegarde et quitte.
        hasClicked = true;
    }

    IEnumerator LoadAsync(Text _text)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Desert");
        string initialText = _text.text;       

        while (!operation.isDone)
        {
            float progress = 100f * (operation.progress / 0.9f);
            _text.text = initialText + " : " + progress + "%";
//            Debug.Log(operation.progress);
            yield return null;
        }

        hasClicked = false;

    }

    IEnumerator PlaySound()
    {
        SoundManager.Instance.Play("Menu1");

        while (true)
        {
           SoundManager.Instance.CheckSound("Menu1", out bool play);
            if (play) yield return null;
            else yield return false;

        }      
    }

    private void SetTexts()
    {
        Text[] Texts = GetComponentsInChildren<Text>();
        foreach(Text txt in Texts)
        {
            TextData textdata = txt.gameObject.GetComponent<TextData>();
            if(textdata != null)
            {
                DialogueManager.Instance.GetText(textdata.lineCode, out string getTxt);
                txt.text = getTxt;
            }
        }
    }

    
}
