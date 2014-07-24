namespace Linky
{
    public class Link
    {
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