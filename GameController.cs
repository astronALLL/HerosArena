using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//*****************************************
//创建人： Lin
//功能说明：游戏逻辑控制
//***************************************** 
public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public float leftTime;
    public float energyValue;
    public List<UnitInfo> unitInfos ;
    public GameObject[] unitGos;//所有预制体（游戏物体）资源
    public Building[] purpleBuildings;
    public Building[] orangeBuildings;
    void Awake()
    {
        Instance = this;
        energyValue = 1;
        leftTime = 180;
        unitInfos = new List<UnitInfo>()
        {
            new UnitInfo(){ id=1,unitName="精灵弓箭手",cost=3,hp=10,attackArea=4,speed=1,attackValue=2},
            new UnitInfo(){ id=2,unitName="治愈天使",cost=4,hp=10,attackArea=3,speed=1,attackValue=1},
            new UnitInfo(){ id=3,unitName="三头狼",cost=6,hp=30,attackArea=2,speed=1,attackValue=5},
            new UnitInfo(){ id=4,unitName="堕天使",cost=6,hp=10,attackArea=2.5f,speed=2,attackValue=6},
            new UnitInfo(){ id=5,unitName="熔岩巨兽",cost=4,hp=30,attackArea=2,speed=1,attackValue=4},
            new UnitInfo(){ id=6,unitName="弓箭手兄弟",cost=5,hp=10,attackArea=4,speed=1,attackValue=2},
            new UnitInfo(){ id=7,unitName="装甲熊军团",cost=7,hp=10,attackValue=8,attackArea=2,speed=4},
            new UnitInfo(){ id=8,unitName="死神",cost=6,hp=10,attackValue=7,attackArea=3,speed=2},
            new UnitInfo(){ id=9,unitName="毒瘟疫",cost=4,attackArea=1.5f,speed=1,attackValue=1,canCreateAnywhere=true},
            new UnitInfo(){ id=10,unitName="大火球",cost=4,attackArea=2f,attackValue=3,speed=18,canCreateAnywhere=true},
            new UnitInfo(){ id=11,unitName="骷髅怪",cost=0,hp=2,attackArea=1.5f,speed=1,attackValue=1},
            new UnitInfo(){ id=12,unitName="治疗光环",cost=0,attackArea=2f,attackValue=-2,speed=18},
            new UnitInfo(){ id=13,unitName="防御塔",cost=0,hp=100,attackArea=5,speed=0,attackValue=3}
        };
    }

     void Update()
    {
        if(energyValue < 10)
        {
            energyValue += Time.deltaTime;
            UIManager.Instance.SetEnergySliderValue();
            DecreaseTime();

        }
     }    
    public void DecreaseTime()
        {
            leftTime -= Time.deltaTime;
            int min = (int)leftTime / 60;
            int sec = (int)leftTime % 60;
            UIManager.Instance.SetTimeValue(min, sec);

        }
    public bool CanUseCard(int id)
        {
       
            return unitInfos[id-1].cost <= energyValue;
        
        }
    public void DecreaseEnergyValue(int id)
    {
        int value = unitInfos[id - 1].cost;
        energyValue -= value;
    }
    public void CreateUnit(int id,Vector3 pos,bool isOrange=true)
    {
        GameObject go =Instantiate(unitGos[id - 1]);
        go.transform.position = pos;
        switch (id)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 8:
            case 11:
                Unit unit = go.GetComponent<Unit>();
                unit.isOrange = isOrange;
                unit.unitInfo =unitInfos[id - 1]; 
                break;
            case 6:
            case 7:
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Unit u = go.transform.GetChild(i).GetComponent<Unit>();
                    u.isOrange = isOrange;
                    u.unitInfo = unitInfos[id - 1];
                }
                break;
            default:
                break;
        }

    }

    public void UnitGetTargetPos(Unit unit, bool isOrange)//传进来的是单位的isOrange,橙色单位的目标是紫色
    {
        Building[] buildings = isOrange ?  purpleBuildings: orangeBuildings;
        if (!buildings[0])//国王没有挂掉
        {
            //国王已死
            return;
        }
        int index = unit.transform.position.x <= buildings[0].transform.position.x? 1:2;
        if (buildings[index].isDead)
        {
            unit.defaultTarget = buildings[0];
        }
        else
        {
            unit.defaultTarget = buildings[index];
        }
    }

    public void EnableKing(bool isOrange)
    {

    }
} 