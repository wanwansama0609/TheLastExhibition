using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 证物按钮控制器，用于创建可交互的证物
/// </summary>
public class EvidenceButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("证物设置")]
    [SerializeField] private string evidenceId;  // 证物唯一标识符
    [SerializeField] private string evidenceName; // 证物名称
    [SerializeField] private string evidenceDescription; // 证物描述

    [Header("游标设置")]
    [SerializeField] private Texture2D interactiveCursor; // 可交互光标（未收集）
    [SerializeField] private Texture2D collectedCursor; // 已收集光标
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero; // 光标热点

    // 标记是否已被收集
    private bool isCollected = false;

    // 点击事件委托
    public delegate void EvidenceClickedWithIdDelegate(string evidenceId);
    public static event EvidenceClickedWithIdDelegate OnEvidenceClickedWithId;

    private void Start()
    {
        // 检查是否已经收集过
        isCollected = EvidenceManager.Instance.IsEvidenceUnlocked(evidenceId);
    }

    /// <summary>
    /// 设置证物ID和相关信息
    /// </summary>
    public void SetEvidence(string id, string name, string description)
    {
        evidenceId = id;
        evidenceName = name;
        evidenceDescription = description;
    }

    /// <summary>
    /// 处理鼠标点击事件
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {


        // 如果已经被收集过了，不再触发收集逻辑
        if (isCollected)
        {
            // 触发证物点击事件，直接传递ID
            OnEvidenceClickedWithId?.Invoke(evidenceId);
            return;
        }

        // 标记为已收集，防止重复点击触发
        isCollected = true;

        // 触发证物点击事件，直接传递ID
        OnEvidenceClickedWithId?.Invoke(evidenceId);

        // 播放点击音效（如果有）
        PlayClickSound();

        // 触发收集证物的逻辑
        CollectEvidence();
    }

    /// <summary>
    /// 鼠标进入事件
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 只有当所有光标纹理都有效时才修改光标
        if (interactiveCursor != null && collectedCursor != null)
        {
            // 设置适当的光标
            if (isCollected)
            {
                Cursor.SetCursor(isCollected ? collectedCursor : interactiveCursor, cursorHotspot, CursorMode.Auto);
            }
        }
    }

    /// <summary>
    /// 鼠标离开事件
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // 还原为系统默认光标
    }

    /// <summary>
    /// 播放点击音效
    /// </summary>
    private void PlayClickSound()
    {
        // 这里可以添加音效播放代码
        // 例如：AudioManager.Instance.PlaySound("evidence_click");
    }

    /// <summary>
    /// 收集证物逻辑
    /// </summary>
    private void CollectEvidence()
    {
        // 调用证物管理器来解锁证物
        EvidenceManager.Instance.UnlockEvidence(evidenceId);

        // 注意：不再销毁对象，由对话系统统一管理销毁
    }

    /// <summary>
    /// 设置按钮大小
    /// </summary>
    public void SetSize(float width, float height)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(width, height);
        }
    }

    /// <summary>
    /// 设置按钮位置
    /// </summary>
    public void SetPosition(Vector2 position)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
        }
    }

    /// <summary>
    /// 检查证物是否已被收集
    /// </summary>
    public bool IsCollected()
    {
        return isCollected;
    }

    /// <summary>
    /// 获取证物ID
    /// </summary>
    public string GetEvidenceId()
    {
        return evidenceId;
    }

    /// <summary>
    /// 获取证物名称
    /// </summary>
    public string GetEvidenceName()
    {
        return evidenceName;
    }

    /// <summary>
    /// 设置收集状态
    /// </summary>
    public void SetCollected(bool collected)
    {
        isCollected = collected;
    }
}