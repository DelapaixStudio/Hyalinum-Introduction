using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CameraPuits : MonoBehaviour
{
    public UnityEngine.Video.VideoPlayer Video;
    private bool _video = false;
    public GameObject Joueur;

    public Animator _anim;

    void Start()
    {
        if(Joueur == null) Joueur = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Cinematique());
    }

    private IEnumerator Cinematique()
    {
        Video.Prepare(); // Charge la vidéo.
        yield return new WaitForSecondsRealtime(3);
        _anim.SetBool("Fade", true); // Fondu.
        yield return new WaitForSecondsRealtime(1);
        _anim.SetBool("Fade", false);
        Video.Play(); // Lance la cinématique.
        _video = true;
    }

    void Update()
    {
        Vector3 Pos = Joueur.transform.position;
        Vector3 targetPostition = new Vector3(Pos.x,
                                              transform.position.y, // Création d'un Quaternion avec l'axe Y fixe.
                                              Pos.z);
        transform.LookAt(targetPostition);

        if (_video)  // Attend que la cinématique soit finie.
            if (!Video.isPlaying) 
                SceneManager.LoadScene(1, LoadSceneMode.Single); // Chargement de la cave.       
    }
}
