using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }

    // Exemple: SoundManager.Play("NomSample")
    public void Play(string name, float volume = 0f, float pitch = 0f)
    {
        bool found = false;

        foreach (Sound s in sounds)
        {            
            if (s._name == name)
            {
                if (s.source == null) return;

                s.source.Play();
                if (volume != 0f) s.source.volume = volume;
                if (pitch != 0f) s.source.pitch = pitch;

                found = true;
            }           
        }
        if (!found) Debug.LogWarning(name + "SON NON TROUVé");
    }

    public void Set(string name, float volume, float pitch)
    {
        foreach (Sound s in sounds)
        {
            if (s._name == name)
            {
                if (s.source == null) return;

                s.source.volume = volume;
                s.source.pitch = pitch;
                
                return;
            }
        }
         
        Debug.LogWarning(name + "SON NON TROUVé");
    }


    public void Stop(string name)
    {

        foreach (Sound s in sounds)
        {           
            if (s._name == name)
            {
                if (s.source == null) return;
                if(s.source != null && s.source.isPlaying)  s.source.Stop();

                return;                
            }
        }
        
        Debug.LogWarning(name + "SON NON TROUVé");
    }

    public void CheckSound(string name, out bool play)
    {
        play = false;

        foreach (Sound s in sounds)
        {
            if (s._name == name)
            {
                if (s.source != null && s.source.isPlaying) play = true;
                return;
            }
        }

        Debug.LogWarning(name + "SON NON TROUVé");
    }
}
