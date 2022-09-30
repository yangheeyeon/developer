using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    [SerializeField]
    float _scanRange;
    [SerializeField]
    float _attackRange;


    Stat _stat;
    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;

        _stat = gameObject.GetComponent<Stat>();
        if(gameObject.GetComponentInChildren<UI_HPBar>()==null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

        _scanRange = 6;
        _attackRange = 2;
    }

    protected override void UpdateDie() { }
    //idle상태 -> 추적상태로 만들기
    protected override void UpdateIdle() {
        GameObject player = Managers.Game.GetPlayer;
        if (player == null)
            return;
            
        float dist = (player.transform.position - transform.position).magnitude;
        
        if (dist <= _scanRange)
        {

            lockTarget = player;
            State = Define.State.Moving;
            
            return;
        }
           
    }
    protected override void UpdateMoving() {
        if (lockTarget != null)
        {
            _destPos = lockTarget.transform.position;
            //플레이어가 가까이 있으면 공격모드로 전환
            float distance = (_destPos - transform.position).magnitude;


            if (distance <= _attackRange)
            {
                State = Define.State.Skill;
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);
                return;
            }
        }


        //쫓아서 이동
        Vector3 dir = _destPos - transform.position;

        //이동할 거리가 너무 작을 때
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            //목적지 보다 멀리가지 않도록
            
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);//Vector3
            nma.speed = _stat.MoveSpeed;

           
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 20.0f);
        }
    }
    protected override void UpdateSkill() {
        //공격대상을 바라보도록
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);//dir을 바라볼때의 벡터라고 생각
            transform.rotation = Quaternion.Slerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }
    public void OnHitEvent()
    {
        Stat lockStat = lockTarget.GetComponent<Stat>();

        //HP감소
        if (lockTarget != null)
        {
            lockStat.OnAttacked(_stat);
            
            //공격거리에 있으면 ATTACK모드
            //아니면 RUN모드

            if (lockStat.Hp > 0)
            {
                float dist = (lockStat.transform.position - transform.position).magnitude;
                if (dist <= _attackRange)
                    State = Define.State.Skill;
                else
                    State = Define.State.Moving;
            }
            else
            {
                State = Define.State.Idle;
            }
                
        }
    }
}
