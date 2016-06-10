using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Collections.ObjectModel;

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
        private static List<CSEntryChange> queue;

        /// <summary>
        /// Initializes the static instance of the CSEntryChangeQueue
        /// </summary>
        static CSEntryChangeQueue()
        {
            CSEntryChangeQueue.queue = new List<CSEntryChange>();
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
                    CSEntryChangeQueue.Add(csentry);
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
                CSEntryChangeSerializer.Serialize(CSEntryChangeQueue.queue, filename, schema);
            }
        }

        /// <summary>
        /// Adds an item to the CSEntryChange queue. 
        /// </summary>
        /// <param name="csentry">The CSEntryChange to add</param>
        public static void Add(CSEntryChange csentry)
        {
            lock (CSEntryChangeQueue.queue)
            {
                CSEntryChangeQueue.queue.Add(csentry);
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
                
                CSEntryChange csentry = CSEntryChangeQueue.queue.First();
                CSEntryChangeQueue.queue.Remove(csentry);

                return csentry;
            }
        }
    }
}
