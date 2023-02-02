using MakeEmulator.Graph;
using MakeEmulator.Nodes;
using MakeEmulator.Results;

namespace MakeEmulator.Parsers
{
    /// <summary>
    /// Parses makefile into <see cref="TaskGraph"/>
    /// </summary>
    public class TaskGraphParser
    {
        private readonly MakefileParserBase makefileParser;

        /// <summary>
        /// Default parser. Uses <see cref="MakefileParserDefault"/> to get individual tasks.
        /// </summary>
        public TaskGraphParser() : this(new MakefileParserDefault()) {
        }

        public TaskGraphParser(MakefileParserBase makefileParser) {
            this.makefileParser = makefileParser;
        }

        /// <summary>
        /// Reads contents of a makefile and returns TaskGraph.
        /// </summary>
        /// <param name="path">Path to makefile</param>
        /// 
        /// <remarks>
        /// Algorithm:
        /// <para></para>
        /// 
        /// 1. Read and parse the whole makefile into tasks. 
        /// It is possible to start checking for errors while reading, but the code will be harder to understand.
        /// <para></para>
        /// 
        /// 2. Loop over the tasks and check it for problems.
        /// It's better to return all of them, so user can fix it before trying to parse the file again.
        /// <para></para>
        /// 
        /// 3. Link tasks together.
        /// We can check for loops right after linking, but: <para></para>
        ///     - build phase will be shorter, but similar anyway <para></para>
        ///     - checking for every loop doesn't have that much sense 
        ///     as checking for every data error (like unresolved dependency).
        /// </remarks>
        public TaskGraphParseResult Parse(string path) {
            if (!File.Exists(path)) {
                return new($"File {path} does not exist. Make sure you typed it correctly");
            }

            List<LonelyTaskNode> lonelyNodes;
            try {
                lonelyNodes = makefileParser.ReadAll(path);
            }
            catch (Exception ex) {
                return new($"Something went wrong during parsing process: {ex.Message}");
            }

            var errors = GetDataErrors(lonelyNodes);
            if (errors.Count > 0) {
                var joined = string.Join(Environment.NewLine, errors);
                return new($"Makefile has the following problems:{Environment.NewLine}{joined}");
            }

            var graph = CreateGraph(lonelyNodes);

            return new(graph);
        }

        /// <summary>
        /// Checks nodes for problems that makefile may have 
        /// (except loops - they are checked after linking at the build stage)
        /// </summary>
        private List<string> GetDataErrors(List<LonelyTaskNode> lonelyTaskNodes) {
            var result = new List<string>();

            var tasks = new HashSet<string>();
            var dependencies = new HashSet<string>();

            foreach (var node in lonelyTaskNodes) {
                if (!tasks.Add(node.Name)) {
                    result.Add($"Task {node.Name} occurs more than once");
                }

                if (node.Dependencies.GroupBy(d => d).Any(g => g.Count() > 1)) {
                    result.Add($"Task {node.Name} has duplicate dependencies");
                }
                dependencies.UnionWith(node.Dependencies);
            }

            dependencies.ExceptWith(tasks);

            if (dependencies.Count > 0) {
                var list = string.Join(", ", dependencies);
                result.Add($"Dependency(es) {list} do not have corresponding task");
            }

            return result;
        }

        /// <summary>
        /// Transforms lonely nodes into task nodes and links them
        /// </summary>
        private TaskGraph CreateGraph(List<LonelyTaskNode> lonelyNodes) {
            var graphNodes = new Dictionary<string, TaskNode>();

            TaskNode GetOrCreate(string name) {
                if (!graphNodes.TryGetValue(name, out var node)) {
                    node = new TaskNode(name);
                    graphNodes.Add(name, node);
                }
                return node;
            }

            foreach (var lonelyNode in lonelyNodes) {
                var taskNode = GetOrCreate(lonelyNode.Name);

                lonelyNode.Actions.ForEach(taskNode.Actions.Add);

                foreach (var dependency in lonelyNode.Dependencies) {
                    var dependencyNode = GetOrCreate(dependency);
                    taskNode.Dependencies.Add(dependencyNode);
                }
            }

            return new TaskGraph(graphNodes);
        }
    }
}
