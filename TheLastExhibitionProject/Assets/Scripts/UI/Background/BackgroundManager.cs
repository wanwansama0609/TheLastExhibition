using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��Canvas�µı��������������𱳾�ͼ�л���ȷ������ʼ��λ����ײ�
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    [Tooltip("���п��õı���ͼƬ")]
    public List<Sprite> backgroundSprites = new List<Sprite>();

    [Tooltip("��ʾ������Image���")]
    public Image backgroundImage;

    [Tooltip("���뵭��Ч���ĳ���ʱ�䣨�룩")]
    public float fadeTime = 0.5f;

    private int currentBackgroundIndex = 0;
    private bool isTransitioning = false;

    private void Start()
    {
        // ȷ������ͼƬλ��Canvas�㼶��ײ���������Ⱦ��
        SetAsLowestLayer();

        // ���ó�ʼ����
        if (backgroundSprites.Count > 0 && backgroundImage != null)
        {
            backgroundImage.sprite = backgroundSprites[0];
        }
        else
        {
            Debug.LogWarning("��ȷ������˱���ͼƬ��������Image�����");
        }
    }

    /// <summary>
    /// ȷ������λ��Canvas�е���ײ�
    /// </summary>
    public void SetAsLowestLayer()
    {
        if (backgroundImage != null)
        {
            // ��������Ϊ��һ���Ӷ�����Canvas�Ĳ㼶��ͼ����ײ���
            backgroundImage.transform.SetAsFirstSibling();
        }
    }

    /// <summary>
    /// �л�����һ������
    /// </summary>
    public void NextBackground()
    {
        if (isTransitioning || backgroundSprites.Count <= 1) return;

        int nextIndex = (currentBackgroundIndex + 1) % backgroundSprites.Count;
        SwitchToBackground(nextIndex);
    }

    /// <summary>
    /// �л�����һ������
    /// </summary>
    public void PreviousBackground()
    {
        if (isTransitioning || backgroundSprites.Count <= 1) return;

        int prevIndex = (currentBackgroundIndex - 1 + backgroundSprites.Count) % backgroundSprites.Count;
        SwitchToBackground(prevIndex);
    }

    /// <summary>
    /// �л���ָ�������ı���
    /// </summary>
    public void SwitchToBackground(int index)
    {
        if (isTransitioning || index < 0 || index >= backgroundSprites.Count || index == currentBackgroundIndex)
            return;

        StartCoroutine(TransitionBackground(index));
    }

    /// <summary>
    /// �����л���Э�̣����е��뵭��Ч����
    /// </summary>
    private IEnumerator TransitionBackground(int newIndex)
    {
        isTransitioning = true;

        // ������ǰ����
        float elapsedTime = 0;
        Color startColor = backgroundImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsedTime < fadeTime)
        {
            backgroundImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �л�ͼƬ
        backgroundImage.sprite = backgroundSprites[newIndex];
        currentBackgroundIndex = newIndex;

        // ȷ��������Ȼ����ײ�
        SetAsLowestLayer();

        // �����±���
        elapsedTime = 0;
        startColor = targetColor;
        targetColor = new Color(startColor.r, startColor.g, startColor.b, 1);

        while (elapsedTime < fadeTime)
        {
            backgroundImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        backgroundImage.color = targetColor;
        isTransitioning = false;
    }
}