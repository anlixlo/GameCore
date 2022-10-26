using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCore
{
    [CreateAssetMenu(fileName = "NewLoadSceneCommand", menuName = "GameCore/ConsoleModule/Commands/LoadSceneCommand")]
    public class LoadSceneCommand : ConsoleCommand
    {

        public override bool Process(string[] args)
        {
            int _sceneIndex;

            if (Int32.TryParse(args[0], out _sceneIndex))
            {
                SceneManager.LoadScene(_sceneIndex);
            }
            else
            {
                SceneManager.LoadScene(args[0]);
            }

            return true;
        }
    }
}