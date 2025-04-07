/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;

/// <summary>
/// �ı����ػ���������������غ͹�������Ե�UI�ı�
/// </summary>
public class TextLocalizationManager : MonoBehaviour
{
    // ����ʵ��
    public static TextLocalizationManager Instance { get; private set; }

    // �Ƿ����ڼ���
    private bool isLoading = false;

    // ��ǰ�Ѽ��ص�UI�ı�����
    private Dictionary<string, string> textDatabase = new Dictionary<string, string>();

    // JSON�ļ��ĸ�·��
    [SerializeField]
    private string textJsonFolderPath = "Assets/Resources/Text/";

    // ʵ��Resources����·��
    private string resourcesPath = "Text/";

    // Ԥ����״̬
    [SerializeField]
    private bool preloadOnAwake = true;

    private void Awake()
    {
        // ��������
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

        // �������ΪԤ���أ�����Awakeʱ�����ı�����
        if (preloadOnAwake)
        {
            _ = LoadTextDataAsync();
        }
    }

    /// <summary>
    /// ��ȡ��ǰ��������
    /// </summary>
    /// <returns>��ǰ���Դ���</returns>
    public string GetCurrentLanguage()
    {
        if (LanguageSetting.Instance != null)
        {
            return LanguageSetting.Instance.GetLanguage();
        }
        return "zh"; // ���LanguageSetting�����ڣ�����Ĭ������
    }

    /// <summary>
    /// �첽�����ı�����
    /// </summary>
    public async Task<bool> LoadTextDataAsync()
    {
        // ��ֹ�ظ�����
        if (isLoading)
        {
            Debug.LogWarning("�ı��������ڼ����У���ȴ����");
            return false;
        }

        isLoading = true;
        textDatabase.Clear();

        // ��ȡ��ǰ����
        string currentLanguage = GetCurrentLanguage();

        // ����JSON�ļ�������·�������������ļ���
        string jsonFilePath = Path.Combine(textJsonFolderPath, currentLanguage, "UIText.json");
        string resourcePath = $"{resourcesPath}{currentLanguage}/UIText";

        bool success = false;

        try
        {
            string jsonText = "";

            // ����ļ��Ƿ����
            if (File.Exists(jsonFilePath))
            {
                // ʹ��File.ReadAllText��ȡ(�༭������)
                jsonText = await Task.Run(() => File.ReadAllText(jsonFilePath));
                success = true;
            }
            else
            {
                // ���Դ�Resources����
                TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

                if (textAsset != null)
                {
                    jsonText = textAsset.text;
                    success = true;
                }
                else
                {
                    Debug.LogWarning($"��ǰ����[{currentLanguage}]��UI�ı��ļ�������: {resourcePath}");

                    // �����ǰ����Ĭ�����ԣ����Լ���Ĭ�����Ե��ļ�
                    if (currentLanguage != "zh")
                    {
                        Debug.Log($"���Լ���Ĭ������(zh)��UI�ı��ļ�");
                        string defaultResourcePath = $"{resourcesPath}zh/UIText";
                        textAsset = Resources.Load<TextAsset>(defaultResourcePath);

                        if (textAsset == null)
                        {
                            Debug.LogError($"Ĭ������(zh)��UI�ı��ļ�Ҳ������: {defaultResourcePath}");
                        }
                        else
                        {
                            jsonText = textAsset.text;
                            success = true;
                        }
                    }
                }
            }

            // ����ɹ���ȡ��JSON�ı���������
            if (success && !string.IsNullOrEmpty(jsonText))
            {
                success = await ParseTextJsonAsync(jsonText);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"����UI�ı����ݳ���: {e.Message}");
            success = false;
        }
        finally
        {
            isLoading = false;
        }

        if (success)
        {
            Debug.Log($"�ɹ�����UI�ı����ݣ�����: {currentLanguage}������Ŀ: {textDatabase.Count}");
        }
        else
        {
            Debug.LogError($"����UI�ı�����ʧ�ܣ�����: {currentLanguage}");
        }

        return success;
    }

    /// <summary>
    /// �����ı�JSON����
    /// </summary>
    private async Task<bool> ParseTextJsonAsync(string jsonText)
    {
        bool success = false;

        await Task.Run(() => {
            try
            {
                // ʹ��Unity��JsonUtility����JSON
                UITextData textData = JsonUtility.FromJson<UITextData>(jsonText);

                if (textData != null && textData.entries != null && textData.entries.Length > 0)
                {
                    // ��������ʽת��Ϊ�ֵ���ʽ
                    foreach (var entry in textData.entries)
                    {
                        if (string.IsNullOrEmpty(entry.key))
                        {
                            Debug.LogWarning("�����ı���ĿkeyΪ��");
                            continue;
                        }

                        textDatabase[entry.key] = entry.value;
                    }
                    success = true;
                }
                else
                {
                    Debug.LogError($"JSON���ݸ�ʽ����ȷ��Ϊ��");
                    success = false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JSON��������: {ex.Message}");
                success = false;
            }
        });

        return success;
    }

    /// <summary>
    /// ��ȡָ��key�ı��ػ��ı�
    /// </summary>
    /// <param name="key">�ı��ļ�</param>
    /// <returns>���ػ�����ı������δ�ҵ��򷵻�key����</returns>
    public string GetText(string key)
    {
        // ����ı����ݿ�Ϊ�ջ����ڼ��أ�����ͬ������
        if ((textDatabase.Count == 0 || isLoading) && !preloadOnAwake)
        {
            LoadTextDataAsync().Wait();
        }

        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("���Ի�ȡ�յ��ı�key");
            return string.Empty;
        }

        // ���key�Ƿ����
        if (textDatabase.TryGetValue(key, out string value))
        {
            return value;
        }
        else
        {
            Debug.LogWarning($"δ�ҵ��ı�key: {key}");
            return key; // ����key������Ϊ������ʾ
        }
    }

    /// <summary>
    /// �������Բ����¼����ı�����
    /// </summary>
    /// <param name="languageCode">�µ����Դ���</param>
    /// <returns>�Ƿ�ɹ�</returns>
    public async Task<bool> ChangeLanguageAsync(string languageCode)
    {
        // ������������
        if (LanguageSetting.Instance != null)
        {
            LanguageSetting.Instance.SetLanguage(languageCode);
        }
        else
        {
            Debug.LogWarning("LanguageSettingʵ�������ڣ��޷���������");
        }

        // ���¼����ı�����
        return await LoadTextDataAsync();
    }

    /// <summary>
    /// ��������Ѽ��ص��ı�����
    /// </summary>
    public void ClearTextData()
    {
        textDatabase.Clear();
        Debug.Log("���������UI�ı�����");
    }
}

/// <summary>
/// UI�ı����ݵ�JSON�ṹ
/// </summary>
[System.Serializable]
public class UITextData
{
    public TextEntry[] entries;
}

/// <summary>
/// �ı���Ŀ
/// </summary>
[System.Serializable]
public class TextEntry
{
    public string key;
    public string value;
}*/