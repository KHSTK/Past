using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    //定义音乐和音效的Sound数组
    public List<Sound> musicSounds = new List<Sound>();
    public List<Sound> sfxSounds = new List<Sound>();
    //音乐和音效的AudioSource
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //在场景切换时不销毁该对象
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        PlayMusic("BGM1");
    }

    //播放音乐的方法，参数为音乐名称
    public void PlayMusic(string name)
    {
        Sound s = musicSounds.Find(x => x.name == name);
        //如果找不到对应的Sound，输出错误信息
        if (s == null)
        {
            Debug.Log("没有找到音乐");
        }
        //否则将音乐源的clip设置为对应Sound的clip并播放
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    //播放音效的方法，参数为音效名称
    public void PlaySFX(string name)
    {
        Sound s = sfxSounds.Find(x => x.name == name);
        //如果找不到对应的Sound，输出错误信息
        if (s == null)
        {
            Debug.Log("没有找到音效");
        }
        //否则播放对应Sound的clip
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
