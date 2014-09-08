namespace Linky
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LinkTable
    {
        private readonly List<LinkBuilder> _builders = new List<LinkBuilder>();
        private readonly object _sync = new object();

        public void Add(LinkBuilder builder)
        {
            lock (_sync)
            {
                _builders.Add(builder);
            }
        }

        public LinkyValueProvider GetValueProvider(Type modelType)
        {
            var builders = _builders.Where(b => b.ModelType.IsAssignableFrom(modelType)).Distinct(LinkBuilder.DefaultComparer).ToList();
            if (builders.Count == 0) return null;
            return new LinkyValueProvider(builders);
        }
    }
}