using System;
using System.Collections.Generic;

namespace Cogworks.ExamineFileIndexer.Helper
{
    public static class DictionaryHelper
    {
        public static void AddRange<T, TS>(this Dictionary<T, TS> source, Dictionary<T, TS> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection is null");
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                else
                {
                    // handle duplicate key issue here
                }
            }
        }
    }
}
