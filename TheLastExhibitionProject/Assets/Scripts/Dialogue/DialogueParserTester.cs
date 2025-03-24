using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// DialogueParser 测试脚本，用于测试对话管理器的功能
/// </summary>
public class DialogueParserTester : MonoBehaviour
{
    // 要测试的对话事件ID
    [SerializeField]
    private string testEventId = "test_dialogue";

    // 要测试的句子ID
    [SerializeField]
    private string testSentenceId = "sentence_001";

    // 是否在启动时自动测试
    [SerializeField]
    private bool testOnStart = true;

    // 批量测试的事件ID数组
    [SerializeField]
    private string[] batchTestEventIds;

    // Start is called before the first frame update
    async void Start()
    {
        if (testOnStart)
        {
            await TestDialogueParser();
        }
    }

    /// <summary>
    /// 测试按钮调用方法
    /// </summary>
    public async void OnTestButtonClicked()
    {
        await TestDialogueParser();
    }

    /// <summary>
    /// 批量测试按钮调用方法
    /// </summary>
    public async void OnBatchTestButtonClicked()
    {
        await TestBatchLoading();
    }

    /// <summary>
    /// 测试对话解析器的主要方法
    /// </summary>
    private async Task TestDialogueParser()
    {
        Debug.Log("===== 开始测试 DialogueParser =====");

        // 检查实例是否存在
        if (DialogueParser.Instance == null)
        {
            Debug.LogError("错误：DialogueParser 实例不存在，请确保场景中有 DialogueParser 组件");
            return;
        }

        Debug.Log("1. 测试加载对话事件");
        bool loadSuccess = await DialogueParser.Instance.LoadDialogueEventAsync(testEventId);
        Debug.Log($"加载事件 {testEventId} " + (loadSuccess ? "成功" : "失败"));

        if (!loadSuccess)
        {
            Debug.LogError($"测试停止：无法加载对话事件 {testEventId}");
            return;
        }

        Debug.Log("2. 测试检查事件是否已加载");
        bool isLoaded = DialogueParser.Instance.IsEventLoaded(testEventId);
        Debug.Log($"事件 {testEventId} 已加载: {isLoaded}");

        Debug.Log("3. 测试获取所有句子");
        var allSentences = DialogueParser.Instance.GetDialoguesByEventId(testEventId);
        Debug.Log($"事件 {testEventId} 包含 {(allSentences != null ? allSentences.Count : 0)} 个句子");

        if (allSentences != null && allSentences.Count > 0)
        {
            Debug.Log("句子列表:");
            foreach (var sentenceId in allSentences.Keys)
            {
                Debug.Log($"  - {sentenceId}");
            }
        }

        Debug.Log("4. 测试获取特定句子的属性");
        var attributes = DialogueParser.Instance.GetSentenceAttributes(testEventId, testSentenceId);

        if (attributes != null)
        {
            Debug.Log($"句子 {testSentenceId} 的属性:");
            foreach (var attr in attributes)
            {
                Debug.Log($"  - {attr.Key}: {attr.Value}");
            }

            Debug.Log("5. 测试获取特定属性");
            string speakerName = DialogueParser.Instance.GetSentenceAttribute(testEventId, testSentenceId, "speaker");
            string text = DialogueParser.Instance.GetSentenceAttribute(testEventId, testSentenceId, "text");

            Debug.Log($"说话者: {speakerName}");
            Debug.Log($"对白内容: {text}");
        }
        else
        {
            Debug.LogWarning($"句子 {testSentenceId} 不存在于事件 {testEventId} 中");
        }

        Debug.Log("===== DialogueParser 测试完成 =====");
    }

    /// <summary>
    /// 测试批量加载功能
    /// </summary>
    private async Task TestBatchLoading()
    {
        if (batchTestEventIds == null || batchTestEventIds.Length == 0)
        {
            Debug.LogWarning("批量测试事件ID数组为空");
            return;
        }

        Debug.Log("===== 开始批量加载测试 =====");
        Debug.Log($"准备加载 {batchTestEventIds.Length} 个事件");

        await DialogueParser.Instance.LoadMultipleEventsAsync(batchTestEventIds);

        Debug.Log("检查加载结果:");
        foreach (string eventId in batchTestEventIds)
        {
            bool isLoaded = DialogueParser.Instance.IsEventLoaded(eventId);
            Debug.Log($"  - 事件 {eventId} 已加载: {isLoaded}");

            if (isLoaded)
            {
                var sentences = DialogueParser.Instance.GetDialoguesByEventId(eventId);
                Debug.Log($"    包含 {sentences.Count} 个句子");
            }
        }

        Debug.Log("===== 批量加载测试完成 =====");
    }

    /// <summary>
    /// 清除所有对话数据
    /// </summary>
    public void OnClearDataButtonClicked()
    {
        DialogueParser.Instance.ClearDialogueData();
        Debug.Log("已清除所有对话数据");
    }
}