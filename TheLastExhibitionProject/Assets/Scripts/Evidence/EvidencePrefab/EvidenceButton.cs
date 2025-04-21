using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ֤�ﰴť�����������ڴ����ɽ�����֤��
/// </summary>
public class EvidenceButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("֤������")]
    [SerializeField] private string evidenceId;  // ֤��Ψһ��ʶ��
    [SerializeField] private string evidenceName; // ֤������
    [SerializeField] private string evidenceDescription; // ֤������

    [Header("�α�����")]
    [SerializeField] private Texture2D interactiveCursor; // �ɽ�����꣨δ�ռ���
    [SerializeField] private Texture2D collectedCursor; // ���ռ����
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero; // ����ȵ�

    // ����Ƿ��ѱ��ռ�
    private bool isCollected = false;

    // ����¼�ί��
    public delegate void EvidenceClickedWithIdDelegate(string evidenceId);
    public static event EvidenceClickedWithIdDelegate OnEvidenceClickedWithId;

    private void Start()
    {
        // ����Ƿ��Ѿ��ռ���
        isCollected = EvidenceManager.Instance.IsEvidenceUnlocked(evidenceId);
    }

    /// <summary>
    /// ����֤��ID�������Ϣ
    /// </summary>
    public void SetEvidence(string id, string name, string description)
    {
        evidenceId = id;
        evidenceName = name;
        evidenceDescription = description;
    }

    /// <summary>
    /// ����������¼�
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {


        // ����Ѿ����ռ����ˣ����ٴ����ռ��߼�
        if (isCollected)
        {
            // ����֤�����¼���ֱ�Ӵ���ID
            OnEvidenceClickedWithId?.Invoke(evidenceId);
            return;
        }

        // ���Ϊ���ռ�����ֹ�ظ��������
        isCollected = true;

        // ����֤�����¼���ֱ�Ӵ���ID
        OnEvidenceClickedWithId?.Invoke(evidenceId);

        // ���ŵ����Ч������У�
        PlayClickSound();

        // �����ռ�֤����߼�
        CollectEvidence();
    }

    /// <summary>
    /// �������¼�
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ֻ�е����й��������Чʱ���޸Ĺ��
        if (interactiveCursor != null && collectedCursor != null)
        {
            // �����ʵ��Ĺ��
            if (isCollected)
            {
                Cursor.SetCursor(isCollected ? collectedCursor : interactiveCursor, cursorHotspot, CursorMode.Auto);
            }
        }
    }

    /// <summary>
    /// ����뿪�¼�
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // ��ԭΪϵͳĬ�Ϲ��
    }

    /// <summary>
    /// ���ŵ����Ч
    /// </summary>
    private void PlayClickSound()
    {
        // ������������Ч���Ŵ���
        // ���磺AudioManager.Instance.PlaySound("evidence_click");
    }

    /// <summary>
    /// �ռ�֤���߼�
    /// </summary>
    private void CollectEvidence()
    {
        // ����֤�������������֤��
        EvidenceManager.Instance.UnlockEvidence(evidenceId);

        // ע�⣺�������ٶ����ɶԻ�ϵͳͳһ��������
    }

    /// <summary>
    /// ���ð�ť��С
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
    /// ���ð�ťλ��
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
    /// ���֤���Ƿ��ѱ��ռ�
    /// </summary>
    public bool IsCollected()
    {
        return isCollected;
    }

    /// <summary>
    /// ��ȡ֤��ID
    /// </summary>
    public string GetEvidenceId()
    {
        return evidenceId;
    }

    /// <summary>
    /// ��ȡ֤������
    /// </summary>
    public string GetEvidenceName()
    {
        return evidenceName;
    }

    /// <summary>
    /// �����ռ�״̬
    /// </summary>
    public void SetCollected(bool collected)
    {
        isCollected = collected;
    }
}