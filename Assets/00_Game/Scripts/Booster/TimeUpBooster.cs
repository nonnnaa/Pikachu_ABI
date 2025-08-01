using UnityEngine;

public class TimeUpBooster : BoosterBase
{
    [SerializeField] private float timeUpAmout = 10f;
   protected override void OnActive()
   {
       if (numberBooster <= 0 || !isInteractive || !BoardManager.Instance.IsActive)
       {
           Debug.Log("booster is inactive");
           return;
       }
       base.OnActive();
       TimeManager.Instance.TimeUp(timeUpAmout);
   }
}
