﻿using System;

namespace ManualMovementsManager.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DataTypeValidatorAttribute : Attribute
    {
        public string Type { get; }

        public DataTypeValidatorAttribute(string type)
        {
            Type = type;
        }
    }

}
