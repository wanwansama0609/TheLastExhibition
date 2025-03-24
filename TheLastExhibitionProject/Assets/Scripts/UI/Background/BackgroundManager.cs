using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单Canvas下的背景管理器：负责背景图切换并确保背景始终位于最底层
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    [Tooltip("所有可用的背景图片")]
    public List<Sprite> backgroundSprites = new List<Sprite>();

    [Tooltip("显示背景的Image组件")]
    public Image backgroundImage;

    [Tooltip("淡入淡出效果的持续时间（秒）")]
    public float fadeTime = 0.5f;

    private int currentBackgroundIndex = 0;
    private bool isTransitioning = false;

    private void Start()
    {
        // 确保背景图片位于Canvas层级最底部（最先渲染）
        SetAsLowestLayer();

        // 设置初始背景
        if (backgroundSprites.Count > 0 && backgroundImage != null)
        {
            backgroundImage.sprite = backgroundSprites[0];
        }
        else
        {
            Debug.LogWarning("请确保添加了背景图片并设置了Image组件！");
        }
    }

    /// <summary>
    /// 确保背景位于Canvas中的最底层
    /// </summary>
    public void SetAsLowestLayer()
    {
        if (backgroundImage != null)
        {
            // 将背景设为第一个子对象（在Canvas的层级视图中最底部）
            backgroundImage.transform.SetAsFirstSibling();
        }
    }

    /// <summary>
    /// 切换到下一个背景
    /// </summary>
    public void NextBackground()
    {
        if (isTransitioning || backgroundSprites.Count <= 1) return;

        int nextIndex = (currentBackgroundIndex + 1) % backgroundSprites.Count;
        SwitchToBackground(nextIndex);
    }

    /// <summary>
    /// 切换到上一个背景
    /// </summary>
    public void PreviousBackground()
    {
        if (isTransitioning || backgroundSprites.Count <= 1) return;

        int prevIndex = (currentBackgroundIndex - 1 + backgroundSprites.Count) % backgroundSprites.Count;
        SwitchToBackground(prevIndex);
    }

    /// <summary>
    /// 切换到指定索引的背景
    /// </summary>
    public void SwitchToBackground(int index)
    {
        if (isTransitioning || index < 0 || index >= backgroundSprites.Count || index == currentBackgroundIndex)
            return;

        StartCoroutine(TransitionBackground(index));
    }

    /// <summary>
    /// 背景切换的协程（带有淡入淡出效果）
    /// </summary>
    private IEnumerator TransitionBackground(int newIndex)
    {
        isTransitioning = true;

        // 淡出当前背景
        float elapsedTime = 0;
        Color startColor = backgroundImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsedTime < fadeTime)
        {
            backgroundImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 切换图片
        backgroundImage.sprite = backgroundSprites[newIndex];
        currentBackgroundIndex = newIndex;

        // 确保背景依然在最底层
        SetAsLowestLayer();

        // 淡入新背景
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