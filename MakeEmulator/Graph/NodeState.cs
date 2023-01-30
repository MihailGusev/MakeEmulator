namespace MakeEmulator.Graph
{   
    /// <summary>
    /// Used to process nodes in dependency graph
    /// </summary>
    internal enum NodeState
    {
        White,  // Not visited
        Gray,   // In the process of visiting children
        Black,  // Finished visiting children
    }
}
