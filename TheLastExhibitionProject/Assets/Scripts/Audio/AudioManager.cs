using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// ȫ����Ƶ����������������������ֺ���Ч�Ĳ��ź�����
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ����ʵ��
    public static AudioManager Instance { get; private set; }

    [Header("��ƵԴ")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("��������")]
    [SerializeField] private float musicVolume = 0.75f;
    [SerializeField] private float sfxVolume = 0.75f;

    [Header("��Ƶ����")]
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;

    // ��Ч�ֵ䣬����ͨ�����Ʋ���
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // ��������
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ��ʼ����ƵԴ
            InitializeAudioSources();

            // ��ʼ����Ч�ֵ�
            InitializeSFXDictionary();

            // ���ر������������
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ��ʼ����ƵԴ
    /// </summary>
    private void InitializeAudioSources()
    {
        // ���û������������ƵԴ������һ��
        if (musicSource == null)
        {
            GameObject musicSourceObj = new GameObject("MusicSource");
            musicSourceObj.transform.SetParent(transform);
            musicSource = musicSourceObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        // ���û��������Ч��ƵԴ������һ��
        if (sfxSource == null)
        {
            GameObject sfxSourceObj = new GameObject("SFXSource");
            sfxSourceObj.transform.SetParent(transform);
            sfxSource = sfxSourceObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// ��ʼ����Ч�ֵ�
    /// </summary>
    private void InitializeSFXDictionary()
    {
        sfxDictionary.Clear();

        if (sfxClips != null && sfxClips.Length > 0)
        {
            foreach (var clip in sfxClips)
            {
                if (clip != null && !string.IsNullOrEmpty(clip.name))
                {
                    sfxDictionary[clip.name] = clip;
                }
            }
        }
    }

    /// <summary>
    /// ��PlayerPrefs������������
    /// </summary>
    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        // Ӧ����������
        ApplyVolumeSettings();
    }

    /// <summary>
    /// Ӧ���������õ���ƵԴ
    /// </summary>
    private void ApplyVolumeSettings()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    /// <summary>
    /// ������������
    /// </summary>
    /// <param name="volume">����ֵ(0-1)</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        // ��������
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ������Ч����
    /// </summary>
    /// <param name="volume">����ֵ(0-1)</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }

        // ��������
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="musicIndex">���ּ�������</param>
    public void PlayMusic(int musicIndex)
    {
        if (musicSource == null || musicClips == null || musicIndex < 0 || musicIndex >= musicClips.Length)
        {
            return;
        }

        if (musicClips[musicIndex] != null)
        {
            musicSource.clip = musicClips[musicIndex];
            musicSource.Play();
        }
    }

    /// <summary>
    /// �����Ʋ��ű�������
    /// </summary>
    /// <param name="musicName">���ּ�������</param>
    public void PlayMusic(string musicName)
    {
        if (musicSource == null || musicClips == null)
        {
            return;
        }

        for (int i = 0; i < musicClips.Length; i++)
        {
            if (musicClips[i] != null && musicClips[i].name == musicName)
            {
                PlayMusic(i);
                return;
            }
        }

        Debug.LogWarning($"�Ҳ�����Ϊ {musicName} ������");
    }

    /// <summary>
    /// ֹͣ��������
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// ��ͣ��������
    /// </summary>
    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    /// <summary>
    /// �ָ����ű�������
    /// </summary>
    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="sfxIndex">��Ч��������</param>
    public void PlaySFX(int sfxIndex)
    {
        if (sfxSource == null || sfxClips == null || sfxIndex < 0 || sfxIndex >= sfxClips.Length)
        {
            return;
        }

        if (sfxClips[sfxIndex] != null)
        {
            sfxSource.PlayOneShot(sfxClips[sfxIndex], sfxVolume);
        }
    }

    /// <summary>
    /// �����Ʋ�����Ч
    /// </summary>
    /// <param name="sfxName">��Ч��������</param>
    public void PlaySFX(string sfxName)
    {
        if (sfxSource == null)
        {
            return;
        }

        if (sfxDictionary.TryGetValue(sfxName, out AudioClip clip) && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
        else
        {
            // ��������Ч�����в���
            for (int i = 0; i < sfxClips.Length; i++)
            {
                if (sfxClips[i] != null && sfxClips[i].name == sfxName)
                {
                    sfxSource.PlayOneShot(sfxClips[i], sfxVolume);

                    // ��ӵ��ֵ��Լ��ٽ����Ĳ���
                    sfxDictionary[sfxName] = sfxClips[i];
                    return;
                }
            }

            Debug.LogWarning($"�Ҳ�����Ϊ {sfxName} ����Ч");
        }
    }

    /// <summary>
    /// ʹ���ⲿAudioSource������Ч
    /// </summary>
    /// <param name="source">Ҫʹ�õ�AudioSource</param>
    /// <param name="sfxName">��Ч��������</param>
    public void PlaySFXThroughSource(AudioSource source, string sfxName)
    {
        if (source == null)
        {
            return;
        }

        if (sfxDictionary.TryGetValue(sfxName, out AudioClip clip) && clip != null)
        {
            source.PlayOneShot(clip, sfxVolume);
        }
        else
        {
            // ��������Ч�����в���
            for (int i = 0; i < sfxClips.Length; i++)
            {
                if (sfxClips[i] != null && sfxClips[i].name == sfxName)
                {
                    source.PlayOneShot(sfxClips[i], sfxVolume);

                    // ��ӵ��ֵ��Լ��ٽ����Ĳ���
                    sfxDictionary[sfxName] = sfxClips[i];
                    return;
                }
            }

            Debug.LogWarning($"�Ҳ�����Ϊ {sfxName} ����Ч");
        }
    }

    /// <summary>
    /// ʹ���ⲿAudioSource��ָ����AudioClip������Ч
    /// </summary>
    /// <param name="source">Ҫʹ�õ�AudioSource</param>
    /// <param name="clip">Ҫ���ŵ�AudioClip</param>
    public void PlaySFXThroughSource(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null)
        {
            return;
        }

        source.PlayOneShot(clip, sfxVolume);
    }

    /// <summary>
    /// ֹͣ������Ч
    /// </summary>
    public void StopAllSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }

    /// <summary>
    /// ��ȡ��ǰ��������
    /// </summary>
    /// <returns>��������(0-1)</returns>
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    /// <summary>
    /// ��ȡ��ǰ��Ч����
    /// </summary>
    /// <returns>��Ч����(0-1)</returns>
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    /// <summary>
    /// ������ȡ������������Ƶ
    /// </summary>
    /// <param name="mute">�Ƿ���</param>
    public void MuteAll(bool mute)
    {
        if (musicSource != null)
        {
            musicSource.mute = mute;
        }

        if (sfxSource != null)
        {
            sfxSource.mute = mute;
        }
    }

    /// <summary>
    /// ������ȡ����������
    /// </summary>
    /// <param name="mute">�Ƿ���</param>
    public void MuteMusic(bool mute)
    {
        if (musicSource != null)
        {
            musicSource.mute = mute;
        }
    }

    /// <summary>
    /// ������ȡ��������Ч
    /// </summary>
    /// <param name="mute">�Ƿ���</param>
    public void MuteSFX(bool mute)
    {
        if (sfxSource != null)
        {
            sfxSource.mute = mute;
        }
    }
}