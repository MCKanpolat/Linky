namespace Linky
{
    using System.Collections.Generic;
    using System.Reflection;

    internal class PathPropertyGetter : IPropertyGetter
    {
        private readonly bool _optional;
        private readonly string _parameter;
        private readonly PropertyInfo _property;

        public PathPropertyGetter(string parameter, PropertyInfo property, bool optional)
        {
            _parameter = parameter;
            _property = property;
            _optional = optional;
        }

        public bool Optional
        {
            get { return _optional; }
        }

        public bool GetValue(object obj, ref string path, List<string> queryStringParameters)
        {
            object value = _property.GetValue(obj);
            if (IsNullOrWhitespace(value)) return _optional;
            path = path.Replace(_parameter, value.ToString());
            return true;
        }

        private bool IsNullOrWhitespace(object value)
        {
            if (_property.PropertyType == typeof (string))
            {
                return string.IsNullOrWhiteSpace((string) value);
            }
            return value == null;
        }
    }
}