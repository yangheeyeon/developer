using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField]
    protected int _exp;
    [SerializeField]
    protected int _gold;

    public int Exp
    {
        get
        { 
            return _exp;
        }

        set
        {
            _exp = value;
            //레벨업 체크
            int level = Level;
            while (true)
            {
                Data.Stat stat;
                if (Managers.Data.statDict.TryGetValue(level + 1, out stat) == false)
                    break;

                if (_exp < stat.totalExp)
                    break;

                level++;

            }
            if(Level != level)
            {
                SetStat(level);
                Debug.Log("level up!");
            }
               
           
        } 
    
        
    }
    public int Gold { get { return _gold; } set { _gold = value; } }

    public override void Start()
    {
        _level = 1;
        _defence = 10;
        _moveSpeed = 10.0f;
        _gold = 0;
        SetStat(_level);
    }

    public void SetStat(int level)
    {
        Data.Stat stat;
        Managers.Data.statDict.TryGetValue(level, out stat);
        Level = stat.level;
        Hp = stat.hp;
        MaxHp = stat.maxHp;
        Attack = stat.attack;
        Exp = stat.totalExp;

    }
    public override void OnDead(Stat attacker)
    {
        Debug.Log("player dead");
    }
}
