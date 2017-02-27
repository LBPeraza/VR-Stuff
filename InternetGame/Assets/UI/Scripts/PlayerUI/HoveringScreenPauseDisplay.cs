using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class HoveringScreenPauseDisplay : PauseDisplay
    {
        public GameObject PauseDisplayScreenPrefab;
        public GameObject PauseDisplayScreen;

        public float ScreenOffset = 1.0f;

        public override void LoadResources()
        {
            PauseDisplayScreenPrefab = Resources.Load<GameObject>("Prefabs/PauseDisplayScreen");
        }

        public override void ShowPause()
        {
            var head = GameManager.GetInstance().HeadCamera;
            PauseDisplayScreen = Instantiate(PauseDisplayScreenPrefab, this.transform);
            PauseDisplayScreen.transform.position = head.transform.position + (head.transform.forward * ScreenOffset);
            PauseDisplayScreen.transform.LookAt(head.transform);
        }

        public override void ShowUnpause()
        {
            Destroy(PauseDisplayScreen);
        }
    }
}

