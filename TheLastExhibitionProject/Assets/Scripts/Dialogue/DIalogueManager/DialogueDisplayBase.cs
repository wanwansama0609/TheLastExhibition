using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// 对话显示基类（非MonoBehaviour），负责从DialogueParser获取数据并控制文本显示
/// </summary>
public abstract class DialogueDisplayBase
{
    // 引用MonoBehaviour组件来执行协程
    protected MonoBehaviour coroutineRunner;

    // UI引用
    protected TextMeshProUGUI dialogueText;
    protected TextMeshProUGUI speakerNameText;
    protected GameObject dialogueContainer;
    protected UnityEngine.UI.Image characterImage;

    // 打字效果设置
    protected float typingSpeed = 0.05f;
    protected bool useTypingEffect = true;

    // 当前对话数据缓存
    protected Dictionary<string, Dictionary<string, string>> dialogueData;

    // 显示状态
    protected bool isDisplayingText = false;
    protected Coroutine typingCoroutine;

    // 对话回调
    protected System.Action onDialogueComplete;
    protected System.Action<string> onSentenceDisplayed;

    // 角色图像回调事件
    public System.Action<Sprite> onCharacterSpriteChanged;

    // 当前对话状态
    protected int dialogueState = 0;

    // 固定的事件ID，每个子类关联一个特定事件
    public abstract string EventId { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="runner">用于运行协程的MonoBehaviour组件</param>
    /// <param name="dialogueText">对话文本UI</param>
    /// <param name="speakerNameText">说话者名称UI</param>
    /// <param name="characterImage">角色图像UI</param>
    /// <param name="dialogueContainer">对话UI容器</param>
    public DialogueDisplayBase(
        MonoBehaviour runner,
        TextMeshProUGUI dialogueText,
        TextMeshProUGUI speakerNameText,
        UnityEngine.UI.Image characterImage,
        GameObject dialogueContainer)
    {
        this.coroutineRunner = runner;
        this.dialogueText = dialogueText;
        this.speakerNameText = speakerNameText;
        this.characterImage = characterImage;
        this.dialogueContainer = dialogueContainer;
    }

    /// <summary>
    /// 设置打字速度
    /// </summary>
    /// <param name="speed">字符显示的时间间隔（秒）</param>
    public void SetTypingSpeed(float speed)
    {
        this.typingSpeed = speed;
    }

    /// <summary>
    /// 设置是否使用打字效果
    /// </summary>
    /// <param name="use">true表示使用打字效果，false表示直接显示全部文本</param>
    public void SetUseTypingEffect(bool use)
    {
        this.useTypingEffect = use;
    }

    /// <summary>
    /// 初始化对话显示器，加载事件ID的对话数据
    /// </summary>
    public virtual async Task Initialize()
    {
        // 确保对话事件已加载
        if (!DialogueParser.Instance.IsEventLoaded(EventId))
        {
            Debug.Log($"正在加载对话事件: {EventId}");
            bool success = await DialogueParser.Instance.LoadDialogueEventAsync(EventId);
            if (!success)
            {
                Debug.LogError($"加载对话事件失败: {EventId}");
                return;
            }
        }

        // 获取对话数据
        dialogueData = DialogueParser.Instance.GetDialoguesByEventId(EventId);

        if (dialogueData == null || dialogueData.Count == 0)
        {
            Debug.LogError($"对话事件数据为空: {EventId}");
            return;
        }

        // 重置对话状态
        dialogueState = 0;

        Debug.Log($"已成功初始化对话事件: {EventId}，共 {dialogueData.Count} 条句子");
    }

    /// <summary>
    /// 开始对话
    /// </summary>
    public virtual void StartDialogue()
    {
        // 显示对话UI
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(true);
        }

        // 重置对话状态并展示第一个对话
        dialogueState = 0;
        AdvanceDialogue();

        Debug.Log($"开始显示对话事件: {EventId}");
    }

    /// <summary>
    /// 推进对话到下一步 - 抽象方法，子类必须实现
    /// </summary>
    public abstract void AdvanceDialogue();

    /// <summary>
    /// 显示指定ID的句子
    /// </summary>
    /// <param name="sentenceId">要显示的句子ID</param>
    protected virtual IEnumerator DisplaySentence(string sentenceId)
    {
        isDisplayingText = true;

        if (!dialogueData.ContainsKey(sentenceId))
        {
            Debug.LogError($"在事件 {EventId} 中未找到句子: {sentenceId}");
            isDisplayingText = false;
            yield break;
        }

        // 获取句子数据
        Dictionary<string, string> attributes = dialogueData[sentenceId];

        // 显示说话者名称
        if (speakerNameText != null && attributes.ContainsKey("speaker"))
        {
            speakerNameText.text = attributes["speaker"];
        }

        // 显示文本内容
        if (dialogueText != null && attributes.ContainsKey("text"))
        {
            string fullText = attributes["text"];

            if (useTypingEffect)
            {
                // 使用打字机效果
                dialogueText.text = "";

                foreach (char c in fullText)
                {
                    dialogueText.text += c;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }
            else
            {
                // 直接显示全部文本
                dialogueText.text = fullText;
            }
        }

        // 显示角色图像
        if (characterImage != null)
        {
            Sprite characterSprite = null;

            // 检查是否有sprite属性并尝试加载
            if (attributes.ContainsKey("sprite") && !string.IsNullOrEmpty(attributes["sprite"]))
            {
                characterSprite = Resources.Load<Sprite>(attributes["sprite"]);
            }

            // 触发角色图像变更回调
            onCharacterSpriteChanged?.Invoke(characterSprite);

            // 如果加载成功显示图像
            if (characterSprite != null)
            {
                characterImage.sprite = characterSprite;
                characterImage.gameObject.SetActive(true);
            }
            else if (attributes.ContainsKey("sprite") && !string.IsNullOrEmpty(attributes["sprite"]))
            {
                Debug.LogWarning($"无法加载角色图像: {attributes["sprite"]}");
            }
        }

        // 触发句子显示完成回调
        onSentenceDisplayed?.Invoke(sentenceId);

        isDisplayingText = false;
    }

    /// <summary>
    /// 启动协程
    /// </summary>
    /// <param name="routine">要启动的协程</param>
    /// <returns>协程句柄</returns>
    protected Coroutine StartCoroutine(IEnumerator routine)
    {
        if (coroutineRunner != null)
        {
            return coroutineRunner.StartCoroutine(routine);
        }
        return null;
    }

    /// <summary>
    /// 停止协程
    /// </summary>
    /// <param name="routine">要停止的协程</param>
    protected void StopCoroutine(Coroutine routine)
    {
        if (coroutineRunner != null && routine != null)
        {
            coroutineRunner.StopCoroutine(routine);
        }
    }

    /// <summary>
    /// 立即显示完整句子文本（跳过打字效果）
    /// </summary>
    protected virtual void CompleteCurrentSentence(string sentenceId)
    {
        if (isDisplayingText && dialogueText != null)
        {
            // 停止打字效果协程
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            // 直接显示完整文本
            string fullText = DialogueParser.Instance.GetSentenceAttribute(EventId, sentenceId, "text");
            if (!string.IsNullOrEmpty(fullText))
            {
                dialogueText.text = fullText;
            }

            isDisplayingText = false;

            // 触发句子显示完成回调
            onSentenceDisplayed?.Invoke(sentenceId);
        }
    }

    /// <summary>
    /// 结束对话流程
    /// </summary>
    protected virtual void EndDialogue()
    {
        // 隐藏对话UI
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(false);
        }

        // 触发对话完成回调
        onDialogueComplete?.Invoke();

        Debug.Log($"对话事件 {EventId} 已结束");
    }

    /// <summary>
    /// 设置对话完成回调
    /// </summary>
    /// <param name="callback">对话完成时要调用的回调函数</param>
    public virtual void SetDialogueCompleteCallback(System.Action callback)
    {
        onDialogueComplete = callback;
    }

    /// <summary>
    /// 设置句子显示完成回调
    /// </summary>
    /// <param name="callback">每个句子显示完成时要调用的回调函数</param>
    public virtual void SetSentenceDisplayedCallback(System.Action<string> callback)
    {
        onSentenceDisplayed = callback;
    }

    /// <summary>
    /// 获取指定句子的属性
    /// </summary>
    /// <param name="sentenceId">句子ID</param>
    /// <param name="attributeName">属性名称</param>
    /// <returns>属性值，如果不存在则返回null</returns>
    protected string GetAttribute(string sentenceId, string attributeName)
    {
        return DialogueParser.Instance.GetSentenceAttribute(EventId, sentenceId, attributeName);
    }

    /// <summary>
    /// 检查当前是否正在显示文本
    /// </summary>
    /// <returns>如果正在显示文本则返回true，否则返回false</returns>
    public bool IsDisplayingText()
    {
        return isDisplayingText;
    }
}