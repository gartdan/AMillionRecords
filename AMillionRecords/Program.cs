using AMillionRecords.DocumentDB;
using AMillionRecords.SQL;
using AMillionRecords.Utilities;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMillionRecords
{
    class Program
    {
        static Stopwatch sw = new Stopwatch();
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Million Record Adder");
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1. Enter records into SQL Server.");
            Console.WriteLine("2. Enter records into Document DB.");

            var option = Console.ReadLine();
            if ("1" == option)
            {
                InsertIntoSQL();
            } else if ("2" == option)
            {
                InsertIntoDocDb();
            }
            
        }

        private static void InsertIntoDocDb()
        {
            Console.WriteLine("Please Provide an Endpoint Uri");
            var endpointUri = Console.ReadLine();
            Console.WriteLine("Please provide a primary key");
            var primaryKey = Console.ReadLine();
            Console.WriteLine("Please provide a database name");
            var databaseName = Console.ReadLine();
            Console.WriteLine("How many records would you like to add?");
            var numRecords = Console.ReadLine();
            Console.WriteLine($"Great. We will enter {numRecords} using endpoint {endpointUri}.");
            EnterRecordsIntoDocDb(endpointUri, primaryKey, databaseName, Convert.ToInt32(numRecords)).Wait();
        }

        private async static Task EnterRecordsIntoDocDb(string endpointUri, string primaryKey, string databaseName, int numRecords)
        {
            try
            {
                var adder = new DocumentDBRecordAdder(endpointUri, primaryKey, databaseName, numRecords);
                adder.HundredRecordsAddedEvent += Adder_HundredRecordsAddedEvent;
                adder.CompleteEvent += Adder_CompleteEvent;
                StartTimer();
                await adder.InsertRecordsAsync();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        private static void InsertIntoSQL()
        {
            Console.WriteLine("Please Provide a Connection String");
            var connectionString = Console.ReadLine();
            Console.WriteLine("How many records would you like to add?");
            var numRecords = Console.ReadLine();
            Console.WriteLine($"Great. We will enter {numRecords} using connectionstring {connectionString}.");

            EnterRecordsInSQL(connectionString, Convert.ToInt32(numRecords));
        }

        private static void StartTimer()
        {
            sw.Start();
        }

        private static void StopTimer()
        {
            sw.Stop();
        }

        private static void EnterRecordsInSQL(string connectionString, int numRecords)
        {
            var sqlAdder = new SQLRecordAdder(connectionString, numRecords);
            sqlAdder.HundredRecordsAddedEvent += Adder_HundredRecordsAddedEvent;
            sqlAdder.CompleteEvent += Adder_CompleteEvent;
            StartTimer();
            sqlAdder.InsertRecords();

        }

        private static void Adder_CompleteEvent(object sender, EventArgs e)
        {
            StopTimer();
            Console.WriteLine($"All done. Duration was {sw.ElapsedMilliseconds / 1000}s. Hit any key to exit.");
            Console.ReadKey();
        }

        private static void Adder_HundredRecordsAddedEvent(object sender, EventArgs e)
        {
            var adder = sender as IRecordAdder;
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000}s: {adder.NumRecordsAdded} records have been added thus far. {adder.NumRecordsToAdd - adder.NumRecordsAdded} to go.");
        }
    }
}
