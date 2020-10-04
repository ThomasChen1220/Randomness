using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    public TextMeshProUGUI endGameText;
    public GameObject endGameScreen;

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
    public void OnGameOver() {
        if (playerScore > 0)
        {
            endGameText.text = "You Won!";
        }
        else if(playerScore < 0)
        {
            endGameText.text = "You Lost";
        }
        else
        {
            endGameText.text = "Tied";
        }
        endGameScreen.SetActive(true);
    }
    public void StartOver() {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        
        endGameScreen.SetActive(false);
        playerScore = 0;
        mState = GameState.PlayerTurn;
        InitializeGame();
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
        for (int i = 0; i < enemyCards.Count; i++)
        {
            var temp = enemyCards[i];
            int randomIndex = Random.Range(i, enemyCards.Count);
            enemyCards[i] = enemyCards[randomIndex];
            enemyCards[randomIndex] = temp;
        }
        onGameInitialized?.Invoke();
    }
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.2f);
        //Your Function You Want to Call
        InitializeGame();
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
