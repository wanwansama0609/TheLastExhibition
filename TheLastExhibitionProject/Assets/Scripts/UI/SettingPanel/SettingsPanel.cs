using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ��Ϸ��������������������AudioManager
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    [Header("UIԪ��")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("�ı�Ԫ��")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI musicVolumeLabel;
    [SerializeField] private TextMeshProUGUI sfxVolumeLabel;
    [SerializeField] private TextMeshProUGUI languageLabel;

    [Header("����")]
    [SerializeField] private MainMenuUI mainMenuUI;

    // �ı���
    private const string TITLE_KEY = "settings_title";
    private const string MUSIC_VOLUME_KEY = "settings_music_volume";
    private const string SFX_VOLUME_KEY = "settings_sfx_volume";
    private const string LANGUAGE_KEY = "settings_language";

    private bool isInitialized = false;

    private void Awake()
    {
        // ���û����Inspector���������ã����Ի�ȡMainMenuUI
        if (mainMenuUI == null)
        {
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
        SetupLocalizedText(languageLabel, LANGUAGE_KEY);
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
        // ��AudioManager��ȡ��������ã����AudioManager���������PlayerPrefs��ȡ

        // ��������
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

        // ��Ч����
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

        // ���֧�ֵ�����ѡ��
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        // �������ѡ��
        options.Add(new TMP_Dropdown.OptionData("��������"));

        // ���Ӣ��ѡ��
        options.Add(new TMP_Dropdown.OptionData("English"));

        // ���ѡ������б�
        languageDropdown.AddOptions(options);

        // ���õ�ǰѡ�е�����
        string currentLang = LanguageSetting.Instance.GetLanguage();
        languageDropdown.value = currentLang == "en" ? 1 : 0;
    }

    private void OnCloseButtonClicked()
    {
        // ���Ű�ť�����Ч
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }

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
        // ʹ��AudioManager������������
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
        else
        {
            // ���AudioManager�����ڣ���ֱ�ӱ��浽PlayerPrefs
            PlayerPrefs.SetFloat("MusicVolume", value);
            Debug.Log($"������������Ϊ: {value}");
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        // ʹ��AudioManager������Ч����
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);

            // ���Ų�����Ч�����û��ܹ��������������仯
            if (Random.value > 0.7f) // ���������Ч���������Ƶ��
            {
                AudioManager.Instance.PlaySFX("ButtonClick");
            }
        }
        else
        {
            // ���AudioManager�����ڣ���ֱ�ӱ��浽PlayerPrefs
            PlayerPrefs.SetFloat("SFXVolume", value);
            Debug.Log($"��Ч��������Ϊ: {value}");
        }
    }

    private void OnLanguageChanged(int languageIndex)
    {
        if (LanguageSetting.Instance == null)
            return;

        // ���Ű�ť�����Ч
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }

        // ���������˵�������������
        string newLanguage = languageIndex == 1 ? "en" : "zh";

        // ����������
        LanguageSetting.Instance.SetLanguage(newLanguage);

        // TextLocalizationManager���Զ�������Ա���������ı�
        Debug.Log($"�������ø���Ϊ: {newLanguage}");
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
}