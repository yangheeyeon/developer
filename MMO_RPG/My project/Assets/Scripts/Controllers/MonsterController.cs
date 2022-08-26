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
    //idle���� -> �������·� �����
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
            //�÷��̾ ������ ������ ���ݸ��� ��ȯ
            float distance = (_destPos - transform.position).magnitude;


            if (distance <= _attackRange)
            {
                State = Define.State.Skill;
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);
                return;
            }
        }


        //�ѾƼ� �̵�
        Vector3 dir = _destPos - transform.position;

        //�̵��� �Ÿ��� �ʹ� ���� ��
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            //������ ���� �ָ����� �ʵ���
            
            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);//Vector3
            nma.speed = _stat.MoveSpeed;

           
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 20.0f);
        }
    }
    protected override void UpdateSkill() {
        //���ݴ���� �ٶ󺸵���
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);//dir�� �ٶ󺼶��� ���Ͷ�� ����
            transform.rotation = Quaternion.Slerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }
    public void OnHitEvent()
    {
        Stat lockStat = lockTarget.GetComponent<Stat>();

        //HP����
        if (lockTarget != null)
        {
            lockStat.OnAttacked(_stat);
            
            //���ݰŸ��� ������ ATTACK���
            //�ƴϸ� RUN���

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
