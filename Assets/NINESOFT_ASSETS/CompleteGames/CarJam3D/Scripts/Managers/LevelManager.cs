using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class LevelManager : Manager<LevelManager>
    {
        public LevelProperties CurrentLevel
        {
            get
            {
                if (_currentLevel == null)
                {
                    var levelData = Resources.LoadAll<LevelProperties>("LevelData");
                    MaxLevel = levelData.Length;

                    if (CurrentLevelIndex <= MaxLevel)
                    {
                        _currentLevel = Resources.Load<LevelProperties>("LevelData/Level " + loadLastIndex());
                    }
                    else
                    {
                        int levelIndex = (CurrentLevelIndex % MaxLevel) + 1;
                        if (levelIndex == 1) levelIndex = 2;

                        _currentLevel = Resources.Load<LevelProperties>("LevelData/Level " + levelIndex);
                    }
                }
                return _currentLevel;
            }
        }
        LevelProperties _currentLevel;


        private new void Awake()
        {
            base.Awake();

            Instantiate(CurrentLevel);

        }
        public int CurrentLevelIndex { get => PlayerPrefs.GetInt("SceneSaved"); set => PlayerPrefs.SetInt("SceneSaved", value); }
        private int MaxLevel;

        public static void save()
        {
            PlayerPrefs.SetInt("SceneSaved", loadLastIndex() + 1);
        }

        public static void load()
        {
            SceneManager.LoadScene("Game");
        }

        public static int loadLastIndex()
        {
            return PlayerPrefs.GetInt("SceneSaved", 1);
        }

        public void LoadNextScene()
        {
            OpenMenu = false;

            save();
            StartCoroutine(LoadAsyncScene());
        }
        public bool OpenMenu { get => (PlayerPrefs.GetInt("menu") == 1); set => PlayerPrefs.SetInt("menu", value ? 1 : 0); }
        public void RestartScene()
        {
            OpenMenu = false;
            StartCoroutine(LoadAsyncScene());
        }
        public void GoToMenu()
        {
            OpenMenu = true;
            StartCoroutine(LoadAsyncScene());
        }

        private void OnApplicationQuit()
        {
            OpenMenu = true;
        }


        IEnumerator LoadAsyncScene()
        {
            UIManager.Instance.ShowLoading(true);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            UIManager.Instance.ShowLoading(false);
        }
    }
}