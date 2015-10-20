using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    public static class CSEntryChangeQueue
    {
        private static Dictionary<string, CSEntryChange> queue;

        static CSEntryChangeQueue()
        {
            CSEntryChangeQueue.queue = new Dictionary<string, CSEntryChange>();
        }

        public static void LoadQueue(string filename)
        {
            foreach (CSEntryChange csentry in CSEntryChangeDeserializer.Deserialize(filename))
            {
                if (CSEntryChangeQueue.queue.ContainsKey(csentry.DN))
                {
                    CSEntryChangeQueue.queue[csentry.DN] = csentry;
                }
                else
                {
                    CSEntryChangeQueue.queue.Add(csentry.DN, csentry);
                }
            }
        }

        public static void SaveQueue(string filename, Schema schema)
        {
            CSEntryChangeSerializer.Serialize(CSEntryChangeQueue.queue.Values, filename, schema);
        }

        public static void Add(CSEntryChange csentry)
        {
            CSEntryChangeQueue.Add(csentry, true);
        }

        public static void Add(CSEntryChange csentry, bool overwrite)
        {
            if (CSEntryChangeQueue.queue.ContainsKey(csentry.DN))
            {
                if (overwrite)
                {
                    CSEntryChangeQueue.queue[csentry.DN] = csentry;
                }
                else
                {
                    throw new InvalidOperationException("A CSEntryChange with this DN already exists in the queue");
                }
            }
            else
            {
                CSEntryChangeQueue.queue.Add(csentry.DN, csentry);
            }
        }

        public static int Count
        {
            get
            {
                return CSEntryChangeQueue.queue.Count;
            }
        }

        public static void Clear()
        {
            CSEntryChangeQueue.queue.Clear();
        }

        public static CSEntryChange Take()
        {
            if (CSEntryChangeQueue.queue.Count == 0)
            {
                throw new ArgumentOutOfRangeException("There are no items left in the collection");
            }

            CSEntryChange csentry = CSEntryChangeQueue.queue.First().Value;
            CSEntryChangeQueue.queue.Remove(csentry.DN);
            return csentry;
        }

    }
}
