using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cityton.Api.Hubs.Helper
{
    public static class ExtensionMethod
    {
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }

    }
}
