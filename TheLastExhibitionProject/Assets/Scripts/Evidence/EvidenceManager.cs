using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ֤������������𴴽������ú�����֤��
/// </summary>
public class EvidenceManager : MonoBehaviour
{
    // ����ʵ��
    public static EvidenceManager Instance { get; private set; }

    [Header("Ԥ��������")]
    [SerializeField] private GameObject evidencePrefab; // ֤�ﰴťԤ����

    // ��ǰ�����е�����֤��
    private Dictionary<string, EvidenceButton> activeEvidence = new Dictionary<string, EvidenceButton>();

    // �ѽ�����֤��ID�б�
    private HashSet<string> unlockedEvidence = new HashSet<string>();

    // ֤�ﱻ�ռ����¼�ί��
    public delegate void EvidenceCollectedDelegate(string evidenceId);
    public event EvidenceCollectedDelegate OnEvidenceCollected;

    private void Awake()
    {
        // ����ģʽ��ʼ��
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

        // ����֤�����¼�
        EvidenceButton.OnEvidenceClickedWithId += HandleEvidenceClicked;
    }

    private void OnDestroy()
    {
        // ȡ�������¼�
        EvidenceButton.OnEvidenceClickedWithId -= HandleEvidenceClicked;
    }

    /// <summary>
    /// ����֤�ﱻ������¼�
    /// </summary>
    private void HandleEvidenceClicked(string evidenceId)
    {
        // ���Ϊ�ѽ���
        unlockedEvidence.Add(evidenceId);

        // ת���¼�
        OnEvidenceCollected?.Invoke(evidenceId);
    }

    /// <summary>
    /// ����֤�ﰴť��������ָ��λ��
    /// </summary>
    /// <param name="evidenceId">֤��ID</param>
    /// <param name="name">֤������</param>
    /// <param name="description">֤������</param>
    /// <param name="parent">������Transform</param>
    /// <param name="position">λ��</param>
    /// <param name="size">��С</param>
    /// <returns>������֤�ﰴť���</returns>
    public EvidenceButton CreateEvidence(string evidenceId, string name, string description,
                                         Transform parent, Vector2 position, Vector2 size)
    {
        // ����Ƿ��Ѵ�����ͬID��֤��
        if (activeEvidence.ContainsKey(evidenceId))
        {
            Debug.LogWarning($"֤���Ѵ���: {evidenceId}");
            return activeEvidence[evidenceId];
        }

        // ʵ����Ԥ����
        GameObject evidenceObj = Instantiate(evidencePrefab, parent);
        evidenceObj.SetActive(true);

        // ��ȡEvidenceButton���
        EvidenceButton evidenceButton = evidenceObj.GetComponent<EvidenceButton>();

        if (evidenceButton != null)
        {
            // ����֤����Ϣ
            evidenceButton.SetEvidence(evidenceId, name, description);

            // ����λ�úʹ�С
            evidenceButton.SetPosition(position);
            evidenceButton.SetSize(size.x, size.y);

            // �����ռ�״̬
            evidenceButton.SetCollected(IsEvidenceUnlocked(evidenceId));

            // ��ӵ���Ծ֤���ֵ�
            activeEvidence[evidenceId] = evidenceButton;

            return evidenceButton;
        }

        Debug.LogError($"����֤��ʧ��: {evidenceId}");
        Destroy(evidenceObj);
        return null;
    }

    /// <summary>
    /// ����֤�ﰴť - �򻯰汾��ʹ��Ĭ�ϴ�С
    /// </summary>
    public EvidenceButton CreateEvidence(string evidenceId, string name, string description, Transform parent, Vector2 position)
    {
        // ʹ��Ĭ�ϴ�С 100x100
        return CreateEvidence(evidenceId, name, description, parent, position, new Vector2(100, 100));
    }

    /// <summary>
    /// �����ض�ID��֤��
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
    /// �������л�Ծ��֤��
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
    /// ���֤���Ƿ��ѱ�����
    /// </summary>
    public bool IsEvidenceUnlocked(string evidenceId)
    {
        return unlockedEvidence.Contains(evidenceId);
    }

    /// <summary>
    /// �ֶ�����֤��
    /// </summary>
    public void UnlockEvidence(string evidenceId)
    {
        if (!unlockedEvidence.Contains(evidenceId))
        {
            unlockedEvidence.Add(evidenceId);

            // ���֤�ﵱǰ�ڳ����У�������״̬
            if (activeEvidence.TryGetValue(evidenceId, out EvidenceButton button))
            {
                button.SetCollected(true);
            }

            // �����¼�
            OnEvidenceCollected?.Invoke(evidenceId);
        }
    }

    /// <summary>
    /// ��ȡ��Ծ��֤�ﰴť
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
    /// ��Resources�ļ��м���Ԥ����
    /// </summary>
    public void LoadEvidencePrefab(string prefabPath = "Prefabs/EvidenceButton")
    {
        evidencePrefab = Resources.Load<GameObject>(prefabPath);
        if (evidencePrefab == null)
        {
            Debug.LogError($"�޷�����֤��Ԥ����: {prefabPath}");
        }
    }
}