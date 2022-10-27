using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class UpdateHandler : MonoBehaviour
    {
        public delegate void UpdatePool(float delta);

        private UpdatePool _updatePool;
        private UpdatePool _fixedUpdatePool;

        public void InitializeUpdate(BaseModule module)
        {
            _updatePool += module.OnUpdate;
            _fixedUpdatePool += module.OnFixedUpdate;
        }

        public void ReleaseUpdate(BaseModule module)
        {
            _updatePool -= module.OnUpdate;
            _fixedUpdatePool -= module.OnFixedUpdate;
        }

        private void Update()
        {
            float delta = Time.deltaTime;

            _updatePool?.Invoke(delta);
        }

        private void FixedUpdate()
        {
            float delta = Time.deltaTime;

            _fixedUpdatePool?.Invoke(delta);
        }

    }
}
