using System.Text;

namespace MakeEmulator.Graph
{
    /// <summary>
    /// Represents a set of related tasks in a makefile
    /// </summary>
    class TaskGraph
    {
        /// <summary>
        /// Set of tasks
        /// </summary>
        private readonly HashSet<TaskNode> nodes;

        private TaskGraph(HashSet<TaskNode> nodes) {
            this.nodes = nodes;
        }

        /// <summary>
        /// Parses makefile into <see cref="TaskGraph"/>
        /// </summary>
        /// <param name="path">Path to the makefile</param>
        /// <param name="graph">Graph in case of successful parsing</param>
        /// <param name="errorMessage">Error message in case of faulty one</param>
        /// <returns>True in case of successful parsing, false otherwise</returns>
        public static bool TryParseMakefile(string path, out TaskGraph? graph, out string? errorMessage) {
            graph = null;
            errorMessage = null;

            if (!File.Exists(path)) {
                errorMessage = $"File {path} does not exist. Make sure you typed it correctly";
                return false;
            }

            try {
                graph = ParseMakefile(path);
            }
            // Exception thrown by the method itself
            catch (FormatException ex){
                errorMessage = ex.Message;
                return false;
            }
            // All other exceptions, thrown by StreamReader
            catch(Exception ex) {
                errorMessage = $"Something went wrong during parsing process: {ex.Message}";
                return false;
            }

            return true;
        }

        public string Build(string taskName) {
            if (!nodes.TryGetValue(new TaskNode(taskName), out var existingNode)){
                return $"File does not contain a task with {taskName} Name";    
            }
            var stringBuilder = new StringBuilder();
            var seen = new HashSet<string>();
            try {
                BuildDfs(existingNode, stringBuilder, existingNode, seen);
                return stringBuilder.ToString();
            }
            catch (LoopException) {
                return "Unable to determine build order due to dependency loop";
            }
        }


        private void BuildDfs(TaskNode node, StringBuilder builder, TaskNode firstNode, HashSet<string> seen) {
            if (node.Dependencies.Contains(firstNode)) {
                throw new LoopException();
            }

            if (!seen.Add(node.Name)) {
                return;
            }

            foreach (var dependency in node.Dependencies) {
                BuildDfs(dependency, builder, firstNode, seen);
            }

            builder.AppendLine(string.Join(Environment.NewLine, node.Actions));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        private static TaskGraph ParseMakefile(string path) {
            var nodes = new HashSet<TaskNode>();
            uint currentLine = 0;
            using (var stream = new StreamReader(path)) {
                string? taskLine;
                // Two loops. Outer loop is for names + dependencies
                while ((taskLine = stream.ReadLine()) != null) {
                    currentLine++;
                    var match = TaskNode.NameAndDependency.Match(taskLine);
                    if (!match.Success) {
                        throw new FormatException($"Unable to parse the file at line {currentLine}");
                    }

                    var task = CreateOrGet(match.Groups[1].Value, nodes);

                    var dependencies = match.Groups[2].Value.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var dependency in dependencies) {
                        task.Dependencies.Add(CreateOrGet(dependency, nodes));
                    }

                    int iChar;
                    // Inner loop for actions
                    while ((iChar = stream.Peek()) != -1) {
                        // If the first char is not white space, then it must be a new line with task's Name and dependencies
                        if (!char.IsWhiteSpace((char)iChar)) {
                            break;
                        }
                        var action = stream.ReadLine()!.TrimStart();
                        currentLine++;
                        task.Actions.Add(action);
                    }
                }
            }

            return new TaskGraph(nodes);
        }

        //Side effect
        private static TaskNode CreateOrGet(string name, HashSet<TaskNode> nodes) {
            var task = new TaskNode(name);
            if (nodes.TryGetValue(task, out var existingTask)) {
                return existingTask;
            }

            nodes.Add(task);
            return task;
        }
    }
}
