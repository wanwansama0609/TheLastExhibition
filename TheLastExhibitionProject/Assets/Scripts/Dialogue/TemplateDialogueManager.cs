using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// ���ԶԻ���������ʵ�ְ�˳����ʾ�Ի�������
/// </summary>
public class TemplateDialogueManager : DialogueDisplayBase
{
    // �Ի�����ID�б���˳��
    private List<string> sentenceIds = new List<string>();

    // ��ǰ��������
    private int currentIndex = 0;

    /// <summary>
    /// ���캯��
    /// </summary>
    public TemplateDialogueManager(
        MonoBehaviour runner,
        TextMeshProUGUI dialogueText,
        TextMeshProUGUI speakerNameText,
        UnityEngine.UI.Image characterImage,
        GameObject dialogueContainer)
        : base(runner, dialogueText, speakerNameText, characterImage, dialogueContainer)
    {
    }

    /// <summary>
    /// ��д��ʼ������������׼������˳���б�
    /// </summary>
    public override async Task Initialize(string eventId)
    {
        // �ȵ��û����ʼ������
        await base.Initialize(eventId);

        // ׼������ID�б�
        PrepareDialogueSequence();
    }

    /// <summary>
    /// ׼���Ի�˳��
    /// ���Ը����Զ������ȷ�����ӵ���ʾ˳��
    /// </summary>
    private void PrepareDialogueSequence()
    {
        sentenceIds.Clear();

        // ��ȡ���о���ID
        foreach (var sentenceId in currentDialogueData.Keys)
        {
            sentenceIds.Add(sentenceId);
        }

        // ���ݾ���ID���򣨼���ID��ʽΪ"sentence_1", "sentence_2"�ȣ�
        sentenceIds.Sort((a, b) => {
            // ��ȡID�е����ֲ���
            if (int.TryParse(a.Split('_')[1], out int numA) &&
                int.TryParse(b.Split('_')[1], out int numB))
            {
                return numA.CompareTo(numB);
            }
            return string.Compare(a, b);
        });

        Debug.Log($"��׼�� {sentenceIds.Count} �����ӵ���ʾ˳��");
    }

    /// <summary>
    /// ��ʼ�Ի����̣���ʾ��һ������
    /// </summary>
    public override void StartDialogue()
    {
        if (sentenceIds.Count == 0)
        {
            Debug.LogWarning("û�о��ӿ�����ʾ");
            EndDialogue();
            return;
        }

        // ��ʾ�Ի�UI
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(true);
        }

        // ������������ʾ��һ������
        currentIndex = 0;
        typingCoroutine = StartCoroutine(DisplaySentence(sentenceIds[currentIndex]));

        Debug.Log($"��ʼ��ʾ�Ի��¼�: {currentEventId}");
    }

    /// <summary>
    /// �ƽ��Ի�����һ��
    /// </summary>
    public override void AdvanceDialogue()
    {
        // ���������ʾ�ı�����������ɵ�ǰ������ʾ
        if (IsDisplayingText())
        {
            CompleteCurrentSentence();
            return;
        }

        // ǰ������һ������
        currentIndex++;

        // ����Ƿ񵽴����һ������
        if (currentIndex >= sentenceIds.Count)
        {
            Debug.Log("����ʾȫ�����ӣ��Ի�����");
            EndDialogue();
            return;
        }

        // ��ʾ��һ������
        typingCoroutine = StartCoroutine(DisplaySentence(sentenceIds[currentIndex]));
    }

    /// <summary>
    /// ��ת��ָ�������ľ���
    /// </summary>
    /// <param name="index">Ҫ��ת�ľ�������</param>
    public void JumpToSentence(int index)
    {
        if (index < 0 || index >= sentenceIds.Count)
        {
            Debug.LogError($"��Ч�ľ�������: {index}");
            return;
        }

        // ���µ�ǰ����
        currentIndex = index;

        // ֹͣ��ǰ��ʾ
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // ��ʾ�¾���
        typingCoroutine = StartCoroutine(DisplaySentence(sentenceIds[currentIndex]));
    }

    /// <summary>
    /// ��ȡ��ǰ���ӵĽ�����Ϣ
    /// </summary>
    /// <returns>��ǰ���ȣ���ʽΪ"��ǰ/����"</returns>
    public string GetProgressInfo()
    {
        return $"{currentIndex + 1}/{sentenceIds.Count}";
    }
}
