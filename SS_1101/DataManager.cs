using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using DataInfo;

public class DataManager : MonoBehaviour {

    private string dataPath;

    public void Initialize()
    {
        dataPath = Application.persistentDataPath
            + "/gameData.dat";
    }

    public void Save(GameData _gameData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(dataPath);
        GameData data = new GameData();
        data.killCount = _gameData.killCount;
        data.hp = _gameData.hp;
        data.speed = _gameData.speed;
        data.damage = _gameData.damage;
        data.equipItem = _gameData.equipItem;
        bf.Serialize(file, data);
        file.Close();
    }

    public GameData Load()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            GameData data = new GameData();
            return data;
        }
    }
}