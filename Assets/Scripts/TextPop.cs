using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class TextPop : MonoBehaviour
{
    public TextMeshProUGUI mText;
    public Color transparent;

    public void GoIn(string text)
    {
        mText.text = text;
        mText.DOColor(Color.white, 0.2f).SetEase(CardManager.instance.moveEase);
    }

    public void Fade() {
        StartCoroutine(lateFade());
    }

    IEnumerator lateFade() {
        yield return new WaitForSeconds(1f);
        mText.DOColor(transparent, 0.5f).SetEase(Ease.InQuad);
    }
}
