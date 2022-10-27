using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameCore;

public class DebugScript : MonoBehaviour
{
    [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

    // Start is called before the first frame update
    void Start()
    {
        CoreManager.Instance.InitializeModule<ConsoleModule>();
        CoreManager.Instance.GetModule<ConsoleModule>().SetCommands(commands);

        CoreManager.Instance.InitializeModule<ConfigModule>();

        //CoreManager.Instance.InitializeModule<AudioModule>();
        CoreManager.Instance.InitializeModule<FMODAudioModule>();
    }
}
