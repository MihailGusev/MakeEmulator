namespace MakeEmulator.Other
{
    /// <summary>
    /// Thrown when dependency loop is detected
    /// </summary>
    internal class LoopException : Exception
    {
        public LoopException(string message) : base(message) {
        }
    }
}
