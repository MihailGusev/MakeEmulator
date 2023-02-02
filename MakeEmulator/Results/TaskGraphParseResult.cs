using MakeEmulator.Graph;

namespace MakeEmulator.Results
{
    /// <summary>
    /// Represents the results of parsing makefile into <see cref="TaskGraph"/>
    /// </summary>
    public class TaskGraphParseResult : BaseResult<TaskGraph>
    {
        public TaskGraphParseResult(TaskGraph value) : base(value) {
        }

        public TaskGraphParseResult(string error) : base(error) {
        }
    }
}
