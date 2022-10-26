using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
    #region Singleton Code

    protected static T sInstance = null;

    public static T Instance
    {
        get
        {
            if (!sInstance)
            {
                T[] _instanceArray = Resources.FindObjectsOfTypeAll<T>();
                if (_instanceArray.Length > 0)
                {
                    sInstance = _instanceArray[0];
                }
            }

            if (sInstance == null)
            {
                sInstance = ScriptableObject.CreateInstance<T>();
                sInstance.hideFlags = HideFlags.HideAndDontSave;
            }

            return sInstance;
        }
    }

    private void OnDestroy()
    {
        Debug.Log("destroyed");
        sInstance = null;
        Destroy(this);
    }

    #endregion
}
