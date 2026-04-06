using System;

namespace ExecutionContextLib
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TrackExecutionAttribute : Attribute
    {
    }
}