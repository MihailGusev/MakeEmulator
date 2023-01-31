namespace MakeEmulator.Graph.Exceptions
{
    /// <summary>
    /// Thrown when one task depends on another, that is not present in the graph
    /// </summary>
    internal class TaskNotConfirmedException : Exception
    {
        public TaskNotConfirmedException(string taskName) : base($"Unable to find a task for dependency named \"{taskName}\"")
        {
        }
    }
}
