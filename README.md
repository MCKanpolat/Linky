# Linky

Automatic link collection generation for ASP.NET Web API + JSON.NET

## Summary

An add-on for Web API that automatically adds a `_links` property to JSON objects as they are serialized, based on Route information from the application.

### Example

Set up **Linky** like this:

```csharp
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            LinkyConfiguration.Configure(config);
        }
    }
```

Given this controller with the `LinksFrom` attribute on the `Get(int id)` action...

```csharp
    [RoutePrefix("api/ships")]
    public class ShipsController : ApiController
    {
        [Route]
        public IEnumerable<Ship> Get()
        {
            return new[] {
                new Ship {Id = 1, Name = "Millennium Falcon"}, 
                new Ship {Id = 42, Name = "Heart of Gold"}};
        }

        [Route("{id}")]
        [LinksFrom(typeof (Ship), "self")]
        public Ship Get(int id)
        {
            return id == 1
                ? new Ship {Id = 1, Name = "Millennium Falcon"}
                : id == 42
                    ? new Ship {Id = 42, Name = "Heart of Gold"}
                    : null;
        }
    }
```

... **Linky** augments the JSON (created by [JSON.NET](http://james.newtonking.com/json)) like this:

```json
    [{
        "Id": 1,
        "Name": "Millennium Falcon",
        "_links": {
            "self": "/api/ships/1"
        }
    }, {
        "Id": 42,
        "Name": "Heart of Gold",
        "_links": {
            "self": "/api/ships/42"
        }
    }]
```

## Status

This is the first cut of code, hacked together to get something working. It works with the basic use case as shown here, but needs work to bring it up to par with the equivalent functionality found in [Simple.Web](http://github.com/markrendle/Simple.Web).

### Todo

(Forks and Pull Requests appreciated)

* Configuration
  * Allow customization of the JSON output formatting and `_links` property name
  * Allow override of the `Route` URI template in the `LinksFrom` attribute
  * Add a `QueryString` property to the `LinksFrom` attribute to allow parameters to be added to the URI
* Tests
* Performance
  * Currently reflecting on every Request, better to use runtime code-gen.
* Publish on NuGet

## License

Released under the [MIT license](https://github.com/zudio/Linky/blob/master/LICENSE).
&copy; Copyright [Zudio Cloud Tech](http://zud.io) 2014
