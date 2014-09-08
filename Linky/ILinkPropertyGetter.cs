namespace Linky
{
    using System.Collections.Generic;

    internal interface ILinkPropertyGetter
    {
        bool Optional { get; }
        bool GetValue(object obj, ref string path, List<string> queryStringParameters);
    }
}