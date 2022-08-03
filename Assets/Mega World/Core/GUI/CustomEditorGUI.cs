#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VladislavTsurikov.Editor
{
    public static class CustomEditorGUI
    {
		public static bool isInspector = true;

		public static Rect screenRect;

		public static float layoutWidth = 240;

        public static GUISkin skin {get {return AssetDatabase.LoadAssetAtPath<GUISkin>(GeneralPath.skinPath);}}

        private static Vector2 sliderClickPos;
		private static int sliderDraggingId = -20000000;
		private static float sliderOriginalValue;

		public static bool changeCheck = false;
		private static bool beginChangeCheck = false;

		private static readonly Texture2D BackgroundTexture = Texture2D.whiteTexture;
    	private static readonly GUIStyle LabelStyle = new GUIStyle("Label")
    	{
    	    normal = new GUIStyleState { background = BackgroundTexture, textColor = Color.white },
    	    fontStyle = FontStyle.Italic
    	};

		public static float GetCurrentSpace()
		{	
			if(EditorGUI.indentLevel == 0)
            {
				if(isInspector)
				{
					return 0;
				}

                return 5;
            }

			return 15 * EditorGUI.indentLevel;
		}

		public static void GeneralDrawGUIParamater(GUIContent text, Action drawEditorGUILayout)
		{
			Color initialGUIColor = GUI.color;

    		GUIStyle labelTextStyle;
    		GUIStyle barStyle;

    		SetLabelGUIStyle(out labelTextStyle, out barStyle);

    		GUILayout.BeginHorizontal();
            {
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				labelRect.width += 25;
				labelRect.x += 1;
	
				labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
				UnityEditor.EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);
    			
				drawEditorGUILayout.Invoke();
    			
				GUILayout.Space(5);
    		}
    		GUILayout.EndHorizontal();
    		GUILayout.Space(2);
		}

		public static bool Toggle(GUIContent text, bool value)
        {
			bool initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.Toggle(value)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
        }

		public static System.Enum EnumPopup(GUIContent text, System.Enum value)
		{
			System.Enum initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.EnumPopup(value)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static int Popup(GUIContent text, int value, GUIContent[] displayedOptions, params GUILayoutOption[] options)
		{
			int initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.Popup(value, displayedOptions, options)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static int Popup(GUIContent text, int value, string[] displayedOptions)
		{
			int initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.Popup(value, displayedOptions)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static float Slider(GUIContent text, float value, float min, float max)
		{
			float initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.Slider(value, min, max)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static int IntSlider(GUIContent text, int value, int min, int max)
		{
			int initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.IntSlider(value, min, max)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static Bounds BoundsField(GUIContent text, Bounds value)
		{
			Bounds initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.BoundsField(value)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static float FloatField(GUIContent text, float value)
		{
			float initialValue = value;

			Color initialGUIColor = GUI.color;

    		GUIStyle labelTextStyle;
    		GUIStyle barStyle;

    		SetLabelGUIStyle(out labelTextStyle, out barStyle);

    		GUILayout.BeginHorizontal();
            {
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				UnityEditor.EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);
    			    			
				value = DragChangeField(value, labelRect, 0, 0);
				value = EditorGUILayout.FloatField(value);

				if(!initialValue.Equals(value))
				{
					if(beginChangeCheck)
					{
						changeCheck = true;
					}
				}
    			
				GUILayout.Space(5);
    		}
    		GUILayout.EndHorizontal();

			GUILayout.Space(2);

			return value;
		}

		public static int IntField(GUIContent text, int value)
		{
			int initialValue = value;
			Color initialGUIColor = GUI.color;

    		GUIStyle labelTextStyle;
    		GUIStyle barStyle;

    		SetLabelGUIStyle(out labelTextStyle, out barStyle);

    		GUILayout.BeginHorizontal();
            {
    			labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
				
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				UnityEditor.EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);
    			
				value = (int)DragChangeField(value, labelRect, 0, 0);
				value = EditorGUILayout.IntField(value);
				if(!initialValue.Equals(value))
				{
					if(beginChangeCheck)
					{
						changeCheck = true;
					}
				}
    			
				GUILayout.Space(5);
    		}
    		GUILayout.EndHorizontal();

			GUILayout.Space(2);

			return value;
		}

		public static Color ColorField(GUIContent text, Color value)
		{
			Color initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.ColorField(value)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static Vector3 Vector3Field(GUIContent text, Vector3 value)
		{
			Vector3 initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.Vector3Field(GUIContent.none, value)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static Vector2 Vector2Field(GUIContent text, Vector2 value)
		{
			Vector2 initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.Vector2Field(GUIContent.none, value)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static string TextField(GUIContent text, string value)
		{
			string initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = EditorGUILayout.TextField(GUIContent.none, value)));

			if(initialValue != null)
			{
				if(!initialValue.Equals(value))
				{
					if(beginChangeCheck)
					{
						changeCheck = true;
					}
				}
			}
			
			return value;
		}

        public static LayerMask LayerField(GUIContent text, LayerMask value)
		{
			LayerMask initialValue = value;

			GeneralDrawGUIParamater(text, new Action(() => value = LayerMaskField(value)));

			if(!initialValue.Equals(value))
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static UnityEngine.Object ObjectField(GUIContent text, bool isObjectNull, UnityEngine.Object value, System.Type objType, int endHorizontalSpace = 5)
		{
			UnityEngine.Object initialValue = value;

			Color initialGUIColor = GUI.color;
			Color GUIcolor;

			GUIStyle labelTextStyle;
			GUIStyle barStyle;

			SetCurrentColorForObjectGUIParameter(isObjectNull, out GUIcolor);
			SetLabelGUIStyle(out labelTextStyle, out barStyle);

			GUIStyle localBarStyle = new GUIStyle(barStyle);

			localBarStyle.fixedHeight = 22;

			GUILayout.BeginHorizontal();
            {
				GUILayout.Space(GetCurrentSpace());

				labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
				GUI.color = GUIcolor;
				GUILayout.BeginVertical();
				{
					GUILayout.BeginHorizontal(localBarStyle);
					{
						GUILayout.Space(5);

						GUI.color = initialGUIColor;
						labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
						GUILayout.Label(new GUIContent(text), labelTextStyle);
						
						value = EditorGUILayout.ObjectField(value, objType, true);
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();

				GUILayout.Space(endHorizontalSpace);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(2);

			if(initialValue != value)
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

		public static UnityEngine.Object ObjectField(GUIContent text, UnityEngine.Object value, System.Type objType, int endHorizontalSpace = 5)
		{
			UnityEngine.Object initialValue = value;

			Color initialGUIColor = GUI.color;

    		GUIStyle labelTextStyle;
    		GUIStyle barStyle;

    		SetLabelGUIStyle(out labelTextStyle, out barStyle);

    		GUILayout.BeginHorizontal();
            {
    			labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
    			
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				labelRect.width += 25;
				labelRect.x += 1;

				GUI.color = initialGUIColor;
				labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
				UnityEditor.EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);
				GUILayout.Space(2);
				value = EditorGUILayout.ObjectField(value, objType, true);

    			GUILayout.Space(endHorizontalSpace);
    		}
    		GUILayout.EndHorizontal();

			GUILayout.Space(2);

			if(initialValue != value)
			{
				if(beginChangeCheck)
				{
					changeCheck = true;
				}
			}

			return value;
		}

        public static void PropertyField(GUIContent text, SerializedProperty property)
		{
			Color initialGUIColor = GUI.color;

    		GUIStyle labelTextStyle;
    		GUIStyle barStyle;

    		SetLabelGUIStyle(out labelTextStyle, out barStyle);

    		GUILayout.BeginHorizontal();
            {
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				labelRect.width += 25;
				labelRect.x += 1;
				labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
				UnityEditor.EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);
    			
				EditorGUILayout.PropertyField(property, GUIContent.none);
    			
				GUILayout.Space(5);
    		}
    		GUILayout.EndHorizontal();
    		GUILayout.Space(2);
		}

		public static void MinMaxSlider(GUIContent text, ref float min, ref float max, float minimumValue, float maximumValue)
        {
			float initialMinValue = min;
			float initialMaxValue = max;

			float localMinValue = min;
			float localMaxValue = max;

			Color initialGUIColor = GUI.color;

    		GUIStyle labelTextStyle;
    		GUIStyle barStyle;

    		SetLabelGUIStyle(out labelTextStyle, out barStyle);

    		GUILayout.BeginHorizontal();
            {
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				labelRect.width += 25;
				labelRect.x += 1;
				labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
				UnityEditor.EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);
    			
                MinMaxSlider(ref localMinValue, ref localMaxValue, minimumValue, maximumValue);

				if(!initialMinValue.Equals(localMinValue))
				{
					if(beginChangeCheck)
					{
						changeCheck = true;
					}
				}
				else if(!initialMaxValue.Equals(localMaxValue))
				{
					if(beginChangeCheck)
					{
						changeCheck = true;
					}
				}

				min = localMinValue;
				max = localMaxValue;

    			GUI.color = initialGUIColor;
    			
				GUILayout.Space(5);
    		}
    		GUILayout.EndHorizontal();
    		GUILayout.Space(2);
        }

        public static void MinMaxIntSlider(GUIContent text, ref int min, ref int max, int minimumValue, int maximumValue)
        {
			int initialMinValue = min;
			int initialMaxValue = max;

			int localMinValue = min;
			int localMaxValue = max;
			
			Color initialGUIColor = GUI.color;

    		GUIStyle labelTextStyle;
    		GUIStyle barStyle;

    		SetLabelGUIStyle(out labelTextStyle, out barStyle);

    		GUILayout.BeginHorizontal();
            {
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				labelRect.width += 25;
				labelRect.x += 1;
				labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
				UnityEditor.EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);
    			    			
                MinMaxIntSlider(ref localMinValue, ref localMaxValue, minimumValue, maximumValue);

				if(!initialMinValue.Equals(localMinValue))
				{
					if(beginChangeCheck)
					{
						changeCheck = true;
					}
				}
				else if(!initialMaxValue.Equals(localMaxValue))
				{
					if(beginChangeCheck)
					{
						changeCheck = true;
					}
				}

				min = localMinValue;
				max = localMaxValue;

    			GUI.color = initialGUIColor;
    			
				GUILayout.Space(5);
    		}
    		GUILayout.EndHorizontal();
    		GUILayout.Space(2);
        }

        private static void MinMaxSlider(ref float min, ref float max, float minimumValue, float maximumValue)
        {
			float minimumTmp = min;
            float maximumTmp = max;

			minimumTmp = EditorGUILayout.DelayedFloatField(minimumTmp, GUILayout.MaxWidth(120));
			EditorGUILayout.MinMaxSlider(ref minimumTmp, ref maximumTmp, minimumValue, maximumValue, GUILayout.MinWidth(5));
			maximumTmp = Mathf.Max(minimumTmp, EditorGUILayout.DelayedFloatField(maximumTmp, GUILayout.MaxWidth(120)));

			min = minimumTmp;
            max = maximumTmp;
        }

		private static void MinMaxIntSlider(ref int min, ref int max, int minimumValue, int maximumValue)
        {
            float minimumTmp = min;
            float maximumTmp = max;

			minimumTmp = EditorGUILayout.DelayedIntField((int)minimumTmp, GUILayout.MaxWidth(120));
            EditorGUILayout.MinMaxSlider(ref minimumTmp, ref maximumTmp, minimumValue, maximumValue);
            maximumTmp = Mathf.Max(minimumTmp, EditorGUILayout.DelayedIntField((int)maximumTmp, GUILayout.MaxWidth(120)));

			min = (int)minimumTmp;
            max = (int)maximumTmp;
        }

		public static void CustomGradient(GUIContent text, CustomGradient gradient)
        {
    		Color initialGUIColor = GUI.color;

    		GUIStyle labelTextStyle;
    		GUIStyle barStyle;

    		SetLabelGUIStyle(out labelTextStyle, out barStyle);

    		GUILayout.BeginHorizontal();
            {
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				labelRect.x += 1;
				
    			labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;

				UnityEditor.EditorGUI.LabelField(labelRect, text, labelTextStyle);

    			GUILayout.BeginVertical();
    			{
    				GUILayout.BeginHorizontal();
    				{
    					GUI.color = initialGUIColor;
    					GUILayout.Space(5);
    					DrawGradientButton(gradient, labelRect.width);
    				}
    				GUILayout.EndHorizontal();
    			}
    			GUILayout.EndVertical();
    			GUILayout.Space(5);
    		}
    		GUILayout.EndHorizontal();
    		GUILayout.Space(2);
        }

		public static void DrawGradientButton(CustomGradient gradient, float width)
		{
			if(GradientButton(gradient, width))
			{
				GradientEditor window = EditorWindow.GetWindow<GradientEditor>();
    	        window.SetGradient(gradient);
			}
		}

		public static bool GradientButton(CustomGradient gradient, float width)
		{
			float height = 18;

            float iconSize = height * 0.6f;
            float iconPadding = height * 0.1f;

            var options = new List<GUILayoutOption> {GUILayout.Height(height)};

			options.Add(GUILayout.ExpandWidth(true));
            bool result = false;

            GUILayout.BeginVertical(options.ToArray());
            {
				GUIStyle gradientStyle = new GUIStyle();
    	    	gradientStyle.normal.background = gradient.GetTexture((int)width);

				result = GUILayout.Button(GUIContent.none, gradientStyle, options.ToArray());
            }
            GUILayout.EndVertical();

			return result;
		}

		public static bool DrawIcon(StyleName styleName, Color iconColor, float rowHeight = -1)
        {
			GUIStyle style = GetStyle(styleName);

            Color color = GUI.color;
            if (rowHeight > 0)
            {
                GUILayout.BeginVertical(GUILayout.Width(style.fixedWidth), GUILayout.Height(rowHeight));
                GUILayout.Space((rowHeight - style.fixedHeight) / 2);
            }

            GUI.color = iconColor;
            bool buttonClicked = GUILayout.Button(GUIContent.none, style);
            GUI.color = color;
            if (rowHeight > 0) GUILayout.EndVertical();

            if (!buttonClicked) return false;
            GUIUtility.keyboardControl = 0;
            Event.current.Use();
            return true;
        }

		public static bool ToggleButton(string text, bool included, ButtonStyle colorSpace, ButtonSize buttonSize = ButtonSize.General)
		{
			GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
			GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

			Color barColor;
			Color labelColor = EditorColors.Instance.LabelColor;

			if(included)
			{
				barColor = EditorColors.Instance.ToggleButtonActiveColor;
			}
			else
			{
				barColor = EditorColors.Instance.ToggleButtonInactiveColor;
			}

			if(buttonSize == ButtonSize.ToolButton)
			{
				return Button(text, labelStyle, barStyle, labelColor, barColor, 25);
			}
			else
			{
				return Button(text, labelStyle, barStyle, labelColor, barColor, 21);
			}
		}

		public static bool ClickButton(string text)
		{
			return ClickButton(text, ButtonStyle.ButtonClick);
		}

		public static bool ClickButton(string text, ButtonStyle buttonStyle, ButtonSize buttonSize = ButtonSize.General)
		{
			GUIStyle labelStyle = GetStyle(StyleName.LabelButtonClick);
			GUIStyle barStyle = GetStyle(StyleName.ActiveBar);
			
			Color labelColor = EditorColors.Instance.LabelColor;
			Color barColor = EditorColors.Instance.ClickButtonColor;

			SetButtonColors(buttonStyle, out barColor, out labelColor);

			if(buttonSize == ButtonSize.DragAndDropButton)
			{
				return Button(text, labelStyle, barStyle, labelColor, barColor, 40);
			}
			else
			{
				return Button(text, labelStyle, barStyle, labelColor, barColor, 20);
			}
		}

        public static bool Button(string text, GUIStyle labelStyle, GUIStyle barStyle, Color labelColor, Color barColor, float height)
		{
			GUIStyle localBarStyle = new GUIStyle(barStyle);
			localBarStyle.fixedHeight = height;

			float iconPadding = height * 0.1f;

			bool result;

			Color color = GUI.color;

			GUILayout.BeginVertical();
            {
				GUI.color = barColor;
				result = GUILayout.Button(GUIContent.none, localBarStyle);
				GUILayout.Space(-height);
				GUI.color = color;
	
				labelStyle.normal.textColor = labelColor;
	
            	GUILayout.BeginHorizontal();
            	{
					GUILayout.Label(new GUIContent(text), labelStyle, GUILayout.ExpandWidth(true), GUILayout.Height(height));
            	    GUILayout.Space(iconPadding);
            	}
            	GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();

			return result;
		}

		public static void Label(GUIContent text)
		{
			Color initialGUIColor = GUI.color;

    		GUIStyle labelTextStyle;
    		GUIStyle barStyle;

    		SetLabelGUIStyle(out labelTextStyle, out barStyle);

    		GUILayout.BeginHorizontal();
            {
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				labelRect.width += 25;
				labelRect.x += 1;
				
    			labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
				labelTextStyle.fontStyle = FontStyle.Italic;

				UnityEditor.EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);
    			    			
				GUILayout.Space(5);
    		}
    		GUILayout.EndHorizontal();
    		GUILayout.Space(2);

			labelTextStyle.fontStyle = FontStyle.Normal;
		}

		public static void Label(string text, GUIStyle style)
		{
			Color initialGUIColor = GUI.color;

    		GUILayout.BeginHorizontal();
            {
				Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(layoutWidth));
				labelRect.width += 25;
				labelRect.x += 1;

				UnityEditor.EditorGUI.LabelField(labelRect, new GUIContent(text), style);
			    			
				GUILayout.Space(5);
    		}
    		GUILayout.EndHorizontal();
    		GUILayout.Space(2);
		}

		public static void RectTab(Rect rect, string text, ButtonStyle colorSpace, float height, int fontSize)
		{
			GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
			GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

			Color barColor; 
			Color labelColor = EditorColors.Instance.LabelColor;

			SetButtonColors(colorSpace, out barColor, out labelColor);

			GUIStyle localBarStyle = new GUIStyle(barStyle);
			localBarStyle.fixedHeight = height;

			GUIStyle localLabelStyle = new GUIStyle(labelStyle);
			localLabelStyle.normal.textColor = labelColor;
			localLabelStyle.fontSize = fontSize;

			RectTab(text, rect, localLabelStyle, localBarStyle, barColor);
		}

		public static void RectTab(Rect rect, string text, bool included, float height, ButtonStyle colorSpace = ButtonStyle.General)
		{
			GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
			GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

			Color barColor;
			Color labelColor = EditorColors.Instance.LabelColor;

			if(included)
			{
				barColor = EditorColors.Instance.ToggleButtonActiveColor;
			}
			else
			{
				barColor = EditorColors.Instance.ToggleButtonInactiveColor;
			}

			GUIStyle localBarStyle = new GUIStyle(barStyle);
			localBarStyle.fixedHeight = height;

			GUIStyle localLabelStyle = new GUIStyle(labelStyle);
			localLabelStyle.normal.textColor = labelColor;

			RectTab(text, rect, localLabelStyle, localBarStyle, barColor);
		}

		public static void RectTab(string text, Rect tabRect, GUIStyle labelStyle, GUIStyle barStyle, Color barColor)
		{
            Color initialColor = GUI.color;

			GUI.color = barColor;
			EditorGUI.LabelField(tabRect, "", barStyle); 
        	GUI.color = initialColor;

			EditorGUI.LabelField(tabRect, text, labelStyle);
		}

		public static bool FoldoutRemove(bool foldout, string content, Action remove)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

			if(isInspector == true)
			{
				rect.x -= 5;
			}

			GUIStyle removeStyle = GetStyle(StyleName.IconButtonMinus);

			Rect buttonRect = rect;
            buttonRect.x += rect.width - EditorGUIUtility.singleLineHeight;
            buttonRect.width = EditorGUIUtility.singleLineHeight;

			Color color = GUI.color;
            GUI.color = EditorColors.Instance.Red;
            if (GUI.Button(buttonRect, GUIContent.none, removeStyle))
            {
                remove.Invoke();
            }
			GUI.color = color;

			if (GUI.Button(EditorGUI.IndentedRect(rect), "", GUIStyle.none))
            {
                foldout = !foldout;
            }

			Texture texture;

			if(foldout)
			{
				texture = AssetDatabase.LoadAssetAtPath<Texture>(GeneralPath.foldoutDownPath);
			}
			else
			{
				texture = AssetDatabase.LoadAssetAtPath<Texture>(GeneralPath.foldoutRightPath);
			}

			Color initialColor = GUI.color;
            GUI.color = EditorColors.Instance.LabelColor;
            GUI.Label(EditorGUI.IndentedRect(rect), texture);
            GUI.color = initialColor;

            rect.x += EditorGUIUtility.singleLineHeight;
			GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
			labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(rect, content, labelStyle);

            return foldout;
        }

		public static bool Foldout(bool foldout, string content)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);
			
			if(isInspector == true)
			{
				rect.x -= 5;
			}
			
			Color initialColor = GUI.color;

			if (GUI.Button(EditorGUI.IndentedRect(rect), "", GUIStyle.none))
            {
                foldout = !foldout;
            }

			Texture texture;

			if(foldout)
			{
				texture = AssetDatabase.LoadAssetAtPath<Texture>(GeneralPath.foldoutDownPath);
			}
			else
			{
				texture = AssetDatabase.LoadAssetAtPath<Texture>(GeneralPath.foldoutRightPath);
			}

            GUI.color = EditorColors.Instance.LabelColor;
            GUI.Label(EditorGUI.IndentedRect(rect), texture);
            GUI.color = initialColor;

            rect.x += EditorGUIUtility.singleLineHeight;
			GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
			labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(rect, content, labelStyle);

            return foldout;
        }

		public static void Separator()
        {
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(3));
			r = EditorGUI.IndentedRect(r);
            Vector2 start = new Vector2(r.min.x, (r.min.y + r.max.y) / 2);
            Vector2 end = new Vector2(r.max.x, (r.min.y + r.max.y) / 2);
            Handles.BeginGUI();
			Handles.color = EditorColors.Instance.separatorColor;
            Handles.DrawLine(start, end);
            Handles.EndGUI();
        }

		private static void SetCurrentColorForObjectGUIParameter(bool isObjectNull, out Color GUIcolor)
		{
			if(isObjectNull)
			{
				GUIcolor = EditorColors.Instance.Red.WithAlpha(0.4f);
			}
			else
			{
				GUIcolor = EditorColors.Instance.Green.WithAlpha(0.4f);
			}
		}

		public static void SetLabelGUIStyle(out GUIStyle labelTextStyle, out GUIStyle barStyle)
		{
			barStyle = GetStyle(StyleName.ActiveBar);
			labelTextStyle = GetStyle(StyleName.LabelText);
		}

		public static void SetButtonColors(ButtonStyle colorSpace, out Color barColor, out Color labelColor)
		{
			if(colorSpace == ButtonStyle.Add)
			{
				labelColor = EditorColors.Instance.LabelColor;
				barColor = EditorColors.Instance.Green.WithAlpha(0.4f);
			}
			else if(colorSpace == ButtonStyle.Remove)
			{
				labelColor = EditorColors.Instance.LabelColor;
				barColor = EditorColors.Instance.Red.WithAlpha(0.4f);
			}
			else
			{
				labelColor = EditorColors.Instance.LabelColor;
				barColor = EditorColors.Instance.ClickButtonColor;
			}
		}

        private static LayerMask LayerMaskField(LayerMask layerMask)
        {
            List<string> layers = new List<string>(32);
            List<int> layerNumbers = new List<int>(32);

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "") {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++) {
                if (((1 << layerNumbers[i]) & layerMask.value) != 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++) {
                if ((maskWithoutEmpty & (1 << i)) != 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }

		private static float DragChangeField(float value, Rect sliderRect, float min = 0, float max = 0, float minStep = 0.2f)
		{
			int controlId = GUIUtility.GetControlID(FocusType.Passive);
			#if UNITY_EDITOR
			UnityEditor.EditorGUIUtility.AddCursorRect (sliderRect, UnityEditor.MouseCursor.SlideArrow);
			#endif
			if (Event.current.type == EventType.MouseDown && sliderRect.Contains(Event.current.mousePosition) ) 
			{ 
				sliderClickPos = Event.current.mousePosition; 
				sliderOriginalValue = value;
				sliderDraggingId = controlId; 
			}

			if (Event.current.rawType == EventType.MouseUp) 
			{
				sliderDraggingId = -20000000;

				#if UNITY_EDITOR
				if (UnityEditor.EditorWindow.focusedWindow!=null) UnityEditor.EditorWindow.focusedWindow.Repaint(); 
				#endif

				return value;
			}

			if (sliderDraggingId == controlId && Event.current.type == EventType.MouseDrag)
			{
				int steps = (int)((Event.current.mousePosition.x - sliderClickPos.x) / 5);
				
				value = sliderOriginalValue;

				for (int i = 0; i< Mathf.Abs(steps); i++)
				{
					float absVal = value>=0? value : -value;

					float step = 0.01f;
					if (absVal > 0.99f) step=0.02f; if (absVal > 1.99f) step=0.1f;   if (absVal > 4.999f) step = 0.2f; if (absVal > 9.999f) step=0.5f;
					if (absVal > 39.999f) step=1f;  if (absVal > 99.999f) step = 2f; if (absVal > 199.999f) step = 5f; if (absVal > 499.999f) step = 10f; 
					if (step < minStep) step = minStep;

					value = steps>0? value+step : value-step;
					value = Mathf.Round(value*10000)/10000f;

					if (Mathf.Abs(min)>0.001f && value<min) value=min;
					if (Mathf.Abs(max)>0.001f && value>max) value=max;
				}

				#if UNITY_EDITOR
				if (UnityEditor.EditorWindow.focusedWindow!=null) UnityEditor.EditorWindow.focusedWindow.Repaint(); 
				UnityEditor.EditorGUI.FocusTextInControl("");
				#endif
			}
			if (Event.current.isMouse && sliderDraggingId == controlId) Event.current.Use();

			return value;
		}

		public static void DrawHelpBanner(string helpURL, string title = "Help")
        {
			GUIStyle labelStyle = GetStyle(StyleName.LabelHelp);
			GUIStyle barStyle = GetStyle(StyleName.ActiveBar);
			
			Color labelColor = EditorColors.Instance.docsLabelColor;
			Color barColor = EditorColors.Instance.docsButtonColor;

			if(Button(title, labelStyle, barStyle, labelColor, barColor, 25))
			{
				Application.OpenURL(helpURL);
			}
        }

		public static bool DrawTab(Rect rect, string text, bool included, int index, float tabWidth, float tabHeight, ButtonStyle buttonStyle = ButtonStyle.General)
		{
            Rect tabRect = new Rect(rect.x + tabWidth * index, rect.y, tabWidth, tabHeight);
			CustomEditorGUI.RectTab(tabRect, text, included, tabHeight, buttonStyle);

			Event e = Event.current;

			if(tabRect.Contains(e.mousePosition))
            {	
				switch(e.type)
           		{
           			case EventType.MouseDown:
					{
						return true;
					}
				}
            }

			return false;
		}

		public static void HelpBox(string text)
		{
			EditorGUILayout.HelpBox(text, MessageType.Info);    
		}

		public static void WarningBox(string text)
		{
			EditorGUILayout.HelpBox(text, MessageType.Warning);    
		}

		public static void Header(string text)
		{
			EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
		}

		public static GUIStyle GetStyle(string styleName)
        {
            GUIStyle style = skin.GetStyle(styleName);
            return style;
        }
        
        public static GUIStyle GetStyle(StyleName styleName) { return GetStyle(styleName.ToString()); }

		static public void BeginChangeCheck()
		{
			beginChangeCheck = true;
			changeCheck = false;
		}

		static public bool EndChangeCheck()
		{
			beginChangeCheck = false;
			bool localChangeCheck = changeCheck;
			changeCheck = false;

			return localChangeCheck;
		}
    }
}
#endif
