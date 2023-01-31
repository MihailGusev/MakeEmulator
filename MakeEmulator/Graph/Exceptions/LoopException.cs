namespace MakeEmulator.Graph.Exceptions
{
    /// <summary>
    /// Thrown when dependency loop is detected
    /// </summary>
    internal class LoopException : Exception
    {
        public LoopException(string message) : base(message)
        {
        }
    }
}
