#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System;
using UnityEngine;

[System.Serializable]
public class DataModel
{
    public bool IsLoaded;
    public object data;

    public string DataName;
    private readonly int debounceTime = 5000;
    private Thread dataCleaner;
    [ThreadStatic]
    public static volatile bool IsDirty = false;
    public class DataEventArgs : EventArgs
    {
        public DataEventArgs(byte eventType, object dataChunk)
        {
            EventType = eventType;
            DataChunk = dataChunk;
        }
        public static readonly byte UpdateEvent = 0;
        public static readonly byte SaveEvent = 1;
        public static readonly byte LoadEvent = 2;
        public byte EventType;
        public object DataChunk;
    }

    public event EventHandler<DataEventArgs> DataEvent;
    protected virtual void OnDataEvent(byte eventType, object dataChunk)
    {
        if (DataEvent != null)
        {
            DataEventArgs dataEventArgs = new DataEventArgs(eventType, dataChunk);
            DataEvent(this, dataEventArgs);
        }
    }
    private protected bool p_Save()
    {
        try
        {
            System.Console.WriteLine("Saving...");
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(DataName, FileMode.Create);
            binaryFormatter.Serialize(fileStream, data);
            fileStream.Close();
            Debug.Log(DataName);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    private protected bool p_Load()
    {
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(DataName, FileMode.Open);
            data = binaryFormatter.Deserialize(fileStream);
            Debug.Log(string.Format("Loading data back..." + data.ToString()));
            fileStream.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
    }

    private protected void p_Update(object dataChunk)
    {
        data = dataChunk;
        Debug.Log("Updating data...");
        DataChanged();
    }

    private void DataChanged()
    {
        IsDirty = true;
        if (dataCleaner == null)
        {
            dataCleaner = new Thread(new ThreadStart(DataCleaner));
            dataCleaner.Start();
        }
    }

    private void DataCleaner()
    {
        do
        {
            IsDirty = false;
            Debug.Log("Looping...");
            Thread.Sleep(debounceTime);
        }
        while (IsDirty);

        p_Save();
        dataCleaner = null;
    }
}