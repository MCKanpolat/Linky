namespace Linky
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Serialization;

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