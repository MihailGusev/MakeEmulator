using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MakeEmulator.Graph
{
    /// <summary>
    /// Represents a task in a makefile
    /// </summary>
    [DebuggerDisplay("{Name} | {Dependencies.Count} | {Actions.Count}")]
    public class TaskNode
    {
        /// <summary>
        /// Mathes lines containing task's name and dependencies
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Contains 2 capturing groups - first for name and second for dependencies
        /// </para>
        /// 
        /// <para>
        /// There are 2 valid patterns:
        ///     1) Name of the task following by a colon and 1 or more dependencies:
        ///         target1: dependency1_1 dependency1_2 ... dependency1_N
        ///     2) just name of the task:
        ///         target2
        /// </para>
        /// </remarks>
        internal static readonly Regex NameAndDependencies = new(@"^([a-zA-Z0-9_]+)(?>: ([a-zA-Z0-9_\s]+))?$", RegexOptions.Compiled);

        /// <summary>
        /// It is possible to solve the problem without Name and HashCode+Equals
        /// (using only keys from the graph), thus removing string duplication
        /// but i thought that nodes shouldn't depend on the graph to get their own dependencies
        /// </summary>
        public readonly string Name;
        public readonly HashSet<TaskNode> Dependencies = new();
        public readonly List<string> Actions = new();

        /// <summary>
        /// False for dependencies that haven't yet 
        /// been confirmed to be present in the makefile as tasks
        /// </summary>
        /// 
        /// <example>
        /// Here T2's ConfirmedTask property is false at line 1 and becomes true at line 2:
        /// -----Makefile start------
        /// T1: T2
        /// T2
        /// -----Makefile end--------
        /// </example>
        public bool ConfirmedTask { get; internal set; }

        internal NodeState State { get; set; } = NodeState.White;

        public TaskNode(string name) {
            Name = name;
        }

        #region Equals & GetHashCode

        public override bool Equals(object? obj) {
            return obj is TaskNode node &&
                   Name == node.Name;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name);
        }

        #endregion
    }
}
