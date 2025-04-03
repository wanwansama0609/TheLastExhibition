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

    // ��ǰ�Ի�״̬
    protected int dialogueState = 0;

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
    /// ��ʾָ��ID�ľ���
    /// </summary>
    /// <param name="sentenceId">Ҫ��ʾ�ľ���ID</param>
    protected virtual IEnumerator DisplaySentence(string sentenceId)
    {
        isDisplayingText = true;

        if (!dialogueData.ContainsKey(sentenceId))
        {
            Debug.LogError($"���¼� {EventId} ��δ�ҵ�����: {sentenceId}");
            isDisplayingText = false;
            yield break;
        }

        // ��ȡ��������
        Dictionary<string, string> attributes = dialogueData[sentenceId];

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
        if (characterImage != null)
        {
            Sprite characterSprite = null;

            // ����Ƿ���sprite���Բ����Լ���
            if (attributes.ContainsKey("sprite") && !string.IsNullOrEmpty(attributes["sprite"]))
            {
                characterSprite = Resources.Load<Sprite>(attributes["sprite"]);
            }

            // ������ɫͼ�����ص�
            onCharacterSpriteChanged?.Invoke(characterSprite);

            // ������سɹ���ʾͼ��
            if (characterSprite != null)
            {
                characterImage.sprite = characterSprite;
                characterImage.gameObject.SetActive(true);
            }
            else if (attributes.ContainsKey("sprite") && !string.IsNullOrEmpty(attributes["sprite"]))
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
    protected virtual void CompleteCurrentSentence(string sentenceId)
    {
        if (isDisplayingText && dialogueText != null)
        {
            // ֹͣ����Ч��Э��
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            // ֱ����ʾ�����ı�
            string fullText = DialogueParser.Instance.GetSentenceAttribute(EventId, sentenceId, "text");
            if (!string.IsNullOrEmpty(fullText))
            {
                dialogueText.text = fullText;
            }

            isDisplayingText = false;

            // ����������ʾ��ɻص�
            onSentenceDisplayed?.Invoke(sentenceId);
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
}