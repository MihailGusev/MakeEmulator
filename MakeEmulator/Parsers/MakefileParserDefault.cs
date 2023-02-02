using MakeEmulator.Nodes;
using MakeEmulator.Other;
using System.Text.RegularExpressions;

namespace MakeEmulator.Parsers
{
    public class MakefileParserDefault : MakefileParserBase
    {
        /// <summary>
        /// Mathes lines containing task's name and dependencies
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Contains 2 capturing groups - first for name and second for dependencies
        /// </para>
        /// 
        /// <para>
        /// There are 2 valid patterns:
        ///     1) Name of the task following by a colon and 1 or more dependencies:
        ///         target1: dependency1_1 dependency1_2 ... dependency1_N
        ///     2) just name of the task:
        ///         target2
        /// </para>
        /// </remarks>
        private readonly static Regex NameAndDependencies = new(@"^([a-zA-Z0-9_]+)(?>: ([a-zA-Z0-9_\s]+))?$", RegexOptions.Compiled);

        protected override bool HasNext(LineCountingStreamReader reader) => !reader.EndOfStream;

        protected override LonelyTaskNode ReadNext(LineCountingStreamReader reader) {
            var header = reader.ReadLine();

            var match = NameAndDependencies.Match(header!);
            if (!match.Success) {
                throw new FormatException($"Unable to parse the file at line {reader.CurrentLine}");
            }

            var taskName = match.Groups[1].Value;
            var dependencies = match.Groups[2].Value
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var actions = new List<string>();
            int iChar;

            while ((iChar = reader.Peek()) != -1) {
                // If the first char is not white space, then it must be a new line with task's name and dependencies
                if (!char.IsWhiteSpace((char)iChar)) {
                    break;
                }
                var action = reader.ReadLine()!.Trim();

                if (!string.IsNullOrEmpty(action)) {
                    actions.Add(action);
                }
            }

            return new LonelyTaskNode(taskName, actions, dependencies);
        }
    }
}
