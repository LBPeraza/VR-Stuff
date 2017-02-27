using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class PauseDisplay : MonoBehaviour, ResourceLoadable
    {
        public virtual void Initialize()
        {
            LoadResources();
        }
        public abstract void LoadResources();
        public abstract void ShowPause();
        public abstract void ShowUnpause();
    }

}
