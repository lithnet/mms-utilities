using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Collections.ObjectModel;

namespace Lithnet.MetadirectoryServices
{
    public class MASample : IMAExtensible2CallExport, IMAExtensible2CallImport
    {
        private string filename = @"D:\MAData\MyMA\Delta.xml";

        private Schema operationSchema;

        private OpenImportConnectionRunStep importRunStepParameters;

        public int ImportDefaultPageSize
        {
            get { return 500; }
        }

        public int ImportMaxPageSize
        {
            get { return 9999; }
        }

        public int ExportDefaultPageSize
        {
            get { return 100; }
        }

        public int ExportMaxPageSize
        {
            get { return 1000; }
        }

        public void OpenExportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenExportConnectionRunStep exportRunStep)
        {
            // The schema types are required for serialization, so save them into a local variable for use in CloseExportConnection
            this.operationSchema = types;

            // Load the existing items from the queue. We want to add to this collection, rather than overwrite it as an import operation may not have been run yet
            CSEntryChangeQueue.LoadQueue(this.filename);

            // ... Export initialization code
        }

        public PutExportEntriesResults PutExportEntries(IList<CSEntryChange> csentries)
        {
            foreach (CSEntryChange item in csentries)
            {
                try
                {
                    // Export Code
                    // ...
                    // ...


                    // On successful export, add the CSEntryChange to the queue
                    CSEntryChangeQueue.Add(item);
                }
                catch (Exception)
                {
                    // If the export failed, do not add the CSEntryChange to the queue
                }
            }

            return new PutExportEntriesResults();
        }

        public void CloseExportConnection(CloseExportConnectionRunStep exportRunStep)
        {
            // Save the items in the queue to the specified file
            CSEntryChangeQueue.SaveQueue(this.filename, this.operationSchema);
        }


        public OpenImportConnectionResults OpenImportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenImportConnectionRunStep importRunStep)
        {
            // The schema types are required for serialization, so save them into a local variable for use in CloseImportConnection
            this.operationSchema = types;

            this.importRunStepParameters = importRunStep;

            if (this.importRunStepParameters.ImportType == OperationType.Delta)
            {
                // Load the items from the queue
                CSEntryChangeQueue.LoadQueue(this.filename);
            }

            return new OpenImportConnectionResults();
        }

        public GetImportEntriesResults GetImportEntries(GetImportEntriesRunStep importRunStep)
        {
            GetImportEntriesResults results = new GetImportEntriesResults();

            if (this.importRunStepParameters.ImportType == OperationType.Delta)
            {
                int count = 0;

                // Add items to the result collection until the page size has been exceeded or
                // The queue is empty
                while (CSEntryChangeQueue.Count > 0 && (count < this.importRunStepParameters.PageSize))
                {
                    results.CSEntries.Add(CSEntryChangeQueue.Take());
                    count++;
                }

                // If the queue is not yet empty, tell the sync engine that we have more to import
                results.MoreToImport = CSEntryChangeQueue.Count > 0;
            }
            else
            {
                // Perform normal full import
            }

            return results;
        }

        public CloseImportConnectionResults CloseImportConnection(CloseImportConnectionRunStep importRunStep)
        {
            if (this.importRunStepParameters.ImportType == OperationType.Full)
            {
                // We just performed a full import so clear the queue
                CSEntryChangeQueue.Clear();
            }

            // Save the remaining items in the queue. If we have done a full import, the queue will be empty, and will clear any existing delta records
            CSEntryChangeQueue.SaveQueue(this.filename, this.operationSchema);

            return new CloseImportConnectionResults();
        }
    }
}
