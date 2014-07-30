namespace Linky
{
    using System.Collections.Generic;
    using System.Reflection;

    internal class QueryStringPropertyGetter : IPropertyGetter
    {
        private readonly string _parameter;
        private readonly string _template;
        private readonly PropertyInfo _property;
        private readonly bool _optional;

        public bool Optional
        {
            get { return _optional; }
        }

        public QueryStringPropertyGetter(string parameter, string template, PropertyInfo property, bool optional)
        {
            _parameter = parameter;
            _template = template;
            _property = property;
            _optional = optional;
        }

        public bool GetValue(object obj, ref string path, List<string> queryStringParameters)
        {
            var value = _property.GetValue(obj);
            if (value == null) return _optional;
            queryStringParameters.Add(_template.Replace(_parameter, obj.ToString()));
            return true;
        }
    }
}