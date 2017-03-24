using UnityEngine;
using UnityEngine.UI;

namespace CurvedVRKeyboard {
    [SelectionBase]
    public class KeyboardStatus: KeyboardComponent {

        //------SET IN UNITY-------
        [Tooltip("Text field receiving input from the keyboard")]
        public Text output;
        [Tooltip("Maximum output text length")]
        public int maxOutputLength;

        //----CurrentKeysStatus----
        private KeyboardItem[] keys;
        private bool areLettersActive = true;
        private bool isLowercase = true;
        private static readonly char BLANKSPACE = ' ';

        public delegate void OnSubmittedName(string name);
        public delegate void OnSkippedEntry();

        public event OnSubmittedName SubmittedName;
        public event OnSkippedEntry SkippedEntry;


        /// <summary>
        /// Handles click on keyboarditem
        /// </summary>
        /// <param name="clicked">keyboard item clicked</param>
        public void HandleClick ( KeyboardItem clicked ) {
            string value = clicked.GetValue();
            if(value.Equals(SPACE)) {
                TypeKey(BLANKSPACE);
            } else if(value.Equals(BACK)) {
                BackspaceKey();
            } else {// Normal letter
                TypeKey(value[0]);
            }
        }

        private void BackspaceKey () {
            if(output.text.Length >= 1)
                output.text = output.text.Remove(output.text.Length - 1, 1);
        }

        private void TypeKey ( char key ) {
            if(output.text.Length < maxOutputLength)
                output.text = output.text + key.ToString();
        }

        public void SetKeys ( KeyboardItem[] keys ) {
            this.keys = keys;
        }
    }
}
