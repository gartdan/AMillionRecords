using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using AMillionRecords.Utilities;

namespace AMillionRecords.DocumentDB
{
    public class DocumentDBRecordAdder : IRecordAdder
    {
        
        public static readonly string CollectionName = "Transactions";
        public int NumRecordsAdded { get; private set; }
        public int NumRecordsToAdd { get; private set; }

        public string EndpointUri { get; private set; }
        public string PrimaryKey { get; private set; }
        public string DatabaseName { get; private set; }


        public event EventHandler CompleteEvent;
        public event EventHandler HundredRecordsAddedEvent;
        public event EventHandler ThousandRecordsAddedEvent;

        private DocumentClient client;
        private RandomRecordGenerator recordGenerator;

        public DocumentDBRecordAdder(string endpointUri, string primaryKey, string databaseName, int numRecordsToAdd)
        {
            this.EndpointUri = endpointUri;
            this.PrimaryKey = primaryKey;
            this.DatabaseName = databaseName;
            this.client = new DocumentClient(new Uri(this.EndpointUri), this.PrimaryKey);
            this.recordGenerator = new RandomRecordGenerator();
            this.NumRecordsToAdd = numRecordsToAdd;

        }

        public async Task CreateCollectionIfNotExists()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
            var uri = UriFactory.CreateDatabaseUri(DatabaseName);
            await this.client.CreateDocumentCollectionIfNotExistsAsync(
                uri,
                new DocumentCollection() {  Id = CollectionName }
                );

        }

        public void InsertRecords()
        {
            InsertRecordsAsync().Wait();
        }

        public async Task InsertRecordsAsync()
        {
            await CreateCollectionIfNotExists();
            var records = recordGenerator.GenerateRandomRecords(this.NumRecordsToAdd);
            foreach (var record in records)
            {
                await InsertRecord(record);
                NumRecordsAdded++;
                if (NumRecordsAdded % 1000 == 0)
                    OnThousandRecordsAdded(EventArgs.Empty);
                else if (NumRecordsAdded % 100 == 0)
                    OnHundredRecordsAdded(EventArgs.Empty);
            }
            OnComplete(EventArgs.Empty);
        }

        private async Task InsertRecord(Record record)
        {
            
            await this.client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                record);
        }

        private void OnHundredRecordsAdded(EventArgs e)
        {
            HundredRecordsAddedEvent?.Invoke(this, e);
        }

        private void OnThousandRecordsAdded(EventArgs e)
        {
            ThousandRecordsAddedEvent?.Invoke(this, e);
        }

        private void OnComplete(EventArgs e)
        {
            CompleteEvent?.Invoke(this, e);
        }
    }
}
