using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 客厅搜证场景示例，演示如何使用EvidenceSceneBase基类
/// </summary>
public class LivingRoomEvidenceScene : EvidenceSceneBase
{
    [Header("场景设置")]
    [SerializeField] private string sceneId = "scene_living_room"; // 场景唯一标识符

    // 重写获取场景ID方法
    public override string GetSceneId()
    {
        return sceneId;
    }

    // 重写配置证物方法
    protected override void ConfigureEvidences()
    {
        // 配置客厅场景中的证物
        // 添加证物配置，包括ID、位置和大小
        sceneEvidenceConfigs.Add(new EvidenceConfig
        {
            EvidenceId = "evidence_001", // 破损的信件
            Position = new Vector2(120, 250),
            Width = 100,
            Height = 50
        });

        sceneEvidenceConfigs.Add(new EvidenceConfig
        {
            EvidenceId = "evidence_002", // 神秘钥匙
            Position = new Vector2(450, 180),
            Width = 80,
            Height = 40
        });

        sceneEvidenceConfigs.Add(new EvidenceConfig
        {
            EvidenceId = "evidence_003", // 指纹痕迹
            Position = new Vector2(320, 400),
            Width = 120,
            Height = 60
        });
    }

    // 示例：在Start中注册回调事件
    protected override void Start()
    {
        base.Start();

        // 注册单个证物解锁的回调
        OnEvidenceUnlocked += HandleSingleEvidenceUnlocked;

        // 注册所有证物找齐的回调
        OnAllEvidenceCollected += HandleAllEvidencesCollected;
    }

    private void OnDestroy()
    {
        // 取消注册回调，防止内存泄漏
        OnEvidenceUnlocked -= HandleSingleEvidenceUnlocked;
        OnAllEvidenceCollected -= HandleAllEvidencesCollected;
    }

    // 处理单个证物解锁的回调
    private void HandleSingleEvidenceUnlocked(string evidenceId)
    {
        // 获取证物数据
        EvidenceData evidenceData = EvidenceManager.Instance.GetEvidenceData(evidenceId);

        if (evidenceData != null)
        {
            // 这里可以显示UI提示或触发对话等
            Debug.Log($"客厅场景 - 发现证物: {evidenceData.Name}");

            // 例如：显示证物发现提示
            // UIManager.Instance.ShowEvidenceDiscoveredMessage(evidenceData.Name);

            // 或者：触发相关对话
            // DialogueManager.Instance.TriggerDialogue("found_" + evidenceId);
        }
    }

    // 处理所有证物找齐的回调
    private void HandleAllEvidencesCollected(string sceneId)
    {
        Debug.Log($"恭喜！客厅中的所有证物已找齐！");

        // 这里可以触发下一步剧情或奖励等
        // 例如：解锁新场景
        // GameManager.Instance.UnlockNextScene();

        // 或者：触发结算对话
        // DialogueManager.Instance.TriggerDialogue("scene_complete_" + sceneId);
    }

    // 示例：额外的自定义方法，展示更多用法

    // 重写证物按钮点击事件处理
    protected override void HandleEvidenceClicked(string evidenceId)
    {
        base.HandleEvidenceClicked(evidenceId);

        // 添加额外的点击效果，例如播放特定音效或动画
        Debug.Log($"客厅场景 - 证物按钮被点击: {evidenceId}");

        // 例如：播放特定音效
        // AudioManager.Instance.PlaySound("evidence_click_" + evidenceId);
    }

    // 示例：场景特定的公共方法
    public void ResetAndRestart()
    {
        // 清理当前证物
        ClearAllEvidenceButtons();

        // 重新初始化场景
        InitializeScene();

        Debug.Log("客厅搜证场景已重置");
    }
}