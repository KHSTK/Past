using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIScale : MonoBehaviour
{
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float animationDuration = 0.3f;

    private Vector3 originalScale;

    void Start() => originalScale = transform.localScale;

    public void OnPointEnter() =>
        transform.DOScale(hoverScale, animationDuration).SetEase(Ease.OutBack);

    public void OnPointExit() =>
        transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutQuad);
}
