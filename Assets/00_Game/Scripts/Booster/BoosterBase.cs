using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BoosterBase : MonoBehaviour
{
     [SerializeField] protected int numberBooster;
     [SerializeField] protected TextMeshProUGUI numberBoosterText;
     [SerializeField] protected Button boosterButton;
     [SerializeField] protected Image boosterImage;
     [SerializeField] protected float timeLockBooster;
     [SerializeField] protected bool isInteractive;
     protected virtual void Awake()
     {
          if (boosterButton == null)
          {
               boosterButton = GetComponent<Button>();
          }
          boosterButton.onClick.AddListener(OnActive);
          GameManager.Instance.OnLevelStart += OnInit;
     }
     protected virtual void Start()
     {
          numberBooster = 99;
          UpdateTextNumberBooster(numberBooster);
          isInteractive = true;
     }

     protected virtual void OnInit()
     {
          numberBooster = 99;
          UpdateTextNumberBooster(numberBooster);
          boosterImage.fillAmount = 1f;
          isInteractive = true;
     }

     protected virtual void OnActive()
     {
          UpdateTextNumberBooster(--numberBooster);
          CoolDownBooster();
          if (numberBooster <= 0)
          {
               numberBooster = 0;
               OnDespawn();
          }
     }
     
     protected virtual void OnDespawn()
     {
          enabled = false;
          UpdateFillAmountImage(0f);
     }

     protected virtual void UpdateTextNumberBooster(int  value)
     {
          numberBoosterText.text = value.ToString();
     }

     protected virtual void CoolDownBooster()
     {
          StartCoroutine(CoolDown(timeLockBooster));
     }

     private void UpdateFillAmountImage(float value)
     {
          boosterImage.fillAmount = value;
     }

     private IEnumerator CoolDown(float time)
     {
          isInteractive = false;
          float duration = time; 
          float elapsed = 0f;
          float startValue = 0;
          float endValue = 1;
          float fillAmount;
          float t;
          while (elapsed < duration)
          {
               elapsed += Time.deltaTime;
               t = Mathf.Clamp01(elapsed / duration);
               fillAmount = Mathf.Lerp(startValue, endValue, t);
               UpdateFillAmountImage(fillAmount);
               yield return null;
          }
          UpdateFillAmountImage(1f);
          isInteractive = true;
     }
}