using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// ��Ϸ���˵�UI��������������AudioManager
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("UIԪ��")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private GameObject settingsPanel;

    [Header("��Ƶ")]
    [SerializeField] private AudioClip buttonClickSound; // ��ѡ�����AudioManagerû��������Чʱʹ��

    private AudioSource localAudioSource; // ����AudioManager������ʱʹ�õı�����ƵԴ

    // ��ť�ı���
    private const string START_GAME_KEY = "btn_start_game";
    private const string LOAD_GAME_KEY = "btn_load_game";
    private const string SETTINGS_KEY = "btn_settings";
    private const string EXIT_GAME_KEY = "btn_exit_game";

    private void Awake()
    {
        // ��ȡ����ӱ�����ƵԴ������Ϊ���ã�
        localAudioSource = GetComponent<AudioSource>();
        if (localAudioSource == null)
        {
            localAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // ȷ����������ʼ״̬Ϊ�ر�
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void Start()
    {
        // ��ʼ����ť
        InitializeButtons();

        // ȷ�����������Ѽ���
        if (TextLocalizationManager.Instance == null)
        {
            Debug.LogWarning("TextLocalizationManagerʵ�������ڣ��޷����ر��ػ��ı�");
        }

        // �����AudioManager������AudioManagerע����Ч
        if (AudioManager.Instance != null && buttonClickSound != null)
        {
            // ��ѡ������ť��Ч��ӵ�AudioManager�У������û�����õĻ�
            // �������AudioManager��һ��AddSFX������ʵ��ȡ�������AudioManagerʵ��
            // AudioManager.Instance.AddSFX("ButtonClick", buttonClickSound);
        }
        else
        {
            // ʹ�ñ�����ƵԴ��������
            if (localAudioSource != null)
            {
                localAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            }
        }
    }

    private void InitializeButtons()
    {
        // ��ʼ��Ϸ��ť
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClicked);
            SetupButtonText(startGameButton, START_GAME_KEY);
        }

        // ������Ϸ��ť
        if (loadGameButton != null)
        {
            loadGameButton.onClick.AddListener(OnLoadGameClicked);
            SetupButtonText(loadGameButton, LOAD_GAME_KEY);
        }

        // ���ð�ť
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsClicked);
            SetupButtonText(settingsButton, SETTINGS_KEY);
        }

        // �˳���Ϸ��ť
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(OnExitGameClicked);
            SetupButtonText(exitGameButton, EXIT_GAME_KEY);
        }
    }

    /// <summary>
    /// ���ð�ť�ı��ػ��ı�
    /// </summary>
    /// <param name="button">��ť</param>
    /// <param name="textKey">�ı���</param>
    private void SetupButtonText(Button button, string textKey)
    {
        // ��鰴ť�Ƿ�����LocalizedText���
        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            // ����Ƿ�����LocalizedText���
            LocalizedText localizedText = tmpText.GetComponent<LocalizedText>();
            if (localizedText == null)
            {
                // ���LocalizedText���
                localizedText = tmpText.gameObject.AddComponent<LocalizedText>();
            }

            // �����ı���
            localizedText.SetTextKey(textKey);
        }
        else
        {
            Debug.LogWarning($"��ť {button.name} û��TextMeshProUGUI��������޷����ñ��ػ��ı�");
        }
    }

    // ��ť����¼�������
    private void OnStartGameClicked()
    {
        PlayButtonClickSound();
        Debug.Log("��ʼ��Ϸ��ť�����");
        // ���������ӳ������ػ�������Ϸ��ʼ�߼�
        // SceneManager.LoadScene("GameScene");
    }

    private void OnLoadGameClicked()
    {
        PlayButtonClickSound();
        Debug.Log("������Ϸ��ť�����");
        // ���������Ӵ浵�����߼�
    }

    private void OnSettingsClicked()
    {
        PlayButtonClickSound();
        Debug.Log("���ð�ť�����");

        // ���������
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    private void OnExitGameClicked()
    {
        PlayButtonClickSound();
        Debug.Log("�˳���Ϸ��ť�����");

        // �ڱ༭����
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        // ��ʵ�ʹ�����Ӧ����
#else
        Application.Quit();
#endif
    }

    // ���Ű�ť�����Ч
    private void PlayButtonClickSound()
    {
        // ����ʹ��AudioManager������Ч
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }
        // ���û��AudioManager����Ч���Ʋ�ƥ�䣬����ʹ�ñ���AudioSource
        else if (localAudioSource != null && buttonClickSound != null)
        {
            localAudioSource.PlayOneShot(buttonClickSound, localAudioSource.volume);
        }
    }

    // �ر��������ķ�������������������еĹرհ�ť���ã�
    public void CloseSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// ����Ч�����ı�ʱ���±�����ƵԴ������
    /// </summary>
    public void UpdateLocalAudioVolume(float volume)
    {
        if (localAudioSource != null)
        {
            localAudioSource.volume = volume;
        }
    }
}