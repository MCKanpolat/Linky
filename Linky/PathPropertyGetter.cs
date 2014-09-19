namespace Linky
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Zudio.Sys.Annotations;

    internal abstract class PropertyGetterBase : ILinkPropertyGetter
    {
        private readonly PropertyInfo _property;
        private readonly bool _optional;
        private static readonly Func<object, string> FormatDateTimeOffset = obj => ((DateTimeOffset?) obj).Value.ToString("o");
        private static readonly Func<object, string> FormatDateTime = obj => ((DateTime?) obj).Value.ToString("o");
        private static readonly Func<object, string> FormatObject = obj => obj.ToString();
        private readonly Func<object, string> _format;

        protected PropertyGetterBase(PropertyInfo property, bool optional)
        {
            _property = property;
            _optional = optional;
            if (property.PropertyType == typeof (DateTime) || property.PropertyType == typeof (DateTime?))
            {
                _format = FormatDateTime;
            }
            else if (property.PropertyType == typeof (DateTimeOffset) || property.PropertyType == typeof (DateTimeOffset?))
            {
                _format = FormatDateTimeOffset;
            }
            else
            {
                _format = FormatObject;
            }
        }

        public bool Optional { get { return _optional; } }
        public abstract bool GetValue(object obj, ref string path, List<string> queryStringParameters);

        protected string GetFormattedValue(object obj)
        {
            var value = _property.GetValue(obj);
            if (value == null) return null;
            return Uri.EscapeDataString(_format(value));
        }
    }

    internal class PathPropertyGetter : PropertyGetterBase
    {
        private readonly Regex _parameter;

        public PathPropertyGetter(Regex parameter, PropertyInfo property, bool optional) : base(property, optional)
        {
            _parameter = parameter;
        }

        public override bool GetValue(object obj, ref string path, List<string> queryStringParameters)
        {
            var str = GetFormattedValue(obj) ?? string.Empty;
            bool found = (!string.IsNullOrWhiteSpace(str)) || Optional;
            path = _parameter.Replace(path, str);
            return found;
        }
    }
}