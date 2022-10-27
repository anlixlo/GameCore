using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class ConsoleModule : BaseModule
    {
        private string _prefix = "/";
        private IEnumerable<IConsoleCommand> _commands;

        private ConsoleGUI _consoleGUI;

        public override void Initialize(params object[] param)
        {
            SetupCommandGUI();
        }

        public void SetupCommandGUI()
        {
            if (_consoleGUI == null)
            {
                GameObject _obj = new GameObject("ConsoleGUI");
                _consoleGUI = _obj.AddComponent<ConsoleGUI>();
                _consoleGUI.processCommandCallback = ProcessCommand;
            }
        }

        public void SetCommands(string prefix, IEnumerable<IConsoleCommand> commands)
        {
            this._prefix = prefix;
            this._commands = commands;
        }

        public void SetCommands(IEnumerable<IConsoleCommand> commands)
        {
            this._commands = commands;
        }

        public void ProcessCommand(string inputValue)
        {
            if (!inputValue.StartsWith(_prefix)) { return; }

            inputValue = inputValue.Remove(0, _prefix.Length);

            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            ProcessCommand(commandInput, args);
        }

        public void ProcessCommand(string commandInput, string[] args)
        {
            foreach(var command in _commands)
            {
                if (!commandInput.Equals(command.commandWord, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (command.Process(args))
                {
                    return;
                }
            }
        }
    }
}
