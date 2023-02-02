using System.Diagnostics;

namespace MakeEmulator.Nodes
{
    /// <summary>
    /// Contains information about a task including links to dependency tasks
    /// </summary>
    [DebuggerDisplay("{Name} | {Dependencies.Count} | {Actions.Count}")]
    public class TaskNode : TaskNodeBase<TaskNode>
    {
        public TaskNode(string name) : base(name) { }
    }
}
