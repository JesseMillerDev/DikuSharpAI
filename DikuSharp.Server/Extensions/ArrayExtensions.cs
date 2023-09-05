using System.Collections.Generic;
using System.Linq;
using DikuSharp.Server.Characters;

namespace DikuSharp.Server.Extensions
{
    public static class ArrayExtensions
    {
        public static void Clear(this byte[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
        }
    }
}
