using UnityEngine;
using UnityEngine.UI;

public class CanvasStartGame : UICanvas
{
    [SerializeField]
    private Button startGameButton;
    private void Start()
    {
        startGameButton.onClick.AddListener(OnclickStartGameButton);
    }

    public void OnclickStartGameButton()
    {
        GameManager.Instance.GoToMainMenu();
    }
}
