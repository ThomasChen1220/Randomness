﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public GameObject cardPrefab;
    public Transform playerDeck;
    public Transform enemyDeck;
    public Transform playerPlayed, enemyPlayed;
    public Transform usedCard;
    public float cardPosX = 7.69f;
    public Ease moveEase;
    public List<CardDisplay> playerCards, enemyCards;
    public CardDisplay playerC, enemyC;
    public bool cardRevealed = false;

    [Header("Card Anim")]
    public float hoverScale = 1.4f;
    

    GameManager gm;
    private float cardDist;
    private CardInfo.CompareRes lastRes;

    void Awake()
    {
        //If a Game Manager exists and this isn't it...
        if (instance != null && instance != this)
        {
            //...destroy this and exit. There can only be one Game Manager
            Destroy(gameObject);
            return;
        }

        //Set this as the current game manager
        instance = this;

        //Persis this object between scene reloads
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        gm = GameManager.instance;
        gm.onGameInitialized += OnGameInit;
        gm.onCardCompared += RemoveUsedCards;
        cardDist = 2 * cardPosX / 12;
    }
    private void OnDestroy()
    {
        gm.onGameInitialized -= OnGameInit;
        gm.onCardCompared -= RemoveUsedCards;
    }
    void OnGameInit() {
        float pos = -cardPosX;
        for(int i = 0; i < gm.playerCards.Count; i++)
        {
            GameObject c = Instantiate(cardPrefab, playerDeck);
            c.GetComponent<CardDisplay>().MakeCard(gm.playerCards[i]);
            c.transform.localPosition = new Vector3(pos, 0, 0);
            pos += cardDist;
            c.GetComponent<CardDisplay>().mIndex = i;
            playerCards.Add(c.GetComponent<CardDisplay>());
        }
        pos = -cardPosX;
        for (int i = 0; i < gm.enemyCards.Count; i++)
        {
            GameObject c = Instantiate(cardPrefab, enemyDeck);
            c.GetComponent<CardDisplay>().MakeCard(gm.playerCards[i], true);
            c.transform.localPosition = new Vector3(pos, 0, 0);
            pos += cardDist;
            c.GetComponent<CardDisplay>().mIndex = i;
            enemyCards.Add(c.GetComponent<CardDisplay>());
        }
    }
    public void ArrangeCard()
    {
        cardDist = 2 * cardPosX / (playerCards.Count - 1);
        float pos = -cardPosX;
        for(int i = 0; i < playerCards.Count; i++)
        {
            playerCards[i].mIndex = i;
            playerCards[i].transform.DOLocalMove(new Vector3(pos, 0, 0), 0.8f).SetDelay(0.4f).SetEase(moveEase);
            pos += cardDist;
        }

        cardDist = 2 * cardPosX / (enemyCards.Count - 1);
        pos = -cardPosX;
        for (int i = 0; i < enemyCards.Count; i++)
        {
            enemyCards[i].mIndex = i;
            enemyCards[i].transform.DOLocalMove(new Vector3(pos, 0, 0), 0.8f).SetDelay(0.4f).SetEase(moveEase);
            pos += cardDist;
        }
    }

    //play enemy's card at index in the list
    public void EnemyPlay(int index) {
        CardDisplay e = enemyCards[index];
        e.transform.localScale = new Vector3(1, 1, 1) * hoverScale;
        e.transform.DOMove(enemyPlayed.position, 0.8f).SetEase(moveEase);

        enemyCards.RemoveAt(index);
        enemyC = e;
        ArrangeCard();
    }
    public void DoCompareCards() {
        lastRes = playerC.info.BiggerThan(enemyC.info);
        enemyC.mSprite.color = Color.white;
        cardRevealed = true;
    }
    private void Update()
    {
        if (cardRevealed && Input.GetMouseButtonDown(0)) {
            cardRevealed = false;
            gm.UpdateScore(lastRes);
        }
    }
    public void RemoveUsedCards(CardInfo.CompareRes res) {
        playerC.transform.DOMove(usedCard.position, 0.8f).SetEase(moveEase);
        enemyC .transform.DOMove(usedCard.position, 0.8f).SetEase(moveEase);
        playerC.mSprite.color = Color.black;
        enemyC.mSprite.color = Color.black;

        enemyC = null;
        playerC = null;

        EnemyPlay(0);

        gm.mState = GameManager.GameState.PlayerTurn;
    }
}