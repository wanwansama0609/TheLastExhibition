using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// 任务对话管理器，处理村长分配任务的对话
/// </summary>
public class FirstSceneDialogueManager : DialogueDisplayBase
{
    // 当前正在显示的句子ID
    private string currentSentenceId;

    // 固定的事件ID
    public override string EventId => "FirstScene";

    /// <summary>
    /// 构造函数
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
    /// 推进对话
    /// </summary>
    public override void AdvanceDialogue()
    {
        // 如果正在显示文本，则立即完成当前句子显示
        if (IsDisplayingText())
        {
            CompleteCurrentSentence(currentSentenceId);
            return;
        }

        // 使用switch-case结构控制对话流程
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
                Debug.LogWarning("未处理的对话状态: " + dialogueState);
                EndDialogue();
                break;
        }
    }
}