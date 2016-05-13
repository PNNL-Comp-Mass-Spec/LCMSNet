using System;
/// <summary>
/// Thrown when a custom fluidics device is invalid and cannot be constructed (or was not IRenderable)
/// </summary>
public class InvalidCustomFluidicsDeviceException : Exception
{
    public InvalidCustomFluidicsDeviceException()
    {
    }

    public InvalidCustomFluidicsDeviceException(string message) :
        base(message)
    {
    }

    public InvalidCustomFluidicsDeviceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public InvalidCustomFluidicsDeviceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    {
    }
}