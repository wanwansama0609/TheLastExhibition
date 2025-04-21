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

    // 新增：对话文本变更回调
    public System.Action<string> onDialogueTextChanged;

    // 新增：说话者名称变更回调
    public System.Action<string> onSpeakerNameChanged;

    // 证物的状态
    protected bool isWaitingForEvidence = false;
    protected string waitingForEvidenceId = null;
    protected EvidenceButton activeEvidenceButton = null;

    // 当前点击的证物ID
    protected string currentClickedEvidenceId = null;

    // 当前对话状态
    protected int dialogueState = 0;
    // 当前正在打字的句子ID
    protected string activeTypingSentenceId;


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
    /// 检查是否正在等待证物收集
    /// </summary>
    public bool IsWaitingForEvidence()
    {
        return isWaitingForEvidence;
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
        currentClickedEvidenceId = null;

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
    /// 开始监听证物点击事件
    /// </summary>
    protected void StartListeningForEvidenceClicks()
    {
        EvidenceButton.OnEvidenceClickedWithId += OnEvidenceClicked;
    }

    /// <summary>
    /// 停止监听证物点击事件
    /// </summary>
    protected void StopListeningForEvidenceClicks()
    {
        EvidenceButton.OnEvidenceClickedWithId -= OnEvidenceClicked;
    }

    /// <summary>
    /// 处理证物点击事件
    /// </summary>
    /// <param name="evidenceId">被点击的证物ID</param>
    protected virtual void OnEvidenceClicked(string evidenceId)
    {
        currentClickedEvidenceId = evidenceId;
        Debug.Log($"点击了证物: {evidenceId}");

        // 子类可以重写此方法以提供具体的处理逻辑
        // 默认行为是推进对话
        AdvanceDialogue();
    }

    /// <summary>
    /// 显示指定ID的句子
    /// </summary>
    /// <param name="sentenceId">要显示的句子ID</param>
    protected virtual IEnumerator DisplaySentence(string sentenceId)
    {
        isDisplayingText = true;
        activeTypingSentenceId = sentenceId;

        if (!dialogueData.ContainsKey(sentenceId))
        {
            Debug.LogError($"在事件 {EventId} 中未找到句子: {sentenceId}");
            isDisplayingText = false;
            yield break;
        }

        // 获取句子数据
        Dictionary<string, string> attributes = dialogueData[sentenceId];

        // 获取说话者名称
        string speakerName = attributes.ContainsKey("speaker") ? attributes["speaker"] : null;

        // 获取文本内容
        string fullText = attributes.ContainsKey("text") ? attributes["text"] : null;

        // 获取角色图像
        Sprite characterSprite = null;
        if (attributes.ContainsKey("sprite") && !string.IsNullOrEmpty(attributes["sprite"]))
        {
            characterSprite = Resources.Load<Sprite>(attributes["sprite"]);
            if (characterSprite == null && !string.IsNullOrEmpty(attributes["sprite"]))
            {
                Debug.LogWarning($"无法加载角色图像: {attributes["sprite"]}");
            }
        }

        // 分别处理各个UI元素的显示

        // 1. 处理说话者名称 - 使用回调或直接设置
        if (onSpeakerNameChanged != null)
        {
            onSpeakerNameChanged.Invoke(speakerName);
        }
        else if (speakerNameText != null)
        {
            if (string.IsNullOrEmpty(speakerName))
            {
                speakerNameText.gameObject.SetActive(false);
            }
            else
            {
                speakerNameText.gameObject.SetActive(true);
                speakerNameText.text = speakerName;
            }
        }

        // 2. 处理角色图像 - 使用回调
        onCharacterSpriteChanged?.Invoke(characterSprite);

        // 3. 处理对话文本 - 使用回调或直接设置
        if (string.IsNullOrEmpty(fullText))
        {
            // 文本为空，隐藏文本组件
            if (onDialogueTextChanged != null)
            {
                onDialogueTextChanged.Invoke(null);
            }
            else if (dialogueText != null)
            {
                dialogueText.gameObject.SetActive(false);
            }
        }
        else
        {
            // 有文本内容，使用打字效果或直接显示
            if (useTypingEffect)
            {
                // 先清空文本，重要修改点！
                if (onDialogueTextChanged != null)
                {
                    onDialogueTextChanged.Invoke("");
                }
                else if (dialogueText != null)
                {
                    dialogueText.gameObject.SetActive(true);
                    dialogueText.text = "";
                }

                // 使用打字机效果
                // 创建一个临时字符串，而不是在现有文本上添加
                string currentText = "";

                foreach (char c in fullText)
                {
                    currentText += c; // 修改点：使用临时变量累加字符

                    if (onDialogueTextChanged != null)
                    {
                        onDialogueTextChanged.Invoke(currentText);
                    }
                    else if (dialogueText != null)
                    {
                        dialogueText.text = currentText;
                    }

                    yield return new WaitForSeconds(typingSpeed);
                }
            }
            else
            {
                // 直接显示全部文本
                if (onDialogueTextChanged != null)
                {
                    onDialogueTextChanged.Invoke(fullText);
                }
                else if (dialogueText != null)
                {
                    dialogueText.gameObject.SetActive(true);
                    dialogueText.text = fullText;
                }
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
    protected virtual void CompleteCurrentSentence()
    {
        if (isDisplayingText)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            string fullText = DialogueParser.Instance.GetSentenceAttribute(EventId, activeTypingSentenceId, "text");

            if (!string.IsNullOrEmpty(fullText))
            {
                if (onDialogueTextChanged != null)
                {
                    onDialogueTextChanged.Invoke(fullText);
                }
                else if (dialogueText != null)
                {
                    dialogueText.gameObject.SetActive(true);
                    dialogueText.text = fullText;
                }
            }

            isDisplayingText = false;

            onSentenceDisplayed?.Invoke(activeTypingSentenceId);
        }
    }

    /// <summary>
    /// 结束对话流程
    /// </summary>
    protected virtual void EndDialogue()
    {
        // 确保停止监听证物点击
        StopListeningForEvidenceClicks();

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

    /// <summary>
    /// 设置UI组件的可见性
    /// </summary>
    /// <param name="visible">是否可见</param>
    protected virtual void SetComponentsVisibility(bool visible)
    {
        // 设置各个组件的可见性
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(visible);

        if (speakerNameText != null)
            speakerNameText.gameObject.SetActive(visible);

        if (characterImage != null)
            characterImage.gameObject.SetActive(visible);
    }

    /// <summary>
    /// 放置证物并开始监听点击
    /// </summary>
    /// <param name="evidenceInfos">证物信息列表，包含ID、名称、描述和位置</param>
    protected void PlaceEvidenceAndListenForClicks(List<(string id, string name, string description, Vector2 position, Vector2 size)> evidenceInfos)
    {
        // 放置证物
        foreach (var info in evidenceInfos)
        {
            EvidenceManager.Instance.CreateEvidence(
                info.id, info.name, info.description,
                dialogueContainer.transform, info.position, info.size);
        }

        // 开始监听证物点击
        StartListeningForEvidenceClicks();

        // 暂时禁用普通点击推进对话
        DialogueController.Instance.SetInputEnabled(false);
    }

    /// <summary>
    /// 清理证物并停止监听点击
    /// </summary>
    protected void CleanupEvidenceAndStopListening()
    {
        // 停止监听证物点击
        StopListeningForEvidenceClicks();

        // 销毁所有证物
        EvidenceManager.Instance.DestroyAllEvidence();

        // 重新启用普通点击推进对话
        DialogueController.Instance.SetInputEnabled(true);
    }

    /// <summary>
    /// 等待证物被收集后推进对话
    /// </summary>
    /// <param name="evidenceId">需要收集的证物ID</param>
    /// <param name="name">证物名称</param>
    /// <param name="description">证物描述</param>
    /// <param name="position">放置位置</param>
    /// <param name="size">证物大小</param>
    protected virtual IEnumerator WaitForEvidenceCollection(string evidenceId, string name, string description,
                                                           Vector2 position, Vector2 size)
    {
        // 注册证物收集事件
        EvidenceManager.Instance.OnEvidenceCollected += OnEvidenceCollected;

        // 创建证物
        activeEvidenceButton = EvidenceManager.Instance.CreateEvidence(
            evidenceId, name, description,
            dialogueContainer.transform, position, size);

        // 保存需要等待收集的证物ID
        waitingForEvidenceId = evidenceId;

        // 等待证物被收集
        isWaitingForEvidence = true;

        // 等待直到证物被收集
        while (isWaitingForEvidence)
        {
            yield return null;
        }

        // 取消注册事件
        EvidenceManager.Instance.OnEvidenceCollected -= OnEvidenceCollected;

        // 销毁证物按钮（可选，视游戏需求而定）
        if (activeEvidenceButton != null)
        {
            EvidenceManager.Instance.DestroyEvidence(evidenceId);
            activeEvidenceButton = null;
        }
    }

    // 证物收集回调
    private void OnEvidenceCollected(string collectedEvidenceId)
    {
        // 检查是否是我们正在等待的证物
        if (isWaitingForEvidence && collectedEvidenceId == waitingForEvidenceId)
        {
            // 停止等待
            isWaitingForEvidence = false;
        }
    }

    // 简化版方法，使用默认大小
    protected virtual IEnumerator WaitForEvidenceCollection(string evidenceId, string name, string description, Vector2 position)
    {
        return WaitForEvidenceCollection(evidenceId, name, description, position, new Vector2(100, 100));
    }
}