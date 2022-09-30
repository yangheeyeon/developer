using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� ���� ��ųʸ� ����
public interface ILoader<key, value>
{
    public Dictionary<key, value> MakeDic();
}
public class DataManagers
{
    //������ ���� �ɷ� ������ ��ųʸ�
    public Dictionary<int, Data.Stat> statDict { get; private set; } = new Dictionary<int, Data.Stat>();
    
   public void Init()
    {
      //LoadJson<> ��ȯ == Loader��ü
        statDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDic();

    }
    //Loader == Data.StatData �� �ǹ���(Loader == ��ȯ ���� )
    Loader LoadJson<Loader ,key, value>(string path) where Loader : ILoader<key, value>
    {
        //FromJson<> ��ȯ == Loader��ü
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
       
    }

}
/*using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int lives;
    public float health;

    public static PlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonString);
    }

    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.
}*/