using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Line : MonoBehaviour
{
    [Header("浮现动画")]
    public float appearDuration = 0.3f;
    public Ease appearEase = Ease.OutQuad;

    [Header("材质偏移设置")]
    public float horizontalOffsetSpeed = 0.1f;
    public float verticalOffsetSpeed = 0f;
    public bool autoTile = true;

    [Header("宽度动画")]
    public float maxWidth = 0.05f; // 线段最大宽度
    public bool animateWidth = true; // 是否启用宽度动画

    private LineRenderer lineRenderer;
    private Vector2 initialOffset;
    private float originalStartWidth;
    private float originalEndWidth;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        initialOffset = lineRenderer.material.mainTextureOffset;

        // 保存原始宽度
        originalStartWidth = lineRenderer.startWidth;
        originalEndWidth = lineRenderer.endWidth;

        // 初始宽度设为0
        if (animateWidth)
        {
            lineRenderer.startWidth = 0;
            lineRenderer.endWidth = 0;
        }

        if (autoTile)
            AdjustTextureTiling();
    }

    public void PlayLineReveal()
    {
        gameObject.SetActive(true);

        if (animateWidth)
        {
            // 使用DOTween动画宽度
            DOTween.To(
                () => lineRenderer.startWidth,
                width =>
                {
                    lineRenderer.startWidth = width;
                    lineRenderer.endWidth = width;
                },
                originalStartWidth,
                appearDuration
            ).SetEase(appearEase);
        }
        else
        {
            // 直接设置最终宽度
            lineRenderer.startWidth = originalStartWidth;
            lineRenderer.endWidth = originalEndWidth;
        }
        // 可选：同时播放材质偏移动画
        PlayTextureOffsetAnimation();
    }

    private void PlayTextureOffsetAnimation()
    {
        // 创建材质偏移动画
        float targetOffsetX = initialOffset.x + 1f; // 偏移一个完整周期
        DOTween.To(
            () => lineRenderer.material.mainTextureOffset.x,
            x =>
            {
                Vector2 offset = lineRenderer.material.mainTextureOffset;
                offset.x = x;
                lineRenderer.material.mainTextureOffset = offset;
            },
            targetOffsetX,
            appearDuration * 2f
        ).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    private void AdjustTextureTiling()
    {
        float totalLength = CalculateTotalLength();
        lineRenderer.material.mainTextureScale = new Vector2(
            totalLength * 0.5f,
            lineRenderer.material.mainTextureScale.y
        );
    }

    private float CalculateTotalLength()
    {
        float length = 0;
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            length += Vector3.Distance(
                lineRenderer.GetPosition(i - 1),
                lineRenderer.GetPosition(i)
            );
        }
        return length;
    }

    private void Update()
    {
        // 基础偏移（如果不需要额外动画可以移除）
        Vector2 offset = lineRenderer.material.mainTextureOffset;
        offset.x += horizontalOffsetSpeed * Time.deltaTime;
        offset.y += verticalOffsetSpeed * Time.deltaTime;
        lineRenderer.material.mainTextureOffset = offset;
    }
}