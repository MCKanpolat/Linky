namespace Linky
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LinkTable
    {
        private readonly Dictionary<Type, List<LinkBuilder>> _builders = new Dictionary<Type, List<LinkBuilder>>();
        private readonly object _sync = new object();

        public void Add(LinkBuilder builder)
        {
            List<LinkBuilder> builders;
            lock (_sync)
            {
                if (!_builders.TryGetValue(builder.ModelType, out builders))
                {
                    builders = new List<LinkBuilder>();
                    _builders.Add(builder.ModelType, builders);
                }
            }
            builders.Add(builder);
        }

        public IEnumerable<Link> GetLinks(object model)
        {
            if (model == null) return null;
            List<LinkBuilder> builders;
            if (!_builders.TryGetValue(model.GetType(), out builders))
            {
                return Enumerable.Empty<Link>();
            }

            return builders.Select(b => b.GetLink(model)).Where(l => l != null);
        }

        public LinkyValueProvider GetValueProvider(Type modelType)
        {
            List<LinkBuilder> builders;
            if (!_builders.TryGetValue(modelType, out builders))
            {
                return null;
            }
            return new LinkyValueProvider(builders);
        }
    }
}