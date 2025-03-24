using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// 对话控制器，负责管理和创建对话显示器实例
/// </summary>
public class DialogueController : MonoBehaviour
{
    // 单例实例
    public static DialogueController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private UnityEngine.UI.Image characterImage;
    [SerializeField] private GameObject dialogueContainer;


    [Header("Dialog Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool useTypingEffect = true;
    [SerializeField] private float inputCooldown = 0.2f;

    // 当前活跃的对话显示器
    private DialogueDisplayBase activeDisplayer;

    // 对话显示器缓存，用于避免重复创建
    private Dictionary<string, DialogueDisplayBase> displayerCache =
        new Dictionary<string, DialogueDisplayBase>();

    // 输入冷却计时器
    private float lastInputTime = 0f;

    // 对话是否活跃
    private bool isDialogueActive = false;

    private void Awake()
    {
        // 设置单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始时隐藏对话面板
        dialogueContainer.SetActive(false);

    }

    private void Update()
    {
        // 只有在对话活跃时才检测输入
        if (isDialogueActive && activeDisplayer != null)
        {
            // 检查是否超过输入冷却时间
            if (Time.time - lastInputTime >= inputCooldown)
            {
                // 检测鼠标左键或空格键输入
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    AdvanceDialogue();
                    lastInputTime = Time.time; // 更新最后输入时间
                }
            }
        }
    }

    /// <summary>
    /// 启动对话
    /// </summary>
    /// <param name="eventId">对话事件ID</param>
    public async void StartDialogue(string eventId)
    {
        // 尝试从缓存获取显示器，如果不存在则创建新的
        if (!displayerCache.TryGetValue(eventId, out activeDisplayer))
        {
            activeDisplayer = CreateDisplayer(eventId);
            if (activeDisplayer != null) // 添加空检查
            {
                displayerCache[eventId] = activeDisplayer;
            }
        }

        if (activeDisplayer != null)
        {
            // 显示对话UI
            dialogueContainer.SetActive(true);

            // 初始化和开始对话
            await activeDisplayer.Initialize(eventId);
            activeDisplayer.StartDialogue();
        }
        else
        {
            Debug.LogError($"无法创建对话显示器，事件ID: {eventId}");
        }
    }

    /// <summary>
    /// 根据事件ID创建适当的对话显示器
    /// </summary>
    /// <param name="eventId">对话事件ID</param>
    /// <returns>对话显示器实例</returns>
    private DialogueDisplayBase CreateDisplayer(string eventId)
    {
        DialogueDisplayBase newDisplayer;

        // 根据具体的eventId精确匹配对应的对话管理器
        switch (eventId)
        {
            case "EventTemplate":
                newDisplayer = new TemplateDialogueManager(
                    this, dialogueText, speakerNameText, characterImage, dialogueContainer);
                break;

            // 其他所有事件ID默认使用线性对话管理器
            default:
                newDisplayer = new TemplateDialogueManager(
                    this, dialogueText, speakerNameText, characterImage, dialogueContainer);
                break;
        }

        // 设置对话显示参数 - 解决useTypingEffect未使用的警告
        if (newDisplayer != null)
        {
            ConfigureDisplayer(newDisplayer);
        }

        return newDisplayer;
    }

    /// <summary>
    /// 配置对话显示器的参数设置
    /// </summary>
    /// <param name="displayer">要配置的对话显示器</param>
    private void ConfigureDisplayer(DialogueDisplayBase displayer)
    {
        if (displayer == null)
            return;

        // 传递打字效果设置
        displayer.SetTypingSpeed(typingSpeed);
        displayer.SetUseTypingEffect(useTypingEffect);

        // 设置对话完成回调
        displayer.SetDialogueCompleteCallback(OnDialogueComplete);
    }

    /// <summary>
    /// 对话完成回调
    /// </summary>
    private void OnDialogueComplete()
    {
        isDialogueActive = false;
        dialogueContainer.SetActive(false);
        Debug.Log("对话已完成");
    }

    /// <summary>
    /// 推进对话（连接到UI按钮）
    /// </summary>
    public void AdvanceDialogue()
    {
        if (activeDisplayer != null)
        {
            activeDisplayer.AdvanceDialogue();
        }
    }

    /// <summary>
    /// 结束当前对话
    /// </summary>
    public void EndCurrentDialogue()
    {
        // 隐藏对话UI
        dialogueContainer.SetActive(false);
        isDialogueActive = false;
        activeDisplayer = null;
    }

    /// <summary>
    /// 清除对话缓存
    /// </summary>
    public void ClearCache()
    {
        displayerCache.Clear();
    }

    /// <summary>
    /// 设置对话完成回调
    /// </summary>
    /// <param name="callback">对话完成时要调用的回调函数</param>
    public void SetDialogueCompleteCallback(System.Action callback)
    {
        if (activeDisplayer != null)
        {
            // 使用链式回调 - 先调用内部的OnDialogueComplete，再调用自定义回调
            activeDisplayer.SetDialogueCompleteCallback(() => {
                OnDialogueComplete();
                callback?.Invoke();
            });
        }
    }
}