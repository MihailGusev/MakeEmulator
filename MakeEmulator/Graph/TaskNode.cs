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
        internal static readonly Regex NameAndDependencies = new(@"^(\S*\b)(?>: (.+))?$", RegexOptions.Compiled);

        public readonly string Name;
        public readonly HashSet<TaskNode> Dependencies = new();
        public readonly List<string> Actions = new();

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
