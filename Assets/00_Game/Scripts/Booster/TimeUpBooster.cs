using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUpBooster : BoosterBase
{
    [SerializeField] private float timeUpAmout = 10f;
   protected override void Start()
   {
       base.Start();
   }

   protected override void OnInit()
   {
       base.OnInit();
   }

   protected override void OnActive()
   {
       if (numberBooster > 0)
       {
           TimeManager.Instance.TimeUp(timeUpAmout);
       }
       base.OnActive();
       
   }
     
   protected override void OnDespawn()
   {
       base.OnDespawn();
   }
}
