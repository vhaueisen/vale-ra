using System;

[Serializable]
public class ValeRAPackage
{
    [Serializable]
    public struct PackageFile
    {
        public PackageFile(string fileName, int size)
        {
            FileName = fileName;
            Size = size;
        }
        public string FileName;
        public int Size;
    }
    public PackageFile[] Files
    {
        get => m_files;
        set => m_files = value;
    }
    private PackageFile[] m_files;
    public bool Encrypted = false;
    public byte[] Content
    {
        get => m_content;
        set => m_content = value;
    }
    private byte[] m_content;
    public float CompressedSize
    {
        get => m_compressedSize;
        set => m_compressedSize = value;
    }
    private float m_compressedSize;
    public float UncompressedSize
    {
        get => m_uncompressedSize;
        set => m_uncompressedSize = value;
    }
    private float m_uncompressedSize;
}