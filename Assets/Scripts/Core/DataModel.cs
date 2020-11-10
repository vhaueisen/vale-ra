#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System;

[System.Serializable]
public class DataModel
{
    public bool IsLoaded;
    public object data;
    public string DataName;
    public int debounceTime = 750;
    private Thread dataCleaner;
    [ThreadStatic]
    public volatile bool IsDirty = false;

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
    public virtual void Load()
    {

    }

    public void OnSettingsEvent(object sender, DataEventArgs dataEventArgs)
    {
        if (dataEventArgs.EventType == DataEventArgs.LoadEvent)
        {
            Load();
        }
        else if (dataEventArgs.EventType == DataEventArgs.SaveEvent)
        {
            p_Save();
        }
        else
        {
            p_Update(dataEventArgs.DataChunk);
        }
    }

    private protected bool p_Save()
    {
        try
        {
            using (FileStream fileStream = new FileStream(DataName, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, data);
                IsLoaded = true;
                return true;
            }
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private protected bool p_Load()
    {
        try
        {
            using (FileStream fileStream = new FileStream(DataName, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                data = binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
                return true;
            }
        }
        catch (Exception e)
        {
            return false;
        }
    }
    public virtual void Update()
    {

    }
    private protected void p_Update(object dataChunk)
    {
        data = dataChunk;
        DataChanged();
        Update();
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
            Thread.Sleep(debounceTime);
        }
        while (IsDirty);

        p_Save();
        dataCleaner = null;
    }
}