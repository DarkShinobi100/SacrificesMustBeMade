﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public float TurnDelay = 0.1f;
    public float LevelStartDelay = 2f;
    public static GameManager Instance = null;
    public BoardManager BoardScript;
    public int PlayerFriendPoints = 10;
    public int PlayerStamina = 100;
    public int ExcapeCounter = 5;

    [HideInInspector] public bool PlayersTurn = true;

    private Text LevelText;
    private GameObject LevelImage;
    private int Level = 1;
    private List<Enemy> enemies;
    private bool EnemiesMoving;
    private bool DoingSetUp;
    private bool firstRun = true;

    // Use this for initialization
    void Awake()
    {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);

            enemies = new List<Enemy>();
            BoardScript = GetComponent<BoardManager>();
            InitGame();
        
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (firstRun)
        {
            firstRun = false;
            return;
        }

        Level++;
        InitGame();
    }

    void InitGame()
    {

        if (Level != 6)
        {
            DoingSetUp = true;

            LevelImage = GameObject.Find("LevelImage");
            LevelText = GameObject.Find("LevelText").GetComponent<Text>();

            LevelText.text = "Room " + Level;
            LevelImage.SetActive(true);
            Invoke("HideLevelImage", LevelStartDelay);


            enemies.Clear();
            BoardScript.SetUpScene(Level);
        }
        else
        {//delete self if level = 6
            GameManager.Destroy(this);
        }
    }

    private void HideLevelImage()
    {
        LevelImage.SetActive(false);
        DoingSetUp = false;
    }

    public void GameOver()
    {
        LevelText.text = "After " + Level + " room(s). You were caught.";
        LevelImage.SetActive(true);
        enabled = false;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (PlayersTurn || EnemiesMoving || DoingSetUp)
        {
            return;
        }
        else
        {
            StartCoroutine(MoveEnemies());
        }
    }

    public void AddEnemiesToList(Enemy Script)
    {
        enemies.Add(Script);
    }

    IEnumerator MoveEnemies()
    {
        EnemiesMoving = true;

        yield return new WaitForSeconds(TurnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(TurnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].MoveTime);
        }

        PlayersTurn = true;
        EnemiesMoving = false;
    }
}
