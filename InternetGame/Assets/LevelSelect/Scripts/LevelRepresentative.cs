using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InternetGame
{
    public class LevelRepresentative : Interactable
    {
        public string SceneName;

        private float DepressedDepth = -.1f;
        private float DepressStep = -.001f;

        private delegate void AnimateFinishedHandler();

        private IEnumerator Animate(AnimateFinishedHandler onDone)
        {
            var accumulator = 0.0f;
            while (accumulator > DepressedDepth)
            {
                this.transform.position = this.transform.position - new Vector3(0, 0, DepressStep);
                accumulator += DepressStep;

                yield return null;
            }

            onDone();
        }

        private void OnSelected()
        {
            var animateCoroutine = Animate(LoadLevel /* On Done Callback */);
            StartCoroutine(animateCoroutine);
        }

        private void LoadLevel()
        {
            if (SceneName != "")
            {
                SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            }
        }

        public override CursorEventArgs OnClick()
        {
            OnSelected();
            return cursorEventArgs;
        }
    }
}

