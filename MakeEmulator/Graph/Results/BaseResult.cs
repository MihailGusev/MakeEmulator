﻿namespace MakeEmulator.Graph.Results
{
    /// <summary>
    /// Base class for representing the results of operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseResult<T>
    {
        public readonly bool IsSuccess;
        public readonly T? Value;
        public readonly string? Error;

        public BaseResult(T value) {
            Value = value;
            IsSuccess = true;
        }

        protected BaseResult(string error) {
            Error = error;
        }
    }
}
