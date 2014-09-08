namespace Linky
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web.Http.Controllers;
    using System.Web.Http.Description;

    public class LinkBuilder
    {
        private sealed class ModelTypeRelEqualityComparer : IEqualityComparer<LinkBuilder>
        {
            public bool Equals(LinkBuilder x, LinkBuilder y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x._modelType == y._modelType && string.Equals(x._rel, y._rel);
            }

            public int GetHashCode(LinkBuilder obj)
            {
                unchecked
                {
                    return ((obj._modelType != null ? obj._modelType.GetHashCode() : 0)*397) ^ (obj._rel != null ? obj._rel.GetHashCode() : 0);
                }
            }
        }

        private static readonly IEqualityComparer<LinkBuilder> DefaultComparerInstance = new ModelTypeRelEqualityComparer();

        public static IEqualityComparer<LinkBuilder> DefaultComparer
        {
            get { return DefaultComparerInstance; }
        }

        private readonly ILinkPropertyGetter[] _getters;
        private readonly bool _queryString;
        private readonly Type _modelType;
        private readonly string _rel;
        private readonly string _uriTemplate;

        private LinkBuilder(Type modelType, string rel, ILinkPropertyGetter[] getters, string uriTemplate)
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

        public static LinkBuilder Create(LinksFromAttribute attribute, ApiDescription apiDescription)
        {
            var modelTypeName = attribute.ModelType.Name;
            var getters = new List<ILinkPropertyGetter>();
            Dictionary<string, Tuple<string, bool>> parameters =
                apiDescription.ParameterDescriptions.Where(p => p.Source == ApiParameterSource.FromUri)
                    .ToDictionary(s => s.Name, s => Tuple.Create("{" + s.Name + "}", IsOptional(s, attribute)), StringComparer.OrdinalIgnoreCase);

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

            foreach (var parameterDescription in apiDescription.ParameterDescriptions.Where(p => p.Source == ApiParameterSource.FromUri))
            {
                PropertyInfo property = null;
                if (attribute.Resolve != null)
                {
                    int resolveIndex = Array.FindIndex(attribute.Resolve, s => string.Equals(s, parameterDescription.Name, StringComparison.OrdinalIgnoreCase));
                    if (resolveIndex >= 0)
                    {
                        property = attribute.ModelType.GetProperty(attribute.Resolve[resolveIndex + 1]);
                    }
                }

                if (property == null)
                {
                    property = attribute.ModelType.GetProperties().FirstOrDefault(p => p.Name.Equals(parameterDescription.Name, StringComparison.OrdinalIgnoreCase));
                }

                var regex = new Regex(@"\{\*?" + parameterDescription.Name + @"[:\?]?.*?\}");
                if (property != null)
                {
                    bool isOptional = IsOptional(parameterDescription, attribute);
                    if (regex.IsMatch(path))
                    {
                        getters.Add(new PathPropertyGetter(regex, property, isOptional));
                    }
                    else if (regex.IsMatch(queryString))
                    {
                        var segment = queryStringSegments.FirstOrDefault(regex.IsMatch);
                        getters.Add(new QueryStringPropertyGetter(regex, segment, property, isOptional));
                    }
                }
                else
                {
                    if (regex.IsMatch(path))
                    {
                        getters.Add(new NullPropertyGetter(parameterDescription.Name));
                    }
                }
            }

            if (getters.Count(g => !g.Optional) != parameters.Count(p => !p.Value.Item2)) return null;
            return new LinkBuilder(attribute.ModelType, attribute.Rel, getters.ToArray(), apiDescription.Route.RouteTemplate);
        }

        private static bool IsOptional(ApiParameterDescription parameterDescription, LinksFromAttribute attribute)
        {
            return (parameterDescription.ParameterDescriptor != null && parameterDescription.ParameterDescriptor.IsOptional)
                && !attribute.IsRequired(parameterDescription.Name);
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
                path = path + "?" + string.Join("&", queryStringList);
            }

            return new Link(_rel, path);
        }
    }
}