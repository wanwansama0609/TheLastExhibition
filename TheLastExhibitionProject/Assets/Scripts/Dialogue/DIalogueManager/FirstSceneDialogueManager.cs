using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 任务对话管理器，处理村长分配任务的对话
/// </summary>
public class FirstSceneDialogueManager : DialogueDisplayBase
{
    // 当前正在显示的句子ID
    private string currentSentenceId;

    private RectTransform evidencePanel;

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
        GameObject dialogueContainer,
        RectTransform evidencePanel = null)
        : base(runner, dialogueText, speakerNameText, characterImage, dialogueContainer)
    {
        this.evidencePanel = evidencePanel;
    }

    

    /// <summary>
    /// 推进对话
    /// </summary>
    public override void AdvanceDialogue()
    {
        // 如果正在显示文本，则立即完成当前句子显示
        if (IsDisplayingText())
        {
            CompleteCurrentSentence();
            return;
        }

        // 使用switch-case结构控制对话流程
        switch (dialogueState)
        {
            case 0:
                BackgroundManager.Instance.SwitchToBackground(1);
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
                dialogueState = 27;
                DialogueController.Instance.SetInputEnabled(false);
                StartCoroutine(WaitAndAdvance());
                break;

            case 16:
                currentSentenceId = "sentence_018";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState = 18;
                break;

            case 17:
                if (IsDisplayingText())
                {
                    CompleteCurrentSentence();
                    break;
                }

                if (currentClickedEvidenceId != null)
                {
                    if (currentClickedEvidenceId == "evidence_package")
                    {
                        dialogueState = 16;
                        CleanupEvidencesAndRestoreInput();
                        AdvanceDialogue();
                    }
                    else
                    {
                        currentSentenceId = currentClickedEvidenceId;
                        typingCoroutine = StartCoroutine(ShowSentenceAndRepeat(currentSentenceId));
                    }

                    currentClickedEvidenceId = null;
                }
                break;

            case 18:
                currentSentenceId = "sentence_019";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 19:
                currentSentenceId = "sentence_020";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 20:
                currentSentenceId = "sentence_021";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 21:
                currentSentenceId = "sentence_022";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 22:
                currentSentenceId = "sentence_023";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 23:
                currentSentenceId = "sentence_017";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 24:
                currentSentenceId = "sentence_024";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 25:
                currentSentenceId = "sentence_025";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 27:
                currentSentenceId = "empty";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                PlaceEvidences();
                dialogueState = 17;
                break;

            default:
                Debug.LogWarning("未处理的对话状态: " + dialogueState);
                EndDialogue();
                break;
        }
    }

    private void PlaceEvidences()
    {
        // 如果没有指定面板，就使用对话容器
        Transform parentTransform = evidencePanel != null ?
        (Transform)evidencePanel : dialogueContainer.transform;

        // 创建证物信息列表
        List<(string id, string name, string description, Vector2 position, Vector2 size)> evidenceInfos =
            new List<(string, string, string, Vector2, Vector2)>
        {
        ("evidence_plant", "绿植", "绿植", new Vector2(-238.2f, 31.1f), new Vector2(367.73f, 761.38f)),
        ("evidence_sign1", "告示牌1", "告示牌2", new Vector2(77.64f, 362.4f), new Vector2(155.3f, 234.73f)),
        ("evidence_sign2", "告示牌2", "告示牌2", new Vector2(360.43f, 381.57f), new Vector2(212.12f, 291.58f)),
        ("evidence_bell", "按铃", "按铃", new Vector2(-910.48f, 130f), new Vector2(99.05f, 101.88f)),
        ("evidence_shelf", "编码牌架子", "编码牌架子", new Vector2(-765.66f, 385.23f), new Vector2(388.67f, 309.54f)),
        ("evidence_package", "沙发上的包", "沙发上的包", new Vector2(543f, -261f), new Vector2(368.731f, 363.4f))


        };

        // 放置证物
        foreach (var info in evidenceInfos)
        {
            EvidenceManager.Instance.CreateEvidence(
                info.id, info.name, info.description,
                parentTransform, info.position, info.size);
        }

        // 开始监听证物点击
        StartListeningForEvidenceClicks();

        Debug.Log("已放置证物并禁用常规对话推进");
    }

    /// <summary>
    /// 清理证物并恢复对话推进
    /// </summary>
    private void CleanupEvidencesAndRestoreInput()
    {
        // 停止监听证物点击
        StopListeningForEvidenceClicks();

        // 销毁所有证物
        EvidenceManager.Instance.DestroyAllEvidence();

        // 重新启用常规对话推进
        DialogueController.Instance.SetInputEnabled(true);

        Debug.Log("已清理证物并重新启用常规对话推进");
    }

    private IEnumerator WaitAndAdvance()
    {
        yield return StartCoroutine(DisplaySentence(currentSentenceId));
        yield return new WaitForSeconds(0.5f); // 等待 0.5 秒再推进
        AdvanceDialogue();
    }

    private IEnumerator ShowSentenceAndRepeat(string sentenceId)
    {
        // 显示错误点击的句子
        yield return StartCoroutine(DisplaySentence(sentenceId));
        // 稍微等一下（防止点击残留）
        yield return new WaitForSeconds(0.5f);

        // 隐藏对话容器，避免遮挡证物
        if (!IsDisplayingText())
        {
            dialogueContainer.SetActive(false);
        }
        
        // 再次触发 Advance，重返 case 17
        AdvanceDialogue();
    }
}