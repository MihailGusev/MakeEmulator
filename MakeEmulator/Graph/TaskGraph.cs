using MakeEmulator.Nodes;
using MakeEmulator.Other;
using MakeEmulator.Results;

namespace MakeEmulator.Graph
{
    /// <summary>
    /// Represents related tasks in a makefile
    /// </summary>
    public class TaskGraph
    {
        /// <summary>
        /// Key - name of the task, value - task itself
        /// </summary>
        private readonly Dictionary<string, TaskNode> nodes;

        public TaskGraph(Dictionary<string, TaskNode> nodes) {
            this.nodes = nodes;
        }

        /// <summary>
        /// Looks for dependencies of a given task and returns them in the correct build order
        /// </summary>
        /// <param name="taskName">Name of the task to build</param>
        public BuildTaskResult Build(string taskName) {
            if (!nodes.TryGetValue(taskName, out var taskNode)) {
                return new($"Makefile does not contain a task named \"{taskName}\"");
            }
            try {
                return new(BuildPrivate(taskNode));
            }
            catch (LoopException ex) {
                return new(ex.Message);
            }
        }

        public List<TaskNode> BuildPrivate(TaskNode initial) {
            // Dictionary and stack operate on the same wrappers.
            // Dictionary is for checking if node was encountered before
            // and stack is for depth first search
            var seen = new Dictionary<string, TaskNodeLoopWrapper>();
            var nodesInProcess = new Stack<TaskNodeLoopWrapper>();

            void AddNew(TaskNode node) {
                var wrapper = new TaskNodeLoopWrapper(node);
                seen.Add(node.Name, wrapper);
                nodesInProcess.Push(wrapper);
            }

            AddNew(initial);

            var result = new List<TaskNode>();

            while (nodesInProcess.Count > 0) {
                var topWrapper = nodesInProcess.Peek();

                // Check if top element has at least one unprocessed dependency
                if (!topWrapper.HasNext()) {
                    // If not, then this task is fully processed - mark it and add to the result
                    nodesInProcess.Pop();
                    result.Add(topWrapper.TaskNode);
                    topWrapper.IsProcessed = true;
                    continue;
                }

                // Otherwise get next dependency
                var next = topWrapper.GetNext();

                // If we haven't seen it - add it to the stack and deal with it on the next iteration
                if (!seen.TryGetValue(next.Name, out var dependencyWrapper)) {
                    AddNew(next);
                    continue;
                }

                // Otherwise it might be one of the two:
                // 1) dependency loop

                // 2) two tasks have the same dependency:
                // T1: T2 T3
                // T2: T3
                // T3 will be encountered twice, because T1 and T2 both depend on it
                if (!dependencyWrapper.IsProcessed) {
                    var loopedNodes = nodesInProcess.Select(n => n.TaskNode.Name).Reverse().ToList();
                    loopedNodes.Add(loopedNodes[0]);

                    var joined = string.Join(" → ", loopedNodes);
                    var message = $"Unable to build {initial.Name} due to dependency loop: {Environment.NewLine}{joined}";
                    throw new LoopException(message);
                }
            }

            return result;
        }
    }
}
