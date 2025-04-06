namespace Polyfunks
{
    using System;
    using XNode;

    // This is used in xNode to connect nodes that send or receive Gates.
    // Those nodes are required to implement OnGate() events which trigger when e.g. a master clock fires.
    // this makes it possible to connect gates without worrying about the ordering of stuff, and multiple gate inputs will trigger the OnGate event on a node.
    [Serializable]
    public class Gate
    {
        internal static void FireGate(Node node, string gateOutName)
        {
            // get the port
            var gateOutPort = node.GetOutputPort(gateOutName);
            // loop through each connection of this port
            for (int i = 0; i < gateOutPort.ConnectionCount; i++)
            {
                var otherPortIn = gateOutPort.GetConnection(i);
                if (otherPortIn != null)
                {
                    // if other node is a gate node
                    var otherGateNode = (otherPortIn.node as IPolyfunksGate);
                    if (otherGateNode != null)
                    {
                        // fire gate event w/ ref to output node jic
                        otherGateNode.OnGate(gateOutPort, otherPortIn);
                    }
                }
            }
        }
    }


}