namespace Linky
{
    using System;

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
    }
}