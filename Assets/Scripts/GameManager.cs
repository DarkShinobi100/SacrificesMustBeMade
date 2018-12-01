using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float TurnDelay = 0.1f;
    public float LevelStartDelay = 2f;
    public static GameManager Instance = null;
    public BoardManager BoardScript;
    public int PlayerFoodPoints = 100;

    [HideInInspector] public bool PlayersTurn = true;

    private Text LevelText;
    private GameObject LevelImage;
    private int Level = 1;
    private List<Enemy> enemies;
    private bool EnemiesMoving;
    private bool DoingSetUp;


	// Use this for initialization
	void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();
        BoardScript = GetComponent<BoardManager>();
        InitGame();
	}

    //This is called each time a scene is loaded.
    void OnLevelWasLoaded(int index)
    {
        //Add one to our level number.
        Level++;
        //Call InitGame to initialize our level.
        InitGame();
    }

    void InitGame()
    {
        DoingSetUp = true;

        LevelImage = GameObject.Find("LevelImage");
        LevelText = GameObject.Find("LevelText").GetComponent<Text>();

        LevelText.text = "Day " + Level;
        LevelImage.SetActive(true);
        Invoke("HideLevelImage", LevelStartDelay);


        enemies.Clear();
        BoardScript.SetUpScene(Level);
    }

    private void HideLevelImage()
    {
        LevelImage.SetActive(false);
        DoingSetUp = false;
    }

	public void GameOver()
    {
        LevelText.text = "After " + Level + " number of days. You Starved.";
        LevelImage.SetActive(true);
        enabled = false;
    }


	// Update is called once per frame
	void Update ()
    {
		if(PlayersTurn || EnemiesMoving || DoingSetUp)
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

        for (int i= 0; i< enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].MoveTime);
        }

        PlayersTurn = true;
        EnemiesMoving = false;
    }
}
