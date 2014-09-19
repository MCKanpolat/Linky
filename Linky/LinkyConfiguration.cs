namespace Linky
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using Newtonsoft.Json.Serialization;

    public static class LinkyConfiguration
    {
        public static void Configure(HttpConfiguration config, string propName = null)
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
                            var linkBuilder = LinkBuilder.Create(attribute, apiDescription);
                            if (linkBuilder != null)
                            {
                                table.Add(linkBuilder);
                            }
                        }
                    }
                }
            }

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new LinkyContractResolver(table, json.SerializerSettings.ContractResolver, propName);
        }
    }

    internal class LinkyContractResolver : IContractResolver
    {
        private readonly LinkTable _table;
        private readonly IContractResolver _contractResolver;
        private readonly Dictionary<Type, JsonContract> _contracts = new Dictionary<Type, JsonContract>();
        private readonly object _sync = new object();

        public LinkyContractResolver(LinkTable table, IContractResolver contractResolver)
        {
            _table = table;
            _contractResolver = contractResolver ?? new DefaultContractResolver();
        }

        public JsonContract ResolveContract(Type type)
        {

            if (type == null) return null;
            JsonContract contract;
            if (_contracts.TryGetValue(type, out contract))
            {
                return contract;
            }

            lock (_sync)
            {
                if (_contracts.TryGetValue(type, out contract))
                {
                    return contract;
                }
                contract = _contractResolver.ResolveContract(type);
                var objectContract = contract as JsonObjectContract;
                if (objectContract != null)
                {
                    var linkyValueProvider = _table.GetValueProvider(type);
                    if (linkyValueProvider != null)
                    {
                        objectContract.Properties.AddProperty(new JsonProperty
                        {
                            PropertyType = typeof(IDictionary<string,string>),
                            PropertyName = "_links",
                            ValueProvider = linkyValueProvider,
                            Readable = true
                        });
                    }
                }
                _contracts.Add(type, contract);
            }

            return contract;
        }
    }
}