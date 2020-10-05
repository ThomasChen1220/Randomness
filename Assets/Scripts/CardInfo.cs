using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo
{
    public enum CompareRes { Win, Tie, Lose }
    public int mNum;

    public enum CardEffect { None, Pioneer, GiantKiller, Governor }
    public CardEffect mEffect;

    public CardInfo(int num)
    {
        mNum = num;
    }
    public CompareRes DoCompare(CardInfo other)
    {
        CompareRes res;
        if(mEffect == CardEffect.GiantKiller && other.mNum >= 11)
        {
            res = CompareRes.Win;
            CardManager.DisplayMessage("Giant Killer Effective");
        }
        else if(other.mEffect == CardEffect.GiantKiller && mNum >= 11)
        {
            res = CompareRes.Lose;
            CardManager.DisplayMessage("Giant Killer Effective");
        }
        else if (mNum == 13 && other.mNum == 1)
            res = CompareRes.Lose;
        else if (mNum == other.mNum)
            res = CompareRes.Tie;
        else if (mNum > other.mNum)
            res = CompareRes.Win;
        else
            res = CompareRes.Lose;

        if(mEffect==CardEffect.Governor && res == CompareRes.Lose)
        {
            res = CompareRes.Tie;
            CardManager.DisplayMessage("Governor Effective");
        }
        if (other.mEffect == CardEffect.Governor && res == CompareRes.Win)
        {
            res = CompareRes.Tie;
            CardManager.DisplayMessage("Governor Effective");
        }
        return res;
    }
}
