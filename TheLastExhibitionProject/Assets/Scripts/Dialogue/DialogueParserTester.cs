using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// DialogueParser ���Խű������ڲ��ԶԻ��������Ĺ���
/// </summary>
public class DialogueParserTester : MonoBehaviour
{
    // Ҫ���ԵĶԻ��¼�ID
    [SerializeField]
    private string testEventId = "test_dialogue";

    // Ҫ���Եľ���ID
    [SerializeField]
    private string testSentenceId = "sentence_001";

    // �Ƿ�������ʱ�Զ�����
    [SerializeField]
    private bool testOnStart = true;

    // �������Ե��¼�ID����
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
    /// ���԰�ť���÷���
    /// </summary>
    public async void OnTestButtonClicked()
    {
        await TestDialogueParser();
    }

    /// <summary>
    /// �������԰�ť���÷���
    /// </summary>
    public async void OnBatchTestButtonClicked()
    {
        await TestBatchLoading();
    }

    /// <summary>
    /// ���ԶԻ�����������Ҫ����
    /// </summary>
    private async Task TestDialogueParser()
    {
        Debug.Log("===== ��ʼ���� DialogueParser =====");

        // ���ʵ���Ƿ����
        if (DialogueParser.Instance == null)
        {
            Debug.LogError("����DialogueParser ʵ�������ڣ���ȷ���������� DialogueParser ���");
            return;
        }

        Debug.Log("1. ���Լ��ضԻ��¼�");
        bool loadSuccess = await DialogueParser.Instance.LoadDialogueEventAsync(testEventId);
        Debug.Log($"�����¼� {testEventId} " + (loadSuccess ? "�ɹ�" : "ʧ��"));

        if (!loadSuccess)
        {
            Debug.LogError($"����ֹͣ���޷����ضԻ��¼� {testEventId}");
            return;
        }

        Debug.Log("2. ���Լ���¼��Ƿ��Ѽ���");
        bool isLoaded = DialogueParser.Instance.IsEventLoaded(testEventId);
        Debug.Log($"�¼� {testEventId} �Ѽ���: {isLoaded}");

        Debug.Log("3. ���Ի�ȡ���о���");
        var allSentences = DialogueParser.Instance.GetDialoguesByEventId(testEventId);
        Debug.Log($"�¼� {testEventId} ���� {(allSentences != null ? allSentences.Count : 0)} ������");

        if (allSentences != null && allSentences.Count > 0)
        {
            Debug.Log("�����б�:");
            foreach (var sentenceId in allSentences.Keys)
            {
                Debug.Log($"  - {sentenceId}");
            }
        }

        Debug.Log("4. ���Ի�ȡ�ض����ӵ�����");
        var attributes = DialogueParser.Instance.GetSentenceAttributes(testEventId, testSentenceId);

        if (attributes != null)
        {
            Debug.Log($"���� {testSentenceId} ������:");
            foreach (var attr in attributes)
            {
                Debug.Log($"  - {attr.Key}: {attr.Value}");
            }

            Debug.Log("5. ���Ի�ȡ�ض�����");
            string speakerName = DialogueParser.Instance.GetSentenceAttribute(testEventId, testSentenceId, "speaker");
            string text = DialogueParser.Instance.GetSentenceAttribute(testEventId, testSentenceId, "text");

            Debug.Log($"˵����: {speakerName}");
            Debug.Log($"�԰�����: {text}");
        }
        else
        {
            Debug.LogWarning($"���� {testSentenceId} ���������¼� {testEventId} ��");
        }

        Debug.Log("===== DialogueParser ������� =====");
    }

    /// <summary>
    /// �����������ع���
    /// </summary>
    private async Task TestBatchLoading()
    {
        if (batchTestEventIds == null || batchTestEventIds.Length == 0)
        {
            Debug.LogWarning("���������¼�ID����Ϊ��");
            return;
        }

        Debug.Log("===== ��ʼ�������ز��� =====");
        Debug.Log($"׼������ {batchTestEventIds.Length} ���¼�");

        await DialogueParser.Instance.LoadMultipleEventsAsync(batchTestEventIds);

        Debug.Log("�����ؽ��:");
        foreach (string eventId in batchTestEventIds)
        {
            bool isLoaded = DialogueParser.Instance.IsEventLoaded(eventId);
            Debug.Log($"  - �¼� {eventId} �Ѽ���: {isLoaded}");

            if (isLoaded)
            {
                var sentences = DialogueParser.Instance.GetDialoguesByEventId(eventId);
                Debug.Log($"    ���� {sentences.Count} ������");
            }
        }

        Debug.Log("===== �������ز������ =====");
    }

    /// <summary>
    /// ������жԻ�����
    /// </summary>
    public void OnClearDataButtonClicked()
    {
        DialogueParser.Instance.ClearDialogueData();
        Debug.Log("��������жԻ�����");
    }
}