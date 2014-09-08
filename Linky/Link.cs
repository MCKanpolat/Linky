namespace Linky
{
    using System.Collections.Generic;

    public class Link
    {
        private sealed class RelEqualityComparer : IEqualityComparer<Link>
        {
            public bool Equals(Link x, Link y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x._rel, y._rel);
            }

            public int GetHashCode(Link obj)
            {
                return (obj._rel != null ? obj._rel.GetHashCode() : 0);
            }
        }

        private static readonly IEqualityComparer<Link> RelComparerInstance = new RelEqualityComparer();

        public static IEqualityComparer<Link> RelComparer
        {
            get { return RelComparerInstance; }
        }

        private readonly string _rel;
        private readonly string _href;

        public Link(string rel, string href)
        {
            _rel = rel;
            _href = href;
        }

        public string Href
        {
            get { return _href; }
        }

        public string Rel
        {
            get { return _rel; }
        }
    }
}