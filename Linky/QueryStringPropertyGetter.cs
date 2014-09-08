namespace Linky
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;

    internal class QueryStringPropertyGetter : PropertyGetterBase
    {
        private readonly Regex _parameter;
        private readonly string _template;

        public QueryStringPropertyGetter(Regex parameter, string template, PropertyInfo property, bool optional) : base(property, optional)
        {
            _parameter = parameter;
            _template = template;
        }

        public override bool GetValue(object obj, ref string path, List<string> queryStringParameters)
        {
            var str = GetFormattedValue(obj);
            if (string.IsNullOrWhiteSpace(str)) return Optional;
            queryStringParameters.Add(_parameter.Replace(_template, str));
            return true;
        }
    }
}