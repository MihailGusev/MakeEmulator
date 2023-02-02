namespace MakeEmulator.Nodes
{
    /// <summary>
    /// Represents a task in a makefile
    /// </summary>
    public abstract class TaskNodeBase<T>
    {
        public readonly string Name;
        public readonly List<string> Actions;
        public readonly List<T> Dependencies;

        protected TaskNodeBase(string name) : this(name, new List<string>(), new List<T>()) {
        }

        protected TaskNodeBase(string name, IEnumerable<string> actions, IEnumerable<T> dependencies) {
            Name = name;
            Actions = actions.ToList();
            Dependencies = dependencies.ToList();
        }
    }
}
