using UnityEngine;
using UnityEngine.UI;

public class ShuffleBooster : BoosterBase
{
    protected override void OnActive()
    {
        if (numberBooster <= 0 || !isInteractive || !BoardManager.Instance.IsActive)
        {
            Debug.Log("booster is inactive");
            return;
        }
        base.OnActive();
        //Debug.Log("ShuffleBooster: OnActive");
        BoardManager.Instance.Shuffle();
    }
}
