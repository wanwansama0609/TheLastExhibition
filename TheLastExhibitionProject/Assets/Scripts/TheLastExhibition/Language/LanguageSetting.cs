using UnityEngine;
/// <summary>
/// 简化的游戏设置管理类，包含语言设置
/// </summary>
public class LanguageSetting : MonoBehaviour
{
    // 单例实例
    public static LanguageSetting Instance { get; private set; }
    // 默认语言设置
    [SerializeField] private string currentLanguage = "zh";
    private void Awake()
    {
        // 单例设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 获取当前语言设置
    /// </summary>
    /// <returns>当前语言代码</returns>
    public string GetLanguage()
    {
        return currentLanguage;
    }
    /// <summary>
    /// 设置当前语言
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            Debug.LogWarning("语言代码不能为空，将使用默认值");
            return;
        }
        // 记录之前的语言，检查是否变化
        string previousLanguage = currentLanguage;
        currentLanguage = languageCode;
        if (previousLanguage != currentLanguage)
        {
            Debug.Log($"当前语言设置为: {currentLanguage}");
            // 语言变更时清除DialogueParser的缓存
            if (DialogueParser.Instance != null)
            {
                DialogueParser.Instance.ClearDialogueData();
            }
        }
    }
}