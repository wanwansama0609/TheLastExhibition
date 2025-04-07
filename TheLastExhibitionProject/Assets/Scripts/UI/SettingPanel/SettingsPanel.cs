/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// ��Ϸ������������
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    [Header("UIԪ��")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("�ı�Ԫ��")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI musicVolumeLabel;
    [SerializeField] private TextMeshProUGUI sfxVolumeLabel;
    [SerializeField] private TextMeshProUGUI fullscreenLabel;
    [SerializeField] private TextMeshProUGUI languageLabel;
    [SerializeField] private TextMeshProUGUI closeButtonText;

    [Header("����")]
    [SerializeField] private MainMenuUI mainMenuUI;

    // �ı���
    private const string TITLE_KEY = "settings_title";
    private const string MUSIC_VOLUME_KEY = "settings_music_volume";
    private const string SFX_VOLUME_KEY = "settings_sfx_volume";
    private const string FULLSCREEN_KEY = "settings_fullscreen";
    private const string LANGUAGE_KEY = "settings_language";
    private const string CLOSE_KEY = "btn_close";

    private bool isInitialized = false;

    private void Awake()
    {
        // ���û����Inspector���������ã����Ի�ȡMainMenuUI
        if (mainMenuUI == null)
        {
            // ʹ���µķ����÷���
            mainMenuUI = Object.FindAnyObjectByType<MainMenuUI>();

            if (mainMenuUI == null)
            {
                Debug.LogWarning("�Ҳ���MainMenuUI���ã��������Ĺرչ��ܿ����޷���������");
            }
        }
    }

    private void OnEnable()
    {
        // ÿ���������ʱ��ʼ��
        StartCoroutine(InitializePanel());
    }

    private IEnumerator InitializePanel()
    {
        // �ȴ�TextLocalizationManager��ʼ�����
        while (TextLocalizationManager.Instance == null)
        {
            yield return null;
        }

        // ���ñ��ػ��ı�
        SetupLocalizedTexts();

        // ��ʼ���ؼ�ֵ
        InitializeControlValues();

        // ���ֵ������������״���Ҫ��
        if (!isInitialized)
        {
            AddValueChangeListeners();
            isInitialized = true;
        }

        // ���ùرհ�ť
        if (closeButton != null && closeButton.onClick.GetPersistentEventCount() == 0)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        // ������������б�
        PopulateLanguageDropdown();
    }

    /// <summary>
    /// ���������ı�Ԫ�صı��ػ�
    /// </summary>
    private void SetupLocalizedTexts()
    {
        // Ϊÿ���ı�Ԫ�����LocalizedText��������ü�
        SetupLocalizedText(titleText, TITLE_KEY);
        SetupLocalizedText(musicVolumeLabel, MUSIC_VOLUME_KEY);
        SetupLocalizedText(sfxVolumeLabel, SFX_VOLUME_KEY);
        SetupLocalizedText(fullscreenLabel, FULLSCREEN_KEY);
        SetupLocalizedText(languageLabel, LANGUAGE_KEY);
        SetupLocalizedText(closeButtonText, CLOSE_KEY);
    }

    /// <summary>
    /// Ϊ�ı�������ñ��ػ��ı�
    /// </summary>
    /// <param name="textComponent">�ı����</param>
    /// <param name="textKey">�ı���</param>
    private void SetupLocalizedText(TextMeshProUGUI textComponent, string textKey)
    {
        if (textComponent == null)
            return;

        // ����Ƿ�����LocalizedText���
        LocalizedText localizedText = textComponent.GetComponent<LocalizedText>();

        if (localizedText == null)
        {
            // ���LocalizedText���
            localizedText = textComponent.gameObject.AddComponent<LocalizedText>();
        }

        // �����ı���
        localizedText.SetTextKey(textKey);
    }

    private void InitializeControlValues()
    {
        // ��PlayerPrefs���ر�������ã���ʹ��Ĭ��ֵ

        // ��������
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        }

        // ��Ч����
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        }

        // ȫ��ģʽ
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        }

        // ����ѡ�� - ����PopulateLanguageDropdown������
    }

    private void AddValueChangeListeners()
    {
        // �����������
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        // ��Ч�������
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        // ȫ��ģʽ���
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        }

        // ����ѡ����
        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
    }

    /// <summary>
    /// ������������б�
    /// </summary>
    private void PopulateLanguageDropdown()
    {
        if (languageDropdown == null || LanguageSetting.Instance == null)
            return;

        // �������ѡ��
        languageDropdown.ClearOptions();

        // ��ȡ֧�ֵ������б�
        List<string> supportedLanguages = LanguageSetting.Instance.GetSupportedLanguages();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        // ����ÿ�����Ե�ѡ��
        foreach (string lang in supportedLanguages)
        {
            string displayName = LanguageSetting.Instance.GetLanguageDisplayName(lang);
            options.Add(new TMP_Dropdown.OptionData(displayName));
        }

        // ���ѡ������б�
        languageDropdown.AddOptions(options);

        // ���õ�ǰѡ�е�����
        string currentLang = LanguageSetting.Instance.GetLanguage();
        int currentIndex = supportedLanguages.IndexOf(currentLang);
        if (currentIndex >= 0)
        {
            languageDropdown.value = currentIndex;
        }
    }

    private void OnCloseButtonClicked()
    {
        // ������������
        SaveAllSettings();

        // �ر��������
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
        // ������������
        PlayerPrefs.SetFloat("MusicVolume", value);

        // �������ֱ��Ӧ����������
        // AudioManager.Instance.SetMusicVolume(value);
        Debug.Log($"������������Ϊ: {value}");
    }

    private void OnSFXVolumeChanged(float value)
    {
        // ������Ч����
        PlayerPrefs.SetFloat("SFXVolume", value);

        // �������ֱ��Ӧ����������
        // AudioManager.Instance.SetSFXVolume(value);
        Debug.Log($"��Ч��������Ϊ: {value}");
    }

    private void OnFullscreenToggled(bool isFullscreen)
    {
        // ����ȫ������
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);

        // Ӧ��ȫ������
        Screen.fullScreen = isFullscreen;
        Debug.Log($"ȫ��ģʽ����Ϊ: {isFullscreen}");
    }

    private async void OnLanguageChanged(int languageIndex)
    {
        if (LanguageSetting.Instance == null)
            return;

        // ��ȡѡ�е����Դ���
        List<string> supportedLanguages = LanguageSetting.Instance.GetSupportedLanguages();
        if (languageIndex < 0 || languageIndex >= supportedLanguages.Count)
            return;

        string newLanguage = supportedLanguages[languageIndex];

        // �����������ò����¼����ı�
        if (TextLocalizationManager.Instance != null)
        {
            await TextLocalizationManager.Instance.ChangeLanguageAsync(newLanguage);

            // �ı���ͨ���¼��Զ����£�����Ҫ�ֶ�ˢ��
            Debug.Log($"�������ø���Ϊ: {newLanguage}");
        }
    }

    private void SaveAllSettings()
    {
        // ȷ�����и��Ķ�������
        PlayerPrefs.Save();
        Debug.Log("���������ѱ���");
    }

    private void OnDisable()
    {
        // ȷ�����������ر�ʱ����
        SaveAllSettings();
    }
}*/