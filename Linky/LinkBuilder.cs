namespace Linky
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http.Description;

    public class LinkBuilder
    {
        private readonly IPropertyGetter[] _getters;
        private readonly bool _queryString;
        private readonly Type _modelType;
        private readonly string _rel;
        private readonly string _uriTemplate;

        private LinkBuilder(Type modelType, string rel, IPropertyGetter[] getters, string uriTemplate)
        {
            _modelType = modelType;
            _rel = rel;
            _getters = getters;
            _uriTemplate = uriTemplate;
            _queryString = _getters.OfType<QueryStringPropertyGetter>().Any();
        }

        public Type ModelType
        {
            get { return _modelType; }
        }

        public static LinkBuilder Create(Type modelType, string rel, ApiDescription apiDescription)
        {
            var getters = new List<IPropertyGetter>();
            Dictionary<string, Tuple<string, bool>> parameters =
                apiDescription.ParameterDescriptions.Where(p => p.Source == ApiParameterSource.FromUri)
                    .ToDictionary(s => s.Name, s => Tuple.Create('{' + s.Name + '}', s.ParameterDescriptor.IsOptional), StringComparer.OrdinalIgnoreCase);

            int questionMark = apiDescription.RelativePath.IndexOf('?');

            string path;
            string queryString;
            string[] queryStringSegments;

            if (questionMark > -1)
            {
                path = apiDescription.RelativePath.Substring(0, questionMark);
                queryString = apiDescription.RelativePath.Substring(questionMark + 1);
                queryStringSegments = queryString.Split('&');
            }
            else
            {
                path = apiDescription.RelativePath;
                queryString = string.Empty;
                queryStringSegments = new string[0];
            }

            foreach (PropertyInfo property in modelType.GetProperties())
            {
                var parameterDescription =
                    apiDescription.ParameterDescriptions.FirstOrDefault(
                        p => p.Source == ApiParameterSource.FromUri && p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                if (parameterDescription != null)
                {
                    var name = '{' + parameterDescription.Name + '}';
                    if (path.Contains(name))
                    {
                        getters.Add(new PathPropertyGetter(name, property, parameterDescription.ParameterDescriptor.IsOptional));
                    }
                    else if (queryString.Contains(name))
                    {
                        var segment = queryStringSegments.FirstOrDefault(s => s.Contains(name));
                        getters.Add(new QueryStringPropertyGetter(name, segment, property, parameterDescription.ParameterDescriptor.IsOptional));
                    }
                    
                }
            }

            if (getters.Count(g => !g.Optional) != parameters.Count(p => !p.Value.Item2)) return null;
            return new LinkBuilder(modelType, rel, getters.ToArray(), apiDescription.Route.RouteTemplate);
        }

        public Link GetLink(object obj)
        {
            var queryStringList = _queryString ? new List<string>() : null;
            var path = _uriTemplate;
            if (_getters.Any(getter => !getter.GetValue(obj, ref path, queryStringList)))
            {
                return null;
            }

            if (queryStringList != null && queryStringList.Any())
            {
                path = path + string.Join("&", queryStringList);
            }

            return new Link(_rel, path);
        }
    }
}