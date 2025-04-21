using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// 模板对话管理器，使用switch-case结构实现简单对话流程
/// </summary>
public class TemplateDialogueManager : DialogueDisplayBase
{
    // 当前正在显示的句子ID
    private string currentSentenceId;

    // 固定的事件ID
    public override string EventId => "EventTemplate";

    /// <summary>
    /// 构造函数
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
    /// 推进对话，使用switch-case结构控制对话流程
    /// </summary>
    public override void AdvanceDialogue()
    {
        // 如果正在显示文本，则立即完成当前句子显示
        if (IsDisplayingText())
        {
            CompleteCurrentSentence();
            return;
        }

        // 使用switch-case根据当前状态决定显示哪个句子
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
                // 这里可以添加一些对话结束前的特殊逻辑
                Debug.Log("对话结束");
                EndDialogue();
                break;

            default:
                Debug.LogWarning("未处理的对话状态: " + dialogueState);
                EndDialogue();
                break;
        }
    }
}