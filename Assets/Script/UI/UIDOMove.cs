using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class UIDOMove : MonoBehaviour
{
    [SerializeField]
    private List<RectTransform> rectTransforms;
    private bool isRecilMove = false;
    private bool RulerTagLeft = true;
    private bool GuideTagLeft = false;
    private bool CardTagLeft = false;
    private bool isAnimator = false;
    //初始位置
    public void RecilBarMove()
    {
        if (isAnimator == true) return;
        if (isRecilMove == false)
        {
            isAnimator = true;
            Debug.Log(rectTransforms[0].anchoredPosition.x);
            TagLeftMove(0, 200, 0.5f);
            isRecilMove = true;
        }
        else
        {
            isAnimator = true;
            Debug.Log(rectTransforms[0].anchoredPosition.x);
            TagRightMove(0, 200, 0.5f);
            isRecilMove = false;
        }
    }
    public void RulerTagMove()
    {
        if (RulerTagLeft == true) return;
        TagLeftMove(1, 50, 0.3f);
        if (GuideTagLeft == true)
        {
            TagRightMove(2, 50, 0.3f);
        }
        if (CardTagLeft == true)
        {
            TagRightMove(3, 50, 0.3f);
        }
        RulerTagLeft = true;
        GuideTagLeft = false;
        CardTagLeft = false;
    }
    public void GuideTagMove()
    {
        if (GuideTagLeft == true) return;
        if (RulerTagLeft == true)
        {
            TagRightMove(1, 50, 0.3f);
        }
        TagLeftMove(2, 50, 0.3f);
        if (CardTagLeft == true)
        {
            TagRightMove(3, 50, 0.3f);
        }
        RulerTagLeft = false;
        GuideTagLeft = true;
        CardTagLeft = false;
    }
    public void CardTagMove()
    {
        if (CardTagLeft == true) return;
        if (RulerTagLeft == true)
            TagRightMove(1, 50, 0.3f);
        if (GuideTagLeft == true)
            TagRightMove(2, 50, 0.3f);
        TagLeftMove(3, 50, 0.3f);
        RulerTagLeft = false;
        GuideTagLeft = false;
        CardTagLeft = true;
    }
    public void TagLeftMove(int i, int moveX, float time)
    {
        rectTransforms[i].DOAnchorPosX(rectTransforms[i].anchoredPosition.x - moveX, time).onComplete += () => isAnimator = false;
    }
    public void TagRightMove(int i, int moveX, float time)
    {
        rectTransforms[i].DOAnchorPosX(rectTransforms[i].anchoredPosition.x + moveX, time).onComplete += () => isAnimator = false;
    }
    public async Task LeftMove(RectTransform rectTransform, int moveX, float time)
    {
        await rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x - moveX, time).AsyncWaitForCompletion();
    }
    public async Task RightMove(RectTransform rectTransform, int moveX, float time)
    {
        await rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x + moveX, time).AsyncWaitForCompletion();
    }
    public async Task LeftMoveTo(RectTransform rectTransform, int moveX, float time)
    {
        await rectTransform.DOAnchorPosX(moveX, time).AsyncWaitForCompletion();
    }
    public async Task RightMoveTo(RectTransform rectTransform, int moveX, float time)
    {
        await rectTransform.DOAnchorPosX(moveX, time).AsyncWaitForCompletion();
    }
}
