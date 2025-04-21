using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 证物管理器，负责创建、放置和销毁证物
/// </summary>
public class EvidenceManager : MonoBehaviour
{
    // 单例实例
    public static EvidenceManager Instance { get; private set; }

    [Header("预制体设置")]
    [SerializeField] private GameObject evidencePrefab; // 证物按钮预制体

    // 当前场景中的所有证物
    private Dictionary<string, EvidenceButton> activeEvidence = new Dictionary<string, EvidenceButton>();

    // 已解锁的证物ID列表
    private HashSet<string> unlockedEvidence = new HashSet<string>();

    // 证物被收集的事件委托
    public delegate void EvidenceCollectedDelegate(string evidenceId);
    public event EvidenceCollectedDelegate OnEvidenceCollected;

    private void Awake()
    {
        // 单例模式初始化
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

        // 订阅证物点击事件
        EvidenceButton.OnEvidenceClickedWithId += HandleEvidenceClicked;
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        EvidenceButton.OnEvidenceClickedWithId -= HandleEvidenceClicked;
    }

    /// <summary>
    /// 处理证物被点击的事件
    /// </summary>
    private void HandleEvidenceClicked(string evidenceId)
    {
        // 标记为已解锁
        unlockedEvidence.Add(evidenceId);

        // 转发事件
        OnEvidenceCollected?.Invoke(evidenceId);
    }

    /// <summary>
    /// 创建证物按钮并放置在指定位置
    /// </summary>
    /// <param name="evidenceId">证物ID</param>
    /// <param name="name">证物名称</param>
    /// <param name="description">证物描述</param>
    /// <param name="parent">父物体Transform</param>
    /// <param name="position">位置</param>
    /// <param name="size">大小</param>
    /// <returns>创建的证物按钮组件</returns>
    public EvidenceButton CreateEvidence(string evidenceId, string name, string description,
                                         Transform parent, Vector2 position, Vector2 size)
    {
        // 检查是否已存在相同ID的证物
        if (activeEvidence.ContainsKey(evidenceId))
        {
            Debug.LogWarning($"证物已存在: {evidenceId}");
            return activeEvidence[evidenceId];
        }

        // 实例化预制体
        GameObject evidenceObj = Instantiate(evidencePrefab, parent);
        evidenceObj.SetActive(true);

        // 获取EvidenceButton组件
        EvidenceButton evidenceButton = evidenceObj.GetComponent<EvidenceButton>();

        if (evidenceButton != null)
        {
            // 设置证物信息
            evidenceButton.SetEvidence(evidenceId, name, description);

            // 设置位置和大小
            evidenceButton.SetPosition(position);
            evidenceButton.SetSize(size.x, size.y);

            // 设置收集状态
            evidenceButton.SetCollected(IsEvidenceUnlocked(evidenceId));

            // 添加到活跃证物字典
            activeEvidence[evidenceId] = evidenceButton;

            return evidenceButton;
        }

        Debug.LogError($"创建证物失败: {evidenceId}");
        Destroy(evidenceObj);
        return null;
    }

    /// <summary>
    /// 创建证物按钮 - 简化版本，使用默认大小
    /// </summary>
    public EvidenceButton CreateEvidence(string evidenceId, string name, string description, Transform parent, Vector2 position)
    {
        // 使用默认大小 100x100
        return CreateEvidence(evidenceId, name, description, parent, position, new Vector2(100, 100));
    }

    /// <summary>
    /// 销毁特定ID的证物
    /// </summary>
    public void DestroyEvidence(string evidenceId)
    {
        if (activeEvidence.TryGetValue(evidenceId, out EvidenceButton evidenceButton))
        {
            if (evidenceButton != null && evidenceButton.gameObject != null)
            {
                Destroy(evidenceButton.gameObject);
            }

            activeEvidence.Remove(evidenceId);
        }
    }

    /// <summary>
    /// 销毁所有活跃的证物
    /// </summary>
    public void DestroyAllEvidence()
    {
        foreach (var evidence in activeEvidence.Values)
        {
            if (evidence != null && evidence.gameObject != null)
            {
                Destroy(evidence.gameObject);
            }
        }

        activeEvidence.Clear();
    }

    /// <summary>
    /// 检查证物是否已被解锁
    /// </summary>
    public bool IsEvidenceUnlocked(string evidenceId)
    {
        return unlockedEvidence.Contains(evidenceId);
    }

    /// <summary>
    /// 手动解锁证物
    /// </summary>
    public void UnlockEvidence(string evidenceId)
    {
        if (!unlockedEvidence.Contains(evidenceId))
        {
            unlockedEvidence.Add(evidenceId);

            // 如果证物当前在场景中，更新其状态
            if (activeEvidence.TryGetValue(evidenceId, out EvidenceButton button))
            {
                button.SetCollected(true);
            }

            // 触发事件
            OnEvidenceCollected?.Invoke(evidenceId);
        }
    }

    /// <summary>
    /// 获取活跃的证物按钮
    /// </summary>
    public EvidenceButton GetEvidenceButton(string evidenceId)
    {
        if (activeEvidence.TryGetValue(evidenceId, out EvidenceButton button))
        {
            return button;
        }
        return null;
    }

    /// <summary>
    /// 从Resources文件夹加载预制体
    /// </summary>
    public void LoadEvidencePrefab(string prefabPath = "Prefabs/EvidenceButton")
    {
        evidencePrefab = Resources.Load<GameObject>(prefabPath);
        if (evidencePrefab == null)
        {
            Debug.LogError($"无法加载证物预制体: {prefabPath}");
        }
    }
}