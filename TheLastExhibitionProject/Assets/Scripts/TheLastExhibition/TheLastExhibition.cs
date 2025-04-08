using UnityEngine;

/// <summary>
/// 简易游戏主控脚本，用于测试对话系统
/// </summary>
public class TheLastExhibition : MonoBehaviour
{
    [Header("对话事件设置")]
    [SerializeField] private string testDialogueEventId = "FirstScene";
    [SerializeField] private KeyCode dialogueTestKey = KeyCode.BackQuote;  // ~ 键


    private void Start()
    {
        // 确保必要的系统组件存在
        InitializeRequiredSystems();
    }

    private void Update()
    {
        // 检测按键输入
        if (Input.GetKeyDown(dialogueTestKey))
        {
            Debug.Log($"启动测试对话: {testDialogueEventId}");
            StartTestDialogue();
        }
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
            DialogueController controller = controllerObj.AddComponent<DialogueController>();

        }

        if (AudioManager.Instance != null)
        {
            // 通过名称播放音乐
            AudioManager.Instance.PlayMusic("TestMusic");
        }
    }


    /// <summary>
    /// 启动测试对话
    /// </summary>
    private void StartTestDialogue()
    {
        if (DialogueController.Instance != null)
        {
            Debug.Log($"启动测试对话: {testDialogueEventId}");
            DialogueController.Instance.StartDialogue(testDialogueEventId);
        }
        else
        {
            Debug.LogError("DialogueController不存在，无法启动对话");
        }
    }
}