namespace Linky
{
    using System;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class LinksFromAttribute : Attribute
    {
        private readonly Type _modelType;
        private readonly string _rel;

        public LinksFromAttribute(Type modelType, string rel)
        {
            _modelType = modelType;
            _rel = rel;
        }

        public Type ModelType
        {
            get { return _modelType; }
        }

        public string Rel
        {
            get { return _rel; }
        }

        public string[] Required { get; set; }

        public object Resolve { get; set; }

        public bool IsRequired(string parameterName)
        {
            return Required != null && Required.Contains(parameterName, StringComparer.OrdinalIgnoreCase);
        }
    }
}