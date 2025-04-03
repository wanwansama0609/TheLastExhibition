using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// ����Ի�������������峤��������ĶԻ�
/// </summary>
public class FirstSceneDialogueManager : DialogueDisplayBase
{
    // ��ǰ������ʾ�ľ���ID
    private string currentSentenceId;

    // �̶����¼�ID
    public override string EventId => "FirstScene";

    /// <summary>
    /// ���캯��
    /// </summary>
    public FirstSceneDialogueManager(
        MonoBehaviour runner,
        TextMeshProUGUI dialogueText,
        TextMeshProUGUI speakerNameText,
        UnityEngine.UI.Image characterImage,
        GameObject dialogueContainer)
        : base(runner, dialogueText, speakerNameText, characterImage, dialogueContainer)
    {
    }

    /// <summary>
    /// �ƽ��Ի�
    /// </summary>
    public override void AdvanceDialogue()
    {
        // ���������ʾ�ı�����������ɵ�ǰ������ʾ
        if (IsDisplayingText())
        {
            CompleteCurrentSentence(currentSentenceId);
            return;
        }

        // ʹ��switch-case�ṹ���ƶԻ�����
        switch (dialogueState)
        {
            case 0:
                currentSentenceId = "sentence_001";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 1:
                currentSentenceId = "sentence_002";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 2:
                currentSentenceId = "sentence_003";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 3:
                currentSentenceId = "sentence_004";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 4:
                currentSentenceId = "sentence_005";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 5:
                currentSentenceId = "sentence_006";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 6:
                currentSentenceId = "sentence_007";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 7:
                currentSentenceId = "sentence_008";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 8:
                currentSentenceId = "sentence_009";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 9:
                currentSentenceId = "sentence_010";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 10:
                currentSentenceId = "sentence_011";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 11:
                currentSentenceId = "sentence_012";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 12:
                currentSentenceId = "sentence_013";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 13:
                currentSentenceId = "sentence_014";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 14:
                currentSentenceId = "sentence_015";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 15:
                currentSentenceId = "sentence_016";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 16:
                currentSentenceId = "sentence_017";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 17:
                currentSentenceId = "sentence_018";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            default:
                Debug.LogWarning("δ����ĶԻ�״̬: " + dialogueState);
                EndDialogue();
                break;
        }
    }
}