namespace MakeEmulator.Nodes
{
    /// <summary>
    /// Only contains information and not linked dependencies.
    /// </summary>
    public class LonelyTaskNode : TaskNodeBase<string>
    {
        public LonelyTaskNode(string name, IEnumerable<string> dependencies, IEnumerable<string> actions)
            : base(name, dependencies, actions) {
        }
    }
}
