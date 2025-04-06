namespace Polyfunks
{

    using UnityEditor;
    using XNodeEditor;

    /// <summary> 
    /// NodeEditor functions similarly to the Editor class, only it is xNode specific.
    /// Custom node editors should have the CustomNodeEditor attribute that defines which node type it is an editor for.
    /// </summary>
    [CustomNodeEditor(typeof(DisplayDoubleNode))]
    public class DisplayDoubleEditor : NodeEditor
    {

        /// <summary> Called whenever the xNode editor window is updated </summary>
        public override void OnBodyGUI()
        {
            // Draw the default GUI first, so we don't have to do all of that manually.
            base.OnBodyGUI();

            // `target` points to the node, but it is of type `Node`, so cast it.
            DisplayDoubleNode ddn = target as DisplayDoubleNode;

            // Get the value from the node, and display it
            var val = ddn.GetEditorValue();
            EditorGUILayout.LabelField(val.ToString());

            // consider displaying more values here: min/max range... avg...???
        }
    }
}