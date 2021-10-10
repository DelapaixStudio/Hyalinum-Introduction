using UnityEngine;
using UnityEngine.UI;

namespace Slate.ActionClips
{
    [Category("Rendering")]
    public class SetChapterText : DirectorActionClip
    {
        [SerializeField]
        [HideInInspector]
        private ListeObjets _objets;

        [SerializeField]
        [HideInInspector]
        private float _length = 2;
 

        [SerializeField] string LineCode = "";
        [HideInInspector]
        [SerializeField] Text NomChapitre;
        [HideInInspector]
        [SerializeField] Text NumeroChapitre;
        [SerializeField] GameObject TxtObject;                 


        protected override bool OnInitialize()
        {
            _objets = GameObject.FindWithTag("ObjectManager").GetComponent<ListeObjets>();
            if (TxtObject == null) TxtObject = _objets.ChapitreTxtGo;
            if (NomChapitre == null) NomChapitre = _objets.NomTxt;
            if (NumeroChapitre == null) NumeroChapitre = _objets.NumTxt;
            return true;
        }

        protected override void OnEnter()
        {
            if (!Application.isPlaying) return;
            string num = null;
            string nom = null;
            string LineCodeNum = LineCode;
            string LineCodeNom = LineCode;
            LineCodeNum += "NUM";
            LineCodeNom += "NOM";
            DialogueManager.Instance.GetText(LineCodeNum, out num);
            NumeroChapitre.text = num;
            DialogueManager.Instance.GetText(LineCodeNom, out nom);
            NomChapitre.text = nom;

            TxtObject.SetActive(true);
        }

        protected override void OnExit()
        {
            TxtObject.SetActive(false);
        }

        public override string info
        {
            get { return string.Format("'{0}'", LineCode); }
        }

        public override float length
        {
            get { return _length; }
            set { _length = value; }
        }

    }
}
