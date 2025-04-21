using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏主控脚本，使用单一的switch-case链式结构控制整个游戏流程
/// 当前仅包含对话流程，搜证部分已注释
/// </summary>
public class TheLastExhibition : MonoBehaviour
{
    /* 搜证场景部分 - 暂时注释掉
    [Header("搜证场景管理器")]
    [SerializeField] private GameObject evidenceSceneManager; // 单一的GameObject引用

    // 搜证场景组件引用（通过GetComponent获取）
    private TemplateEvidenceScene TemplateScene;
    
    // 当前活动的搜证场景
    private EvidenceSceneBase currentScene;
    */

    [Header("UI组件")]
    [SerializeField] private Button startButton;        // 开始按钮
    [SerializeField] private GameObject panelToDisable;  // 需要禁用的面板
    [SerializeField] private BackgroundManager bgManager;

    // 流程状态枚举
    public enum GameFlowState
    {
        MainMenu,           // 主菜单
        FirstScene,         // 第一场对话
        SecondScene,        // 第二场对话
        ThirdScene,         // 第三场对话
        /* 搜证场景部分 - 暂时注释掉
        LivingRoomInvestigation, // 客厅搜证
        AfterLivingRoomDialogue  // 客厅搜证后对话
        */
    }

    // 当前游戏状态
    private GameFlowState currentState = GameFlowState.MainMenu;


    // 对话ID映射
    private string GetDialogueIdForState(GameFlowState state)
    {
        switch (state)
        {
            case GameFlowState.FirstScene:
                return "FirstScene";
            case GameFlowState.SecondScene:
                return "SecondScene";
            case GameFlowState.ThirdScene:
                return "ThirdScene";
            /* 搜证场景部分 - 暂时注释掉
            case GameFlowState.OpeningDialogue:
                return "FirstScene";
            case GameFlowState.AfterLivingRoomDialogue:
                return "AfterLivingRoom";
            */
            default:
                return "";
        }
    }

    private void Start()
    {
        // 确保必要的系统组件存在
        InitializeRequiredSystems();

        bgManager = GetComponent<BackgroundManager>();
        /* 搜证场景部分 - 暂时注释掉
        // 获取所有搜证场景组件
        InitializeEvidenceScenes();
        */

        // 为开始按钮添加点击事件
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogError("未设置开始按钮，请在Inspector中指定StartButton");
        }

        // 监听对话结束事件
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.OnDialogueEnd += HandleDialogueEnd;
        }

        bgManager.SwitchToBackground(0);
    }

    private void OnDestroy()
    {
        // 取消事件监听
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.OnDialogueEnd -= HandleDialogueEnd;
        }

        /* 搜证场景部分 - 暂时注释掉
        // 清除搜证场景事件监听
        if (currentScene != null)
        {
            currentScene.OnAllEvidenceCollected -= HandleInvestigationCompleted;
        }
        */
    }

    /// <summary>
    /// 初始化必要的系统组件
    /// </summary>
    private void InitializeRequiredSystems()
    {
        // 检查LanguageSetting是否存在
        if (LanguageSetting.Instance == null)
        {
            GameObject settingsObj = new GameObject("LanguageSetting");
            settingsObj.AddComponent<LanguageSetting>();
        }

        // 检查DialogueParser是否存在
        if (DialogueParser.Instance == null)
        {
            GameObject parserObj = new GameObject("DialogueParser");
            parserObj.AddComponent<DialogueParser>();
        }

        // 检查DialogueController是否存在
        if (DialogueController.Instance == null)
        {
            GameObject controllerObj = new GameObject("DialogueController");
            controllerObj.AddComponent<DialogueController>();
        }

        // 检查并播放背景音乐
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic("MainTheme");  // 更改为主题音乐
        }
    }

    /// <summary>
    /// 开始按钮点击事件处理函数
    /// </summary>
    public void OnStartButtonClicked()
    {
        Debug.Log("开始按钮被点击，启动游戏流程");

        // 禁用指定面板及其所有子UI元素
        DisablePanelAndChildren();

        // 设置初始状态并处理流程
        SetState(GameFlowState.FirstScene);
    }

    /// <summary>
    /// 禁用指定面板及其所有子UI元素
    /// </summary>
    private void DisablePanelAndChildren()
    {
        if (panelToDisable != null)
        {
            // 禁用面板本身
            panelToDisable.SetActive(false);
            Debug.Log($"已禁用面板: {panelToDisable.name}及其所有子UI元素");
        }
        else
        {
            Debug.LogWarning("未设置需要禁用的面板，请在Inspector中指定PanelToDisable");
        }
    }

    /// <summary>
    /// 设置游戏状态并处理相应的流程
    /// </summary>
    private void SetState(GameFlowState newState)
    {
        // 更新状态
        currentState = newState;
        Debug.Log($"当前游戏状态: {currentState}");

        // 处理当前状态的流程
        ProcessGameFlow();
    }

    /// <summary>
    /// 游戏流程的主要switch-case处理方法
    /// </summary>
    private void ProcessGameFlow()
    {
        switch (currentState)
        {
            case GameFlowState.MainMenu:
                // 主菜单状态，不做特殊处理
                break;

            case GameFlowState.FirstScene:
            case GameFlowState.SecondScene:
            case GameFlowState.ThirdScene:
                // 启动对应的对话
                StartDialogue(GetDialogueIdForState(currentState));
                break;

                /* 搜证场景部分 - 暂时注释掉
                case GameFlowState.LivingRoomInvestigation:
                    // 客厅搜证
                    StartInvestigation(TemplateScene);
                    break;

                case GameFlowState.AfterLivingRoomDialogue:
                    // 客厅搜证后的对话
                    StartDialogue(GetDialogueIdForState(currentState));
                    break;
                */
        }
    }

    /// <summary>
    /// 处理对话结束事件
    /// </summary>
    private void HandleDialogueEnd(string dialogueId)
    {
        Debug.Log($"对话已结束: {dialogueId}");

        // 根据当前状态决定下一个状态
        switch (currentState)
        {
            case GameFlowState.FirstScene:
                // 第一场对话结束后进入第二场对话
                SetState(GameFlowState.SecondScene);
                break;

            case GameFlowState.SecondScene:
                // 第二场对话结束后进入第三场对话
                SetState(GameFlowState.ThirdScene);
                break;

            case GameFlowState.ThirdScene:
                // 第三场对话结束后，游戏流程结束
                Debug.Log("游戏流程结束");
                // 可以在这里添加结束游戏的逻辑，如显示结束画面、返回主菜单等
                break;

                /* 搜证场景部分 - 暂时注释掉
                case GameFlowState.OpeningDialogue:
                    // 开场对话结束后进入客厅搜证
                    SetState(GameFlowState.LivingRoomInvestigation);
                    break;

                case GameFlowState.AfterLivingRoomDialogue:
                    // 客厅搜证后对话结束后，可以在这里添加下一步逻辑
                    Debug.Log("游戏流程结束");
                    break;
                */
        }
    }

    /// <summary>
    /// 启动对话
    /// </summary>
    private void StartDialogue(string dialogueId)
    {
        if (DialogueController.Instance != null && !string.IsNullOrEmpty(dialogueId))
        {
            Debug.Log($"启动对话: {dialogueId}");
            DialogueController.Instance.StartDialogue(dialogueId);
        }
        else
        {
            Debug.LogError("DialogueController不存在或对话ID为空，无法启动对话");
        }
    }

    /// <summary>
    /// 获取当前游戏状态
    /// </summary>
    public GameFlowState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// 手动设置游戏状态（用于调试）
    /// </summary>
    public void JumpToState(GameFlowState state)
    {
        SetState(state);
    }

    /* 搜证场景部分 - 暂时注释掉
    /// <summary>
    /// 初始化所有搜证场景组件
    /// </summary>
    private void InitializeEvidenceScenes()
    {
        if (evidenceSceneManager == null)
        {
            Debug.LogError("未设置搜证场景管理器，请在Inspector中指定EvidenceSceneManager");
            return;
        }

        // 获取所有搜证场景组件
        TemplateScene = evidenceSceneManager.GetComponent<TemplateEvidenceScene>();

        // 检查是否所有组件都成功获取
        if (TemplateScene == null)
            Debug.LogError("未找到TemplateEvidenceScene组件");
    }

    /// <summary>
    /// 处理搜证完成事件
    /// </summary>
    private void HandleInvestigationCompleted(string sceneId)
    {
        Debug.Log($"搜证完成: {sceneId}");

        // 根据当前状态决定下一个状态
        switch (currentState)
        {
            case GameFlowState.LivingRoomInvestigation:
                // 客厅搜证完成后进入客厅后对话
                SetState(GameFlowState.AfterLivingRoomDialogue);
                break;
        }
    }

    /// <summary>
    /// 启动搜证场景
    /// </summary>
    private void StartInvestigation(EvidenceSceneBase scene)
    {
        // 清除当前场景的事件监听
        if (currentScene != null)
        {
            currentScene.OnAllEvidenceCollected -= HandleInvestigationCompleted;
        }

        if (scene == null)
        {
            Debug.LogError($"搜证场景为空，无法启动");
            return;
        }

        // 设置当前场景
        currentScene = scene;

        // 订阅搜证完成事件
        currentScene.OnAllEvidenceCollected += HandleInvestigationCompleted;

        // 初始化搜证场景
        currentScene.InitializeScene();

        Debug.Log($"开始搜证: {currentScene.GetSceneId()}");
    }
    */
}