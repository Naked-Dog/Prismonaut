using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataPath;


    public FileDataHandler(string dataDirPath, string dataFileName) 
    {
        dataPath = Path.Combine(dataDirPath,dataFileName);
    }

    public PlayerGameData Load() 
    {

        PlayerGameData loadedData = null;

        if (File.Exists(dataPath)) 
        {
            try 
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(dataPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<PlayerGameData>(dataToLoad);
            }
            catch (Exception error) 
            {
                Debug.LogError("Error when trying to load data: " + error);
            }
        }
        return loadedData;
    }

    public void Save(PlayerGameData data) 
    {
        try 
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath));

            string dataToStore = JsonUtility.ToJson(data, true);


            using (FileStream stream = new FileStream(dataPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception error) 
        {
            Debug.LogError("Error when save data: " + error);
        }
    }
}
