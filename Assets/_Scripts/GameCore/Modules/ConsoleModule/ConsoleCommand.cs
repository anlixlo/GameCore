using UnityEngine;

namespace GameCore
{
    public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string _commandWord = string.Empty;

        public string commandWord => _commandWord;

        public abstract bool Process(string[] args);
    }
}