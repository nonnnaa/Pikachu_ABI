public class ShuffleBooster : BoosterBase
{
    protected override void OnActive()
    {
        base.OnActive();
        if (numberBooster > 0)
        {
            BoardManager.Instance.Shuffle();
        }
        
    }
}
