using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Lin
//功能说明：游戏总管理
//***************************************** 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AudioSource audioSource;
   
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    // 播放音乐的方法
    public void PlayMusic(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    //播放音效
    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);//Play()方法不能有参数，PlayOneShoe可以有，所以不需要赋值直接可以播放

    }
}