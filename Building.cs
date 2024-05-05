using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Lin
//功能说明：
//***************************************** 
public class Building : Unit
{
    public bool isKing;
    private float attackCD = 1.4f;
    private float attackTimer;
    public Transform characterTrans;
    public GameObject[] turretGos;

    protected override void Start()
    {
        unitInfo = GameController.Instance.unitInfos[12];
        
        base.Start();
        if (isKing)
        {
            SetColliders(false);
        }
    }



    void Update()
    {
        if (isDead)
        {
            return;
            
        }
        Attack();
    }

    private void Attack()
    {
        if (Time.time - attackTimer >= attackCD)
        {
            attackTimer = Time.time;
            if (currentHP<100)
            {
                animator.SetBool("isAttacking", true);
                characterTrans.LookAt(new Vector3(targetUnit.transform.position.x, transform.position.y, targetUnit.transform.position.z));
                targetUnit.TakeDamage(unitInfo.attackValue, this);
            }
            else
            {
                animator.SetBool("isAttacking", false);
            }
        }
    }

    protected override void Die(Unit attacker)
    {
        base.Die(attacker);
        turretGos[0].SetActive(false);
        turretGos[1].SetActive(true);
        if (isKing)
        {
            //游戏结束
        }
        else
        {
            GameController.Instance.EnableKing(isOrange);
        }
    }

    public override void AttackAnimationEvent()
    {
       
    }
}
