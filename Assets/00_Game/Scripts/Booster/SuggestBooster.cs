using UnityEngine;

public class SuggestBooster : BoosterBase
{
    
    protected override void OnInit()
    {
        numberBooster = 99;
        UpdateTextNumberBooster(numberBooster);
        boosterImage.fillAmount = 1f;
        CoolDownBooster();
    }
    protected override void OnActive()
    {
        if (numberBooster <= 0 || !isInteractive || !BoardManager.Instance.IsActive)
        {
            Debug.Log("booster is inactive");
            return;
        }
        base.OnActive();
        BoardManager.Instance.Suggest();
        
    }
}
