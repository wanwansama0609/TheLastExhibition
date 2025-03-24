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

    // ��ǰ�¼�ID
    protected string currentEventId;

    // ��ǰ����ID
    protected string currentSentenceId;

    // ��ǰ�Ի����ݻ���
    protected Dictionary<string, Dictionary<string, string>> currentDialogueData;

    // ��ʾ״̬
    protected bool isDisplayingText = false;
    protected Coroutine typingCoroutine;

    // �Ի��ص�
    protected System.Action onDialogueComplete;
    protected System.Action<string> onSentenceDisplayed;

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
    /// �����Ƿ�ʹ�ô���Ч��
    /// </summary>
    /// <param name="use">true��ʾʹ�ô���Ч����false��ʾֱ����ʾȫ���ı�</param>
    public void SetUseTypingEffect(bool use)
    {
        this.useTypingEffect = use;
    }

    /// <summary>
    /// ��ʼ���Ի���ʾ��������ָ���¼�ID�ĶԻ�����
    /// </summary>
    /// <param name="eventId">Ҫ��ʾ�ĶԻ��¼�ID</param>
    public virtual async Task Initialize(string eventId)
    {
        currentEventId = eventId;

        // ȷ���Ի��¼��Ѽ���
        if (!DialogueParser.Instance.IsEventLoaded(eventId))
        {
            Debug.Log($"���ڼ��ضԻ��¼�: {eventId}");
            bool success = await DialogueParser.Instance.LoadDialogueEventAsync(eventId);
            if (!success)
            {
                Debug.LogError($"���ضԻ��¼�ʧ��: {eventId}");
                return;
            }
        }

        // ��ȡ�Ի�����
        currentDialogueData = DialogueParser.Instance.GetDialoguesByEventId(eventId);

        if (currentDialogueData == null || currentDialogueData.Count == 0)
        {
            Debug.LogError($"�Ի��¼�����Ϊ��: {eventId}");
            return;
        }

        Debug.Log($"�ѳɹ���ʼ���Ի��¼�: {eventId}���� {currentDialogueData.Count} ������");
    }

    /// <summary>
    /// ��ʼ��ʾ�Ի�
    /// �������ʵ�ִ˷���������Ի��Ŀ�ʼ����
    /// </summary>
    public abstract void StartDialogue();

    /// <summary>
    /// �ƽ��Ի�����һ��
    /// �������ʵ�ִ˷���������Ի������̿���
    /// </summary>
    public abstract void AdvanceDialogue();

    /// <summary>
    /// ��ʾָ��ID�ľ���
    /// </summary>
    /// <param name="sentenceId">Ҫ��ʾ�ľ���ID</param>
    protected virtual IEnumerator DisplaySentence(string sentenceId)
    {
        isDisplayingText = true;
        currentSentenceId = sentenceId;

        if (!currentDialogueData.ContainsKey(sentenceId))
        {
            Debug.LogError($"���¼� {currentEventId} ��δ�ҵ�����: {sentenceId}");
            isDisplayingText = false;
            yield break;
        }

        // ��ȡ��������
        Dictionary<string, string> attributes = currentDialogueData[sentenceId];

        // ��ʾ˵��������
        if (speakerNameText != null && attributes.ContainsKey("speaker"))
        {
            speakerNameText.text = attributes["speaker"];
        }

        // ��ʾ�ı�����
        if (dialogueText != null && attributes.ContainsKey("text"))
        {
            string fullText = attributes["text"];

            if (useTypingEffect)
            {
                // ʹ�ô��ֻ�Ч��
                dialogueText.text = "";

                foreach (char c in fullText)
                {
                    dialogueText.text += c;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }
            else
            {
                // ֱ����ʾȫ���ı�
                dialogueText.text = fullText;
            }
        }

        // ��ʾ��ɫͼ��
        if (characterImage != null && attributes.ContainsKey("sprite") && !string.IsNullOrEmpty(attributes["sprite"]))
        {
            Sprite sprite = Resources.Load<Sprite>(attributes["sprite"]);
            if (sprite != null)
            {
                characterImage.sprite = sprite;
                characterImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"�޷����ؽ�ɫͼ��: {attributes["sprite"]}");
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
        if (isDisplayingText && dialogueText != null && currentSentenceId != null)
        {
            // ֹͣ����Ч��Э��
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            // ֱ����ʾ�����ı�
            string fullText = DialogueParser.Instance.GetSentenceAttribute(currentEventId, currentSentenceId, "text");
            if (!string.IsNullOrEmpty(fullText))
            {
                dialogueText.text = fullText;
            }

            isDisplayingText = false;

            // ����������ʾ��ɻص�
            onSentenceDisplayed?.Invoke(currentSentenceId);
        }
    }

    /// <summary>
    /// �����Ի�����
    /// </summary>
    protected virtual void EndDialogue()
    {
        // ���ضԻ�UI
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(false);
        }

        // �����Ի���ɻص�
        onDialogueComplete?.Invoke();

        Debug.Log($"�Ի��¼� {currentEventId} �ѽ���");
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
        return DialogueParser.Instance.GetSentenceAttribute(currentEventId, sentenceId, attributeName);
    }

    /// <summary>
    /// ��鵱ǰ�Ƿ�������ʾ�ı�
    /// </summary>
    /// <returns>���������ʾ�ı��򷵻�true�����򷵻�false</returns>
    public bool IsDisplayingText()
    {
        return isDisplayingText;
    }
}