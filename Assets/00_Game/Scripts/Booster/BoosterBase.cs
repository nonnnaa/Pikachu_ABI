using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BoosterBase : MonoBehaviour
{
     [SerializeField] protected int numberBooster;
     [SerializeField] protected TextMeshProUGUI numberBoosterText;
     [SerializeField] public Button boosterButton;
     protected virtual void Awake()
     {
          if (boosterButton == null)
          {
               boosterButton = GetComponent<Button>();
          }
          GameManager.Instance.OnLevelStart += OnInit;
     }
     protected virtual void Start()
     {
          boosterButton.onClick.AddListener(OnActive);
          numberBooster = 99;
          UpdateTextNumberBooster(numberBooster);
     }

     protected virtual void OnInit()
     {
          numberBooster = 99;
          UpdateTextNumberBooster(numberBooster);
     }

     protected virtual void OnActive()
     {
          if (numberBooster <= 0) return;
          UpdateTextNumberBooster(--numberBooster);
          if (numberBooster <= 0)
          {
               numberBooster = 0;
               OnDespawn();
          }
     }
     
     protected virtual void OnDespawn()
     {
          enabled = false;
     }

     protected virtual void UpdateTextNumberBooster(int  value)
     {
          numberBoosterText.text = value.ToString();
     }
}