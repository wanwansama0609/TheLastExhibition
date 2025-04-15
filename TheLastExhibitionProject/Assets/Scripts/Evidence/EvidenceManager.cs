using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 证物管理器，负责管理所有证物的状态和信息
/// 纯工具类，不涉及对话流程控制
/// </summary>
public class EvidenceManager : MonoBehaviour
{
    // 单例实例
    public static EvidenceManager Instance { get; private set; }

    [Header("证物设置")]
    [SerializeField] private GameObject evidenceButtonPrefab; // 证物按钮预制体
    [SerializeField] private Transform evidenceParent; // 证物按钮的父物体

    // 已解锁的证物列表
    private HashSet<string> unlockedEvidences = new HashSet<string>();

    // 证物数据字典，存储所有证物的信息
    private Dictionary<string, EvidenceData> evidenceDatabase = new Dictionary<string, EvidenceData>();

    // 证物解锁事件
    public event Action<string> OnEvidenceUnlocked;

    private void Awake()
    {
        // 设置单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化证物数据
        InitializeEvidenceDatabase();
    }

    /// <summary>
    /// 初始化证物数据库
    /// </summary>
    private void InitializeEvidenceDatabase()
    {
        // 这里可以从JSON或ScriptableObject加载证物数据
        // 临时演示数据
        evidenceDatabase.Add("evidence_001", new EvidenceData
        {
            Id = "evidence_001",
            Name = "破损的信件",
            Description = "一封被撕碎的信件，上面有模糊的字迹。"
        });

        evidenceDatabase.Add("evidence_002", new EvidenceData
        {
            Id = "evidence_002",
            Name = "神秘钥匙",
            Description = "一把古老的钥匙，不知道能打开什么。"
        });

        evidenceDatabase.Add("evidence_003", new EvidenceData
        {
            Id = "evidence_003",
            Name = "指纹痕迹",
            Description = "在桌子上发现的清晰指纹。"
        });
    }

    /// <summary>
    /// 创建证物按钮
    /// </summary>
    public GameObject CreateEvidenceButton(string evidenceId, Vector2 position, float width, float height)
    {
        if (!evidenceDatabase.ContainsKey(evidenceId))
        {
            Debug.LogError($"证物ID不存在: {evidenceId}");
            return null;
        }

        // 实例化预制体
        GameObject buttonObj = Instantiate(evidenceButtonPrefab, evidenceParent);
        EvidenceButton button = buttonObj.GetComponent<EvidenceButton>();

        if (button != null)
        {
            // 设置证物信息
            EvidenceData data = evidenceDatabase[evidenceId];
            button.SetEvidence(data.Id, data.Name, data.Description);

            // 设置位置和大小
            button.SetPosition(position);
            button.SetSize(width, height);
        }

        return buttonObj;
    }

    /// <summary>
    /// 解锁证物
    /// </summary>
    public void UnlockEvidence(string evidenceId)
    {
        if (!evidenceDatabase.ContainsKey(evidenceId))
        {
            Debug.LogError($"尝试解锁不存在的证物: {evidenceId}");
            return;
        }

        if (unlockedEvidences.Contains(evidenceId))
        {
            Debug.Log($"证物已经解锁: {evidenceId}");
            return;
        }

        // 添加到已解锁集合
        unlockedEvidences.Add(evidenceId);

        // 触发解锁事件
        OnEvidenceUnlocked?.Invoke(evidenceId);

        Debug.Log($"解锁证物: {evidenceId} - {evidenceDatabase[evidenceId].Name}");
    }

    /// <summary>
    /// 清除所有证物按钮
    /// </summary>
    public void ClearAllEvidenceButtons()
    {
        if (evidenceParent == null)
            return;

        foreach (Transform child in evidenceParent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 检查证物是否已解锁
    /// </summary>
    public bool IsEvidenceUnlocked(string evidenceId)
    {
        return unlockedEvidences.Contains(evidenceId);
    }

    /// <summary>
    /// 获取证物信息
    /// </summary>
    public EvidenceData GetEvidenceData(string evidenceId)
    {
        if (evidenceDatabase.TryGetValue(evidenceId, out EvidenceData data))
        {
            return data;
        }
        return null;
    }

    /// <summary>
    /// 获取所有已解锁证物的ID
    /// </summary>
    public List<string> GetAllUnlockedEvidenceIds()
    {
        return new List<string>(unlockedEvidences);
    }

    /// <summary>
    /// 重置所有证物解锁状态
    /// </summary>
    public void ResetAllEvidences()
    {
        unlockedEvidences.Clear();
        ClearAllEvidenceButtons();
    }

    /// <summary>
    /// 添加新的证物到数据库
    /// </summary>
    public void AddEvidenceToDatabase(EvidenceData evidenceData)
    {
        if (evidenceData == null || string.IsNullOrEmpty(evidenceData.Id))
        {
            Debug.LogError("无法添加无效的证物数据");
            return;
        }

        if (evidenceDatabase.ContainsKey(evidenceData.Id))
        {
            evidenceDatabase[evidenceData.Id] = evidenceData; // 更新现有数据
        }
        else
        {
            evidenceDatabase.Add(evidenceData.Id, evidenceData); // 添加新数据
        }
    }
}

/// <summary>
/// 证物数据结构
/// </summary>
[System.Serializable]
public class EvidenceData
{
    public string Id;
    public string Name;
    public string Description;
    public Sprite Icon; // 证物图标
}