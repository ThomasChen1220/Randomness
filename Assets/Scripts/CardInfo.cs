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
    public CompareRes BiggerThan(CardInfo other)
    {
        if (mNum == 13 && other.mNum == 1)
            return CompareRes.Lose;
        else if (mNum == other.mNum)
            return CompareRes.Tie;
        else if (mNum > other.mNum)
            return CompareRes.Win;
        else
            return CompareRes.Lose;
    }
}
