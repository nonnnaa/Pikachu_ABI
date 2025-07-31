using System;
public class GameManager : SingletonMono<GameManager>
{
    public event Action OnStartGame; // => load vao man hinh game chua phai main menu
    public event Action OnMainMenu;
    public event Action OnLevelStart; // = Playing Game
    public event Action OnGamePause;
    public event Action OnResumeGame;
    public event Action OnLevelEnd;
    public event Action OnGameWin;
    public event Action OnGameLose;
    public event Action OnNextLevel;
    
    private void Start()
    {
        OnStartGame?.Invoke();
    }

    public void GoToMainMenu()
    {
        OnMainMenu?.Invoke();
    }
    
    public void StartLevel(int levelIndex)
    {
        LevelManager.Instance.LoadLevel(levelIndex);
        OnLevelStart?.Invoke();
    }

    public void EndLevel()
    {
        OnLevelEnd?.Invoke();
    }

    public void PauseGame()
    {
        OnGamePause?.Invoke();
    }

    public void ResumeGame()
    {
        OnResumeGame?.Invoke();
    }

    public void WinGame()
    {
        OnGameWin?.Invoke();
    }

    public void LoseGame()
    {
        OnGameLose?.Invoke();
    }

    public void LoadNextLevel()
    {
        OnNextLevel?.Invoke();
    }
}


