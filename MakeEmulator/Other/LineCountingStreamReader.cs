namespace MakeEmulator.Other
{
    public class LineCountingStreamReader : StreamReader
    {
        public int CurrentLine { get; private set; }

        public LineCountingStreamReader(string path) : base(path) {
        }

        public override string? ReadLine() {
            CurrentLine++;
            return base.ReadLine();
        }
    }
}
