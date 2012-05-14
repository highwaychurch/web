﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Highway.Shared.Mvc.Validation
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        // Note: we don't inherit from RequiredAttribute as some elements of the MVC
        // framework specifically look for it and choose not to add a RequiredValidator
        // for non-nullable fields if one is found. This would be invalid if we inherited
        // from it as obviously our RequiredIf only applies if a condition is satisfied.
        // Therefore we're using a private instance of one just so we can reuse the IsValid
        // logic, and don't need to rewrite it.
        private readonly RequiredAttribute _innerAttribute = new RequiredAttribute();
        public string DependentProperty { get; set; }
        public object[] TargetValue { get; set; }
        public bool AlwaysVisible { get; set; }

        public RequiredIfAttribute(string dependentProperty, params object[] targetValue)
        {
            if (dependentProperty == null) throw new ArgumentNullException("dependentProperty");
            if (targetValue == null) throw new ArgumentNullException("targetValue");

            DependentProperty = dependentProperty;
            TargetValue = targetValue;
        }

        public override bool IsValid(object value)
        {
            return _innerAttribute.IsValid(value);
        }
    }
}