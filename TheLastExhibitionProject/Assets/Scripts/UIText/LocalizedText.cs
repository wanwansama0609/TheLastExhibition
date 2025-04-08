using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 本地化文本组件，自动更新TMPro文本组件的内容
/// </summary>
public class LocalizedText : MonoBehaviour
{
    [Tooltip("用于查找本地化文本的键")]
    [SerializeField] private string textKey;

    [Tooltip("是否在激活时自动本地化")]
    [SerializeField] private bool localizeOnEnable = true;

    // 缓存的文本组件
    private TextMeshProUGUI tmpText;
    private Text legacyText;

    private void Awake()
    {
        // 获取文本组件
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
        {
            legacyText = GetComponent<Text>();
            if (legacyText == null)
            {
                Debug.LogWarning($"GameObject {gameObject.name} 没有找到TextMeshProUGUI或Text组件", this);
            }
        }
    }

    private void OnEnable()
    {
        if (localizeOnEnable)
        {
            Localize();
        }
    }

    /// <summary>
    /// 应用本地化文本
    /// </summary>
    public void Localize()
    {
        if (string.IsNullOrEmpty(textKey))
        {
            Debug.LogWarning($"GameObject {gameObject.name} 的LocalizedText组件没有设置textKey", this);
            return;
        }

        // 检查TextLocalizationManager是否初始化
        if (TextLocalizationManager.Instance == null)
        {
            Debug.LogWarning("TextLocalizationManager实例不存在，无法获取本地化文本");
            return;
        }

        // 获取本地化文本
        string localizedText = TextLocalizationManager.Instance.GetText(textKey);

        // 更新文本组件
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
    /// 设置文本键并立即更新
    /// </summary>
    /// <param name="key">新的文本键</param>
    public void SetTextKey(string key)
    {
        if (textKey != key)
        {
            textKey = key;
            Localize();
        }
    }

    /// <summary>
    /// 获取当前文本键
    /// </summary>
    /// <returns>当前文本键</returns>
    public string GetTextKey()
    {
        return textKey;
    }
}