/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;

/// <summary>
/// 文本本地化管理器，负责加载和管理多语言的UI文本
/// </summary>
public class TextLocalizationManager : MonoBehaviour
{
    // 单例实例
    public static TextLocalizationManager Instance { get; private set; }

    // 是否正在加载
    private bool isLoading = false;

    // 当前已加载的UI文本数据
    private Dictionary<string, string> textDatabase = new Dictionary<string, string>();

    // JSON文件的根路径
    [SerializeField]
    private string textJsonFolderPath = "Assets/Resources/Text/";

    // 实际Resources加载路径
    private string resourcesPath = "Text/";

    // 预加载状态
    [SerializeField]
    private bool preloadOnAwake = true;

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
            return;
        }

        // 如果设置为预加载，则在Awake时加载文本数据
        if (preloadOnAwake)
        {
            _ = LoadTextDataAsync();
        }
    }

    /// <summary>
    /// 获取当前语言设置
    /// </summary>
    /// <returns>当前语言代码</returns>
    public string GetCurrentLanguage()
    {
        if (LanguageSetting.Instance != null)
        {
            return LanguageSetting.Instance.GetLanguage();
        }
        return "zh"; // 如果LanguageSetting不存在，返回默认语言
    }

    /// <summary>
    /// 异步加载文本数据
    /// </summary>
    public async Task<bool> LoadTextDataAsync()
    {
        // 防止重复加载
        if (isLoading)
        {
            Debug.LogWarning("文本数据正在加载中，请等待完成");
            return false;
        }

        isLoading = true;
        textDatabase.Clear();

        // 获取当前语言
        string currentLanguage = GetCurrentLanguage();

        // 构建JSON文件的完整路径，包含语言文件夹
        string jsonFilePath = Path.Combine(textJsonFolderPath, currentLanguage, "UIText.json");
        string resourcePath = $"{resourcesPath}{currentLanguage}/UIText";

        bool success = false;

        try
        {
            string jsonText = "";

            // 检查文件是否存在
            if (File.Exists(jsonFilePath))
            {
                // 使用File.ReadAllText读取(编辑器环境)
                jsonText = await Task.Run(() => File.ReadAllText(jsonFilePath));
                success = true;
            }
            else
            {
                // 尝试从Resources加载
                TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

                if (textAsset != null)
                {
                    jsonText = textAsset.text;
                    success = true;
                }
                else
                {
                    Debug.LogWarning($"当前语言[{currentLanguage}]的UI文本文件不存在: {resourcePath}");

                    // 如果当前不是默认语言，尝试加载默认语言的文件
                    if (currentLanguage != "zh")
                    {
                        Debug.Log($"尝试加载默认语言(zh)的UI文本文件");
                        string defaultResourcePath = $"{resourcesPath}zh/UIText";
                        textAsset = Resources.Load<TextAsset>(defaultResourcePath);

                        if (textAsset == null)
                        {
                            Debug.LogError($"默认语言(zh)的UI文本文件也不存在: {defaultResourcePath}");
                        }
                        else
                        {
                            jsonText = textAsset.text;
                            success = true;
                        }
                    }
                }
            }

            // 如果成功获取到JSON文本，解析它
            if (success && !string.IsNullOrEmpty(jsonText))
            {
                success = await ParseTextJsonAsync(jsonText);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载UI文本数据出错: {e.Message}");
            success = false;
        }
        finally
        {
            isLoading = false;
        }

        if (success)
        {
            Debug.Log($"成功加载UI文本数据，语言: {currentLanguage}，总条目: {textDatabase.Count}");
        }
        else
        {
            Debug.LogError($"加载UI文本数据失败，语言: {currentLanguage}");
        }

        return success;
    }

    /// <summary>
    /// 解析文本JSON数据
    /// </summary>
    private async Task<bool> ParseTextJsonAsync(string jsonText)
    {
        bool success = false;

        await Task.Run(() => {
            try
            {
                // 使用Unity的JsonUtility解析JSON
                UITextData textData = JsonUtility.FromJson<UITextData>(jsonText);

                if (textData != null && textData.entries != null && textData.entries.Length > 0)
                {
                    // 将数组形式转换为字典形式
                    foreach (var entry in textData.entries)
                    {
                        if (string.IsNullOrEmpty(entry.key))
                        {
                            Debug.LogWarning("发现文本条目key为空");
                            continue;
                        }

                        textDatabase[entry.key] = entry.value;
                    }
                    success = true;
                }
                else
                {
                    Debug.LogError($"JSON数据格式不正确或为空");
                    success = false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JSON解析错误: {ex.Message}");
                success = false;
            }
        });

        return success;
    }

    /// <summary>
    /// 获取指定key的本地化文本
    /// </summary>
    /// <param name="key">文本的键</param>
    /// <returns>本地化后的文本，如果未找到则返回key本身</returns>
    public string GetText(string key)
    {
        // 如果文本数据库为空或正在加载，则尝试同步加载
        if ((textDatabase.Count == 0 || isLoading) && !preloadOnAwake)
        {
            LoadTextDataAsync().Wait();
        }

        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("尝试获取空的文本key");
            return string.Empty;
        }

        // 检查key是否存在
        if (textDatabase.TryGetValue(key, out string value))
        {
            return value;
        }
        else
        {
            Debug.LogWarning($"未找到文本key: {key}");
            return key; // 返回key本身作为备用显示
        }
    }

    /// <summary>
    /// 更改语言并重新加载文本数据
    /// </summary>
    /// <param name="languageCode">新的语言代码</param>
    /// <returns>是否成功</returns>
    public async Task<bool> ChangeLanguageAsync(string languageCode)
    {
        // 更新语言设置
        if (LanguageSetting.Instance != null)
        {
            LanguageSetting.Instance.SetLanguage(languageCode);
        }
        else
        {
            Debug.LogWarning("LanguageSetting实例不存在，无法设置语言");
        }

        // 重新加载文本数据
        return await LoadTextDataAsync();
    }

    /// <summary>
    /// 清除所有已加载的文本数据
    /// </summary>
    public void ClearTextData()
    {
        textDatabase.Clear();
        Debug.Log("已清除所有UI文本数据");
    }
}

/// <summary>
/// UI文本数据的JSON结构
/// </summary>
[System.Serializable]
public class UITextData
{
    public TextEntry[] entries;
}

/// <summary>
/// 文本条目
/// </summary>
[System.Serializable]
public class TextEntry
{
    public string key;
    public string value;
}*/