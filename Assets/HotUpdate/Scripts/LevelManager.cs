using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

namespace TJ.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public static int randomLevelStartIDX = 1;
        private static List<int> loadedLevels = new List<int>();

        // private void Awake()
        // {
        //     Vibration.Init();
        // }
        private void Start()
        {
#if UNITY_EDITOR

#else
        isLoaded = false;
        PreloadCurLevel();
#endif
        }

        // public static void EnterGame()
        // {
        //     Vibration.Init();
        //     LoadScene();
        // }

        public static void PreloadCurLevel()
        {
            #if UNITY_EDITOR

            #else
                 LoadLevel();
            #endif

        }

        public static void EnterMainScene()
        {
#if UNITY_EDITOR
            SceneManager.LoadScene("Map");
#else
             YooAssets.LoadSceneAsync("Map");
#endif
        }

        public static void LoadScene()
        {
            if (GetCurrentLeveLNumber() > 2)
            {
#if UNITY_EDITOR
                SceneManager.LoadScene("Map");
#else
                YooAssets.LoadSceneAsync("Map");
#endif
                //LoadLevel(1); // Map scene
            }
            else
            {
                LoadLevel(GetCurrentLeveLNumber()); // Levels
            }
        }


        // use this to get the currecnt level number, you can use this for the UI
        public static int GetCurrentLeveLNumber()
        {
            int progress = PlayerPrefs.GetInt(PlayerPrefsManager.LevelProgress, 1);
            return progress;
        }

        public static string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public static void ReloadLevel()
        {
            Debug.Log("Reloading Level");


#if UNITY_EDITOR
            SceneManager.LoadScene(GetCurrentSceneName());
#else
            SceneHandle mSceneHandle=YooAssets.LoadSceneAsync(GetCurrentSceneName());
#endif
           // SceneManager.LoadScene(GetCurrentSceneName());
            //YooAssets.LoadSceneAsync(GetCurrentSceneName());
        }

#if UNITY_EDITOR

#else
        private static int preloadedLevel = -1;
        private static SceneHandle mSceneHandle=null;
        private void OnDestroy()
        {
            mSceneHandle = null;
            preloadedLevel = -1;
        }
#endif
        public static bool  isLoaded = false;
        public static void LoadLevel(int levelIDX)
        {

#if UNITY_EDITOR
            SceneManager.LoadScene(levelIDX);
#else
            if (isLoaded)
            {
                Debug.LogError("levelIDX is Loaded");
                return;
            }
            if(preloadedLevel!=levelIDX&&mSceneHandle==null)
            {
                preloadedLevel=levelIDX;
                mSceneHandle=YooAssets.LoadSceneAsync($"Level {levelIDX}",LoadSceneMode.Single,LocalPhysicsMode.None,true);
                Debug.LogError("preloadedLevel!=levelIDX");
            }
            else
            {
                if(mSceneHandle!=null)
                {
                    if(mSceneHandle.ActivateScene()==false)
                    {
                        mSceneHandle.UnSuspend();
                        mSceneHandle = null;
                        preloadedLevel = -1;
                        isLoaded = true;
                    }
                    //mSceneHandle.WaitForAsyncComplete();
                    Debug.LogError("preloadedLevel==levelIDX");
                }
                else
                {
                    Debug.LogError("mSceneHandle is null");
                }

            }
            Debug.LogError("preloadedLevel--");
#endif
        }




        // call this methode whenver a level is won
        public static void LevelProgressed()
        {
            int progress = PlayerPrefs.GetInt(PlayerPrefsManager.LevelProgress, 1);
            progress++;

            PlayerPrefs.SetInt(PlayerPrefsManager.LevelProgress, progress);
            PlayerPrefs.SetInt(PlayerPrefsManager.giftClaimed, 0);
        }

        private static int  curTotalLevelNum=10 ;
        // call this to load the next level.
        public static void LoadLevel()
        {
            int progress = PlayerPrefs.GetInt(PlayerPrefsManager.LevelProgress, 1);
            //if (progress >= SceneManager.sceneCountInBuildSettings - 1)
            if (progress >= curTotalLevelNum)
            {
                int randomLVL = GetUniqueRandomLevel();
                LoadLevel(randomLVL);
            }
            else
            {
                LoadLevel(progress);
            }
        }

        private static int GetUniqueRandomLevel()
        {
            string loadedLevelsStr = PlayerPrefs.GetString(PlayerPrefsManager.LoadedLevels, "");
            if (!string.IsNullOrEmpty(loadedLevelsStr))
            {
                loadedLevels = new List<int>(Array.ConvertAll(loadedLevelsStr.Split(','), int.Parse));
            }

            //int levelCount = SceneManager.sceneCountInBuildSettings;
            int levelCount = curTotalLevelNum;
            List<int> availableLevels = new List<int>();

            for (int i = randomLevelStartIDX; i < levelCount; i++)
            {
                if (!loadedLevels.Contains(i))
                {
                    availableLevels.Add(i);
                }
            }

            if (availableLevels.Count == 0)
            {
                loadedLevels.Clear();
                for (int i = randomLevelStartIDX; i < levelCount; i++)
                {
                    availableLevels.Add(i);
                }
            }

            int randomIndex = UnityEngine.Random.Range(0, availableLevels.Count);
            int randomLevel = availableLevels[randomIndex];
            loadedLevels.Add(randomLevel);
            PlayerPrefs.SetString(PlayerPrefsManager.LoadedLevels, string.Join(",", loadedLevels));
            return randomLevel;
        }
    }
}