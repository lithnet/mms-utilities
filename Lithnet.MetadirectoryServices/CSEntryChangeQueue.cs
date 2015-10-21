using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.MetadirectoryServices
{
    /// <summary>
    /// Represents a queue of <see cref="Microsoft.MetadirectoryServices.CSEntryChange"/> objects that can be saved and loaded to a file
    /// </summary>
    public static class CSEntryChangeQueue
    {
        /// <summary>
        /// The internal dictionary to hold the CSEntryChange objects
        /// </summary>
        private static Dictionary<string, CSEntryChange> queue;

        /// <summary>
        /// Initializes the static instance of the CSEntryChangeQueue
        /// </summary>
        static CSEntryChangeQueue()
        {
            CSEntryChangeQueue.queue = new Dictionary<string, CSEntryChange>();
        }

        /// <summary>
        /// Loads the queue from the specified file
        /// </summary>
        /// <param name="filename">The name of the file</param>
        public static void LoadQueue(string filename)
        {
            lock (CSEntryChangeQueue.queue)
            {
                CSEntryChangeQueue.queue.Clear();

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
        }

        /// <summary>
        /// Saves the contents of the queue to the specified file
        /// </summary>
        /// <param name="filename">The name of the file to save the queue to. If this file exists, it is overwritten</param>
        /// <param name="schema">The current metadirectory services schema</param>
        public static void SaveQueue(string filename, Schema schema)
        {
            lock (CSEntryChangeQueue.queue)
            {
                CSEntryChangeSerializer.Serialize(CSEntryChangeQueue.queue.Values, filename, schema);
            }
        }

        /// <summary>
        /// Adds an item to the CSEntryChange queue. This method overwrites any existing entry with the same DN
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add</param>
        public static void Add(CSEntryChange csentry)
        {
            CSEntryChangeQueue.Add(csentry, true);
        }

        /// <summary>
        /// Adds an item to the CSEntryChange queue
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add</param>
        /// <param name="overwrite">A value that indicates if an object in the queue with the same DN should be overwritten with this object if found</param>
        /// <exception cref="System.InvalidOperationException">Thrown if <paramref name="overwrite"/> is set to false, and an object with the same DN was found in the queue</exception>
        public static void Add(CSEntryChange csentry, bool overwrite)
        {
            lock (queue)
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
        }

        /// <summary>
        /// Gets the number of items in the queue
        /// </summary>
        public static int Count
        {
            get
            {
                lock (CSEntryChangeQueue.queue)
                {
                    return CSEntryChangeQueue.queue.Count;
                }
            }
        }

        /// <summary>
        /// Clears all items from the queue
        /// </summary>
        public static void Clear()
        {
            lock (CSEntryChangeQueue.queue)
            {
                CSEntryChangeQueue.queue.Clear();
            }
        }

        /// <summary>
        /// Removes an item from the queue
        /// </summary>
        /// <returns>A CSEntryChange object</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Throw when this method is called, and there are no items left in the queue</exception>
        public static CSEntryChange Take()
        {
            lock (CSEntryChangeQueue.queue)
            {
                if (CSEntryChangeQueue.queue.Count == 0)
                {
                    throw new ArgumentOutOfRangeException("There are no items left in the queue");
                }

                CSEntryChange csentry = CSEntryChangeQueue.queue.First().Value;
                CSEntryChangeQueue.queue.Remove(csentry.DN);

                return csentry;
            }
        }
    }
}
