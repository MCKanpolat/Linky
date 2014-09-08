namespace Linky
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class NullPropertyGetter : ILinkPropertyGetter
    {
        private readonly Regex _parameter;

        public bool Optional
        {
            get { return true; }
        }

        public NullPropertyGetter(string name)
        {
            _parameter = new Regex(@"\/?{\*?" + name + @"[:\?]?.*?\}");
        }

        public bool GetValue(object obj, ref string path, List<string> queryStringParameters)
        {
            path = _parameter.Replace(path, "");
            return true;
        }
    }
}