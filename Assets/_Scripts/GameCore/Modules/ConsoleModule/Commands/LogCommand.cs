using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(fileName = "NewLogCommand", menuName = "GameCore/ConsoleModule/Commands/LogCommand")]
    public class LogCommand : ConsoleCommand
    {

        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);

            Debug.Log(logText);

            return true;
        }
    }
}