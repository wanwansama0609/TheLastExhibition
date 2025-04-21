using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// �Ի���ʾ���ࣨ��MonoBehaviour���������DialogueParser��ȡ���ݲ������ı���ʾ
/// </summary>
public abstract class DialogueDisplayBase
{
    // ����MonoBehaviour�����ִ��Э��
    protected MonoBehaviour coroutineRunner;

    // UI����
    protected TextMeshProUGUI dialogueText;
    protected TextMeshProUGUI speakerNameText;
    protected GameObject dialogueContainer;
    protected UnityEngine.UI.Image characterImage;

    // ����Ч������
    protected float typingSpeed = 0.05f;
    protected bool useTypingEffect = true;

    // ��ǰ�Ի����ݻ���
    protected Dictionary<string, Dictionary<string, string>> dialogueData;

    // ��ʾ״̬
    protected bool isDisplayingText = false;
    protected Coroutine typingCoroutine;

    // �Ի��ص�
    protected System.Action onDialogueComplete;
    protected System.Action<string> onSentenceDisplayed;

    // ��ɫͼ��ص��¼�
    public System.Action<Sprite> onCharacterSpriteChanged;

    // �������Ի��ı�����ص�
    public System.Action<string> onDialogueTextChanged;

    // ������˵�������Ʊ���ص�
    public System.Action<string> onSpeakerNameChanged;

    // ֤���״̬
    protected bool isWaitingForEvidence = false;
    protected string waitingForEvidenceId = null;
    protected EvidenceButton activeEvidenceButton = null;

    // ��ǰ�����֤��ID
    protected string currentClickedEvidenceId = null;

    // ��ǰ�Ի�״̬
    protected int dialogueState = 0;
    // ��ǰ���ڴ��ֵľ���ID
    protected string activeTypingSentenceId;


    // �̶����¼�ID��ÿ���������һ���ض��¼�
    public abstract string EventId { get; }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="runner">��������Э�̵�MonoBehaviour���</param>
    /// <param name="dialogueText">�Ի��ı�UI</param>
    /// <param name="speakerNameText">˵��������UI</param>
    /// <param name="characterImage">��ɫͼ��UI</param>
    /// <param name="dialogueContainer">�Ի�UI����</param>
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
    /// ���ô����ٶ�
    /// </summary>
    /// <param name="speed">�ַ���ʾ��ʱ�������룩</param>
    public void SetTypingSpeed(float speed)
    {
        this.typingSpeed = speed;
    }

    /// <summary>
    /// ����Ƿ����ڵȴ�֤���ռ�
    /// </summary>
    public bool IsWaitingForEvidence()
    {
        return isWaitingForEvidence;
    }

    /// <summary>
    /// �����Ƿ�ʹ�ô���Ч��
    /// </summary>
    /// <param name="use">true��ʾʹ�ô���Ч����false��ʾֱ����ʾȫ���ı�</param>
    public void SetUseTypingEffect(bool use)
    {
        this.useTypingEffect = use;
    }

    /// <summary>
    /// ��ʼ���Ի���ʾ���������¼�ID�ĶԻ�����
    /// </summary>
    public virtual async Task Initialize()
    {
        // ȷ���Ի��¼��Ѽ���
        if (!DialogueParser.Instance.IsEventLoaded(EventId))
        {
            Debug.Log($"���ڼ��ضԻ��¼�: {EventId}");
            bool success = await DialogueParser.Instance.LoadDialogueEventAsync(EventId);
            if (!success)
            {
                Debug.LogError($"���ضԻ��¼�ʧ��: {EventId}");
                return;
            }
        }

        // ��ȡ�Ի�����
        dialogueData = DialogueParser.Instance.GetDialoguesByEventId(EventId);

        if (dialogueData == null || dialogueData.Count == 0)
        {
            Debug.LogError($"�Ի��¼�����Ϊ��: {EventId}");
            return;
        }

        // ���öԻ�״̬
        dialogueState = 0;
        currentClickedEvidenceId = null;

        Debug.Log($"�ѳɹ���ʼ���Ի��¼�: {EventId}���� {dialogueData.Count} ������");
    }

    /// <summary>
    /// ��ʼ�Ի�
    /// </summary>
    public virtual void StartDialogue()
    {
        // ��ʾ�Ի�UI
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(true);
        }

        // ���öԻ�״̬��չʾ��һ���Ի�
        dialogueState = 0;
        AdvanceDialogue();

        Debug.Log($"��ʼ��ʾ�Ի��¼�: {EventId}");
    }

    /// <summary>
    /// �ƽ��Ի�����һ�� - ���󷽷����������ʵ��
    /// </summary>
    public abstract void AdvanceDialogue();

    /// <summary>
    /// ��ʼ����֤�����¼�
    /// </summary>
    protected void StartListeningForEvidenceClicks()
    {
        EvidenceButton.OnEvidenceClickedWithId += OnEvidenceClicked;
    }

    /// <summary>
    /// ֹͣ����֤�����¼�
    /// </summary>
    protected void StopListeningForEvidenceClicks()
    {
        EvidenceButton.OnEvidenceClickedWithId -= OnEvidenceClicked;
    }

    /// <summary>
    /// ����֤�����¼�
    /// </summary>
    /// <param name="evidenceId">�������֤��ID</param>
    protected virtual void OnEvidenceClicked(string evidenceId)
    {
        currentClickedEvidenceId = evidenceId;
        Debug.Log($"�����֤��: {evidenceId}");

        // ���������д�˷������ṩ����Ĵ����߼�
        // Ĭ����Ϊ���ƽ��Ի�
        AdvanceDialogue();
    }

    /// <summary>
    /// ��ʾָ��ID�ľ���
    /// </summary>
    /// <param name="sentenceId">Ҫ��ʾ�ľ���ID</param>
    protected virtual IEnumerator DisplaySentence(string sentenceId)
    {
        isDisplayingText = true;
        activeTypingSentenceId = sentenceId;

        if (!dialogueData.ContainsKey(sentenceId))
        {
            Debug.LogError($"���¼� {EventId} ��δ�ҵ�����: {sentenceId}");
            isDisplayingText = false;
            yield break;
        }

        // ��ȡ��������
        Dictionary<string, string> attributes = dialogueData[sentenceId];

        // ��ȡ˵��������
        string speakerName = attributes.ContainsKey("speaker") ? attributes["speaker"] : null;

        // ��ȡ�ı�����
        string fullText = attributes.ContainsKey("text") ? attributes["text"] : null;

        // ��ȡ��ɫͼ��
        Sprite characterSprite = null;
        if (attributes.ContainsKey("sprite") && !string.IsNullOrEmpty(attributes["sprite"]))
        {
            characterSprite = Resources.Load<Sprite>(attributes["sprite"]);
            if (characterSprite == null && !string.IsNullOrEmpty(attributes["sprite"]))
            {
                Debug.LogWarning($"�޷����ؽ�ɫͼ��: {attributes["sprite"]}");
            }
        }

        // �ֱ������UIԪ�ص���ʾ

        // 1. ����˵�������� - ʹ�ûص���ֱ������
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

        // 2. �����ɫͼ�� - ʹ�ûص�
        onCharacterSpriteChanged?.Invoke(characterSprite);

        // 3. ����Ի��ı� - ʹ�ûص���ֱ������
        if (string.IsNullOrEmpty(fullText))
        {
            // �ı�Ϊ�գ������ı����
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
            // ���ı����ݣ�ʹ�ô���Ч����ֱ����ʾ
            if (useTypingEffect)
            {
                // ������ı�����Ҫ�޸ĵ㣡
                if (onDialogueTextChanged != null)
                {
                    onDialogueTextChanged.Invoke("");
                }
                else if (dialogueText != null)
                {
                    dialogueText.gameObject.SetActive(true);
                    dialogueText.text = "";
                }

                // ʹ�ô��ֻ�Ч��
                // ����һ����ʱ�ַ������������������ı������
                string currentText = "";

                foreach (char c in fullText)
                {
                    currentText += c; // �޸ĵ㣺ʹ����ʱ�����ۼ��ַ�

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
                // ֱ����ʾȫ���ı�
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

        // ����������ʾ��ɻص�
        onSentenceDisplayed?.Invoke(sentenceId);

        isDisplayingText = false;
    }

    /// <summary>
    /// ����Э��
    /// </summary>
    /// <param name="routine">Ҫ������Э��</param>
    /// <returns>Э�̾��</returns>
    protected Coroutine StartCoroutine(IEnumerator routine)
    {
        if (coroutineRunner != null)
        {
            return coroutineRunner.StartCoroutine(routine);
        }
        return null;
    }

    /// <summary>
    /// ֹͣЭ��
    /// </summary>
    /// <param name="routine">Ҫֹͣ��Э��</param>
    protected void StopCoroutine(Coroutine routine)
    {
        if (coroutineRunner != null && routine != null)
        {
            coroutineRunner.StopCoroutine(routine);
        }
    }

    /// <summary>
    /// ������ʾ���������ı�����������Ч����
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
    /// �����Ի�����
    /// </summary>
    protected virtual void EndDialogue()
    {
        // ȷ��ֹͣ����֤����
        StopListeningForEvidenceClicks();

        // ���ضԻ�UI
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(false);
        }

        // �����Ի���ɻص�
        onDialogueComplete?.Invoke();

        Debug.Log($"�Ի��¼� {EventId} �ѽ���");
    }

    /// <summary>
    /// ���öԻ���ɻص�
    /// </summary>
    /// <param name="callback">�Ի����ʱҪ���õĻص�����</param>
    public virtual void SetDialogueCompleteCallback(System.Action callback)
    {
        onDialogueComplete = callback;
    }

    /// <summary>
    /// ���þ�����ʾ��ɻص�
    /// </summary>
    /// <param name="callback">ÿ��������ʾ���ʱҪ���õĻص�����</param>
    public virtual void SetSentenceDisplayedCallback(System.Action<string> callback)
    {
        onSentenceDisplayed = callback;
    }

    /// <summary>
    /// ��ȡָ�����ӵ�����
    /// </summary>
    /// <param name="sentenceId">����ID</param>
    /// <param name="attributeName">��������</param>
    /// <returns>����ֵ������������򷵻�null</returns>
    protected string GetAttribute(string sentenceId, string attributeName)
    {
        return DialogueParser.Instance.GetSentenceAttribute(EventId, sentenceId, attributeName);
    }

    /// <summary>
    /// ��鵱ǰ�Ƿ�������ʾ�ı�
    /// </summary>
    /// <returns>���������ʾ�ı��򷵻�true�����򷵻�false</returns>
    public bool IsDisplayingText()
    {
        return isDisplayingText;
    }

    /// <summary>
    /// ����UI����Ŀɼ���
    /// </summary>
    /// <param name="visible">�Ƿ�ɼ�</param>
    protected virtual void SetComponentsVisibility(bool visible)
    {
        // ���ø�������Ŀɼ���
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(visible);

        if (speakerNameText != null)
            speakerNameText.gameObject.SetActive(visible);

        if (characterImage != null)
            characterImage.gameObject.SetActive(visible);
    }

    /// <summary>
    /// ����֤�ﲢ��ʼ�������
    /// </summary>
    /// <param name="evidenceInfos">֤����Ϣ�б�����ID�����ơ�������λ��</param>
    protected void PlaceEvidenceAndListenForClicks(List<(string id, string name, string description, Vector2 position, Vector2 size)> evidenceInfos)
    {
        // ����֤��
        foreach (var info in evidenceInfos)
        {
            EvidenceManager.Instance.CreateEvidence(
                info.id, info.name, info.description,
                dialogueContainer.transform, info.position, info.size);
        }

        // ��ʼ����֤����
        StartListeningForEvidenceClicks();

        // ��ʱ������ͨ����ƽ��Ի�
        DialogueController.Instance.SetInputEnabled(false);
    }

    /// <summary>
    /// ����֤�ﲢֹͣ�������
    /// </summary>
    protected void CleanupEvidenceAndStopListening()
    {
        // ֹͣ����֤����
        StopListeningForEvidenceClicks();

        // ��������֤��
        EvidenceManager.Instance.DestroyAllEvidence();

        // ����������ͨ����ƽ��Ի�
        DialogueController.Instance.SetInputEnabled(true);
    }

    /// <summary>
    /// �ȴ�֤�ﱻ�ռ����ƽ��Ի�
    /// </summary>
    /// <param name="evidenceId">��Ҫ�ռ���֤��ID</param>
    /// <param name="name">֤������</param>
    /// <param name="description">֤������</param>
    /// <param name="position">����λ��</param>
    /// <param name="size">֤���С</param>
    protected virtual IEnumerator WaitForEvidenceCollection(string evidenceId, string name, string description,
                                                           Vector2 position, Vector2 size)
    {
        // ע��֤���ռ��¼�
        EvidenceManager.Instance.OnEvidenceCollected += OnEvidenceCollected;

        // ����֤��
        activeEvidenceButton = EvidenceManager.Instance.CreateEvidence(
            evidenceId, name, description,
            dialogueContainer.transform, position, size);

        // ������Ҫ�ȴ��ռ���֤��ID
        waitingForEvidenceId = evidenceId;

        // �ȴ�֤�ﱻ�ռ�
        isWaitingForEvidence = true;

        // �ȴ�ֱ��֤�ﱻ�ռ�
        while (isWaitingForEvidence)
        {
            yield return null;
        }

        // ȡ��ע���¼�
        EvidenceManager.Instance.OnEvidenceCollected -= OnEvidenceCollected;

        // ����֤�ﰴť����ѡ������Ϸ���������
        if (activeEvidenceButton != null)
        {
            EvidenceManager.Instance.DestroyEvidence(evidenceId);
            activeEvidenceButton = null;
        }
    }

    // ֤���ռ��ص�
    private void OnEvidenceCollected(string collectedEvidenceId)
    {
        // ����Ƿ����������ڵȴ���֤��
        if (isWaitingForEvidence && collectedEvidenceId == waitingForEvidenceId)
        {
            // ֹͣ�ȴ�
            isWaitingForEvidence = false;
        }
    }

    // �򻯰淽����ʹ��Ĭ�ϴ�С
    protected virtual IEnumerator WaitForEvidenceCollection(string evidenceId, string name, string description, Vector2 position)
    {
        return WaitForEvidenceCollection(evidenceId, name, description, position, new Vector2(100, 100));
    }
}