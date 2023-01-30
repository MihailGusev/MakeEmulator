namespace MakeEmulator.Graph.Results
{   
    public class BuildTaskResult : BaseResult<List<TaskNode>>
    {
        public BuildTaskResult(List<TaskNode> value) : base(value) {
        }

        public BuildTaskResult(string error) : base(error) {
        }
    }
}
