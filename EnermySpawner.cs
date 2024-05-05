using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Lin
//功能说明：
//***************************************** 
public class EnermySpawner : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("CreateUnits", 5, 20);
    }

    private void CreateUnits()
    {
        GameController.Instance.CreateUnit(Random.Range(1, 8), transform.position, false);
    }
}
