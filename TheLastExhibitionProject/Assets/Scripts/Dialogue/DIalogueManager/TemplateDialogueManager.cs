using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// ģ��Ի���������ʹ��switch-case�ṹʵ�ּ򵥶Ի�����
/// </summary>
public class TemplateDialogueManager : DialogueDisplayBase
{
    // ��ǰ������ʾ�ľ���ID
    private string currentSentenceId;

    // �̶����¼�ID
    public override string EventId => "EventTemplate";

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
    /// �ƽ��Ի���ʹ��switch-case�ṹ���ƶԻ�����
    /// </summary>
    public override void AdvanceDialogue()
    {
        // ���������ʾ�ı�����������ɵ�ǰ������ʾ
        if (IsDisplayingText())
        {
            CompleteCurrentSentence();
            return;
        }

        // ʹ��switch-case���ݵ�ǰ״̬������ʾ�ĸ�����
        switch (dialogueState)
        {
            case 0:
                currentSentenceId = "sentence_1";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 1:
                currentSentenceId = "sentence_2";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 2:
                currentSentenceId = "sentence_3";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 3:
                currentSentenceId = "sentence_4";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 4:
                // ����������һЩ�Ի�����ǰ�������߼�
                Debug.Log("�Ի�����");
                EndDialogue();
                break;

            default:
                Debug.LogWarning("δ����ĶԻ�״̬: " + dialogueState);
                EndDialogue();
                break;
        }
    }
}