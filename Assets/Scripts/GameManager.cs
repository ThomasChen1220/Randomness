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
        if (res == CardInfo.CompareRes.Win)
        {
            playerScore++;
            CardManager.DisplayMessage("You got a point");
        }
        else if (res == CardInfo.CompareRes.Lose) {
            playerScore--;
            CardManager.DisplayMessage("You lost a point");
        }
        else
        {
            CardManager.DisplayMessage("Tied");
        }
        scoreText.text = "Your Score: " + playerScore;
        onCardCompared?.Invoke(res);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int a, b, c, d;
            GetBonusPos(out a, out b, out c, out d);
        }
    }
    //return the index of first element that is smaller or equal to target from the left 
    int findGreaterOrEqual(List<int> l, int target)
    {
        for(int i = 0; i < l.Count; i++)
        {
            if (l[i] >= target) return i;
        }
        return -1;
    }
    //return the index of first element that is greater or equal to target from the right 
    int findSmallerOrEqual(List<int> l, int target)
    {
        for(int i = l.Count - 1; i >= 0; i--)
        {
            if (l[i] <= target) return i;
        }
        return -1;
    }
    void GetBonusPos(out int pi1, out int pi2, out int gk, out int go)
    {
        int index;
        List<int> emptyNum = new List<int>();
        for(int i = 0; i < 13; i++)
        {
            emptyNum.Add(i);
        }
        index = Random.Range(0, 8);
        pi1 = emptyNum[index];
        emptyNum.RemoveAt(index);

        index = Random.Range(0, findSmallerOrEqual(emptyNum, 8));
        pi2 = emptyNum[index];
        emptyNum.RemoveAt(index);

        index = Random.Range(findGreaterOrEqual(emptyNum, 1), findSmallerOrEqual(emptyNum, 4));
        gk = emptyNum[index];
        emptyNum.RemoveAt(index);

        index = Random.Range(findGreaterOrEqual(emptyNum, 9), findSmallerOrEqual(emptyNum, 12));
        go = emptyNum[index];

        //Debug.Log("Pioneer at: " + (pi1 + 1) + ", " + (pi2 + 1) + ".  " + "GK at : " + (gk + 1) + ", Gov at: " + (go + 1));
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
        int p1, p2, gk, gov;
        GetBonusPos(out p1, out p2, out gk, out gov);
        playerCards[p1].mEffect = CardInfo.CardEffect.Pioneer;
        playerCards[p2].mEffect = CardInfo.CardEffect.Pioneer;
        playerCards[gk].mEffect = CardInfo.CardEffect.GiantKiller;
        playerCards[gov].mEffect = CardInfo.CardEffect.Governor;

        GetBonusPos(out p1, out p2, out gk, out gov);
        enemyCards[p1].mEffect = CardInfo.CardEffect.Pioneer;
        enemyCards[p2].mEffect = CardInfo.CardEffect.Pioneer;
        enemyCards[gk].mEffect = CardInfo.CardEffect.GiantKiller;
        enemyCards[gov].mEffect = CardInfo.CardEffect.Governor;
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
