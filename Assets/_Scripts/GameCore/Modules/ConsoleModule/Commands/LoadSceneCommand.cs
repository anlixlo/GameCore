using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCore
{
    [CreateAssetMenu(fileName = "NewLoadSceneCommand", menuName = "GameCore/ConsoleModule/Commands/LoadSceneCommand")]
    public class LoadSceneCommand : ConsoleCommand
    {

        public override bool Process(string[] args)
        {
            if(args.Length == 0)
            {
                ShowAllSceneName();
                return true;
            }

            int _sceneIndex;

            if (Int32.TryParse(args[0], out _sceneIndex))
            {
                SceneManager.LoadSceneAsync(_sceneIndex);
            }
            else
            {
                SceneManager.LoadSceneAsync(args[0]);
            }

            return true;
        }

        public void ShowAllSceneName()
        {
            var _sceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < _sceneCount; i++)
            {
                Debug.Log("[ " + i + " ] " + Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
            }
        }
    }
}