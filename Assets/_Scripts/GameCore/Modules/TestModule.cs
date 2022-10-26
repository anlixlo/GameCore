using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    /// <summary>
    /// Ra8bit Studio Test Module Class
    /// </summary>
    public class TestModule : BaseModule
    {
        public override void Initialize(params object[] param)
        {
            return;
        }

        public override void OnUpdate(float delta)
        {
            Debug.Log($"[TestModule:OnUpdate]");
            return;
        }

        public override void OnFixedUpdate(float delta)
        {
            Debug.Log($"[TestModule:OnFixedUpdate]");
            return;
        }

        public override void Release(params object[] param)
        {
            return;
        }

        public override void OnApplicationQuit()
        {
            return;
        }
    }
}