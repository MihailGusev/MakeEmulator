namespace MakeEmulator.Graph.Exceptions
{
    /// <summary>
    /// Thrown when one task depends on another, that is not present in the graph
    /// </summary>
    internal class TaskNotConfirmedException : Exception
    {
        public TaskNotConfirmedException(string taskName) : base($"Makefile does not contain \"{taskName}\" task")
        {
        }
    }
}
