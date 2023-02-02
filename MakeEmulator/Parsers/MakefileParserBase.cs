using MakeEmulator.Nodes;
using MakeEmulator.Other;

namespace MakeEmulator.Parsers
{
    /// <summary>
    /// There may be many makefile formats.
    /// Children represent custom parsing algorithms for these formats.
    /// </summary>
    public abstract class MakefileParserBase
    {
        protected abstract bool HasNext(LineCountingStreamReader reader);

        protected abstract LonelyTaskNode ReadNext(LineCountingStreamReader reader);

        /// <summary>
        /// Read every task from the makefile
        /// </summary>
        internal List<LonelyTaskNode> ReadAll(string path) {
            var result = new List<LonelyTaskNode>();

            using (var reader = new LineCountingStreamReader(path)) {
                while (HasNext(reader)) {
                    result.Add(ReadNext(reader));
                }
            }

            return result;
        }
    }
}
