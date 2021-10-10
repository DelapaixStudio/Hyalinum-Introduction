[System.Serializable] 

//Sauvegarde des données du Script SaveManager.cs
public class PlayerData    
{
    public int Currentlanguage;
    public int ProgressPoint;

    public PlayerData(SaveManager playerData) // Script sur GO DATA
    {
        ProgressPoint = playerData.ProgressPoint;
        Currentlanguage = playerData.currentlanguage;        
    }

}
