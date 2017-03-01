using System;

namespace AMillionRecords
{
    public interface IRecordAdder
    {
        int NumRecordsAdded { get; }
        int NumRecordsToAdd { get; }

        event EventHandler CompleteEvent;
        event EventHandler HundredRecordsAddedEvent;
        event EventHandler ThousandRecordsAddedEvent;

        void InsertRecords();
    }
}