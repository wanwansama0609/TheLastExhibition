using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������֤����ʾ������ʾ���ʹ��EvidenceSceneBase����
/// </summary>
public class LivingRoomEvidenceScene : EvidenceSceneBase
{
    [Header("��������")]
    [SerializeField] private string sceneId = "scene_living_room"; // ����Ψһ��ʶ��

    // ��д��ȡ����ID����
    public override string GetSceneId()
    {
        return sceneId;
    }

    // ��д����֤�﷽��
    protected override void ConfigureEvidences()
    {
        // ���ÿ��������е�֤��
        // ���֤�����ã�����ID��λ�úʹ�С
        sceneEvidenceConfigs.Add(new EvidenceConfig
        {
            EvidenceId = "evidence_001", // ������ż�
            Position = new Vector2(120, 250),
            Width = 100,
            Height = 50
        });

        sceneEvidenceConfigs.Add(new EvidenceConfig
        {
            EvidenceId = "evidence_002", // ����Կ��
            Position = new Vector2(450, 180),
            Width = 80,
            Height = 40
        });

        sceneEvidenceConfigs.Add(new EvidenceConfig
        {
            EvidenceId = "evidence_003", // ָ�ƺۼ�
            Position = new Vector2(320, 400),
            Width = 120,
            Height = 60
        });
    }

    // ʾ������Start��ע��ص��¼�
    protected override void Start()
    {
        base.Start();

        // ע�ᵥ��֤������Ļص�
        OnEvidenceUnlocked += HandleSingleEvidenceUnlocked;

        // ע������֤������Ļص�
        OnAllEvidenceCollected += HandleAllEvidencesCollected;
    }

    private void OnDestroy()
    {
        // ȡ��ע��ص�����ֹ�ڴ�й©
        OnEvidenceUnlocked -= HandleSingleEvidenceUnlocked;
        OnAllEvidenceCollected -= HandleAllEvidencesCollected;
    }

    // ������֤������Ļص�
    private void HandleSingleEvidenceUnlocked(string evidenceId)
    {
        // ��ȡ֤������
        EvidenceData evidenceData = EvidenceManager.Instance.GetEvidenceData(evidenceId);

        if (evidenceData != null)
        {
            // ���������ʾUI��ʾ�򴥷��Ի���
            Debug.Log($"�������� - ����֤��: {evidenceData.Name}");

            // ���磺��ʾ֤�﷢����ʾ
            // UIManager.Instance.ShowEvidenceDiscoveredMessage(evidenceData.Name);

            // ���ߣ�������ضԻ�
            // DialogueManager.Instance.TriggerDialogue("found_" + evidenceId);
        }
    }

    // ��������֤������Ļص�
    private void HandleAllEvidencesCollected(string sceneId)
    {
        Debug.Log($"��ϲ�������е�����֤�������룡");

        // ������Դ�����һ�����������
        // ���磺�����³���
        // GameManager.Instance.UnlockNextScene();

        // ���ߣ���������Ի�
        // DialogueManager.Instance.TriggerDialogue("scene_complete_" + sceneId);
    }

    // ʾ����������Զ��巽����չʾ�����÷�

    // ��д֤�ﰴť����¼�����
    protected override void HandleEvidenceClicked(string evidenceId)
    {
        base.HandleEvidenceClicked(evidenceId);

        // ��Ӷ���ĵ��Ч�������粥���ض���Ч�򶯻�
        Debug.Log($"�������� - ֤�ﰴť�����: {evidenceId}");

        // ���磺�����ض���Ч
        // AudioManager.Instance.PlaySound("evidence_click_" + evidenceId);
    }

    // ʾ���������ض��Ĺ�������
    public void ResetAndRestart()
    {
        // ����ǰ֤��
        ClearAllEvidenceButtons();

        // ���³�ʼ������
        InitializeScene();

        Debug.Log("������֤����������");
    }
}