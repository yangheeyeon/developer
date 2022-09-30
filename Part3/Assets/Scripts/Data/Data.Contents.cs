using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//DataManager => Data.Contents 접근
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


    //json파일의 텍스트를 "파싱"할 수 있도록 클래스 정의 -> Serializable
    //Loader 객체로 쓰일거면 ILoader<> 구현
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
