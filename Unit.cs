using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
//*****************************************
//创建人： Lin
//功能说明：
//***************************************** 
public class Unit : MonoBehaviour
{
    //基础属性与状态
    public UnitInfo unitInfo;
    public bool isOrange;
    public bool hasTarget;//如果有目标，则攻击，否则朝国王或者弓箭手移动
    public int currentHP;
    public bool isDead;
    
    //组件引用
    public Animator animator;
    public NavMeshAgent meshAgent;
    protected HPSlider hpslider;
    //其他引用
    public Unit defaultTarget;//默认攻击目标：国王或者两边的一个弓箭手
    public Unit targetUnit;//目标攻击单位
    public List<Unit> targetList = new List<Unit>();//当前攻击范围内可选敌人列表
    public List<Unit> attackerList = new List<Unit>();//攻击我们的敌人列表
    protected Collider[] colliders;
    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        meshAgent = GetComponent<NavMeshAgent>();
        GetComponentInChildren<SphereCollider>().radius = unitInfo.attackArea*2;
        colliders = GetComponentsInChildren<Collider>();
        currentHP = unitInfo.hp;
        if (unitInfo.hp>0)
        {
            hpslider = transform.Find("Canvas_HP").GetComponent<HPSlider>();
            hpslider.SetHPColorSlider(isOrange);
        }
    }

    protected virtual void UnitMove()
    {

       
        if (hasTarget) //有目标
        {
            //目标没有销毁且没有死亡
            if(targetUnit!= null&&!targetUnit.isDead) 
            {
                //朝目标移动
                meshAgent.SetDestination(targetUnit.transform.position);
                JudgeIfReachTargetPos(transform.position, targetUnit.transform.position);


            }
            else
            {
                //重置目标
                ResetTarget(targetUnit);

            }
        }
        else
        {
            //获取当前默认目标位置
            GameController.Instance.UnitGetTargetPos(this, isOrange);
            //默认目标不为空
            if (defaultTarget!= null)
            {
                //朝默认目标进行移动
                meshAgent.SetDestination(defaultTarget.transform.position);//对魔法不需要，如何操作呢？
                JudgeIfReachTargetPos(transform.position, defaultTarget.transform.position);
            }
        }
    }

    public void JudgeIfReachTargetPos(Vector3 currentPos,Vector3 target)//判断是到达攻击范围并执行移动
    {
        //没有达到攻击范围
        if (Vector3.Distance(currentPos,target)>=unitInfo.attackArea)
        {
            //执行相关行为
            UnitBehaviour();
        }
        //达到攻击范围
        else
        {
            meshAgent.isStopped = true;
            //动画控制器播放攻击动画
            animator.SetBool("isAttacking",true);
            animator.SetBool("isMoving", false);
        }
    }
    private void ResetTarget(Unit unit)
    {
        targetList.Remove(unit);
        ClearDeadUnitInList();
        if (targetList.Count>0)
        {
            SetTarget();
        }
        else//如果没有攻击目标则攻击地方建筑
        {
            hasTarget = false;
            targetUnit = null;
            GameController.Instance.UnitGetTargetPos(this, isOrange);
        }
    }
    protected virtual void UnitBehaviour()
    {
        if (meshAgent.enabled)
        {
            meshAgent.isStopped = false;
        }
        //动画控制器播放移动动画
        animator.SetBool("isAttacking", false);
        animator.SetBool("isMoving", true);
    }

    public void TakeDamage(int damageValue,Unit attacker)
    {
        
        currentHP -= damageValue;Debug.Log(currentHP);
        Mathf.Clamp(currentHP,0, unitInfo.hp);
        //显示血量到血条上
        //hpslider.SetHPValue((float)currentHP / unitInfo.hp);
        if (currentHP<=0)
        {
            Die(attacker);
            Debug.Log("Die");
        }
    }

    protected virtual void Die(Unit attacker)
    {
        isDead = true;
        animator.SetTrigger("Die");
        SetColliders(false);
        attacker.ResetTarget(this);
        RemoveSelfFromOtherAttack();
        //血条隐藏to do
        if (meshAgent.enabled)
        {
            meshAgent.isStopped = true;
        }
        Invoke("DestroyGame", 2);
    }

    private void DestroyGame()
    {
        Destroy(gameObject);
    }

    private void RemoveSelfFromOtherAttack()
    {
        for (int i = 0; i < attackerList.Count; i++)
        {
            attackerList[i].targetList.Remove(this);
        }
    }

    public virtual void AttackAnimationEvent()
    {
        //看向敌人
        if (hasTarget)
        {
            transform.LookAt(targetUnit.transform);
        }
        else
        {
            transform.LookAt(defaultTarget.transform);
        }

        if (targetUnit!=null)
        {
            targetUnit.TakeDamage(unitInfo.attackValue, this);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            Unit unit = other.GetComponent<Unit>();
            if (isOrange!=unit.isOrange)
            {
                targetList.Add(unit);
                unit.AddAttackerToList(this);
                ClearDeadUnitInList();
                SetTarget();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            Unit unit = other.GetComponent<Unit>();
            if (isOrange != unit.isOrange&&unit!= null&&targetUnit==unit)
            {
                ResetTarget(unit);
            }
        }
    }

    public void AddAttackerToList(Unit unit)
    {
        attackerList.Add(unit);
    }

    private void ClearDeadUnitInList()
    {
        List<int> clearList = new List<int>();
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i]==null)
            {
                clearList.Add(i);
            }
        }
        for (int i = 0; i < clearList.Count; i++)
        {
            targetList.RemoveAt(clearList[i]);
        }
    }
    private void SetTarget()
    {
        float closestDistance = Mathf.Infinity;
        Unit u = null;
        for (int i = 0; i < targetList.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, targetList[i].transform.position);
            if (distance<closestDistance)
            {
                closestDistance = distance;
                u = targetList[i];
            }
        }
        targetUnit = u;
        hasTarget = true;
    }
    protected void SetColliders(bool state)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = state;
        }
    }
}

[Serializable]
public struct UnitInfo//需要定义在外面
    {
        public int id;
        public string unitName;
        public int cost;
        public int hp;
        public float attackArea;
        public int speed;
        public int attackValue;
        public bool canCreateAnywhere;
    }