using UnityEditor;

namespace Assets.Editor
{
    class PointMaterialInspector : ShaderGUI
    {
        public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
        {
            editor.ShaderProperty(FindProperty("_PointSize", props), "Point Size");
        }
    }
}
