using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;

/// <summary>
/// 纯数据管理的对话管理器，专注于异步加载和数据存取
/// </summary>
public class DialogueParser : MonoBehaviour
{
    // 单例实例
    public static DialogueParser Instance { get; private set; }

    // 当前加载的对话事件ID
    private string currentEventId;

    // 加载状态
    private bool isLoading = false;

    // 已加载的事件ID集合
    private HashSet<string> loadedEventIds = new HashSet<string>();

    // 存储已加载对话数据的字典
    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> dialogueDatabase =
        new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

    // JSON文件的根路径
    [SerializeField]
    private string dialogueJsonFolderPath = "Assets/Resources/Dialogues/";

    private void Awake()
    {
        // 单例设置
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
    /// 异步加载对话事件
    /// </summary>
    /// <param name="eventId">要加载的事件ID</param>
    /// <returns>是否成功加载的Task</returns>
    public async Task<bool> LoadDialogueEventAsync(string eventId)
    {
        // 检查是否已经加载
        if (IsEventLoaded(eventId))
        {
            return true;
        }

        // 检查是否正在加载
        if (isLoading)
        {
            Debug.LogWarning($"已有对话事件正在加载中，请等待完成: {currentEventId}");
            return false;
        }

        isLoading = true;
        currentEventId = eventId;

        // 实际的异步加载方法
        bool success = await LoadDialogueEventTaskAsync(eventId);

        if (success)
        {
            loadedEventIds.Add(eventId);
            Debug.Log($"成功加载对话事件: {eventId}");
        }
        else
        {
            Debug.LogError($"无法加载对话事件: {eventId}");
        }

        isLoading = false;
        return success;
    }

    /// <summary>
    /// 实际执行对话事件加载的异步方法，从JSON文件加载
    /// </summary>
    /// <param name="eventId">要加载的事件ID</param>
    /// <returns>是否成功加载的Task</returns>
    private async Task<bool> LoadDialogueEventTaskAsync(string eventId)
    {
        try
        {
            // 构建JSON文件的完整路径
            string jsonFilePath = Path.Combine(dialogueJsonFolderPath, $"{eventId}.json");

            // 检查文件是否存在
            if (!File.Exists(jsonFilePath))
            {
                // 尝试从Resources加载
                TextAsset textAsset = Resources.Load<TextAsset>($"Dialogues/{eventId}");
                if (textAsset == null)
                {
                    Debug.LogError($"对话JSON文件不存在: {jsonFilePath}，且Resources中也没有找到");
                    return false;
                }

                await ParseDialogueJson(eventId, textAsset.text);
                return dialogueDatabase.ContainsKey(eventId);
            }

            // 使用Task.Run在后台线程中执行IO操作
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
            Debug.LogError($"加载对话事件出错: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 解析对话JSON数据
    /// </summary>
    private async Task ParseDialogueJson(string eventId, string jsonText)
    {
        await Task.Run(() => {
            try
            {
                // 使用Unity的JsonUtility解析JSON
                DialogueEventData dialogueData = JsonUtility.FromJson<DialogueEventData>(jsonText);

                if (dialogueData != null && dialogueData.sentenceList != null && dialogueData.sentenceList.Length > 0)
                {
                    var sentences = new Dictionary<string, Dictionary<string, string>>();

                    // 将数组形式转换为字典形式
                    foreach (var sentence in dialogueData.sentenceList)
                    {
                        if (string.IsNullOrEmpty(sentence.id))
                        {
                            Debug.LogWarning("发现句子ID为空");
                            continue;
                        }

                        var attributes = new Dictionary<string, string>();

                        // 将句子的属性添加到属性字典中
                        attributes.Add("speaker", sentence.speaker);
                        attributes.Add("text", sentence.text);
                        attributes.Add("sprite", sentence.sprite);

                        // 添加其他自定义属性
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
                    Debug.LogError($"JSON数据格式不正确或为空");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JSON解析错误: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// 获取特定事件的所有句子
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <returns>句子ID到属性字典的映射，如果未加载则返回null</returns>
    public Dictionary<string, Dictionary<string, string>> GetDialoguesByEventId(string eventId)
    {
        // 检查是否已加载
        if (!IsEventLoaded(eventId))
        {
            Debug.LogWarning($"事件 {eventId} 尚未加载，无法获取句子");
            return null;
        }

        return dialogueDatabase[eventId];
    }

    /// <summary>
    /// 获取特定句子的所有属性
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <param name="sentenceId">句子ID</param>
    /// <returns>属性字典，如果未找到则返回null</returns>
    public Dictionary<string, string> GetSentenceAttributes(string eventId, string sentenceId)
    {
        // 检查是否已加载
        if (!IsEventLoaded(eventId))
        {
            Debug.LogWarning($"事件 {eventId} 尚未加载，无法获取句子属性");
            return null;
        }

        var dialogues = dialogueDatabase[eventId];
        if (!dialogues.ContainsKey(sentenceId))
        {
            Debug.LogWarning($"在事件 {eventId} 中未找到句子 {sentenceId}");
            return null;
        }

        return dialogues[sentenceId];
    }

    /// <summary>
    /// 获取特定句子的特定属性
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <param name="sentenceId">句子ID</param>
    /// <param name="attributeName">属性名称</param>
    /// <returns>属性值，如果未找到则返回null</returns>
    public string GetSentenceAttribute(string eventId, string sentenceId, string attributeName)
    {
        // 获取句子的所有属性
        var attributes = GetSentenceAttributes(eventId, sentenceId);
        if (attributes == null)
        {
            return null;
        }

        // 检查属性是否存在
        if (!attributes.ContainsKey(attributeName))
        {
            Debug.LogWarning($"在事件 {eventId} 的句子 {sentenceId} 中未找到属性 {attributeName}");
            return null;
        }

        return attributes[attributeName];
    }

    /// <summary>
    /// 检查事件是否已加载
    /// </summary>
    /// <param name="eventId">要检查的事件ID</param>
    /// <returns>如果已加载则返回true，否则返回false</returns>
    public bool IsEventLoaded(string eventId)
    {
        return loadedEventIds.Contains(eventId) && dialogueDatabase.ContainsKey(eventId);
    }

    /// <summary>
    /// 检查事件是否正在加载
    /// </summary>
    /// <param name="eventId">要检查的事件ID</param>
    /// <returns>如果正在加载则返回true，否则返回false</returns>
    public bool IsEventLoading(string eventId)
    {
        return isLoading && currentEventId == eventId;
    }

    /// <summary>
    /// 清除所有已加载的对话数据
    /// </summary>
    public void ClearDialogueData()
    {
        loadedEventIds.Clear();
        dialogueDatabase.Clear();
        isLoading = false;
        Debug.Log("已清除所有对话数据");
    }

    /// <summary>
    /// 批量异步加载多个对话事件
    /// </summary>
    /// <param name="eventIds">要加载的事件ID数组</param>
    /// <returns>加载完成的Task</returns>
    public async Task LoadMultipleEventsAsync(string[] eventIds)
    {
        if (eventIds == null || eventIds.Length == 0)
        {
            return;
        }

        // 逐个加载事件，确保不会同时加载多个事件
        foreach (string eventId in eventIds)
        {
            await LoadDialogueEventAsync(eventId);
        }
    }
}

/// <summary>
/// 对话事件数据的JSON结构
/// </summary>
[System.Serializable]
public class DialogueEventData
{
    public DialogueSentence[] sentenceList;
}

/// <summary>
/// 对话句子数据
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
/// 自定义属性
/// </summary>
[System.Serializable]
public class CustomAttribute
{
    public string key;
    public string value;
}