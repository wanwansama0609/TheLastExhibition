using UnityEngine;
/// <summary>
/// �򻯵���Ϸ���ù����࣬������������
/// </summary>
public class LanguageSetting : MonoBehaviour
{
    // ����ʵ��
    public static LanguageSetting Instance { get; private set; }
    // Ĭ����������
    [SerializeField] private string currentLanguage = "zh";
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
        }
    }
    /// <summary>
    /// ��ȡ��ǰ��������
    /// </summary>
    /// <returns>��ǰ���Դ���</returns>
    public string GetLanguage()
    {
        return currentLanguage;
    }
    /// <summary>
    /// ���õ�ǰ����
    /// </summary>
    /// <param name="languageCode">���Դ���</param>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            Debug.LogWarning("���Դ��벻��Ϊ�գ���ʹ��Ĭ��ֵ");
            return;
        }
        // ��¼֮ǰ�����ԣ�����Ƿ�仯
        string previousLanguage = currentLanguage;
        currentLanguage = languageCode;
        if (previousLanguage != currentLanguage)
        {
            Debug.Log($"��ǰ��������Ϊ: {currentLanguage}");
            // ���Ա��ʱ���DialogueParser�Ļ���
            if (DialogueParser.Instance != null)
            {
                DialogueParser.Instance.ClearDialogueData();
            }
        }
    }
}