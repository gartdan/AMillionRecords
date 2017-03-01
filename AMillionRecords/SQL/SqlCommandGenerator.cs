using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMillionRecords.SQL
{
    public class SqlCommandGenerator
    {
        public SqlCommand Generate(Record record)
        {
            var sb = new StringBuilder();
            sb.Append("INSERT INTO [Transactions] (TransactionId, AccountId, CustomerName, Location, TransactionAmount, TransactionDate) ");
            sb.Append("VALUES (@TransactionId, @AccountId, @CustomerName, @Location, @TransactionAmount, @TransactionDate);");
            var cmd = new SqlCommand(sb.ToString());
            cmd.Parameters.AddWithValue("@TransactionId", record.TransactionId);
            cmd.Parameters.AddWithValue("@AccountId", record.AccountId);
            cmd.Parameters.AddWithValue("@CustomerName", record.CustomerName);
            cmd.Parameters.AddWithValue("@Location", record.Location);
            cmd.Parameters.AddWithValue("@TransactionAmount", record.TransactionAmount);
            cmd.Parameters.AddWithValue("@TransactionDate", record.TransactionDate);
            return cmd;
        }
    }
}


