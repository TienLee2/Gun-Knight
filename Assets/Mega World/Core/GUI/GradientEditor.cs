#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VladislavTsurikov.Editor;

namespace VladislavTsurikov
{
    public class GradientEditor : EditorWindow 
    {
        private CustomGradient gradient;
        const int borderSize = 10;
        const float keyWidth = 10;
        const float keyHeight = 20;

        Rect gradientPreviewRect;
        Rect[] keyRects;
        bool mouseIsDownOverKey;
        int selectedKeyIndex;
        bool needsRepaint;

        Vector2 windowScrollPos;

        private void OnGUI()
        {
            Draw();
            HandleInput();

            if (needsRepaint)
            {
                needsRepaint = false;
                Repaint();
            }
        }

        void Draw()
        {
            Color initialGUIColor = GUI.color;
            
            windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos);

            gradientPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, 25);
    		GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture((int)gradientPreviewRect.width));
    
    		keyRects = new Rect[gradient.NumKeys];
    		for (int i = 0; i < gradient.NumKeys; i++)
    		{
    			CustomGradient.ColourKey key = gradient.GetKey(i);
    			Rect keyRect = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - keyWidth / 2f, gradientPreviewRect.yMax + borderSize, keyWidth, keyHeight);
    			if (i == selectedKeyIndex)
    			{
    				EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), EditorColors.Instance.orangeNormal);
    			}
                else
                {
                    EditorGUI.DrawRect(new Rect(keyRect.x - 1, keyRect.y - 1, keyRect.width + 2, keyRect.height + 2), Color.white);
                }

    			EditorGUI.DrawRect(keyRect, key.Colour);
    			keyRects[i] = keyRect;
    		}

            GUILayout.Space(75);

            EditorGUI.BeginChangeCheck();
    
            float value = gradient.GetKey(selectedKeyIndex).Colour.r;
            value = CustomEditorGUI.Slider(new GUIContent("Value"), value, 0, 1);
    
            Color newColour = new Color(value, value, value);
    
            if (EditorGUI.EndChangeCheck())
            {
                gradient.UpdateKeyColour(selectedKeyIndex, newColour);
            }
    
            gradient.blendMode = (CustomGradient.BlendMode)CustomEditorGUI.EnumPopup(new GUIContent("Blend mode"), gradient.blendMode);

            EditorGUILayout.EndScrollView();
        }

        void HandleInput()
        {
    		Event guiEvent = Event.current;
    		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
    		{
    			for (int i = 0; i < keyRects.Length; i++)
    			{
    				if (keyRects[i].Contains(guiEvent.mousePosition))
    				{
    					mouseIsDownOverKey = true;
    					selectedKeyIndex = i;
    					needsRepaint = true;
    					break;
    				}
    			}

    			if (!mouseIsDownOverKey)
    			{
    				float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                    Color interpolatedColour = gradient.Evaluate(keyTime);

                    selectedKeyIndex = gradient.AddKey(interpolatedColour, keyTime);
    				mouseIsDownOverKey = true;
    				needsRepaint = true;
    			}
    		}

    		if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
    		{
    			mouseIsDownOverKey = false;
    		}

    		if (mouseIsDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
    		{
    			float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
    			selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
    			needsRepaint = true;
                GUI.changed = true;
    		}

    		if (guiEvent.keyCode == KeyCode.Backspace && guiEvent.type == EventType.KeyDown)
    		{
    			gradient.RemoveKey(selectedKeyIndex);
    			if (selectedKeyIndex >= gradient.NumKeys)
    			{
    				selectedKeyIndex--;
    			}
    			needsRepaint = true;
    		}
        }

        public void SetGradient(CustomGradient gradient)
        {
            this.gradient = gradient;
        }

        private void OnEnable()
        {
            titleContent.text = "Gradient Editor";
            position.Set(position.x, position.y, 400, 150);
            minSize = new Vector2(200, 150);
            maxSize = new Vector2(1920, 150);
        }

        private void OnDisable()
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }
}
#endif
