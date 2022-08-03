using UnityEngine;

namespace MegaWorld
{
    public class PrecisePlaceToolAllShortcutCombos : Singleton<PrecisePlaceToolAllShortcutCombos>
    {
        #region Private Variables
        private ShortcutCombo _offsetFromPlacementSurface;
        private ShortcutCombo _mouseRotateAroundX;
        private ShortcutCombo _mouseRotateAroundY;
        private ShortcutCombo _mouseRotateAroundZ;
        private ShortcutCombo _mouseFreeRotate;
        private ShortcutCombo _mouseUniformScale;
        private ShortcutCombo _mouseScaleX;
        private ShortcutCombo _mouseScaleY;
        private ShortcutCombo _mouseScaleZ;
        #endregion

        #region Public Properties
        /// <summary>
        /// Object placement.
        /// </summary>
        public ShortcutCombo OffsetFromPlacementSurface { get { return _offsetFromPlacementSurface; } }
        public ShortcutCombo MouseRotateAroundX { get { return _mouseRotateAroundX; } }
        public ShortcutCombo MouseRotateAroundY { get { return _mouseRotateAroundY; } }
        public ShortcutCombo MouseRotateAroundZ { get { return _mouseRotateAroundZ; } }
        public ShortcutCombo MouseFreeRotate { get { return _mouseFreeRotate; } }
        public ShortcutCombo MouseUniformScale { get { return _mouseUniformScale; } }
        public ShortcutCombo MouseScaleX { get { return _mouseScaleX; } }
        public ShortcutCombo MouseScaleY { get { return _mouseScaleY; } }
        public ShortcutCombo MouseScaleZ { get { return _mouseScaleZ; } }
        #endregion

        #region Constructors
        public PrecisePlaceToolAllShortcutCombos()
        {
            CreateCombos();
        }
        #endregion

        #region Private Methods
        private void CreateCombos()
        {
            _offsetFromPlacementSurface = new ShortcutCombo();
            _offsetFromPlacementSurface.AddKey(KeyCode.Q);
            
            _mouseRotateAroundX = new ShortcutCombo();
            _mouseRotateAroundX.AddKey(KeyCode.X);
            _mouseRotateAroundX.AddKey(KeyCode.LeftShift);

            _mouseRotateAroundY = new ShortcutCombo();
            _mouseRotateAroundY.AddKey(KeyCode.C);
            _mouseRotateAroundY.AddKey(KeyCode.LeftShift);

            _mouseRotateAroundZ = new ShortcutCombo();
            _mouseRotateAroundZ.AddKey(KeyCode.Z);
            _mouseRotateAroundZ.AddKey(KeyCode.LeftShift);

            _mouseFreeRotate = new ShortcutCombo();
            _mouseFreeRotate.AddKey(KeyCode.E);
            _mouseFreeRotate.AddKey(KeyCode.LeftShift);

            _mouseUniformScale = new ShortcutCombo();
            _mouseUniformScale.AddKey(KeyCode.LeftControl);
            _mouseUniformScale.AddKey(KeyCode.LeftShift);

            _mouseScaleX = new ShortcutCombo();
            _mouseScaleX.AddKey(KeyCode.LeftControl);
            _mouseScaleX.AddKey(KeyCode.LeftShift);
            _mouseScaleX.AddKey(KeyCode.X);

            _mouseScaleY = new ShortcutCombo();
            _mouseScaleY.AddKey(KeyCode.LeftControl);
            _mouseScaleY.AddKey(KeyCode.LeftShift);
            _mouseScaleY.AddKey(KeyCode.C);

            _mouseScaleZ = new ShortcutCombo();
            _mouseScaleZ.AddKey(KeyCode.LeftControl);
            _mouseScaleZ.AddKey(KeyCode.LeftShift);
            _mouseScaleZ.AddKey(KeyCode.Z);
        }
        #endregion
    }
}