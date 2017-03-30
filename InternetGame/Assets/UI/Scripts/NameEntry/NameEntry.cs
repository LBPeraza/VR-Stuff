using CurvedVRKeyboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternetGame
{
    public class NameEntry : MonoBehaviour
    {
        public Text NameField;
        public VRTK.VRTK_Button SubmitButton;
        public VRTK.VRTK_Button SkipButton;

        public int MaxNameLength = 3;

        protected InternetGame.KeyboardRaycaster KeyboardRaycaster;
        protected KeyboardCreator KeyboardCreator;
        protected KeyboardStatus KeyboardStatus;

        protected bool shown = false;

        public void Initialize()
        {
            KeyboardStatus = gameObject.GetComponentInChildren<KeyboardStatus>();
            KeyboardRaycaster = gameObject.GetComponentInChildren<InternetGame.KeyboardRaycaster>();
            KeyboardCreator = gameObject.GetComponentInChildren<KeyboardCreator>();

            KeyboardCreator.Initialize();
            KeyboardCreator.RaycastingSource = GameManager.GetInstance().Player.PrimaryCursor.transform;

            KeyboardRaycaster.Initialize();

            KeyboardStatus.maxOutputLength = MaxNameLength;
            
            SubmitButton.Pushed += OnSubmit;
            SkipButton.Pushed += OnSkip;

            shown = false;
            HideSelf();
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
            shown = true;
        }

        protected void HideSelf()
        {
            this.gameObject.SetActive(false);
        }

        protected void HideButtons()
        {
            SkipButton.gameObject.SetActive(false);
            SubmitButton.gameObject.SetActive(false);
        }

        private void OnSkip(object sender, VRTK.Control3DEventArgs e)
        {
            HideSelf();
        }

        private void OnSubmit(object sender, VRTK.Control3DEventArgs e)
        {
            var name = NameField.text;
            if (name.Length > 0 && name.Length <= MaxNameLength)
            {
                // Save score to file.
                var score = GameManager.GetInstance().Score.PacketsDelivered;

                ScoreSaver.SaveScore(name, score);

                HideButtons();
            }
        }
    }
}
