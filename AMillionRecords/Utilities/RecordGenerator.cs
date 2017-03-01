using RandomNameGeneratorLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMillionRecords.Utilities
{
    public class RandomRecordGenerator
    {
        public int NumberOfNames { get; set; }
        public int NumberOfGuids { get; set; }

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        private static readonly Stats stats = new Stats();
        public IList<string> Names { get; private set; }
        private PersonNameGenerator nameGenerator;
        private PlaceNameGenerator placeGenerator;

        public IList<Guid> Guids { get; private set;  }
        public RandomRecordGenerator(int numberNames = 1000, int numberOfGuids = 1000)
        {
            nameGenerator = new PersonNameGenerator();
            placeGenerator = new PlaceNameGenerator();
            this.NumberOfNames = numberNames;
            this.NumberOfGuids = numberOfGuids;
            InitializeNames();
            InitializeGuids();

        }

        private void InitializeGuids()
        {
            this.Guids = new List<Guid>();
            for(int i = 0; i<NumberOfGuids; i++)
            {
                this.Guids.Add(Guid.NewGuid());
            }
        }

        private void InitializeNames()
        {
            Names = nameGenerator.GenerateMultipleFirstAndLastNames(this.NumberOfNames).ToList();
        }

        public string GetRandomCustomerName()
        {
            return this.Names[RandomIndex(this.Names.Count - 1)];
        }

        public DateTime GetRandomDate(DateTime minDate, DateTime maxDate)
        {
            var range = maxDate.Subtract(minDate).Days;
            return minDate.AddDays(RandomNumber(0, range));
        }

        public DateTime GetRandomDate(int lastXDays)
        {
            return GetRandomDate(DateTime.Now.AddDays(0 - lastXDays), DateTime.Now);
        }

        public Guid GetRandomGuid()
        {
            return this.Guids[RandomIndex(this.Guids.Count-1)];
        }

        public string GetRandomLocation()
        {
            return this.placeGenerator.GenerateRandomPlaceName();
        }

        public double GetRandomAmount()
        {
            return GetNormalizedValue();
        }

        public double GetNormalizedValue(double min = 0, double max=1000, double center = 500)
        {
            
            //values between 0 and 1000, centered around 200
            return stats.RandomBiasedPow(0, 1000, 3, 201);
        }

        public Record GenerateRandomRecord()
        {
            var record = new Record();
            record.AccountId = GetRandomGuid();
            record.CustomerName = GetRandomCustomerName();
            record.Location = GetRandomLocation();
            record.TransactionId = Guid.NewGuid();
            record.TransactionAmount = GetRandomAmount();
            record.TransactionDate = GetRandomDate(30);
            record.Location = GetRandomLocation();
            
            return record;
        }

        public IList<Record> GenerateRandomRecords(int numRecords)
        {
            var list = new List<Record>();
            for(int i=0;i<numRecords; i++)
            {
                list.Add(GenerateRandomRecord());
            }
            return list;
        }

        public static int RandomIndex(int max)
        {
            return RandomNumber(0, max);
        }

        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
    }
}
