using UnityEditor;
using UnityEngine;

public static class TweenerParams
{
    [System.Serializable]
    public class Generic<T>
    {
        public bool enabled;
        public T from, to;
    }

    [System.Serializable] public class Vector2 : Generic<UnityEngine.Vector2> { }
    [System.Serializable] public class Vector3 : Generic<UnityEngine.Vector3> { }
    [System.Serializable] public class Float : Generic<float> { }
    [System.Serializable] public class Color : Generic<UnityEngine.Color> { }
    [System.Serializable] public class Move : Generic<RelativeScreenVector> { }
    [System.Serializable]
    public class RelativeScreenVector
    {
        [SerializeField]
        private UnityEngine.Vector2 target;
        public enum Mode
        {
            Target,
            ScreenWidth,
            ScreenHeight,
            NegativeScreenWidth,
            NegativeScreenHeight,
        }
        [SerializeField]
        Mode mode;
        public UnityEngine.Vector2 Target
        {
            get
            {
                switch (mode)
                {
                    case Mode.ScreenWidth:
                        return new UnityEngine.Vector2(target.x + Screen.width, target.y);
                    case Mode.NegativeScreenWidth:
                        return new UnityEngine.Vector2(target.x - Screen.width, target.y);
                    case Mode.ScreenHeight:
                        return new UnityEngine.Vector2(target.x, target.y + Screen.height);
                    case Mode.NegativeScreenHeight:
                        return new UnityEngine.Vector2(target.x, target.y - Screen.height);
                    default:
                        return target;
                }
            }
        }
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Vector3))]
    [CustomPropertyDrawer(typeof(Vector2))]
    [CustomPropertyDrawer(typeof(Float))]
    [CustomPropertyDrawer(typeof(Color))]
    private class Drawer : PropertyDrawer
    {
        private static readonly float lineHeight
            = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        private static Rect PropertyField(Rect rect, SerializedProperty property)
        {
            EditorGUI.PropertyField(rect, property);
            rect.y += lineHeight;
            return rect;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lineCount = property.isExpanded ? 4f : 1f;
            return lineHeight * lineCount;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(rect, label, property);

            // Get reference to relative properties.
            var _enabled = property.FindPropertyRelative("enabled");
            var _from = property.FindPropertyRelative("from");
            var _to = property.FindPropertyRelative("to");

            string newLabel = label.text;
            if (_enabled.boolValue)
            {
                newLabel += " [On]";
            }
            else
            {
                newLabel += " [Off]";
            }

            rect.height = lineHeight;
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, newLabel);
            rect.y += lineHeight;

            if (property.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    rect = PropertyField(rect, _enabled);

                    using (new EditorGUI.DisabledScope(!_enabled.boolValue))
                    {
                        rect = PropertyField(rect, _from);
                        rect = PropertyField(rect, _to);
                    }
                }
            }

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.EndProperty();
        }
    }
#endif
}