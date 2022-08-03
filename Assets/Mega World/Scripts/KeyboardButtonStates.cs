using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaWorld
{
    public class KeyboardButtonStates
    {
        #region Private Variables
        public Dictionary<KeyCode, bool> _keyboardButtonStates = new Dictionary<KeyCode, bool>();
        #endregion

        #region Public Methods
        public void OnKeyboardButtonPressed(KeyCode keyCode)
        {
            AddKeyCodeEntryIfNecessary(keyCode);
            _keyboardButtonStates[keyCode] = true;
        }

        public void OnKeyboardButtonReleased(KeyCode keyCode)
        {
            AddKeyCodeEntryIfNecessary(keyCode);
            _keyboardButtonStates[keyCode] = false;
        }

        public bool IsKeyboardButtonPressed(KeyCode keyCode)
        {
            if (keyCode == KeyCode.LeftShift || keyCode == KeyCode.RightShift) 
            {
                return Event.current.shift;
            }
            if (keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl) return Event.current.control;
            if (keyCode == KeyCode.LeftCommand || keyCode == KeyCode.RightCommand) return Event.current.command;

            if (!DoesKeyCodeEntryExist(keyCode)) 
            {
                return false;
            }

            return _keyboardButtonStates[keyCode];
        }
        #endregion

        #region Private Methods
        private void AddKeyCodeEntryIfNecessary(KeyCode keyCode)
        {
            if (!DoesKeyCodeEntryExist(keyCode)) _keyboardButtonStates.Add(keyCode, false);
        }

        private bool DoesKeyCodeEntryExist(KeyCode keyCode)
        {
            return _keyboardButtonStates.ContainsKey(keyCode);
        }
        #endregion
    }
}