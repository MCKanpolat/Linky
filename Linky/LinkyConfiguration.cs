namespace Linky
{
    using System.Collections;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    public static class LinkyConfiguration
    {
        public static void Configure(HttpConfiguration config)
        {
            config.EnsureInitialized();

            var apiExplorer = config.Services.GetApiExplorer();

            var table = new LinkTable();
            foreach (var apiDescription in apiExplorer.ApiDescriptions)
            {
                var descriptor = apiDescription.ActionDescriptor as ReflectedHttpActionDescriptor;
                if (descriptor != null)
                {
                    var linksFromAttributes = descriptor.MethodInfo.GetCustomAttributes(typeof (LinksFromAttribute), false).Cast<LinksFromAttribute>().ToArray();
                    if (linksFromAttributes.Length > 0)
                    {
                        foreach (var attribute in linksFromAttributes)
                        {
                            var linkBuilder = LinkBuilder.Create(attribute.ModelType, attribute.Rel, apiDescription);
                            if (linkBuilder != null)
                            {
                                table.Add(linkBuilder);
                            }
                        }
                    }
                }
            }

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new LinkyContractResolver(table, json.SerializerSettings.ContractResolver);
        }
    }
}