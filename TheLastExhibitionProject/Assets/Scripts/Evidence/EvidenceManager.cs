using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ֤��������������������֤���״̬����Ϣ
/// �������࣬���漰�Ի����̿���
/// </summary>
public class EvidenceManager : MonoBehaviour
{
    // ����ʵ��
    public static EvidenceManager Instance { get; private set; }

    [Header("֤������")]
    [SerializeField] private GameObject evidenceButtonPrefab; // ֤�ﰴťԤ����
    [SerializeField] private Transform evidenceParent; // ֤�ﰴť�ĸ�����

    // �ѽ�����֤���б�
    private HashSet<string> unlockedEvidences = new HashSet<string>();

    // ֤�������ֵ䣬�洢����֤�����Ϣ
    private Dictionary<string, EvidenceData> evidenceDatabase = new Dictionary<string, EvidenceData>();

    // ֤������¼�
    public event Action<string> OnEvidenceUnlocked;

    private void Awake()
    {
        // ���õ���
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

        // ��ʼ��֤������
        InitializeEvidenceDatabase();
    }

    /// <summary>
    /// ��ʼ��֤�����ݿ�
    /// </summary>
    private void InitializeEvidenceDatabase()
    {
        // ������Դ�JSON��ScriptableObject����֤������
        // ��ʱ��ʾ����
        evidenceDatabase.Add("evidence_001", new EvidenceData
        {
            Id = "evidence_001",
            Name = "������ż�",
            Description = "һ�ⱻ˺����ż���������ģ�����ּ���"
        });

        evidenceDatabase.Add("evidence_002", new EvidenceData
        {
            Id = "evidence_002",
            Name = "����Կ��",
            Description = "һ�ѹ��ϵ�Կ�ף���֪���ܴ�ʲô��"
        });

        evidenceDatabase.Add("evidence_003", new EvidenceData
        {
            Id = "evidence_003",
            Name = "ָ�ƺۼ�",
            Description = "�������Ϸ��ֵ�����ָ�ơ�"
        });
    }

    /// <summary>
    /// ����֤�ﰴť
    /// </summary>
    public GameObject CreateEvidenceButton(string evidenceId, Vector2 position, float width, float height)
    {
        if (!evidenceDatabase.ContainsKey(evidenceId))
        {
            Debug.LogError($"֤��ID������: {evidenceId}");
            return null;
        }

        // ʵ����Ԥ����
        GameObject buttonObj = Instantiate(evidenceButtonPrefab, evidenceParent);
        EvidenceButton button = buttonObj.GetComponent<EvidenceButton>();

        if (button != null)
        {
            // ����֤����Ϣ
            EvidenceData data = evidenceDatabase[evidenceId];
            button.SetEvidence(data.Id, data.Name, data.Description);

            // ����λ�úʹ�С
            button.SetPosition(position);
            button.SetSize(width, height);
        }

        return buttonObj;
    }

    /// <summary>
    /// ����֤��
    /// </summary>
    public void UnlockEvidence(string evidenceId)
    {
        if (!evidenceDatabase.ContainsKey(evidenceId))
        {
            Debug.LogError($"���Խ��������ڵ�֤��: {evidenceId}");
            return;
        }

        if (unlockedEvidences.Contains(evidenceId))
        {
            Debug.Log($"֤���Ѿ�����: {evidenceId}");
            return;
        }

        // ��ӵ��ѽ�������
        unlockedEvidences.Add(evidenceId);

        // ���������¼�
        OnEvidenceUnlocked?.Invoke(evidenceId);

        Debug.Log($"����֤��: {evidenceId} - {evidenceDatabase[evidenceId].Name}");
    }

    /// <summary>
    /// �������֤�ﰴť
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
    /// ���֤���Ƿ��ѽ���
    /// </summary>
    public bool IsEvidenceUnlocked(string evidenceId)
    {
        return unlockedEvidences.Contains(evidenceId);
    }

    /// <summary>
    /// ��ȡ֤����Ϣ
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
    /// ��ȡ�����ѽ���֤���ID
    /// </summary>
    public List<string> GetAllUnlockedEvidenceIds()
    {
        return new List<string>(unlockedEvidences);
    }

    /// <summary>
    /// ��������֤�����״̬
    /// </summary>
    public void ResetAllEvidences()
    {
        unlockedEvidences.Clear();
        ClearAllEvidenceButtons();
    }

    /// <summary>
    /// ����µ�֤�ﵽ���ݿ�
    /// </summary>
    public void AddEvidenceToDatabase(EvidenceData evidenceData)
    {
        if (evidenceData == null || string.IsNullOrEmpty(evidenceData.Id))
        {
            Debug.LogError("�޷������Ч��֤������");
            return;
        }

        if (evidenceDatabase.ContainsKey(evidenceData.Id))
        {
            evidenceDatabase[evidenceData.Id] = evidenceData; // ������������
        }
        else
        {
            evidenceDatabase.Add(evidenceData.Id, evidenceData); // ���������
        }
    }
}

/// <summary>
/// ֤�����ݽṹ
/// </summary>
[System.Serializable]
public class EvidenceData
{
    public string Id;
    public string Name;
    public string Description;
    public Sprite Icon; // ֤��ͼ��
}