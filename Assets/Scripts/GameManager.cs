using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static int CardNum=13;


    //info for game state
    public List<CardInfo> playerCards;
    public List<CardInfo> enemyCards;
    public enum GameState { PlayerTurn, RevealAnim}
    public GameState mState;
    public int playerScore = 0;
    public TextMeshProUGUI scoreText;

    //events
    public delegate void OnGameInitialized();
    public OnGameInitialized onGameInitialized;

    public delegate void OnUserPlayedCard(int cardNum);
    public OnUserPlayedCard onUserPlayCard;

    public delegate void OnCardCompared(CardInfo.CompareRes res);
    public OnCardCompared onCardCompared;

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
    public void UpdateScore(CardInfo.CompareRes res) {
        if (res == CardInfo.CompareRes.Win) playerScore++;
        else if (res == CardInfo.CompareRes.Lose) playerScore--;
        scoreText.text = "Your Score: " + playerScore;
        onCardCompared?.Invoke(res);
    }
    void InitializeGame()
    {
        playerCards = new List<CardInfo>();
        enemyCards = new List<CardInfo>();
        for(int i = 1; i <= 13; i++)
        {
            playerCards.Add(new CardInfo(i));
            enemyCards.Add(new CardInfo(i));
        }

        onGameInitialized?.Invoke();
    }
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.2f);
        //Your Function You Want to Call
        InitializeGame();
        yield return new WaitForSeconds(0.5f);
        CardManager.instance.EnemyPlay(0);
    }
    void OnPlayerPlayedCard(int index)
    {
        Debug.Log("player played" + index + " card");

        CardManager.instance.DoCompareCards();
    }
    private void Start()
    {
        StartCoroutine(LateStart());
        onUserPlayCard += OnPlayerPlayedCard;
    }
    private void OnDestroy()
    {
        onUserPlayCard -= OnPlayerPlayedCard;
    }
}
