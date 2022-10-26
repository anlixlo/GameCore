namespace GameCore
{
    /// <summary>
    /// Ra8bit Studio Base Module Class
    /// </summary>
    public abstract class BaseModule
    {
        public virtual void Initialize(params object[] param) {
            return;
        }

        public virtual void OnUpdate(float delta) {
            return;
        }

        public virtual void OnFixedUpdate(float delta) {
            return;
        }

        public virtual void Release(params object[] param) {
            return;
        }

        public virtual void OnApplicationQuit() {
            return;
        }
    }
}