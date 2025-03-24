using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// 线性对话管理器，实现按顺序显示对话的流程
/// </summary>
public class TemplateDialogueManager : DialogueDisplayBase
{
    // 对话句子ID列表（按顺序）
    private List<string> sentenceIds = new List<string>();

    // 当前句子索引
    private int currentIndex = 0;

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
    /// 重写初始化方法，额外准备句子顺序列表
    /// </summary>
    public override async Task Initialize(string eventId)
    {
        // 先调用基类初始化方法
        await base.Initialize(eventId);

        // 准备句子ID列表
        PrepareDialogueSequence();
    }

    /// <summary>
    /// 准备对话顺序
    /// 可以根据自定义规则确定句子的显示顺序
    /// </summary>
    private void PrepareDialogueSequence()
    {
        sentenceIds.Clear();

        // 提取所有句子ID
        foreach (var sentenceId in currentDialogueData.Keys)
        {
            sentenceIds.Add(sentenceId);
        }

        // 根据句子ID排序（假设ID格式为"sentence_1", "sentence_2"等）
        sentenceIds.Sort((a, b) => {
            // 提取ID中的数字部分
            if (int.TryParse(a.Split('_')[1], out int numA) &&
                int.TryParse(b.Split('_')[1], out int numB))
            {
                return numA.CompareTo(numB);
            }
            return string.Compare(a, b);
        });

        Debug.Log($"已准备 {sentenceIds.Count} 个句子的显示顺序");
    }

    /// <summary>
    /// 开始对话流程，显示第一个句子
    /// </summary>
    public override void StartDialogue()
    {
        if (sentenceIds.Count == 0)
        {
            Debug.LogWarning("没有句子可以显示");
            EndDialogue();
            return;
        }

        // 显示对话UI
        if (dialogueContainer != null)
        {
            dialogueContainer.SetActive(true);
        }

        // 重置索引并显示第一个句子
        currentIndex = 0;
        typingCoroutine = StartCoroutine(DisplaySentence(sentenceIds[currentIndex]));

        Debug.Log($"开始显示对话事件: {currentEventId}");
    }

    /// <summary>
    /// 推进对话到下一句
    /// </summary>
    public override void AdvanceDialogue()
    {
        // 如果正在显示文本，则立即完成当前句子显示
        if (IsDisplayingText())
        {
            CompleteCurrentSentence();
            return;
        }

        // 前进到下一个句子
        currentIndex++;

        // 检查是否到达最后一个句子
        if (currentIndex >= sentenceIds.Count)
        {
            Debug.Log("已显示全部句子，对话结束");
            EndDialogue();
            return;
        }

        // 显示下一个句子
        typingCoroutine = StartCoroutine(DisplaySentence(sentenceIds[currentIndex]));
    }

    /// <summary>
    /// 跳转到指定索引的句子
    /// </summary>
    /// <param name="index">要跳转的句子索引</param>
    public void JumpToSentence(int index)
    {
        if (index < 0 || index >= sentenceIds.Count)
        {
            Debug.LogError($"无效的句子索引: {index}");
            return;
        }

        // 更新当前索引
        currentIndex = index;

        // 停止当前显示
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 显示新句子
        typingCoroutine = StartCoroutine(DisplaySentence(sentenceIds[currentIndex]));
    }

    /// <summary>
    /// 获取当前句子的进度信息
    /// </summary>
    /// <returns>当前进度，格式为"当前/总数"</returns>
    public string GetProgressInfo()
    {
        return $"{currentIndex + 1}/{sentenceIds.Count}";
    }
}
