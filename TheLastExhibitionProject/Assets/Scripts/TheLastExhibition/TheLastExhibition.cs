using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��Ϸ���ؽű���ʹ�õ�һ��switch-case��ʽ�ṹ����������Ϸ����
/// ��ǰ�������Ի����̣���֤������ע��
/// </summary>
public class TheLastExhibition : MonoBehaviour
{
    /* ��֤�������� - ��ʱע�͵�
    [Header("��֤����������")]
    [SerializeField] private GameObject evidenceSceneManager; // ��һ��GameObject����

    // ��֤����������ã�ͨ��GetComponent��ȡ��
    private TemplateEvidenceScene TemplateScene;
    
    // ��ǰ�����֤����
    private EvidenceSceneBase currentScene;
    */

    [Header("UI���")]
    [SerializeField] private Button startButton;        // ��ʼ��ť
    [SerializeField] private GameObject panelToDisable;  // ��Ҫ���õ����
    [SerializeField] private BackgroundManager bgManager;

    // ����״̬ö��
    public enum GameFlowState
    {
        MainMenu,           // ���˵�
        FirstScene,         // ��һ���Ի�
        SecondScene,        // �ڶ����Ի�
        ThirdScene,         // �������Ի�
        /* ��֤�������� - ��ʱע�͵�
        LivingRoomInvestigation, // ������֤
        AfterLivingRoomDialogue  // ������֤��Ի�
        */
    }

    // ��ǰ��Ϸ״̬
    private GameFlowState currentState = GameFlowState.MainMenu;


    // �Ի�IDӳ��
    private string GetDialogueIdForState(GameFlowState state)
    {
        switch (state)
        {
            case GameFlowState.FirstScene:
                return "FirstScene";
            case GameFlowState.SecondScene:
                return "SecondScene";
            case GameFlowState.ThirdScene:
                return "ThirdScene";
            /* ��֤�������� - ��ʱע�͵�
            case GameFlowState.OpeningDialogue:
                return "FirstScene";
            case GameFlowState.AfterLivingRoomDialogue:
                return "AfterLivingRoom";
            */
            default:
                return "";
        }
    }

    private void Start()
    {
        // ȷ����Ҫ��ϵͳ�������
        InitializeRequiredSystems();

        bgManager = GetComponent<BackgroundManager>();
        /* ��֤�������� - ��ʱע�͵�
        // ��ȡ������֤�������
        InitializeEvidenceScenes();
        */

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

        bgManager.SwitchToBackground(0);
    }

    private void OnDestroy()
    {
        // ȡ���¼�����
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.OnDialogueEnd -= HandleDialogueEnd;
        }

        /* ��֤�������� - ��ʱע�͵�
        // �����֤�����¼�����
        if (currentScene != null)
        {
            currentScene.OnAllEvidenceCollected -= HandleInvestigationCompleted;
        }
        */
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
        SetState(GameFlowState.FirstScene);
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

            case GameFlowState.FirstScene:
            case GameFlowState.SecondScene:
            case GameFlowState.ThirdScene:
                // ������Ӧ�ĶԻ�
                StartDialogue(GetDialogueIdForState(currentState));
                break;

                /* ��֤�������� - ��ʱע�͵�
                case GameFlowState.LivingRoomInvestigation:
                    // ������֤
                    StartInvestigation(TemplateScene);
                    break;

                case GameFlowState.AfterLivingRoomDialogue:
                    // ������֤��ĶԻ�
                    StartDialogue(GetDialogueIdForState(currentState));
                    break;
                */
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
            case GameFlowState.FirstScene:
                // ��һ���Ի����������ڶ����Ի�
                SetState(GameFlowState.SecondScene);
                break;

            case GameFlowState.SecondScene:
                // �ڶ����Ի����������������Ի�
                SetState(GameFlowState.ThirdScene);
                break;

            case GameFlowState.ThirdScene:
                // �������Ի���������Ϸ���̽���
                Debug.Log("��Ϸ���̽���");
                // ������������ӽ�����Ϸ���߼�������ʾ�������桢�������˵���
                break;

                /* ��֤�������� - ��ʱע�͵�
                case GameFlowState.OpeningDialogue:
                    // �����Ի���������������֤
                    SetState(GameFlowState.LivingRoomInvestigation);
                    break;

                case GameFlowState.AfterLivingRoomDialogue:
                    // ������֤��Ի������󣬿��������������һ���߼�
                    Debug.Log("��Ϸ���̽���");
                    break;
                */
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

    /* ��֤�������� - ��ʱע�͵�
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
        TemplateScene = evidenceSceneManager.GetComponent<TemplateEvidenceScene>();

        // ����Ƿ�����������ɹ���ȡ
        if (TemplateScene == null)
            Debug.LogError("δ�ҵ�TemplateEvidenceScene���");
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
    */
}