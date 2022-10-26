using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class ConsoleModule : BaseModule
    {
        private string prefix = "/";
        private IEnumerable<IConsoleCommand> commands;

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
            this.prefix = prefix;
            this.commands = commands;
        }

        public void SetCommands(IEnumerable<IConsoleCommand> commands)
        {
            this.commands = commands;
        }

        public void ProcessCommand(string inputValue)
        {
            if (!inputValue.StartsWith(prefix)) { return; }

            inputValue = inputValue.Remove(0, prefix.Length);

            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            ProcessCommand(commandInput, args);
        }

        public void ProcessCommand(string commandInput, string[] args)
        {
            foreach(var command in commands)
            {
                if (!commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
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
