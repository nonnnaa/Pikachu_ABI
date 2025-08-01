using UnityEngine;

public class TimeUpBooster : BoosterBase
{
    [SerializeField] private float timeUpAmout = 10f;
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
           TimeManager.Instance.TimeUp(timeUpAmout);
       }
   }
}
