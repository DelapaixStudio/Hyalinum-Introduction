using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    public static SaveManager SaveInstance;

    // Variables à Sauvegarder.
    public int ProgressPoint = 0;
    public int currentlanguage = 1; // 0 = FR, 1 = EN, 2 = ES
    // -------------

    void Awake()
    {
        if (SaveManager.SaveInstance == null)
        {
           SaveInstance = this;
           DontDestroyOnLoad(gameObject);           
        }
        else
        {
           Destroy(gameObject);
           return;
        }

        CheckSave(out bool save);
        if (save) LoadPlayer();
        else
        {            
            SaveData();
        }
    }

    public void SaveData()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        currentlanguage = data.Currentlanguage;
        ProgressPoint = data.ProgressPoint;
    }

    public void CheckSave(out bool save)
    {
        string path = Application.persistentDataPath + "/joueur.amour"; 
        if (File.Exists(path))
        {
            save = true;
        }
        else
        {
            Debug.Log("Save File Not Found in" + path);
            save = false;
        }
    }

}
