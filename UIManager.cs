using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//*****************************************
//创建人： Lin
//功能说明：游戏场景中UI管理
//***************************************** 
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text energyText;
    public Slider energySlider;
    public Text leftTimeText;
    private List<int> cardIDList = new List<int>();
    public GameObject cardGo;
    public Transform nextCardT;
    public Transform[] boardCardsT;
    public Transform boardTrans;
    public Sprite[] cardSprites;
    public Sprite[] cardDisSprites;
    private int maxContentNum = 4;
    private int currentBoardNum=0;
    public bool goExist=false;
    public int currentCardNum;
    
    void Awake()
    {
        Instance = this;
        CreateNewCard();
    }

    void Update()
    {

    } 

    public void SetEnergySliderValue()
    {
        energyText.text = ((int)GameController.Instance.energyValue).ToString();
        
        energySlider.value = GameController.Instance.energyValue / 10;
    }
    public void SetTimeValue(int min,int sec)
    {
        leftTimeText.text = min.ToString() +":" + sec.ToString();

    }
    
    private void CreateNewCard()
    {
        if (currentBoardNum-1>maxContentNum||currentCardNum>10)
        {
            return;
        }
        GameObject go = Instantiate(cardGo, nextCardT);goExist = true;
        currentCardNum++;
        go.transform.localPosition = Vector3.zero;
        int randomNum = Random.Range(1, 11);
        while (cardIDList.Contains(randomNum))
        {
            randomNum = Random.Range(1, 11);

        }
        cardIDList.Add(randomNum);
        Image image = go.transform.GetChild(0).GetComponent<Image>();
        image.sprite = cardSprites[randomNum-1];
        Button button = go.transform.GetChild(0).GetComponent<Button>();
        SpriteState ss = button.spriteState;
        ss.disabledSprite = cardDisSprites[randomNum - 1];
        button.spriteState = ss;
        go.GetComponent<Card>().id = randomNum;
        if (currentBoardNum<=maxContentNum)
        {
            MoveCardToBoard(currentBoardNum);
            
            
        }
    }

    private void MoveCardToBoard(int posID)
    {

        
           Transform t  = nextCardT.GetChild(0);
            t.SetParent(boardTrans);
        
        
        
        if (posID > 0)
        {
            
            t.localScale=(Vector3.one);
            
        }
        t.GetComponent<Card>().posID = posID;    
        
        t.DOLocalMove(boardCardsT[posID].localPosition, 0.2f).OnComplete(()=> { CompleteMoveTween(t); });//第一次创建nextcard不移动
        
    }
    private void CompleteMoveTween(Transform t)
    {
        currentBoardNum++;
        CreateNewCard();
        
        t.GetComponent<Card>().SetinitPos();
        
    }
    public void UseCard(int posID)
    {
        
        currentBoardNum--;
        MoveCardToBoard(posID);




    }
    public void RemoveCardIDInList(int id)
    {
        cardIDList.Remove(id);
    }
}
