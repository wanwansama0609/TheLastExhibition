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
            if (activeDisplayer != null)
            {
                displayerCache[eventId] = activeDisplayer;
            }
        }

        if (activeDisplayer != null)
        {
            // 显示对话UI
            dialogueContainer.SetActive(true);
            isDialogueActive = true;

            // 初始化和开始对话
            await activeDisplayer.Initialize();
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
        DialogueDisplayBase newDisplayer = null;

        // 根据事件ID创建对应的对话管理器
        switch (eventId)
        {
            case "EventTemplate":
                newDisplayer = new TemplateDialogueManager(
                    this, dialogueText, speakerNameText, characterImage, dialogueContainer);
                break;


            case "FirstScene":
                newDisplayer = new FirstSceneDialogueManager(
                    this, dialogueText, speakerNameText, characterImage, dialogueContainer);
                break;


            // 添加其他对话管理器的case
            // case "EventA":
            //     newDisplayer = new EventADialogueManager(...);
            //     break;

            default:
                Debug.LogError($"未找到对应的对话管理器: {eventId}");
                break;
        }

        // 设置对话显示参数
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

        // 设置角色图像回调 - 现在可以直接对所有类型的对话显示器使用
        displayer.onCharacterSpriteChanged = OnCharacterSpriteChanged;
    }

    /// <summary>
    /// 处理角色图像变更事件
    /// </summary>
    /// <param name="sprite">加载的角色精灵，可能为null</param>
    private void OnCharacterSpriteChanged(Sprite sprite)
    {
        if (characterImage == null)
            return;

        if (sprite == null)
        {
            // 如果sprite为null，直接停用图像组件
            characterImage.gameObject.SetActive(false);
        }
        else
        {
            // 如果sprite有效，启用图像组件
            characterImage.gameObject.SetActive(true);

            // sprite已在DialogueDisplayBase中设置
        }
    }

    /// <summary>
    /// 对话完成回调
    /// </summary>
    private void OnDialogueComplete()
    {
        isDialogueActive = false;
        dialogueContainer.SetActive(false);

        // 获取当前活跃对话器的事件ID
        string eventId = activeDisplayer?.EventId;

        // 从缓存中移除对话显示器
        if (!string.IsNullOrEmpty(eventId) && displayerCache.ContainsKey(eventId))
        {
            displayerCache.Remove(eventId);
        }

        // 释放DialogueParser中加载的JSON数据
        if (!string.IsNullOrEmpty(eventId))
        {
            DialogueParser.Instance.UnloadDialogueEvent(eventId);
        }

        // 清除当前活跃的对话显示器
        activeDisplayer = null;

        Debug.Log($"对话已完成，已释放资源: {eventId}");
    }

    /// <summary>
    /// 推进对话
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