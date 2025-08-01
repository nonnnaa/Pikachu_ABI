using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuggestBooster : BoosterBase
{
    protected override void OnActive()
    {
        if (numberBooster <= 0 || !isInteractive)
        {
            Debug.Log("booster is inactive");
            return;
        }
        base.OnActive();
        if (numberBooster > 0 )
        {
            BoardManager.Instance.Suggest();
        } 
    }
}
