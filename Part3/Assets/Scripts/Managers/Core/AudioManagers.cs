using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagers 
{

    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();


    //MP3 PLAYER -> Audio Source
    //MP3 ���� -> Audio Clip
    //����(��) -> Audio Listener

    public void Init()
    {

        GameObject root = GameObject.Find("@Sound");
        
        if (root == null)
        {
            root = new GameObject("@Sound");
            Object.DontDestroyOnLoad(root);
        }

        string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));

        for(int i=0; i < (int)Define.Sound.MaxCount ; i++)
        {
            GameObject go = new GameObject() { name = soundNames[i] };

            //Bgm,Effect���� audioSource�� �迭�� �־�α�
            _audioSources[i] = go.AddComponent<AudioSource>();
            go.transform.parent = root.transform;
        }

        _audioSources[(int)Define.Sound.Bgm].loop = true;
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type, pitch);
        
        Play(audioClip, type, pitch);


    }

    public void Play( AudioClip audioClip , Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {

        if (audioClip == null)
            return;
        
        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];


            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                Debug.Log("stop");
                
            }
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();


        }
        else
        {
            
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;

            audioSource.PlayOneShot(audioClip);
        }
        

    }


    //������ caching
    AudioClip GetOrAddAudioClip(string path , Define.Sound type , float pitch )
    {
        if (path.Contains("Sounds/") == false)
        {
            path = $"Sounds/{path}";
        }


        AudioClip audioClip = null;


        if( _audioClips.TryGetValue(path, out audioClip) == false)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
            //��ųʸ��� �߰�
            _audioClips.Add(path, audioClip);
        }
        return audioClip;

    }
    public void Clear()
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            //�� �� ���� ȿ���� �޸� �ϱ�
            audioSource.Stop();
            audioSource.clip = null;
            
        }
        _audioClips.Clear();
    }

}
