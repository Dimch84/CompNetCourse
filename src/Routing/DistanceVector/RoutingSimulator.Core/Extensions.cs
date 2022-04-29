using System;
using System.Collections.Generic;
using System.Text;

namespace RoutingSimulator.Core
{
    static class Extensions
    {
        public static int RemoveWhere<T>(this ISet<T> set, Predicate<T> pred)
        {
            var toRemove = new List<T>();
            foreach(var i in set)
            {
                if (pred(i))
                    toRemove.Add(i);
            }
            var cnt = 0;
            foreach (var i in toRemove)
                cnt += set.Remove(i) ? 1 : 0;
            return cnt;
        }
    }
}
