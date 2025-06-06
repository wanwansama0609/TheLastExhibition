using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 游戏设置面板控制器，集成了AudioManager
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    [Header("UI元素")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("文本元素")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI musicVolumeLabel;
    [SerializeField] private TextMeshProUGUI sfxVolumeLabel;
    [SerializeField] private TextMeshProUGUI languageLabel;

    [Header("引用")]
    [SerializeField] private MainMenuUI mainMenuUI;

    // 文本键
    private const string TITLE_KEY = "settings_title";
    private const string MUSIC_VOLUME_KEY = "settings_music_volume";
    private const string SFX_VOLUME_KEY = "settings_sfx_volume";
    private const string LANGUAGE_KEY = "settings_language";

    private bool isInitialized = false;

    private void Awake()
    {
        // 如果没有在Inspector中设置引用，尝试获取MainMenuUI
        if (mainMenuUI == null)
        {
            mainMenuUI = Object.FindAnyObjectByType<MainMenuUI>();

            if (mainMenuUI == null)
            {
                Debug.LogWarning("找不到MainMenuUI引用，设置面板的关闭功能可能无法正常工作");
            }
        }
    }

    private void OnEnable()
    {
        // 每次启用面板时初始化
        StartCoroutine(InitializePanel());
    }

    private IEnumerator InitializePanel()
    {
        // 等待TextLocalizationManager初始化完成
        while (TextLocalizationManager.Instance == null)
        {
            yield return null;
        }

        // 设置本地化文本
        SetupLocalizedTexts();

        // 初始化控件值
        InitializeControlValues();

        // 添加值变更监听（仅首次需要）
        if (!isInitialized)
        {
            AddValueChangeListeners();
            isInitialized = true;
        }

        // 设置关闭按钮
        if (closeButton != null && closeButton.onClick.GetPersistentEventCount() == 0)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        // 填充语言下拉列表
        PopulateLanguageDropdown();
    }

    /// <summary>
    /// 设置所有文本元素的本地化
    /// </summary>
    private void SetupLocalizedTexts()
    {
        // 为每个文本元素添加LocalizedText组件并设置键
        SetupLocalizedText(titleText, TITLE_KEY);
        SetupLocalizedText(musicVolumeLabel, MUSIC_VOLUME_KEY);
        SetupLocalizedText(sfxVolumeLabel, SFX_VOLUME_KEY);
        SetupLocalizedText(languageLabel, LANGUAGE_KEY);
    }

    /// <summary>
    /// 为文本组件设置本地化文本
    /// </summary>
    /// <param name="textComponent">文本组件</param>
    /// <param name="textKey">文本键</param>
    private void SetupLocalizedText(TextMeshProUGUI textComponent, string textKey)
    {
        if (textComponent == null)
            return;

        // 检查是否已有LocalizedText组件
        LocalizedText localizedText = textComponent.GetComponent<LocalizedText>();

        if (localizedText == null)
        {
            // 添加LocalizedText组件
            localizedText = textComponent.gameObject.AddComponent<LocalizedText>();
        }

        // 设置文本键
        localizedText.SetTextKey(textKey);
    }

    private void InitializeControlValues()
    {
        // 从AudioManager获取保存的设置，如果AudioManager不存在则从PlayerPrefs获取

        // 音乐音量
        if (musicVolumeSlider != null)
        {
            if (AudioManager.Instance != null)
            {
                musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
            }
            else
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            }
        }

        // 音效音量
        if (sfxVolumeSlider != null)
        {
            if (AudioManager.Instance != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
            }
            else
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            }
        }

        // 语言选择 - 将在PopulateLanguageDropdown中设置
    }

    private void AddValueChangeListeners()
    {
        // 音乐音量变更
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        // 音效音量变更
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        // 语言选择变更
        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
    }

    /// <summary>
    /// 填充语言下拉列表
    /// </summary>
    private void PopulateLanguageDropdown()
    {
        if (languageDropdown == null || LanguageSetting.Instance == null)
            return;

        // 清除现有选项
        languageDropdown.ClearOptions();

        // 添加支持的语言选项
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        // 添加中文选项
        options.Add(new TMP_Dropdown.OptionData("简体中文"));

        // 添加英文选项
        options.Add(new TMP_Dropdown.OptionData("English"));

        // 添加选项到下拉列表
        languageDropdown.AddOptions(options);

        // 设置当前选中的语言
        string currentLang = LanguageSetting.Instance.GetLanguage();
        languageDropdown.value = currentLang == "en" ? 1 : 0;
    }

    private void OnCloseButtonClicked()
    {
        // 播放按钮点击音效
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }

        // 保存所有设置
        SaveAllSettings();

        // 关闭设置面板
        if (mainMenuUI != null)
        {
            mainMenuUI.CloseSettingsPanel();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        // 使用AudioManager设置音乐音量
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
        else
        {
            // 如果AudioManager不存在，则直接保存到PlayerPrefs
            PlayerPrefs.SetFloat("MusicVolume", value);
            Debug.Log($"音乐音量更改为: {value}");
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        // 使用AudioManager设置音效音量
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);

            // 播放测试音效，让用户能够立即听到音量变化
            if (Random.value > 0.7f) // 随机播放音效，避免过于频繁
            {
                AudioManager.Instance.PlaySFX("ButtonClick");
            }
        }
        else
        {
            // 如果AudioManager不存在，则直接保存到PlayerPrefs
            PlayerPrefs.SetFloat("SFXVolume", value);
            Debug.Log($"音效音量更改为: {value}");
        }
    }

    private void OnLanguageChanged(int languageIndex)
    {
        if (LanguageSetting.Instance == null)
            return;

        // 播放按钮点击音效
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }

        // 根据下拉菜单索引设置语言
        string newLanguage = languageIndex == 1 ? "en" : "zh";

        // 设置新语言
        LanguageSetting.Instance.SetLanguage(newLanguage);

        // TextLocalizationManager会自动检测语言变更并更新文本
        Debug.Log($"语言设置更改为: {newLanguage}");
    }

    private void SaveAllSettings()
    {
        // 确保所有更改都被保存
        PlayerPrefs.Save();
        Debug.Log("所有设置已保存");
    }

    private void OnDisable()
    {
        // 确保设置在面板关闭时保存
        SaveAllSettings();
    }
}