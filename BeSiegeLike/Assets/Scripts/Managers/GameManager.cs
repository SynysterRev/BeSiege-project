using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public delegate void DelegateLevelFinished(TypeMessage _type);
    public event DelegateLevelFinished OnEndLevel;

    public delegate void DelegateBackMenu();
    public event DelegateBackMenu OnReturnMenu;
    public event DelegateBackMenu OnReset;


    public delegate void DelegatePause(bool _pause);
    public event DelegatePause OnPause;
    public enum Objectives
    {
        point,
        destruction
    }
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }

    }

    BuilderManager bm;
    bool isObjectiveDone = false;
    bool isPaused = false;
    string currentLevelName = "";
    bool isInMenu = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        bm = BuilderManager.Instance;
        if (bm != null)
            isInMenu = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isInMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (!isInMenu && bm == null)
        {
            bm = BuilderManager.Instance;
        }
        if (!isObjectiveDone && 
            !isInMenu && bm != null && bm.IsGameStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                Time.timeScale = 0.0f;
                OnPause(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                OnPause(false);
            }
        }
    }

    public void LevelFinished()
    {
        OnEndLevel(TypeMessage.levelDone);
        isObjectiveDone = true;
        OnPause(true);
    }

    public void GoBackMenu()
    {
        isInMenu = true;
        Time.timeScale = 1.0f;
        OnPause(false);
        isPaused = false;
        OnReturnMenu();
        SceneManager.LoadScene("MenuScene");
    }

    public void ResetLevel()
    {
        if(currentLevelName == "")
            currentLevelName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1.0f;
        OnPause(false);
        bm.isReset = true;
        isPaused = false;
        isObjectiveDone = false;
        SceneManager.LoadScene(currentLevelName);
    }

    public void SelectLevel(string _name)
    {
        currentLevelName = _name;
        isInMenu = false;
    }
}
