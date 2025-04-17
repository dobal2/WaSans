    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [System.Serializable]
    public class StageData
    {
        public List<GameObject> enemies;
    }

    public class GameManager : MonoBehaviour
    {
        public List<StageData> stages;
        public Transform[] spawnPoints;

        public int currentStage = 0;
        
        private List<GameObject> spawnedEnemies = new List<GameObject>();

        public GameObject healEffect;

        public CharacterController2D player;

        public TextMeshProUGUI stageText;

        public GameObject gameOverUi;

        public GameObject desc;
        
        
        public static GameManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // 이미 존재하면 새 인스턴스 제거
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }


        private void Start()
        {
            SpawnEnemiesForStage(currentStage);
            StartCoroutine(ShowStageText());
            player = FindAnyObjectByType<CharacterController2D>();
        }
        
        private void Update()
        {
            if (spawnedEnemies.Count > 0)
            {
                bool allDead = true;

                for (int i = 0; i < spawnedEnemies.Count; i++)
                {
                    if (spawnedEnemies[i] != null)
                    {
                        allDead = false;
                        break;
                    }
                }

                if (allDead)
                {
                    NextStage();
                }
            }
        }
        
        void NextStage()
        {
            Heal();
            
            currentStage++;

            StartCoroutine(ShowStageText());

            if (currentStage < stages.Count)
            {
                SpawnEnemiesForStage(currentStage);
            }
        }

        public void ShowGameOverUI()
        {
            desc.SetActive(false);
            AudioManager.Instance.PlayBgm(AudioManager.Bgm.GameOveBGM);
            gameOverUi.SetActive(true);
        }

        public void ReStart()
        {
            desc.SetActive(true);
            gameOverUi.SetActive(false);
            player.Restart();
            AudioManager.Instance.PlayBgm(AudioManager.Bgm.FightBGM);
            StartCoroutine(ShowStageText());
        }
        

        public void Clear()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Bullet");

            foreach (var obj in objects)
            {
                Destroy(obj);
            }
            
            if (spawnedEnemies.Count > 0)
            {
                bool allDead = true;

                for (int i = 0; i < spawnedEnemies.Count; i++)
                {
                    Destroy(spawnedEnemies[i]);
                }
            }
        }

        IEnumerator ShowStageText()
        {
            if (currentStage == 3)
            {
                stageText.text = "Sans";
            }
            else
            {
                stageText.text = "Stage "+(currentStage + 1).ToString();   
            }
            stageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            stageText.gameObject.SetActive(false);
            
        }

        private void Heal()
        {
            player.life = 5;
            GameObject newHealEffect = Instantiate(healEffect, player.gameObject.transform.position, Quaternion.identity);
            newHealEffect.transform.SetParent(player.gameObject.transform);
            Destroy(newHealEffect,5);
        }

        public void GameClear()
        {

            AudioManager.Instance.PlayBgm(AudioManager.Bgm.GameClearBGM);
            SceneManager.LoadScene("Clear");
        }


        public void SpawnEnemiesForStage(int stageIndex)
        {
            if (stageIndex >= stages.Count) return;

            spawnedEnemies.Clear();

            List<GameObject> enemyPrefabs = stages[stageIndex].enemies;
            int count = Mathf.Min(enemyPrefabs.Count, spawnPoints.Length);

            for (int i = 0; i < count; i++)
            {
                Transform spawnPoint = spawnPoints[i];
                GameObject enemy = Instantiate(enemyPrefabs[i], spawnPoint.position, Quaternion.identity);
                spawnedEnemies.Add(enemy);
            }

            Debug.Log($"스테이지 {stageIndex + 1} 스폰 완료");
        }

    }


