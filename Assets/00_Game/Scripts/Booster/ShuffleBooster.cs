using UnityEngine;

public class ShuffleBooster : BoosterBase
{
    protected override void OnActive()
    {
        
        if (numberBooster <= 0 || !isInteractive)
        {
            Debug.Log("booster is inactive");
            return;
        }
        base.OnActive();
        if (numberBooster > 0)
        {
            Debug.Log("ShuffleBooster: OnActive");
            BoardManager.Instance.Shuffle();
        }
    }
}
