using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuggestBooster : BoosterBase
{
    private bool isInteract;

    protected override void Start()
    {
        base.Start();
        isInteract = true;
    }

    protected override void OnActive()
    {
        if (isInteract)
        {
            base.OnActive();
            if (numberBooster > 0)
            {
                BoardManager.Instance.Suggest();
            }
            ChangeInteractive();
            Invoke(nameof(ChangeInteractive), 0.5f);
        }
    }

    void ChangeInteractive()
    {
        isInteract = !isInteract;
    }
}
