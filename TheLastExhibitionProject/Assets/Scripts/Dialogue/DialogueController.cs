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
    [SerializeField] private RectTransform evidencePanel;

    [Header("Dialog Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool useTypingEffect = true;
    [SerializeField] private float inputCooldown = 0.2f;

    // 添加到 DialogueController 类中
    private bool inputEnabled = true;

    // 当前活跃的对话显示器
    private DialogueDisplayBase activeDisplayer;
    private EvidenceManager evidenceManager;

    // 对话显示器缓存，用于避免重复创建
    private Dictionary<string, DialogueDisplayBase> displayerCache =
        new Dictionary<string, DialogueDisplayBase>();


    // 输入冷却计时器
    private float lastInputTime = 0f;

    // 对话是否活跃
    private bool isDialogueActive = false;

    // 当前对话事件ID
    private string currentDialogueEventId;

    // 对话结束事件，返回对话的事件ID
    public System.Action<string> OnDialogueEnd;

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

        // 获取证物管理器引用
        evidenceManager = EvidenceManager.Instance;
        if (evidenceManager == null)
        {
            Debug.LogError("未找到EvidenceManager实例");
        }
    }
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }
    private void Update()
    {
        // 只有在对话活跃时才检测输入
        if (isDialogueActive && activeDisplayer != null && inputEnabled)
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


        // 保存当前对话事件ID
        currentDialogueEventId = eventId;

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
                    this, dialogueText, speakerNameText, characterImage, dialogueContainer, evidencePanel);
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

        // 设置角色图像回调
        displayer.onCharacterSpriteChanged = OnCharacterSpriteChanged;

        // 设置新增的回调
        displayer.onDialogueTextChanged = OnDialogueTextChanged;
        displayer.onSpeakerNameChanged = OnSpeakerNameChanged;
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
            characterImage.sprite = sprite;
        }
    }

    /// <summary>
    /// 处理对话文本变更事件
    /// </summary>
    /// <param name="text">对话文本，可能为空</param>
    private void OnDialogueTextChanged(string text)
    {
        if (dialogueText == null || dialogueContainer == null)
            return;

        if (string.IsNullOrEmpty(text))
        {
            dialogueContainer.SetActive(false); // 改成隐藏整个对话面板
        }
        else
        {
            dialogueContainer.SetActive(true);
            dialogueText.gameObject.SetActive(true);
            dialogueText.text = text;
        }
    }

    /// <summary>
    /// 处理说话者名称变更事件
    /// </summary>
    /// <param name="speakerName">说话者名称，可能为空</param>
    private void OnSpeakerNameChanged(string speakerName)
    {
        if (speakerNameText == null)
            return;

        if (string.IsNullOrEmpty(speakerName))
        {
            // 如果名称为空，停用名称组件
            speakerNameText.gameObject.SetActive(false);
        }
        else
        {
            // 如果名称有效，启用名称组件
            speakerNameText.gameObject.SetActive(true);
            speakerNameText.text = speakerName;
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
        string eventId = currentDialogueEventId;

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

        // 触发对话结束事件
        OnDialogueEnd?.Invoke(eventId);

        // 清除当前活跃的对话显示器
        activeDisplayer = null;
        currentDialogueEventId = null;

        Debug.Log($"对话已完成，已释放资源: {eventId}");

        // 清理当前场景中的所有证物
        if (evidenceManager != null)
        {
            evidenceManager.DestroyAllEvidence();
        }
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

        // 触发对话结束事件
        if (!string.IsNullOrEmpty(currentDialogueEventId))
        {
            OnDialogueEnd?.Invoke(currentDialogueEventId);
        }

        // 清理
        activeDisplayer = null;
        currentDialogueEventId = null;
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


    /// <summary>
    /// 处理证物收集事件
    /// </summary>
    private void OnEvidenceCollected(string evidenceId)
    {
        // 如果当前有活跃的对话显示器且它正在等待证物收集
        if (activeDisplayer != null && activeDisplayer.IsWaitingForEvidence())
        {
            // 有可能需要执行一些额外的逻辑
            // 例如播放音效、显示提示等
        }
    }
}