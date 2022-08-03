using UnityEngine;

namespace MegaWorld.Edit
{
    public class EditToolAllShortcutCombos : Singleton<EditToolAllShortcutCombos>
    {
        #region Private Variables
        private ShortcutCombo _editToolPrecisionModifyMode_Position;
        private ShortcutCombo _editToolPrecisionModifyMode_Raycast;
        private ShortcutCombo _editToolPrecisionModifyMode_MoveAlongDirection;
        private ShortcutCombo _editToolPrecisionModifyMode_Rotation;
        private ShortcutCombo _editToolPrecisionModifyMode_Scale;
        private ShortcutCombo _editToolPrecisionModifyMode_Remove;
        #endregion

        #region Public Properties
        /// <summary>
        /// Object placement.
        /// </summary>
        public ShortcutCombo EditToolPrecisionModifyMode_Position { get { return _editToolPrecisionModifyMode_Position; } }
        public ShortcutCombo EditToolPrecisionModifyMode_Raycast { get { return _editToolPrecisionModifyMode_Raycast; } }
        public ShortcutCombo EditToolPrecisionModifyMode_MoveAlongDirection { get { return _editToolPrecisionModifyMode_MoveAlongDirection; } }
        public ShortcutCombo EditToolPrecisionModifyMode_Rotation { get { return _editToolPrecisionModifyMode_Rotation; } }
        public ShortcutCombo EditToolPrecisionModifyMode_Scale { get { return _editToolPrecisionModifyMode_Scale; } }
        public ShortcutCombo EditToolPrecisionModifyMode_Remove { get { return _editToolPrecisionModifyMode_Remove; } }

        #endregion

        #region Constructors
        public EditToolAllShortcutCombos()
        {
            CreateCombos();
        }
        #endregion

        #region Private Methods
        private void CreateCombos()
        {
            _editToolPrecisionModifyMode_Position = new ShortcutCombo();
            _editToolPrecisionModifyMode_Position.AddKey(KeyCode.W);

            _editToolPrecisionModifyMode_Raycast = new ShortcutCombo();
            _editToolPrecisionModifyMode_Raycast.AddKey(KeyCode.LeftShift);

            _editToolPrecisionModifyMode_MoveAlongDirection = new ShortcutCombo();
            _editToolPrecisionModifyMode_MoveAlongDirection.AddKey(KeyCode.Q);

            _editToolPrecisionModifyMode_Rotation = new ShortcutCombo();
            _editToolPrecisionModifyMode_Rotation.AddKey(KeyCode.E);

            _editToolPrecisionModifyMode_Scale = new ShortcutCombo();
            _editToolPrecisionModifyMode_Scale.AddKey(KeyCode.R);

            _editToolPrecisionModifyMode_Remove = new ShortcutCombo();
            _editToolPrecisionModifyMode_Remove.AddKey(KeyCode.T);
        }
        #endregion
    }
}