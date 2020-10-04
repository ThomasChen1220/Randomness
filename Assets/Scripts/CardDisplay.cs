using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardDisplay : MonoBehaviour
{
    public CardInfo info { get; private set; }
    private CardManager cm;

    public int mIndex;
    public TextMeshProUGUI number;
    public TextMeshProUGUI effect;
    public bool hidden = false;
    public SpriteRenderer mSprite;

    private void Start()
    {
        cm = CardManager.instance;
    }
    public void MakeCard(CardInfo _info, bool _hidden=false)
    {
        info = _info;
        number.text = "" + info.mNum;
        hidden = _hidden;
        if (hidden)
        {
            mSprite.color = Color.black;
        }
        if (info.mEffect == CardInfo.CardEffect.None)
        {
            effect.text = "";
        }
        else
        {
            effect.text = info.mEffect.ToString();
        }
    }
    bool CanPlay() {
        return (!hidden)&&(GameManager.instance.mState==GameManager.GameState.PlayerTurn);
    }
    private void OnMouseEnter()
    {
        if (!CanPlay()) return;
        transform.localScale = new Vector3(1, 1, 1) * cm.hoverScale;
    }
    private void OnMouseExit()
    {
        if (!CanPlay()) return;
        transform.localScale = new Vector3(1, 1, 1);
    }
    private void OnMouseDown()
    {
        if (!CanPlay()) return;
        transform.localScale = new Vector3(1, 1, 1) * cm.hoverScale;
        transform.DOMove(cm.playerPlayed.position, 0.8f).SetEase(cm.moveEase).onComplete = OnPlayed;
        GameManager.instance.mState = GameManager.GameState.RevealAnim;
        cm.playerCards.RemoveAt(mIndex);
        cm.ArrangeCard();
        cm.playerC = this;
    }
    void OnPlayed() {
        GameManager.instance.onUserPlayCard?.Invoke(mIndex);
    }
}
