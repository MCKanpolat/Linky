namespace Linky
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http.Description;

    public class LinkBuilder
    {
        private readonly Type _modelType;
        private readonly string _rel;
        private readonly string _uriTemplate;
        private readonly Dictionary<string, PropertyInfo> _mappings;
        private LinkBuilder(Type modelType, string rel, string uriTemplate, Dictionary<string, PropertyInfo> mappings) 
        {
            _modelType = modelType;
            _rel = rel;
            _uriTemplate = uriTemplate;
            _mappings = mappings;
        }

        public static LinkBuilder Create(Type modelType, string rel, string uriTemplate, IEnumerable<ApiParameterDescription> parameterDescriptions)
        {
            var mappings = new Dictionary<string, PropertyInfo>();
            var parameters = parameterDescriptions.Where(p => p.Source == ApiParameterSource.FromUri).ToDictionary(s => s.Name, s => s.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var property in modelType.GetProperties())
            {
                string parameter;
                if (parameters.TryGetValue(property.Name, out parameter))
                {
                    mappings.Add("{" + parameter + "}", property);
                }
            }

            if (mappings.Count != parameters.Count) return null;
            return new LinkBuilder(modelType, rel, uriTemplate, mappings);
        }

        public Type ModelType
        {
            get { return _modelType; }
        }

        public Link GetLink(object obj)
        {
            var uri = _uriTemplate;
            foreach (var mapping in _mappings)
            {
                var value = mapping.Value.GetValue(obj);
                if (value == null) return null;
                uri = uri.Replace(mapping.Key, value.ToString());
            }

            return new Link(_rel, uri);
        }
    }
}