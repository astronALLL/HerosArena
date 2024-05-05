using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Lin
//功能说明：
//***************************************** 
public class CharacterAnimationEvent : MonoBehaviour
{
    private Unit unit;
    void Start()
    {
        unit = GetComponentInParent<Unit>();
    }


    private void AttackAnimationEvent()
    {
        
        unit.AttackAnimationEvent();
    }
}
