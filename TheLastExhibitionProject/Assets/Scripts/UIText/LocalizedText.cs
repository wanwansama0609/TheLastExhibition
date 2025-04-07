/*Susing System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// ���ػ��ı�������Զ�����TMPro�ı����������
/// </summary>
public class LocalizedText : MonoBehaviour
{
    [Tooltip("���ڲ��ұ��ػ��ı��ļ�")]
    [SerializeField] private string textKey;

    [Tooltip("�Ƿ��ڼ���ʱ�Զ����ػ�")]
    [SerializeField] private bool localizeOnEnable = true;

    [Tooltip("�Ƿ�������Ը����¼�")]
    [SerializeField] private bool listenToLanguageChanges = true;

    // ������ı����
    private TextMeshProUGUI tmpText;
    private Text legacyText;

    private void Awake()
    {
        // ��ȡ�ı����
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
        {
            legacyText = GetComponent<Text>();
            if (legacyText == null)
            {
                Debug.LogWarning($"GameObject {gameObject.name} û���ҵ�TextMeshProUGUI��Text���", this);
            }
        }
    }

    private void OnEnable()
    {
        if (localizeOnEnable)
        {
            Localize();
        }

        // ע�����Ը����¼�
        if (listenToLanguageChanges && LanguageSetting.Instance != null)
        {
            LanguageSetting.Instance.OnLanguageChanged += OnLanguageChanged;
        }
    }

    private void OnDisable()
    {
        // ȡ��ע�����Ը����¼�
        if (listenToLanguageChanges && LanguageSetting.Instance != null)
        {
            LanguageSetting.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }

    /// <summary>
    /// �����Ը���ʱ����
    /// </summary>
    private void OnLanguageChanged(string newLanguage)
    {
        Localize();
    }

    /// <summary>
    /// Ӧ�ñ��ػ��ı�
    /// </summary>
    public void Localize()
    {
        if (string.IsNullOrEmpty(textKey))
        {
            Debug.LogWarning($"GameObject {gameObject.name} ��LocalizedText���û������textKey", this);
            return;
        }

        // ��ȡ���ػ��ı�
        string localizedText = TextLocalizationManager.Instance?.GetText(textKey) ?? textKey;

        // �����ı����
        if (tmpText != null)
        {
            tmpText.text = localizedText;
        }
        else if (legacyText != null)
        {
            legacyText.text = localizedText;
        }
    }

    /// <summary>
    /// �����ı�������������
    /// </summary>
    /// <param name="key">�µ��ı���</param>
    public void SetTextKey(string key)
    {
        if (textKey != key)
        {
            textKey = key;
            Localize();
        }
    }

    /// <summary>
    /// ��ȡ��ǰ�ı���
    /// </summary>
    /// <returns>��ǰ�ı���</returns>
    public string GetTextKey()
    {
        return textKey;
    }
}*/