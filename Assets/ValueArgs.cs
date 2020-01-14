using System;

/// <summary>
/// Inherited event arguments with a single templated value.
/// </summary>
public class ValueArgs<T>
    : EventArgs
{
    public readonly T Value;

    public ValueArgs(T value)
    {
        Value = value;
    }
}