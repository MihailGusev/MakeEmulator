namespace MakeEmulator.Graph.Results
{
    public class TaskGraphParseResult : BaseResult<TaskGraph>
    {
        public TaskGraphParseResult(TaskGraph value) : base(value) {
        }

        public TaskGraphParseResult(string error) : base(error) {
        }
    }
}
