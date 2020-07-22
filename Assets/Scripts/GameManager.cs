using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	public BoardManager boardScript;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;
	public float turnDelay = .1f;
	public float levelStartDelay = 2f;

	private int level = 0;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private Text levelText;
	private GameObject levelImage;
	private bool doingSetup;

	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy>();

		boardScript = GetComponent<BoardManager>();
	}

	void InitGame()
	{
		doingSetup = true;

		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Dia " + level;
		levelImage.SetActive(true);

		Invoke("HideLevelImage", levelStartDelay);

		enemies.Clear();

		boardScript.SetupScene(level);
	}

	private void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		level++;

		InitGame();
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void Update()
	{
		if (playersTurn || enemiesMoving || doingSetup) {
			return;
		}

		StartCoroutine(MoveEnemies());
	}

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}

	public void GameOver()
	{
		levelText.text = "Despues de " + level + " Dias has muerto.";
		levelImage.SetActive(true);

		enabled = false;
	}

	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;

		yield return new WaitForSeconds(turnDelay);

		if (enemies.Count == 0) {
			yield return new WaitForSeconds(turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].MoveEnemy();

			yield return new WaitForSeconds(enemies[i].moveTime);
		}
			
		enemiesMoving = false;

		playersTurn = true;
	}

	
}
