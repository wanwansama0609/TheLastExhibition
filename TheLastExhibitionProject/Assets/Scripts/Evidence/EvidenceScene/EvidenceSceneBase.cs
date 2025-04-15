using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 搜证场景基类，负责管理证物搜集流程
/// 子类需要实现场景ID和证物配置
/// </summary>
public abstract class EvidenceSceneBase : MonoBehaviour
{
    [Header("基础设置")]
    [SerializeField] protected bool autoInitOnStart = true; // 是否在Start时自动初始化

    // 证物配置信息
    protected class EvidenceConfig
    {
        public string EvidenceId; // 证物ID
        public Vector2 Position;  // 位置（UI坐标）
        public float Width;       // 宽度
        public float Height;      // 高度
    }

    // 已创建的证物按钮列表
    protected List<GameObject> createdEvidenceButtons = new List<GameObject>();

    // 当前场景中的证物配置
    protected List<EvidenceConfig> sceneEvidenceConfigs = new List<EvidenceConfig>();

    // 已解锁的证物ID集合
    protected HashSet<string> unlockedEvidences = new HashSet<string>();

    // 事件：单个证物被解锁
    public event Action<string> OnEvidenceUnlocked;

    // 事件：所有证物都被找齐
    public event Action<string> OnAllEvidenceCollected;

    protected virtual void Start()
    {
        if (autoInitOnStart)
        {
            InitializeScene();
        }
    }

    protected virtual void OnEnable()
    {
        // 订阅全局证物解锁事件
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.OnEvidenceUnlocked += HandleEvidenceUnlocked;
        }

        // 订阅证物按钮点击事件
        EvidenceButton.OnEvidenceClicked += HandleEvidenceClicked;
    }

    protected virtual void OnDisable()
    {
        // 取消订阅全局证物解锁事件
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.OnEvidenceUnlocked -= HandleEvidenceUnlocked;
        }

        // 取消订阅证物按钮点击事件
        EvidenceButton.OnEvidenceClicked -= HandleEvidenceClicked;
    }

    /// <summary>
    /// 初始化搜证场景
    /// </summary>
    public virtual void InitializeScene()
    {
        // 清理之前的证物按钮
        ClearAllEvidenceButtons();

        // 重置状态
        unlockedEvidences.Clear();
        sceneEvidenceConfigs.Clear();

        // 配置证物列表（子类实现）
        ConfigureEvidences();

        // 创建证物按钮
        CreateAllEvidenceButtons();

        // 检查已有的解锁状态
        CheckExistingUnlockedEvidences();

        Debug.Log($"初始化搜证场景: {GetSceneId()}, 证物数量: {sceneEvidenceConfigs.Count}");
    }

    /// <summary>
    /// 清理场景中的所有证物按钮
    /// </summary>
    public virtual void ClearAllEvidenceButtons()
    {
        foreach (var button in createdEvidenceButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }

        createdEvidenceButtons.Clear();
    }

    /// <summary>
    /// 创建所有证物按钮
    /// </summary>
    protected virtual void CreateAllEvidenceButtons()
    {
        if (EvidenceManager.Instance == null)
        {
            Debug.LogError("EvidenceManager实例不存在，无法创建证物按钮");
            return;
        }

        foreach (var config in sceneEvidenceConfigs)
        {
            GameObject button = EvidenceManager.Instance.CreateEvidenceButton(
                config.EvidenceId,
                config.Position,
                config.Width,
                config.Height
            );

            if (button != null)
            {
                createdEvidenceButtons.Add(button);
            }
        }
    }

    /// <summary>
    /// 检查已有的解锁状态
    /// </summary>
    protected virtual void CheckExistingUnlockedEvidences()
    {
        if (EvidenceManager.Instance == null) return;

        foreach (var config in sceneEvidenceConfigs)
        {
            if (EvidenceManager.Instance.IsEvidenceUnlocked(config.EvidenceId))
            {
                unlockedEvidences.Add(config.EvidenceId);

                // 更新按钮状态
                UpdateButtonCollectionState(config.EvidenceId, true);
            }
        }

        // 检查是否所有证物都已找到
        CheckAllEvidencesCollected();
    }

    /// <summary>
    /// 更新按钮的收集状态
    /// </summary>
    protected virtual void UpdateButtonCollectionState(string evidenceId, bool isCollected)
    {
        foreach (var buttonObj in createdEvidenceButtons)
        {
            if (buttonObj == null) continue;

            EvidenceButton button = buttonObj.GetComponent<EvidenceButton>();
            if (button != null && button.GetEvidenceId() == evidenceId)
            {
                button.SetCollected(isCollected);
                break;
            }
        }
    }

    /// <summary>
    /// 处理证物解锁事件
    /// </summary>
    protected virtual void HandleEvidenceUnlocked(string evidenceId)
    {
        // 检查是否是当前场景中的证物
        bool isSceneEvidence = false;
        foreach (var config in sceneEvidenceConfigs)
        {
            if (config.EvidenceId == evidenceId)
            {
                isSceneEvidence = true;
                break;
            }
        }

        if (!isSceneEvidence) return;

        // 添加到已解锁集合
        unlockedEvidences.Add(evidenceId);

        // 触发单个证物解锁事件
        OnEvidenceUnlocked?.Invoke(evidenceId);

        // 检查是否所有证物都已找到
        CheckAllEvidencesCollected();
    }

    /// <summary>
    /// 处理证物按钮点击事件
    /// </summary>
    protected virtual void HandleEvidenceClicked(string evidenceId)
    {
        // 子类可以覆盖此方法以添加自定义逻辑
    }

    /// <summary>
    /// 检查是否所有证物都已找到
    /// </summary>
    protected virtual void CheckAllEvidencesCollected()
    {
        if (unlockedEvidences.Count == sceneEvidenceConfigs.Count)
        {
            // 所有证物都已找到
            OnAllEvidenceCollected?.Invoke(GetSceneId());
            Debug.Log($"场景 {GetSceneId()} 中的所有证物已找齐!");
        }
    }

    /// <summary>
    /// 获取场景ID（子类必须实现）
    /// </summary>
    public abstract string GetSceneId();

    /// <summary>
    /// 配置场景中的证物（子类必须实现）
    /// </summary>
    protected abstract void ConfigureEvidences();

    /// <summary>
    /// 是否所有证物都已找到
    /// </summary>
    public bool IsAllEvidencesCollected()
    {
        return unlockedEvidences.Count >= sceneEvidenceConfigs.Count;
    }

    /// <summary>
    /// 获取已解锁的证物数量
    /// </summary>
    public int GetUnlockedEvidenceCount()
    {
        return unlockedEvidences.Count;
    }

    /// <summary>
    /// 获取总证物数量
    /// </summary>
    public int GetTotalEvidenceCount()
    {
        return sceneEvidenceConfigs.Count;
    }
}