namespace Polyfunks
{

    using UnityEditor;
    using XNodeEditor;

    /// <summary> 
    /// NodeEditor functions similarly to the Editor class, only it is xNode specific.
    /// Custom node editors should have the CustomNodeEditor attribute that defines which node type it is an editor for.
    /// </summary>
    [CustomNodeEditor(typeof(PolyfunksOutputNode))]
    public class PolyfunksOutputNodeEditor : NodeEditor
    {

        /// <summary> Called whenever the xNode editor window is updated </summary>
        public override void OnBodyGUI()
        {
            // Draw the default GUI first, so we don't have to do all of that manually.
            base.OnBodyGUI();

            // `target` points to the node, but it is of type `Node`, so cast it.
            PolyfunksOutputNode node = target as PolyfunksOutputNode;

            // Get the value from the node, and display it
            var pState = node.GetPolyfunksState();

            EditorGUILayout.Space();
            EditorGUILayout.DoubleField(ObjectNames.NicifyVariableName(nameof(pState.frequency)), pState.frequency);
            EditorGUILayout.DoubleField(ObjectNames.NicifyVariableName(nameof(pState.spin)), pState.spin);
            EditorGUILayout.DoubleField(ObjectNames.NicifyVariableName(nameof(pState.order)), pState.order);
            EditorGUILayout.DoubleField(ObjectNames.NicifyVariableName(nameof(pState.teeth)), pState.teeth);
            EditorGUILayout.DoubleField(ObjectNames.NicifyVariableName(nameof(pState.amplitude)), pState.amplitude);

        }
    }
}