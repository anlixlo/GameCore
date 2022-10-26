using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(fileName = "NewHelpCommand", menuName = "GameCore/ConsoleModule/Commands/HelpCommand")]
    public class HelpCommand : ConsoleCommand
    {

        public override bool Process(string[] args)
        {
            Debug.Log("All commands: ");
            Debug.Log("/log");
            Debug.Log("/ls <Scene Index / Scene Name>");

            return true;
        }
    }
}