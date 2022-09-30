using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//DataManager => Data.Contents ����
namespace Data
{
    [Serializable]
    public class Stat
    {
        public int level;
        public int hp;
        public int maxHp;
        public int attack;
        public int totalExp;
    }


    //json������ �ؽ�Ʈ�� "�Ľ�"�� �� �ֵ��� Ŭ���� ���� -> Serializable
    //Loader ��ü�� ���ϰŸ� ILoader<> ����
    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
       
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDic()
        {
            Dictionary<int, Stat> dic = new Dictionary<int, Stat>();

            foreach (Stat stat in stats)
            {
                dic.Add(stat.level, stat);
            }
            return dic;
        }
    }

}
