/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏主菜单UI控制器
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("UI元素")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private GameObject settingsPanel;

    [Header("音效")]
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource audioSource;

    // 按钮文本键
    private const string START_GAME_KEY = "btn_start_game";
    private const string LOAD_GAME_KEY = "btn_load_game";
    private const string SETTINGS_KEY = "btn_settings";
    private const string EXIT_GAME_KEY = "btn_exit_game";

    private void Awake()
    {
        // 获取或添加音频源
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 确保设置面板初始状态为关闭
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void Start()
    {
        // 初始化按钮
        InitializeButtons();

        // 确保语言设置已加载
        if (TextLocalizationManager.Instance == null)
        {
            Debug.LogError("TextLocalizationManager实例不存在，无法加载本地化文本");
        }
    }

    private void InitializeButtons()
    {
        // 开始游戏按钮
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClicked);
            SetupButtonText(startGameButton, START_GAME_KEY);
        }

        // 加载游戏按钮
        if (loadGameButton != null)
        {
            loadGameButton.onClick.AddListener(OnLoadGameClicked);
            SetupButtonText(loadGameButton, LOAD_GAME_KEY);
        }

        // 设置按钮
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsClicked);
            SetupButtonText(settingsButton, SETTINGS_KEY);
        }

        // 退出游戏按钮
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(OnExitGameClicked);
            SetupButtonText(exitGameButton, EXIT_GAME_KEY);
        }
    }

    /// <summary>
    /// 设置按钮的本地化文本
    /// </summary>
    /// <param name="button">按钮</param>
    /// <param name="textKey">文本键</param>
    private void SetupButtonText(Button button, string textKey)
    {
        // 检查按钮是否已有LocalizedText组件
        LocalizedText localizedText = button.GetComponentInChildren<LocalizedText>();

        if (localizedText == null)
        {
            // 获取按钮的文本组件
            TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                // 添加LocalizedText组件
                localizedText = tmpText.gameObject.AddComponent<LocalizedText>();
                localizedText.SetTextKey(textKey);
            }
            else
            {
                Debug.LogWarning($"按钮 {button.name} 没有TextMeshProUGUI子组件，无法设置本地化文本");
            }
        }
        else
        {
            // 已有LocalizedText组件，直接设置键
            localizedText.SetTextKey(textKey);
        }
    }

    // 按钮点击事件处理方法
    private void OnStartGameClicked()
    {
        PlayButtonClickSound();
        Debug.Log("开始游戏按钮被点击");
        // 这里可以添加场景加载或其他游戏开始逻辑
        // SceneManager.LoadScene("GameScene");
    }

    private void OnLoadGameClicked()
    {
        PlayButtonClickSound();
        Debug.Log("加载游戏按钮被点击");
        // 这里可以添加存档加载逻辑
    }

    private void OnSettingsClicked()
    {
        PlayButtonClickSound();
        Debug.Log("设置按钮被点击");

        // 打开设置面板
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    private void OnExitGameClicked()
    {
        PlayButtonClickSound();
        Debug.Log("退出游戏按钮被点击");

        // 在编辑器中
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        // 在实际构建的应用中
#else
        Application.Quit();
#endif
    }

    // 播放按钮点击音效
    private void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    // 关闭设置面板的方法（可以由设置面板中的关闭按钮调用）
    public void CloseSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}*/