using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    int _monsterCount = 0;
    int _reserveCount = 0;
    [SerializeField]
    int _keepMonsterCount;

    [SerializeField]
    Vector3 _spawnPos;

    [SerializeField]
    float _spawnRadius = 15.0f;

    [SerializeField]
    float _spawnTime = 5.0f;

    public void AddMonsterCount(int i) { _monsterCount += i;}
    public void SetKeepMonsterCount(int i) { _keepMonsterCount = i; }
   
    void Start()
    {
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        while(_reserveCount+_monsterCount < _keepMonsterCount)
        {
            StartCoroutine("ReserveSpawn");
        }
    }
    IEnumerator ReserveSpawn()
    {
        Vector3 _randPos;
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));
        NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        while (true)
        {
            Vector3 _randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            _randDir.y = 0;
            _randPos = _spawnPos + _randDir;


            //갈수 있는가?
            NavMeshPath path = new NavMeshPath();
            //TargetPosition , storePaht
            if (nma.CalculatePath(_randPos, path))
                break;
        }
        
        GameObject go = Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        go.transform.position = _randPos;
        _reserveCount--;
    }
}
