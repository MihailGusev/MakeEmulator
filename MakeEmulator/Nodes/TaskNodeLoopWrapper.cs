namespace MakeEmulator.Nodes
{
    /// <summary>
    /// Stores additional information about <see cref="Nodes.TaskNode"/>
    /// helping to detect loops
    /// </summary>
    internal class TaskNodeLoopWrapper
    {
        /// <summary>
        /// The number of dependencies that have been processed
        /// </summary>
        private int ProcessedCount;

        public readonly TaskNode TaskNode;

        /// <summary>
        /// True if task and every dependency has been processed. 
        /// </summary>
        /// 
        /// <remarks>
        /// Note, that <see cref="HasNext"/> being false does not mean, that <see cref="IsProcessed"/> is true:
        /// 1. We are processing a task and this task has one unprocessed dependency left
        /// 2. HasNext returns false
        /// 3. We read this last dependency
        /// 4. If we check HasNext now, it will be false, just like IsProcessed, 
        /// because both task and last dependency are still being processed
        /// </remarks>
        public bool IsProcessed { get; internal set; }

        public TaskNodeLoopWrapper(TaskNode taskNode) {
            TaskNode = taskNode;
        }

        public bool HasNext() => ProcessedCount < TaskNode.Dependencies.Count;

        public TaskNode GetNext() => TaskNode.Dependencies[ProcessedCount++];
    }
}
