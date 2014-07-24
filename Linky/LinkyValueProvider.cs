namespace Linky
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Serialization;

    public class LinkyValueProvider : IValueProvider
    {
        private readonly List<LinkBuilder> _builders;

        public LinkyValueProvider(IEnumerable<LinkBuilder> builders)
        {
            _builders = builders.ToList();
        }
        public void SetValue(object target, object value)
        {
        }

        public object GetValue(object target)
        {
            return _builders.Select(b => b.GetLink(target))
                .Where(l => l != null)
                .ToDictionary(l => l.Rel, l => l.Href);
        }
    }
}