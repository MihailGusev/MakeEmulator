using MakeEmulator.Nodes;

namespace MakeEmulator.Results
{
    /// <summary>
    /// Represents build result (list of tasks in the correct dependency order)
    /// so user can see their names and call their actions one by one
    /// </summary>
    public class BuildTaskResult : BaseResult<List<TaskNode>>
    {
        public BuildTaskResult(List<TaskNode> value) : base(value) {
        }

        public BuildTaskResult(string error) : base(error) {
        }
    }
}
