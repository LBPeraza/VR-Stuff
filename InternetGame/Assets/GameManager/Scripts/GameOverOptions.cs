using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class GameOverOptions : MonoBehaviour
    {
        public VRTK.VRTK_Button RetryButton;
        public VRTK.VRTK_Button QuitButton;

        bool handlingButton = false;

        public void LoadResources()
        {

        }

        public void Initialize()
        {
            LoadResources();

            RetryButton.Pushed += RetryButtonPushed;
            QuitButton.Pushed += QuitButtonPushed;

            handlingButton = false;

            Hide();
        }

        private void QuitButtonPushed(
            object sender, 
            VRTK.Control3DEventArgs e)
        {
            if (!handlingButton)
            {
                GameManager.GetInstance().Quit();

                handlingButton = true;
            }
        }

        private void RetryButtonPushed(
            object sender, 
            VRTK.Control3DEventArgs e)
        {
            if (!handlingButton)
            {
                GameManager.GetInstance().Retry();

                handlingButton = true;
            }
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }
        
    }
}

