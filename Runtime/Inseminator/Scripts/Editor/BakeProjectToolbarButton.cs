namespace Inseminator.Scripts.Editor
{
    using ReflectionBaking;
    using UnityEditor;
    using UnityEngine;
    using UnityToolbarExtender;
    
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold,
                fixedWidth = 160
            };
        }
    }

    [InitializeOnLoad()]
    public static class BakeProjectToolbarButton
    {
        static BakeProjectToolbarButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(DrawBakeGUI);
        }

        private static void DrawBakeGUI()
        {
            GUILayout.FlexibleSpace();
            
            if(GUILayout.Button(new GUIContent("Bake Dependencies", "Bake all Inseminator dependencies in project into file."), ToolbarStyles.commandButtonStyle))
            {
                ReflectionBaker.Instance.BakeAll();
            }
        }
    }
}