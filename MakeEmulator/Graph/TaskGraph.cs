using MakeEmulator.Graph.Results;

namespace MakeEmulator.Graph
{
    /// <summary>
    /// Represents a set of related tasks in a makefile
    /// </summary>
    public class TaskGraph
    {
        /// <summary>
        /// Set of tasks. Key - name of the task, value - task itself
        /// </summary>
        private readonly Dictionary<string, TaskNode> nodes;


        private TaskGraph(Dictionary<string, TaskNode> nodes) {
            this.nodes = nodes;
        }

        /// <summary>
        /// Parses makefile into <see cref="TaskGraph"/>
        /// </summary>
        /// <param name="path">Path to the makefile</param>
        public static TaskGraphParseResult ParseMakefile(string path) {
            if (!File.Exists(path)) {
                return new ($"File {path} does not exist. Make sure you typed it correctly");
            }

            try {
                var graph = ParseMakefileInternal(path);
                return new (graph);
            }
            // Exception thrown by the method itself
            catch (FormatException ex) {
                return new (ex.Message);
            }
            // All other exceptions, thrown by StreamReader
            catch (Exception ex) {
                return new ($"Something went wrong during parsing process: {ex.Message}");
            }
        }

        /// <summary>
        /// Looks for dependencies of a given task and returns them in the correct build order
        /// </summary>
        /// <param name="taskName">Name of the task to build</param>
        public BuildTaskResult Build(string taskName) {
            if (!nodes.TryGetValue(taskName, out var taskNode)) {
                return new ($"Makefile does not contain a task with {taskName} name");
            }

            var tasksToRun = new List<TaskNode>();
            try {
                BuildDfs(taskNode, tasksToRun);
            }
            catch (LoopException) {
                return new("Unable to determine build order due to dependency loop");
            }

            foreach(var task in tasksToRun) {
                // Revert state back, so we can call method for another task
                task.State = NodeState.White;
            }
            return new (tasksToRun);
        }


        /// <summary>
        /// Parses makefile into <see cref="TaskGraph"/>
        /// </summary>
        /// <param name="path">Path to the makefile</param>
        /// <exception cref="FormatException"></exception>
        private static TaskGraph ParseMakefileInternal(string path) {
            // Keep track of current line number to point the user to it in case of a failure
            uint currentLine = 0;
            // Processed tasks. Key - name of the task, value - task itself
            var nodes = new Dictionary<string, TaskNode>();

            using (var stream = new StreamReader(path)) {
                string? nameAndDependencies;
                // Two loops. Outer loop is for names + dependencies
                while ((nameAndDependencies = stream.ReadLine()) != null) {
                    currentLine++;
                    var match = TaskNode.NameAndDependencies.Match(nameAndDependencies);
                    if (!match.Success) {
                        throw new FormatException($"Unable to parse the file at line {currentLine}");
                    }

                    var task = GetOrCreate(match.Groups[1].Value, nodes);

                    var dependencies = match.Groups[2].Value.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var dependency in dependencies) {
                        task.Dependencies.Add(GetOrCreate(dependency, nodes));
                    }

                    int iChar;
                    // Inner loop for actions
                    while ((iChar = stream.Peek()) != -1) {
                        // If the first char is not white space, then it must be a new line with task's name and dependencies
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

        /// <summary>
        /// Returns existing <see cref="TaskNode"/> from the dictionary or creates and adds a new one
        /// </summary>
        /// <param name="name">Name of the task</param>
        /// <param name="nodes">Existing nodes</param>
        private static TaskNode GetOrCreate(string name, Dictionary<string, TaskNode> nodes) {
            if (nodes.TryGetValue(name, out var existingTask)) {
                return existingTask;
            }

            var task = new TaskNode(name);
            nodes.Add(name, task);
            return task;
        }

        /// <summary>
        /// Performs depth first search starting from <paramref name="node"/>.
        /// </summary>
        /// <param name="node">Current node</param>
        /// <param name="blackNodes">List of processed</param>
        /// <exception cref="LoopException"></exception>
        private static void BuildDfs(TaskNode node, List<TaskNode> blackNodes) {
            // Black color means, that we encountered fully processed node
            if (node.State == NodeState.Black) {
                return;
            }

            // Gray color here means that we encountered a node
            // before processing it's children, so there's a loop
            if (node.State == NodeState.Gray) {
                throw new LoopException();
            }

            node.State = NodeState.Gray;

            foreach (var dependency in node.Dependencies) {
                BuildDfs(dependency, blackNodes);
            }

            node.State = NodeState.Black;
            blackNodes.Add(node);
        }
    }
}
