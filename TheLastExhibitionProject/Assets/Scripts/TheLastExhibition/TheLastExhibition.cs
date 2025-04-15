using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��Ϸ���ؽű���ʹ�õ�һ��switch-case��ʽ�ṹ����������Ϸ����
/// �����Ի�����֤����
/// </summary>
public class TheLastExhibition : MonoBehaviour
{
    [Header("��֤����������")]
    [SerializeField] private GameObject evidenceSceneManager; // ��һ��GameObject����

    // ��֤����������ã�ͨ��GetComponent��ȡ��
    private LivingRoomEvidenceScene livingRoomScene;

    [Header("UI���")]
    [SerializeField] private Button startButton;        // ��ʼ��ť
    [SerializeField] private GameObject panelToDisable;  // ��Ҫ���õ����

    // ��ǰ�����֤����
    private EvidenceSceneBase currentScene;

    // ����״̬ö�٣������˶Ի�����֤����״̬��
    public enum GameFlowState
    {
        MainMenu,                // ���˵�
        OpeningDialogue,         // �����Ի�
        LivingRoomInvestigation, // ������֤
        AfterLivingRoomDialogue  // ������֤��Ի�
    }

    // ��ǰ��Ϸ״̬
    private GameFlowState currentState = GameFlowState.MainMenu;

    // �Ի�IDӳ��
    private string GetDialogueIdForState(GameFlowState state)
    {
        switch (state)
        {
            case GameFlowState.OpeningDialogue:
                return "FirstScene";
            case GameFlowState.AfterLivingRoomDialogue:
                return "AfterLivingRoom";
            default:
                return "";
        }
    }

    private void Start()
    {
        // ȷ����Ҫ��ϵͳ�������
        InitializeRequiredSystems();

        // ��ȡ������֤�������
        InitializeEvidenceScenes();

        // Ϊ��ʼ��ť��ӵ���¼�
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogError("δ���ÿ�ʼ��ť������Inspector��ָ��StartButton");
        }

        // �����Ի������¼�
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.OnDialogueEnd += HandleDialogueEnd;
        }
    }

    /// <summary>
    /// ��ʼ��������֤�������
    /// </summary>
    private void InitializeEvidenceScenes()
    {
        if (evidenceSceneManager == null)
        {
            Debug.LogError("δ������֤����������������Inspector��ָ��EvidenceSceneManager");
            return;
        }

        // ��ȡ������֤�������
        livingRoomScene = evidenceSceneManager.GetComponent<LivingRoomEvidenceScene>();

        // ����Ƿ�����������ɹ���ȡ
        if (livingRoomScene == null)
            Debug.LogError("δ�ҵ�LivingRoomEvidenceScene���");
    }

    private void OnDestroy()
    {
        // ȡ���¼�����
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.OnDialogueEnd -= HandleDialogueEnd;
        }

        // �����֤�����¼�����
        if (currentScene != null)
        {
            currentScene.OnAllEvidenceCollected -= HandleInvestigationCompleted;
        }
    }

    /// <summary>
    /// ��ʼ����Ҫ��ϵͳ���
    /// </summary>
    private void InitializeRequiredSystems()
    {
        // ���LanguageSetting�Ƿ����
        if (LanguageSetting.Instance == null)
        {
            GameObject settingsObj = new GameObject("LanguageSetting");
            settingsObj.AddComponent<LanguageSetting>();
        }

        // ���DialogueParser�Ƿ����
        if (DialogueParser.Instance == null)
        {
            GameObject parserObj = new GameObject("DialogueParser");
            parserObj.AddComponent<DialogueParser>();
        }

        // ���DialogueController�Ƿ����
        if (DialogueController.Instance == null)
        {
            GameObject controllerObj = new GameObject("DialogueController");
            controllerObj.AddComponent<DialogueController>();
        }

        // ��鲢���ű�������
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic("MainTheme");  // ����Ϊ��������
        }
    }

    /// <summary>
    /// ��ʼ��ť����¼�������
    /// </summary>
    public void OnStartButtonClicked()
    {
        Debug.Log("��ʼ��ť�������������Ϸ����");

        // ����ָ����弰��������UIԪ��
        DisablePanelAndChildren();

        // ���ó�ʼ״̬����������
        SetState(GameFlowState.OpeningDialogue);
    }

    /// <summary>
    /// ����ָ����弰��������UIԪ��
    /// </summary>
    private void DisablePanelAndChildren()
    {
        if (panelToDisable != null)
        {
            // ������屾��
            panelToDisable.SetActive(false);
            Debug.Log($"�ѽ������: {panelToDisable.name}����������UIԪ��");
        }
        else
        {
            Debug.LogWarning("δ������Ҫ���õ���壬����Inspector��ָ��PanelToDisable");
        }
    }

    /// <summary>
    /// ������Ϸ״̬��������Ӧ������
    /// </summary>
    private void SetState(GameFlowState newState)
    {
        // ����״̬
        currentState = newState;
        Debug.Log($"��ǰ��Ϸ״̬: {currentState}");

        // ����ǰ״̬������
        ProcessGameFlow();
    }

    /// <summary>
    /// ��Ϸ���̵���Ҫswitch-case������
    /// </summary>
    private void ProcessGameFlow()
    {
        switch (currentState)
        {
            case GameFlowState.MainMenu:
                // ���˵�״̬���������⴦��
                break;

            case GameFlowState.OpeningDialogue:
                // �����Ի�
                StartDialogue(GetDialogueIdForState(currentState));
                break;

            case GameFlowState.LivingRoomInvestigation:
                // ������֤
                StartInvestigation(livingRoomScene);
                break;

            case GameFlowState.AfterLivingRoomDialogue:
                // ������֤��ĶԻ�
                StartDialogue(GetDialogueIdForState(currentState));
                break;
        }
    }

    /// <summary>
    /// ����Ի������¼�
    /// </summary>
    private void HandleDialogueEnd(string dialogueId)
    {
        Debug.Log($"�Ի��ѽ���: {dialogueId}");

        // ���ݵ�ǰ״̬������һ��״̬
        switch (currentState)
        {
            case GameFlowState.OpeningDialogue:
                // �����Ի���������������֤
                SetState(GameFlowState.LivingRoomInvestigation);
                break;

            case GameFlowState.AfterLivingRoomDialogue:
                // ������֤��Ի������󣬿��������������һ���߼�
                Debug.Log("��Ϸ���̽���");
                break;
        }
    }

    /// <summary>
    /// ������֤����¼�
    /// </summary>
    private void HandleInvestigationCompleted(string sceneId)
    {
        Debug.Log($"��֤���: {sceneId}");

        // ���ݵ�ǰ״̬������һ��״̬
        switch (currentState)
        {
            case GameFlowState.LivingRoomInvestigation:
                // ������֤��ɺ���������Ի�
                SetState(GameFlowState.AfterLivingRoomDialogue);
                break;
        }
    }

    /// <summary>
    /// �����Ի�
    /// </summary>
    private void StartDialogue(string dialogueId)
    {
        if (DialogueController.Instance != null && !string.IsNullOrEmpty(dialogueId))
        {
            Debug.Log($"�����Ի�: {dialogueId}");
            DialogueController.Instance.StartDialogue(dialogueId);
        }
        else
        {
            Debug.LogError("DialogueController�����ڻ�Ի�IDΪ�գ��޷������Ի�");
        }
    }

    /// <summary>
    /// ������֤����
    /// </summary>
    private void StartInvestigation(EvidenceSceneBase scene)
    {
        // �����ǰ�������¼�����
        if (currentScene != null)
        {
            currentScene.OnAllEvidenceCollected -= HandleInvestigationCompleted;
        }

        if (scene == null)
        {
            Debug.LogError($"��֤����Ϊ�գ��޷�����");
            return;
        }

        // ���õ�ǰ����
        currentScene = scene;

        // ������֤����¼�
        currentScene.OnAllEvidenceCollected += HandleInvestigationCompleted;

        // ��ʼ����֤����
        currentScene.InitializeScene();

        Debug.Log($"��ʼ��֤: {currentScene.GetSceneId()}");
    }

    /// <summary>
    /// ��ȡ��ǰ��Ϸ״̬
    /// </summary>
    public GameFlowState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// �ֶ�������Ϸ״̬�����ڵ��ԣ�
    /// </summary>
    public void JumpToState(GameFlowState state)
    {
        SetState(state);
    }
}