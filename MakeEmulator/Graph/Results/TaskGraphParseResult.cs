namespace MakeEmulator.Graph.Results
{
    /// <summary>
    /// Represents connections between different tasks
    /// </summary>
    public class TaskGraphParseResult : BaseResult<TaskGraph>
    {
        public TaskGraphParseResult(TaskGraph value) : base(value) {
        }

        public TaskGraphParseResult(string error) : base(error) {
        }
    }
}
