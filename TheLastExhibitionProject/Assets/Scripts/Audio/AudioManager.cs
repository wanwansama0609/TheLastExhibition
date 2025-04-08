using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 全局音频管理器，负责控制所有音乐和音效的播放和音量
/// </summary>
public class AudioManager : MonoBehaviour
{
    // 单例实例
    public static AudioManager Instance { get; private set; }

    [Header("音频源")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("音量设置")]
    [SerializeField] private float musicVolume = 0.75f;
    [SerializeField] private float sfxVolume = 0.75f;

    [Header("音频剪辑")]
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;

    // 音效字典，用于通过名称查找
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // 单例设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 初始化音频源
            InitializeAudioSources();

            // 初始化音效字典
            InitializeSFXDictionary();

            // 加载保存的音量设置
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化音频源
    /// </summary>
    private void InitializeAudioSources()
    {
        // 如果没有设置音乐音频源，创建一个
        if (musicSource == null)
        {
            GameObject musicSourceObj = new GameObject("MusicSource");
            musicSourceObj.transform.SetParent(transform);
            musicSource = musicSourceObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        // 如果没有设置音效音频源，创建一个
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
    /// 初始化音效字典
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
    /// 从PlayerPrefs加载音量设置
    /// </summary>
    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        // 应用音量设置
        ApplyVolumeSettings();
    }

    /// <summary>
    /// 应用音量设置到音频源
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
    /// 设置音乐音量
    /// </summary>
    /// <param name="volume">音量值(0-1)</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        // 保存设置
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">音量值(0-1)</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }

        // 保存设置
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="musicIndex">音乐剪辑索引</param>
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
    /// 按名称播放背景音乐
    /// </summary>
    /// <param name="musicName">音乐剪辑名称</param>
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

        Debug.LogWarning($"找不到名为 {musicName} 的音乐");
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    /// <summary>
    /// 恢复播放背景音乐
    /// </summary>
    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="sfxIndex">音效剪辑索引</param>
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
    /// 按名称播放音效
    /// </summary>
    /// <param name="sfxName">音效剪辑名称</param>
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
            // 尝试在音效数组中查找
            for (int i = 0; i < sfxClips.Length; i++)
            {
                if (sfxClips[i] != null && sfxClips[i].name == sfxName)
                {
                    sfxSource.PlayOneShot(sfxClips[i], sfxVolume);

                    // 添加到字典以加速将来的查找
                    sfxDictionary[sfxName] = sfxClips[i];
                    return;
                }
            }

            Debug.LogWarning($"找不到名为 {sfxName} 的音效");
        }
    }

    /// <summary>
    /// 使用外部AudioSource播放音效
    /// </summary>
    /// <param name="source">要使用的AudioSource</param>
    /// <param name="sfxName">音效剪辑名称</param>
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
            // 尝试在音效数组中查找
            for (int i = 0; i < sfxClips.Length; i++)
            {
                if (sfxClips[i] != null && sfxClips[i].name == sfxName)
                {
                    source.PlayOneShot(sfxClips[i], sfxVolume);

                    // 添加到字典以加速将来的查找
                    sfxDictionary[sfxName] = sfxClips[i];
                    return;
                }
            }

            Debug.LogWarning($"找不到名为 {sfxName} 的音效");
        }
    }

    /// <summary>
    /// 使用外部AudioSource和指定的AudioClip播放音效
    /// </summary>
    /// <param name="source">要使用的AudioSource</param>
    /// <param name="clip">要播放的AudioClip</param>
    public void PlaySFXThroughSource(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null)
        {
            return;
        }

        source.PlayOneShot(clip, sfxVolume);
    }

    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }

    /// <summary>
    /// 获取当前音乐音量
    /// </summary>
    /// <returns>音乐音量(0-1)</returns>
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    /// <summary>
    /// 获取当前音效音量
    /// </summary>
    /// <returns>音效音量(0-1)</returns>
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    /// <summary>
    /// 静音或取消静音所有音频
    /// </summary>
    /// <param name="mute">是否静音</param>
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
    /// 静音或取消静音音乐
    /// </summary>
    /// <param name="mute">是否静音</param>
    public void MuteMusic(bool mute)
    {
        if (musicSource != null)
        {
            musicSource.mute = mute;
        }
    }

    /// <summary>
    /// 静音或取消静音音效
    /// </summary>
    /// <param name="mute">是否静音</param>
    public void MuteSFX(bool mute)
    {
        if (sfxSource != null)
        {
            sfxSource.mute = mute;
        }
    }
}