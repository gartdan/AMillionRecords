using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using AMillionRecords.Utilities;

namespace AMillionRecords.SQL
{
    public class SQLRecordAdder : IRecordAdder
    {
        public string ConnectionString { get; private set; }
        public int NumRecordsAdded { get; private set; }
        public int NumRecordsToAdd { get; private set; }
        public event EventHandler ThousandRecordsAddedEvent;
        public event EventHandler HundredRecordsAddedEvent;
        public event EventHandler CompleteEvent;
        private RandomRecordGenerator recordGenerator;
        private SqlCommandGenerator commandGenerator;
        public SQLRecordAdder(string connectionString, int numRecordsToAdd)
        {
            this.ConnectionString = connectionString;
            this.NumRecordsToAdd = numRecordsToAdd;
            this.recordGenerator = new RandomRecordGenerator();
            this.commandGenerator = new SqlCommandGenerator();
        }

        public void CreateTransactionsTableIfNotExists()
        {
            using(var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                var sql = @"
IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[Transactions]') AND type in (N'U'))

BEGIN
CREATE TABLE [dbo].[Transactions]
(
	[TransactionId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [AccountId] UNIQUEIDENTIFIER NULL, 
    [CustomerName] VARCHAR(100) NULL, 
    [Location] VARCHAR(100) NULL, 
    [TransactionAmount] MONEY NULL, 
    [TransactionDate] DATETIME NULL
)
END  
";
                var cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertRecords()
        {
            var records = recordGenerator.GenerateRandomRecords(this.NumRecordsToAdd);
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                foreach (var record in records)
                {
                    InsertRecord(record, connection);
                    NumRecordsAdded++;
                    if (NumRecordsAdded % 1000 == 0)
                        OnThousandRecordsAdded(EventArgs.Empty);
                    else if (NumRecordsAdded % 100 == 0)
                        OnHundredRecordsAdded(EventArgs.Empty);
                }
            }
            OnComplete(EventArgs.Empty);
        }

        public void InsertRecord(Record record, SqlConnection connection)
        {
            var cmd = commandGenerator.Generate(record);
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
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

