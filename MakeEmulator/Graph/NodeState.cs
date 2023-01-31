﻿namespace MakeEmulator.Graph
{   
    /// <summary>
    /// Used to identify loops and repeated dependencies in the graph
    /// </summary>
    internal enum NodeState
    {
        White,  // Not visited
        Gray,   // In the process of visiting children
        Black,  // Finished visiting children
    }
}
