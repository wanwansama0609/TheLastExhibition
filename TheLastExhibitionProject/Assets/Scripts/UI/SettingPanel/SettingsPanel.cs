/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// 游戏设置面板控制器
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    [Header("UI元素")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("文本元素")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI musicVolumeLabel;
    [SerializeField] private TextMeshProUGUI sfxVolumeLabel;
    [SerializeField] private TextMeshProUGUI fullscreenLabel;
    [SerializeField] private TextMeshProUGUI languageLabel;
    [SerializeField] private TextMeshProUGUI closeButtonText;

    [Header("引用")]
    [SerializeField] private MainMenuUI mainMenuUI;

    // 文本键
    private const string TITLE_KEY = "settings_title";
    private const string MUSIC_VOLUME_KEY = "settings_music_volume";
    private const string SFX_VOLUME_KEY = "settings_sfx_volume";
    private const string FULLSCREEN_KEY = "settings_fullscreen";
    private const string LANGUAGE_KEY = "settings_language";
    private const string CLOSE_KEY = "btn_close";

    private bool isInitialized = false;

    private void Awake()
    {
        // 如果没有在Inspector中设置引用，尝试获取MainMenuUI
        if (mainMenuUI == null)
        {
            // 使用新的非弃用方法
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
        SetupLocalizedText(fullscreenLabel, FULLSCREEN_KEY);
        SetupLocalizedText(languageLabel, LANGUAGE_KEY);
        SetupLocalizedText(closeButtonText, CLOSE_KEY);
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
        // 从PlayerPrefs加载保存的设置，或使用默认值

        // 音乐音量
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        }

        // 音效音量
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        }

        // 全屏模式
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
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

        // 全屏模式变更
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
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

        // 获取支持的语言列表
        List<string> supportedLanguages = LanguageSetting.Instance.GetSupportedLanguages();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        // 创建每种语言的选项
        foreach (string lang in supportedLanguages)
        {
            string displayName = LanguageSetting.Instance.GetLanguageDisplayName(lang);
            options.Add(new TMP_Dropdown.OptionData(displayName));
        }

        // 添加选项到下拉列表
        languageDropdown.AddOptions(options);

        // 设置当前选中的语言
        string currentLang = LanguageSetting.Instance.GetLanguage();
        int currentIndex = supportedLanguages.IndexOf(currentLang);
        if (currentIndex >= 0)
        {
            languageDropdown.value = currentIndex;
        }
    }

    private void OnCloseButtonClicked()
    {
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
        // 更新音乐音量
        PlayerPrefs.SetFloat("MusicVolume", value);

        // 这里可以直接应用音量更改
        // AudioManager.Instance.SetMusicVolume(value);
        Debug.Log($"音乐音量更改为: {value}");
    }

    private void OnSFXVolumeChanged(float value)
    {
        // 更新音效音量
        PlayerPrefs.SetFloat("SFXVolume", value);

        // 这里可以直接应用音量更改
        // AudioManager.Instance.SetSFXVolume(value);
        Debug.Log($"音效音量更改为: {value}");
    }

    private void OnFullscreenToggled(bool isFullscreen)
    {
        // 更新全屏设置
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);

        // 应用全屏设置
        Screen.fullScreen = isFullscreen;
        Debug.Log($"全屏模式设置为: {isFullscreen}");
    }

    private async void OnLanguageChanged(int languageIndex)
    {
        if (LanguageSetting.Instance == null)
            return;

        // 获取选中的语言代码
        List<string> supportedLanguages = LanguageSetting.Instance.GetSupportedLanguages();
        if (languageIndex < 0 || languageIndex >= supportedLanguages.Count)
            return;

        string newLanguage = supportedLanguages[languageIndex];

        // 更新语言设置并重新加载文本
        if (TextLocalizationManager.Instance != null)
        {
            await TextLocalizationManager.Instance.ChangeLanguageAsync(newLanguage);

            // 文本已通过事件自动更新，不需要手动刷新
            Debug.Log($"语言设置更改为: {newLanguage}");
        }
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
}*/