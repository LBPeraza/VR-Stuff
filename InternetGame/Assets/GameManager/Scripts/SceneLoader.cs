using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InternetGame
{
    public class SceneLoader : MonoBehaviour, ResourceLoadable
    {
        public GameObject Camera;

        public GameObject CameraObstruction;
        public GameObject CameraObstructionPrefab;

        public float CameraObstructionWidth = 10;
        public float CameraObstructionHeight = 10;
        public float CameraObstructionOffset = 0.1f;
        public float FadeRate = 0.7f;

        public Color CameraObstructionStartColor;
        public Color CameraObstructionEndColor = Color.black;

        public bool IsLoadingLevel = false;

        public void LoadResources()
        {
            if (CameraObstructionPrefab == null)
            {
                CameraObstructionPrefab = Resources.Load<GameObject>("CameraObstruction");
            }
        }

        public void Initialize()
        {
            LoadResources();

            Camera = GameManager.GetInstance().HeadCamera;
            if (Camera == null)
            {
                throw new System.Exception("Could not find Camera object in Scene Loader");
            }

            CameraObstructionStartColor = new Color(0, 0, 0, 0);

            CreateCameraObstruction();
            StartCoroutine(GraduallyFadeFromBlack());

            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void CreateCameraObstruction()
        {
            CameraObstruction = Instantiate(CameraObstructionPrefab, Camera.transform, false);
            CameraObstruction.transform.localScale = new Vector3(
                CameraObstructionWidth, CameraObstructionHeight);
            CameraObstruction.transform.localPosition = new Vector3(0, 0, CameraObstructionOffset);
        }

        public void TransitionToScene(string sceneName)
        {
            IsLoadingLevel = true;

            StartCoroutine(GraduallyTransitionToScene(sceneName));
        }

        private IEnumerator GraduallyTransitionToScene(string sceneName)
        {
            CreateCameraObstruction();

            Material mat = CameraObstruction.GetComponent<Renderer>().material;
            mat.color = CameraObstructionStartColor;

            float t = 0.0f;
            float startTime = Time.fixedTime;

            while (t < 1.0f)
            {
                Color toSet = Color.Lerp(CameraObstructionStartColor, CameraObstructionEndColor, t);
                mat.color = toSet;

                t = Time.fixedTime - startTime;

                yield return null;
            }

            mat.color = CameraObstructionEndColor;

            // Load the scene.
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

        private IEnumerator GraduallyFadeFromBlack()
        {
            Material mat = CameraObstruction.GetComponent<Renderer>().material;
            mat.color = CameraObstructionEndColor;

            float t = 0.0f;
            float startTime = Time.fixedTime;

            while (t < 1.0f)
            {
                Color toSet = Color.Lerp(CameraObstructionEndColor, CameraObstructionStartColor, t);
                mat.color = toSet;

                t = (Time.fixedTime - startTime) * FadeRate;

                yield return null;
            }

            // Get rid of obstruction now that level is loaded.
            Destroy(CameraObstruction);

            IsLoadingLevel = false;
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }
    }

}
