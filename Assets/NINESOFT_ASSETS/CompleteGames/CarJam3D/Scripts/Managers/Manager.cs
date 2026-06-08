using System;
using UnityEngine;
namespace NINESOFT.COMPLETEGAMES.CARJAM3D
{
    public class Manager<T> : MonoBehaviour where T : Manager<T>
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = (T)(Manager<T>)FindObjectOfType(typeof(Manager<T>)); ;

                return instance;
            }
            private set { instance = value; }
        }

        protected virtual void Awake()
        {
            Instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

    }
}