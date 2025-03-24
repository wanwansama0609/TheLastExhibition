using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;

/// <summary>
/// �����ݹ���ĶԻ���������רע���첽���غ����ݴ�ȡ
/// </summary>
public class DialogueParser : MonoBehaviour
{
    // ����ʵ��
    public static DialogueParser Instance { get; private set; }

    // ��ǰ���صĶԻ��¼�ID
    private string currentEventId;

    // ����״̬
    private bool isLoading = false;

    // �Ѽ��ص��¼�ID����
    private HashSet<string> loadedEventIds = new HashSet<string>();

    // �洢�Ѽ��ضԻ����ݵ��ֵ�
    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> dialogueDatabase =
        new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

    // JSON�ļ��ĸ�·��
    [SerializeField]
    private string dialogueJsonFolderPath = "Assets/Resources/Dialogues/";

    private void Awake()
    {
        // ��������
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
    }

    /// <summary>
    /// �첽���ضԻ��¼�
    /// </summary>
    /// <param name="eventId">Ҫ���ص��¼�ID</param>
    /// <returns>�Ƿ�ɹ����ص�Task</returns>
    public async Task<bool> LoadDialogueEventAsync(string eventId)
    {
        // ����Ƿ��Ѿ�����
        if (IsEventLoaded(eventId))
        {
            return true;
        }

        // ����Ƿ����ڼ���
        if (isLoading)
        {
            Debug.LogWarning($"���жԻ��¼����ڼ����У���ȴ����: {currentEventId}");
            return false;
        }

        isLoading = true;
        currentEventId = eventId;

        // ʵ�ʵ��첽���ط���
        bool success = await LoadDialogueEventTaskAsync(eventId);

        if (success)
        {
            loadedEventIds.Add(eventId);
            Debug.Log($"�ɹ����ضԻ��¼�: {eventId}");
        }
        else
        {
            Debug.LogError($"�޷����ضԻ��¼�: {eventId}");
        }

        isLoading = false;
        return success;
    }

    /// <summary>
    /// ʵ��ִ�жԻ��¼����ص��첽��������JSON�ļ�����
    /// </summary>
    /// <param name="eventId">Ҫ���ص��¼�ID</param>
    /// <returns>�Ƿ�ɹ����ص�Task</returns>
    private async Task<bool> LoadDialogueEventTaskAsync(string eventId)
    {
        try
        {
            // ����JSON�ļ�������·��
            string jsonFilePath = Path.Combine(dialogueJsonFolderPath, $"{eventId}.json");

            // ����ļ��Ƿ����
            if (!File.Exists(jsonFilePath))
            {
                // ���Դ�Resources����
                TextAsset textAsset = Resources.Load<TextAsset>($"Dialogues/{eventId}");
                if (textAsset == null)
                {
                    Debug.LogError($"�Ի�JSON�ļ�������: {jsonFilePath}����Resources��Ҳû���ҵ�");
                    return false;
                }

                await ParseDialogueJson(eventId, textAsset.text);
                return dialogueDatabase.ContainsKey(eventId);
            }

            // ʹ��Task.Run�ں�̨�߳���ִ��IO����
            string jsonText = await Task.Run(() => File.ReadAllText(jsonFilePath));

            if (string.IsNullOrEmpty(jsonText))
            {
                return false;
            }

            await ParseDialogueJson(eventId, jsonText);
            return dialogueDatabase.ContainsKey(eventId);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���ضԻ��¼�����: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// �����Ի�JSON����
    /// </summary>
    private async Task ParseDialogueJson(string eventId, string jsonText)
    {
        await Task.Run(() => {
            try
            {
                // ʹ��Unity��JsonUtility����JSON
                DialogueEventData dialogueData = JsonUtility.FromJson<DialogueEventData>(jsonText);

                if (dialogueData != null && dialogueData.sentenceList != null && dialogueData.sentenceList.Length > 0)
                {
                    var sentences = new Dictionary<string, Dictionary<string, string>>();

                    // ��������ʽת��Ϊ�ֵ���ʽ
                    foreach (var sentence in dialogueData.sentenceList)
                    {
                        if (string.IsNullOrEmpty(sentence.id))
                        {
                            Debug.LogWarning("���־���IDΪ��");
                            continue;
                        }

                        var attributes = new Dictionary<string, string>();

                        // �����ӵ�������ӵ������ֵ���
                        attributes.Add("speaker", sentence.speaker);
                        attributes.Add("text", sentence.text);
                        attributes.Add("sprite", sentence.sprite);

                        // ��������Զ�������
                        if (sentence.customAttributes != null)
                        {
                            foreach (var attr in sentence.customAttributes)
                            {
                                if (!string.IsNullOrEmpty(attr.key))
                                {
                                    attributes[attr.key] = attr.value;
                                }
                            }
                        }

                        sentences.Add(sentence.id, attributes);
                    }

                    dialogueDatabase[eventId] = sentences;
                }
                else
                {
                    Debug.LogError($"JSON���ݸ�ʽ����ȷ��Ϊ��");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JSON��������: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// ��ȡ�ض��¼������о���
    /// </summary>
    /// <param name="eventId">�¼�ID</param>
    /// <returns>����ID�������ֵ��ӳ�䣬���δ�����򷵻�null</returns>
    public Dictionary<string, Dictionary<string, string>> GetDialoguesByEventId(string eventId)
    {
        // ����Ƿ��Ѽ���
        if (!IsEventLoaded(eventId))
        {
            Debug.LogWarning($"�¼� {eventId} ��δ���أ��޷���ȡ����");
            return null;
        }

        return dialogueDatabase[eventId];
    }

    /// <summary>
    /// ��ȡ�ض����ӵ���������
    /// </summary>
    /// <param name="eventId">�¼�ID</param>
    /// <param name="sentenceId">����ID</param>
    /// <returns>�����ֵ䣬���δ�ҵ��򷵻�null</returns>
    public Dictionary<string, string> GetSentenceAttributes(string eventId, string sentenceId)
    {
        // ����Ƿ��Ѽ���
        if (!IsEventLoaded(eventId))
        {
            Debug.LogWarning($"�¼� {eventId} ��δ���أ��޷���ȡ��������");
            return null;
        }

        var dialogues = dialogueDatabase[eventId];
        if (!dialogues.ContainsKey(sentenceId))
        {
            Debug.LogWarning($"���¼� {eventId} ��δ�ҵ����� {sentenceId}");
            return null;
        }

        return dialogues[sentenceId];
    }

    /// <summary>
    /// ��ȡ�ض����ӵ��ض�����
    /// </summary>
    /// <param name="eventId">�¼�ID</param>
    /// <param name="sentenceId">����ID</param>
    /// <param name="attributeName">��������</param>
    /// <returns>����ֵ�����δ�ҵ��򷵻�null</returns>
    public string GetSentenceAttribute(string eventId, string sentenceId, string attributeName)
    {
        // ��ȡ���ӵ���������
        var attributes = GetSentenceAttributes(eventId, sentenceId);
        if (attributes == null)
        {
            return null;
        }

        // ��������Ƿ����
        if (!attributes.ContainsKey(attributeName))
        {
            Debug.LogWarning($"���¼� {eventId} �ľ��� {sentenceId} ��δ�ҵ����� {attributeName}");
            return null;
        }

        return attributes[attributeName];
    }

    /// <summary>
    /// ����¼��Ƿ��Ѽ���
    /// </summary>
    /// <param name="eventId">Ҫ�����¼�ID</param>
    /// <returns>����Ѽ����򷵻�true�����򷵻�false</returns>
    public bool IsEventLoaded(string eventId)
    {
        return loadedEventIds.Contains(eventId) && dialogueDatabase.ContainsKey(eventId);
    }

    /// <summary>
    /// ����¼��Ƿ����ڼ���
    /// </summary>
    /// <param name="eventId">Ҫ�����¼�ID</param>
    /// <returns>������ڼ����򷵻�true�����򷵻�false</returns>
    public bool IsEventLoading(string eventId)
    {
        return isLoading && currentEventId == eventId;
    }

    /// <summary>
    /// ��������Ѽ��صĶԻ�����
    /// </summary>
    public void ClearDialogueData()
    {
        loadedEventIds.Clear();
        dialogueDatabase.Clear();
        isLoading = false;
        Debug.Log("��������жԻ�����");
    }

    /// <summary>
    /// �����첽���ض���Ի��¼�
    /// </summary>
    /// <param name="eventIds">Ҫ���ص��¼�ID����</param>
    /// <returns>������ɵ�Task</returns>
    public async Task LoadMultipleEventsAsync(string[] eventIds)
    {
        if (eventIds == null || eventIds.Length == 0)
        {
            return;
        }

        // ��������¼���ȷ������ͬʱ���ض���¼�
        foreach (string eventId in eventIds)
        {
            await LoadDialogueEventAsync(eventId);
        }
    }
}

/// <summary>
/// �Ի��¼����ݵ�JSON�ṹ
/// </summary>
[System.Serializable]
public class DialogueEventData
{
    public DialogueSentence[] sentenceList;
}

/// <summary>
/// �Ի���������
/// </summary>
[System.Serializable]
public class DialogueSentence
{
    public string id;
    public string speaker;
    public string text;
    public string sprite;
    public CustomAttribute[] customAttributes;
}

/// <summary>
/// �Զ�������
/// </summary>
[System.Serializable]
public class CustomAttribute
{
    public string key;
    public string value;
}