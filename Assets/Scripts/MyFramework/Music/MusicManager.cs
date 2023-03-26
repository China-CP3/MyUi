using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicManager : BaseManager<MusicManager>
{
    private AudioSource musicSource = null;//����������Ψһ��
    private GameObject soundObj=new GameObject("Sound");//�ÿ������Ͽ����ж����Ч
    private List<AudioSource> soundList=new List<AudioSource>();
    private float bkVolume=1f;
    private float soundVolume=1f;
    public MusicManager()
    {
        MonoManager.GetInstance().AddListener(Update);
    }
    private void Update()
    {
        for (int i = soundList.Count-1; i >=0 ; i--)
        {
            if (!soundList[i].isPlaying)
            {
                Object.Destroy(soundList[i]);
                soundList.RemoveAt(i);              
            }
        }
    }
    public void PlayBkMusic(string name)
    {       
        if (musicSource==null)
        {
            GameObject obj =new GameObject("BkMusic");
            musicSource = obj.AddComponent<AudioSource>();
        }
        ResourcesManager.GetInstance().LoadAsync<AudioClip>("Music/Bk/" + name, (clip) => 
        {
            Debug.Log("����");
            musicSource.clip = clip;
            musicSource.volume = bkVolume;          
            musicSource.Play(); 
        });
    }
    public void StopBkMusic()
    {
        Debug.Log("ֹͣ");
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    public void PauseBkMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }
    public void ChangeBkVolume(float volume)
    {
        if (musicSource != null)
        {
            bkVolume = volume;
            musicSource.volume = volume;
        }
    }

    public void ChangeSoundVolume(float volume)
    {
        soundVolume = volume;
        if (soundList.Count>0)
        {
            for (int i = 0; i < soundList.Count; i++)
            {               
                soundList[i].volume = volume;
            }
        }     
    }
    public void PlaySound(string name,bool isLoop=false,UnityAction<AudioSource> callBack=null)
    {             
        ResourcesManager.GetInstance().LoadAsync<AudioClip>("Music/Sound/"+ name, (clip) =>
        {
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.loop = isLoop;
            audioSource.volume = bkVolume;
            audioSource.Play();
            soundList.Add(audioSource);
            callBack?.Invoke(audioSource);
        });
    }
    public void StopSound(AudioSource audioSource)
    {
        Debug.Log("ֹͣ");
        if (soundList.Contains(audioSource))
        {
            audioSource.Stop();
            soundList.Remove(audioSource);
            Object.Destroy(audioSource);
        }
    }
}
