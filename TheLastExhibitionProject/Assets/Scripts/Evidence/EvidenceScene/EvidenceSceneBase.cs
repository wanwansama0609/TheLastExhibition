using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��֤�������࣬�������֤���Ѽ�����
/// ������Ҫʵ�ֳ���ID��֤������
/// </summary>
public abstract class EvidenceSceneBase : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] protected bool autoInitOnStart = true; // �Ƿ���Startʱ�Զ���ʼ��

    // ֤��������Ϣ
    protected class EvidenceConfig
    {
        public string EvidenceId; // ֤��ID
        public Vector2 Position;  // λ�ã�UI���꣩
        public float Width;       // ���
        public float Height;      // �߶�
    }

    // �Ѵ�����֤�ﰴť�б�
    protected List<GameObject> createdEvidenceButtons = new List<GameObject>();

    // ��ǰ�����е�֤������
    protected List<EvidenceConfig> sceneEvidenceConfigs = new List<EvidenceConfig>();

    // �ѽ�����֤��ID����
    protected HashSet<string> unlockedEvidences = new HashSet<string>();

    // �¼�������֤�ﱻ����
    public event Action<string> OnEvidenceUnlocked;

    // �¼�������֤�ﶼ������
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
        // ����ȫ��֤������¼�
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.OnEvidenceUnlocked += HandleEvidenceUnlocked;
        }

        // ����֤�ﰴť����¼�
        EvidenceButton.OnEvidenceClicked += HandleEvidenceClicked;
    }

    protected virtual void OnDisable()
    {
        // ȡ������ȫ��֤������¼�
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.OnEvidenceUnlocked -= HandleEvidenceUnlocked;
        }

        // ȡ������֤�ﰴť����¼�
        EvidenceButton.OnEvidenceClicked -= HandleEvidenceClicked;
    }

    /// <summary>
    /// ��ʼ����֤����
    /// </summary>
    public virtual void InitializeScene()
    {
        // ����֮ǰ��֤�ﰴť
        ClearAllEvidenceButtons();

        // ����״̬
        unlockedEvidences.Clear();
        sceneEvidenceConfigs.Clear();

        // ����֤���б�����ʵ�֣�
        ConfigureEvidences();

        // ����֤�ﰴť
        CreateAllEvidenceButtons();

        // ������еĽ���״̬
        CheckExistingUnlockedEvidences();

        Debug.Log($"��ʼ����֤����: {GetSceneId()}, ֤������: {sceneEvidenceConfigs.Count}");
    }

    /// <summary>
    /// �������е�����֤�ﰴť
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
    /// ��������֤�ﰴť
    /// </summary>
    protected virtual void CreateAllEvidenceButtons()
    {
        if (EvidenceManager.Instance == null)
        {
            Debug.LogError("EvidenceManagerʵ�������ڣ��޷�����֤�ﰴť");
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
    /// ������еĽ���״̬
    /// </summary>
    protected virtual void CheckExistingUnlockedEvidences()
    {
        if (EvidenceManager.Instance == null) return;

        foreach (var config in sceneEvidenceConfigs)
        {
            if (EvidenceManager.Instance.IsEvidenceUnlocked(config.EvidenceId))
            {
                unlockedEvidences.Add(config.EvidenceId);

                // ���°�ť״̬
                UpdateButtonCollectionState(config.EvidenceId, true);
            }
        }

        // ����Ƿ�����֤�ﶼ���ҵ�
        CheckAllEvidencesCollected();
    }

    /// <summary>
    /// ���°�ť���ռ�״̬
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
    /// ����֤������¼�
    /// </summary>
    protected virtual void HandleEvidenceUnlocked(string evidenceId)
    {
        // ����Ƿ��ǵ�ǰ�����е�֤��
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

        // ��ӵ��ѽ�������
        unlockedEvidences.Add(evidenceId);

        // ��������֤������¼�
        OnEvidenceUnlocked?.Invoke(evidenceId);

        // ����Ƿ�����֤�ﶼ���ҵ�
        CheckAllEvidencesCollected();
    }

    /// <summary>
    /// ����֤�ﰴť����¼�
    /// </summary>
    protected virtual void HandleEvidenceClicked(string evidenceId)
    {
        // ������Ը��Ǵ˷���������Զ����߼�
    }

    /// <summary>
    /// ����Ƿ�����֤�ﶼ���ҵ�
    /// </summary>
    protected virtual void CheckAllEvidencesCollected()
    {
        if (unlockedEvidences.Count == sceneEvidenceConfigs.Count)
        {
            // ����֤�ﶼ���ҵ�
            OnAllEvidenceCollected?.Invoke(GetSceneId());
            Debug.Log($"���� {GetSceneId()} �е�����֤��������!");
        }
    }

    /// <summary>
    /// ��ȡ����ID���������ʵ�֣�
    /// </summary>
    public abstract string GetSceneId();

    /// <summary>
    /// ���ó����е�֤��������ʵ�֣�
    /// </summary>
    protected abstract void ConfigureEvidences();

    /// <summary>
    /// �Ƿ�����֤�ﶼ���ҵ�
    /// </summary>
    public bool IsAllEvidencesCollected()
    {
        return unlockedEvidences.Count >= sceneEvidenceConfigs.Count;
    }

    /// <summary>
    /// ��ȡ�ѽ�����֤������
    /// </summary>
    public int GetUnlockedEvidenceCount()
    {
        return unlockedEvidences.Count;
    }

    /// <summary>
    /// ��ȡ��֤������
    /// </summary>
    public int GetTotalEvidenceCount()
    {
        return sceneEvidenceConfigs.Count;
    }
}