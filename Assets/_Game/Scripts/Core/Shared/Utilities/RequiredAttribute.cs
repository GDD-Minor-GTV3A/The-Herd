using System;
using UnityEngine;

namespace Core.Shared.Utilities
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredAttribute : PropertyAttribute { }
}
