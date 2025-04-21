using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// �Ի����������������ʹ����Ի���ʾ��ʵ��
/// </summary>
public class DialogueController : MonoBehaviour
{
    // ����ʵ��
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

    // ��ӵ� DialogueController ����
    private bool inputEnabled = true;

    // ��ǰ��Ծ�ĶԻ���ʾ��
    private DialogueDisplayBase activeDisplayer;
    private EvidenceManager evidenceManager;

    // �Ի���ʾ�����棬���ڱ����ظ�����
    private Dictionary<string, DialogueDisplayBase> displayerCache =
        new Dictionary<string, DialogueDisplayBase>();


    // ������ȴ��ʱ��
    private float lastInputTime = 0f;

    // �Ի��Ƿ��Ծ
    private bool isDialogueActive = false;

    // ��ǰ�Ի��¼�ID
    private string currentDialogueEventId;

    // �Ի������¼������ضԻ����¼�ID
    public System.Action<string> OnDialogueEnd;

    private void Awake()
    {
        // ���õ���
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

        // ��ʼʱ���ضԻ����
        dialogueContainer.SetActive(false);

        // ��ȡ֤�����������
        evidenceManager = EvidenceManager.Instance;
        if (evidenceManager == null)
        {
            Debug.LogError("δ�ҵ�EvidenceManagerʵ��");
        }
    }
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }
    private void Update()
    {
        // ֻ���ڶԻ���Ծʱ�ż������
        if (isDialogueActive && activeDisplayer != null && inputEnabled)
        {
            // ����Ƿ񳬹�������ȴʱ��
            if (Time.time - lastInputTime >= inputCooldown)
            {
                // �����������ո������
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    AdvanceDialogue();
                    lastInputTime = Time.time; // �����������ʱ��
                }
            }
        }
    }

    /// <summary>
    /// �����Ի�
    /// </summary>
    /// <param name="eventId">�Ի��¼�ID</param>
    public async void StartDialogue(string eventId)
    {


        // ���浱ǰ�Ի��¼�ID
        currentDialogueEventId = eventId;

        // ���Դӻ����ȡ��ʾ��������������򴴽��µ�
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
            // ��ʾ�Ի�UI
            dialogueContainer.SetActive(true);
            isDialogueActive = true;

            // ��ʼ���Ϳ�ʼ�Ի�
            await activeDisplayer.Initialize();
            activeDisplayer.StartDialogue();
        }
        else
        {
            Debug.LogError($"�޷������Ի���ʾ�����¼�ID: {eventId}");
        }
    }

    /// <summary>
    /// �����¼�ID�����ʵ��ĶԻ���ʾ��
    /// </summary>
    /// <param name="eventId">�Ի��¼�ID</param>
    /// <returns>�Ի���ʾ��ʵ��</returns>
    private DialogueDisplayBase CreateDisplayer(string eventId)
    {
        DialogueDisplayBase newDisplayer = null;

        // �����¼�ID������Ӧ�ĶԻ�������
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



            // ��������Ի���������case
            // case "EventA":
            //     newDisplayer = new EventADialogueManager(...);
            //     break;

            default:
                Debug.LogError($"δ�ҵ���Ӧ�ĶԻ�������: {eventId}");
                break;
        }

        // ���öԻ���ʾ����
        if (newDisplayer != null)
        {
            ConfigureDisplayer(newDisplayer);
        }

        return newDisplayer;
    }

    /// <summary>
    /// ���öԻ���ʾ���Ĳ�������
    /// </summary>
    /// <param name="displayer">Ҫ���õĶԻ���ʾ��</param>
    private void ConfigureDisplayer(DialogueDisplayBase displayer)
    {
        if (displayer == null)
            return;

        // ���ݴ���Ч������
        displayer.SetTypingSpeed(typingSpeed);
        displayer.SetUseTypingEffect(useTypingEffect);

        // ���öԻ���ɻص�
        displayer.SetDialogueCompleteCallback(OnDialogueComplete);

        // ���ý�ɫͼ��ص�
        displayer.onCharacterSpriteChanged = OnCharacterSpriteChanged;

        // ���������Ļص�
        displayer.onDialogueTextChanged = OnDialogueTextChanged;
        displayer.onSpeakerNameChanged = OnSpeakerNameChanged;
    }

    /// <summary>
    /// �����ɫͼ�����¼�
    /// </summary>
    /// <param name="sprite">���صĽ�ɫ���飬����Ϊnull</param>
    private void OnCharacterSpriteChanged(Sprite sprite)
    {
        if (characterImage == null)
            return;

        if (sprite == null)
        {
            // ���spriteΪnull��ֱ��ͣ��ͼ�����
            characterImage.gameObject.SetActive(false);
        }
        else
        {
            // ���sprite��Ч������ͼ�����
            characterImage.gameObject.SetActive(true);
            characterImage.sprite = sprite;
        }
    }

    /// <summary>
    /// ����Ի��ı�����¼�
    /// </summary>
    /// <param name="text">�Ի��ı�������Ϊ��</param>
    private void OnDialogueTextChanged(string text)
    {
        if (dialogueText == null || dialogueContainer == null)
            return;

        if (string.IsNullOrEmpty(text))
        {
            dialogueContainer.SetActive(false); // �ĳ����������Ի����
        }
        else
        {
            dialogueContainer.SetActive(true);
            dialogueText.gameObject.SetActive(true);
            dialogueText.text = text;
        }
    }

    /// <summary>
    /// ����˵�������Ʊ���¼�
    /// </summary>
    /// <param name="speakerName">˵�������ƣ�����Ϊ��</param>
    private void OnSpeakerNameChanged(string speakerName)
    {
        if (speakerNameText == null)
            return;

        if (string.IsNullOrEmpty(speakerName))
        {
            // �������Ϊ�գ�ͣ���������
            speakerNameText.gameObject.SetActive(false);
        }
        else
        {
            // ���������Ч�������������
            speakerNameText.gameObject.SetActive(true);
            speakerNameText.text = speakerName;
        }
    }

    /// <summary>
    /// �Ի���ɻص�
    /// </summary>
    private void OnDialogueComplete()
    {
        isDialogueActive = false;
        dialogueContainer.SetActive(false);

        // ��ȡ��ǰ��Ծ�Ի������¼�ID
        string eventId = currentDialogueEventId;

        // �ӻ������Ƴ��Ի���ʾ��
        if (!string.IsNullOrEmpty(eventId) && displayerCache.ContainsKey(eventId))
        {
            displayerCache.Remove(eventId);
        }

        // �ͷ�DialogueParser�м��ص�JSON����
        if (!string.IsNullOrEmpty(eventId))
        {
            DialogueParser.Instance.UnloadDialogueEvent(eventId);
        }

        // �����Ի������¼�
        OnDialogueEnd?.Invoke(eventId);

        // �����ǰ��Ծ�ĶԻ���ʾ��
        activeDisplayer = null;
        currentDialogueEventId = null;

        Debug.Log($"�Ի�����ɣ����ͷ���Դ: {eventId}");

        // ����ǰ�����е�����֤��
        if (evidenceManager != null)
        {
            evidenceManager.DestroyAllEvidence();
        }
    }

    /// <summary>
    /// �ƽ��Ի�
    /// </summary>
    public void AdvanceDialogue()
    {
        if (activeDisplayer != null)
        {
            activeDisplayer.AdvanceDialogue();
        }
    }

    /// <summary>
    /// ������ǰ�Ի�
    /// </summary>
    public void EndCurrentDialogue()
    {

        // ���ضԻ�UI
        dialogueContainer.SetActive(false);
        isDialogueActive = false;

        // �����Ի������¼�
        if (!string.IsNullOrEmpty(currentDialogueEventId))
        {
            OnDialogueEnd?.Invoke(currentDialogueEventId);
        }

        // ����
        activeDisplayer = null;
        currentDialogueEventId = null;
    }

    /// <summary>
    /// ����Ի�����
    /// </summary>
    public void ClearCache()
    {
        displayerCache.Clear();
    }

    /// <summary>
    /// ���öԻ���ɻص�
    /// </summary>
    /// <param name="callback">�Ի����ʱҪ���õĻص�����</param>
    public void SetDialogueCompleteCallback(System.Action callback)
    {
        if (activeDisplayer != null)
        {
            // ʹ����ʽ�ص� - �ȵ����ڲ���OnDialogueComplete���ٵ����Զ���ص�
            activeDisplayer.SetDialogueCompleteCallback(() => {
                OnDialogueComplete();
                callback?.Invoke();
            });
        }
    }


    /// <summary>
    /// ����֤���ռ��¼�
    /// </summary>
    private void OnEvidenceCollected(string evidenceId)
    {
        // �����ǰ�л�Ծ�ĶԻ���ʾ���������ڵȴ�֤���ռ�
        if (activeDisplayer != null && activeDisplayer.IsWaitingForEvidence())
        {
            // �п�����Ҫִ��һЩ������߼�
            // ���粥����Ч����ʾ��ʾ��
        }
    }
}