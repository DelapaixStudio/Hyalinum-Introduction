using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem 
{
    public static void SavePlayer(SaveManager playerData)
    {
       BinaryFormatter formatter = new BinaryFormatter();
       string path = Application.persistentDataPath + "/joueur.amour"; /// /User/Appdata etc difficile à trouver.
       FileStream stream = new FileStream(path, FileMode.Create);

       PlayerData data = new PlayerData(playerData);

       formatter.Serialize(stream, data);
       stream.Close();

       Debug.Log("SAVED, Progress Point = " + data.ProgressPoint);
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/joueur.amour"; 
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            Debug.Log("LOADED at : " + data.ProgressPoint);
            return data;
           
        }
        else
        {
            Debug.Log("Save File Not Found in" + path);
            return null;
        }
    }
    
}
