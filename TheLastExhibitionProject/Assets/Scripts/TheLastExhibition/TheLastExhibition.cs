using UnityEngine;

/// <summary>
/// ������Ϸ���ؽű������ڲ��ԶԻ�ϵͳ
/// </summary>
public class TheLastExhibition : MonoBehaviour
{
    [Header("�Ի��¼�����")]
    [SerializeField] private string testDialogueEventId = "FirstScene";
    [SerializeField] private KeyCode dialogueTestKey = KeyCode.BackQuote;  // ~ ��


    private void Start()
    {
        // ȷ����Ҫ��ϵͳ�������
        InitializeRequiredSystems();
    }

    private void Update()
    {
        // ��ⰴ������
        if (Input.GetKeyDown(dialogueTestKey))
        {
            Debug.Log($"�������ԶԻ�: {testDialogueEventId}");
            StartTestDialogue();
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
            DialogueController controller = controllerObj.AddComponent<DialogueController>();

        }

        if (AudioManager.Instance != null)
        {
            // ͨ�����Ʋ�������
            AudioManager.Instance.PlayMusic("TestMusic");
        }
    }


    /// <summary>
    /// �������ԶԻ�
    /// </summary>
    private void StartTestDialogue()
    {
        if (DialogueController.Instance != null)
        {
            Debug.Log($"�������ԶԻ�: {testDialogueEventId}");
            DialogueController.Instance.StartDialogue(testDialogueEventId);
        }
        else
        {
            Debug.LogError("DialogueController�����ڣ��޷������Ի�");
        }
    }
}