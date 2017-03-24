using UnityEngine;
namespace CurvedVRKeyboard {

    /// <summary>
    /// Setup class derived by all classes who are part of keyboard,
    /// so those variables are easy accessable everywhere
    /// </summary>
    public abstract class KeyboardComponent: MonoBehaviour {

        // Special signs. Feel free to change
        public const string SPACE = "  ";
        public const string BACK = "Back";

        public const int CENTER_ITEM = 15;
        public const int KEY_NUMBER = 27;
        public const int POSITION_SPACE = 28;

        public enum KeyLetterEnum {
            UpperCase
        }

        // Feel free to change (but do not write strings in place of
        // special signs, change variables values instead)
        // Remember to always have 30 values
        public static readonly string[] allLettersUppercase = new string[]
        {
        "Q","W","E","R","T","Y","U","I","O","P",
        "A","S","D","F","G","H","J","K","L",
        BACK,"Z","X","C","V","B","N","M"
        };

        // Number of items in a row
        public static readonly int[] lettersInRowsCount = new int[] { 10, 9, 8 };

        /// <summary>
        /// Checks for errrors with array of keys. 
        /// </summary>
        public static void CheckKeyArrays () {
            if(allLettersUppercase.Length != KEY_NUMBER) {
                ErrorReporter.Instance.SetMessage("There is incorrect amount of letters in Uppercase array. Check KeyboardComponent class", ErrorReporter.Status.Error);
                return;
            }
        }
    }
}