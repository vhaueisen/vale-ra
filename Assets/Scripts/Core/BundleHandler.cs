using System.IO;
using System;
using System.IO.Compression;
using System.Text;
using UnityEngine;
public static class BundleHandler
{
    public static string BundleKey
    {
        get => "2d624454541f6928f7e32007c3ca27757369a744c7aede25b325b6dca5c30cf7";
    }

    public static string BundleSalt
    {
        get => "2f0cfcc7";
    }

    static void CompressFile(string sDir, string sRelativePath, GZipStream zipStream, bool encrypted)
    {
        byte[] bytes = encrypted ? DataEncrypt.Encrypt(Encoding.ASCII.GetBytes(sRelativePath), BundleKey, BundleSalt) : Encoding.ASCII.GetBytes(sRelativePath);
        zipStream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
        zipStream.Write(bytes, 0, bytes.Length);

        bytes = File.ReadAllBytes(Path.Combine(sDir, sRelativePath));

        bytes = encrypted ? DataEncrypt.Encrypt(bytes, BundleKey, BundleSalt) : bytes;

        zipStream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
        zipStream.Write(bytes, 0, bytes.Length);
    }

    static bool DecompressFile(string sDir, GZipStream zipStream, bool encrypted)
    {
        byte[] bytes = new byte[sizeof(int)];
        int Readed = zipStream.Read(bytes, 0, sizeof(int));
        if (Readed < sizeof(int))
            return false;

        int iNameLen = BitConverter.ToInt32(bytes, 0);
        bytes = new byte[iNameLen];

        zipStream.Read(bytes, 0, bytes.Length);
        string sFileName = encrypted ? Encoding.ASCII.GetString(DataEncrypt.Decrypt(bytes, BundleKey, BundleSalt)) : Encoding.ASCII.GetString(bytes);
        sFileName = sFileName.Trim('\0');
        //Decompress file content
        bytes = new byte[sizeof(int)];
        zipStream.Read(bytes, 0, sizeof(int));
        int iFileLen = BitConverter.ToInt32(bytes, 0);

        bytes = new byte[iFileLen];
        zipStream.Read(bytes, 0, bytes.Length);

        bytes = encrypted ? DataEncrypt.Decrypt(bytes, BundleKey, BundleSalt) : bytes;

        string sFilePath = Path.Combine(sDir, sFileName);
        string sFinalDir = Path.GetDirectoryName(sFilePath);
        if (!Directory.Exists(sFinalDir))
            Directory.CreateDirectory(sFinalDir);

        using (FileStream outFile = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            outFile.Write(bytes, 0, iFileLen);

        return true;
    }

    public static void CompressBundle(string filePath, string folderPath, bool encrypted)
    {
        string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
        int dirLen = folderPath[folderPath.Length - 1] == Path.DirectorySeparatorChar ? folderPath.Length : folderPath.Length + 1;
        using (FileStream outFile = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (GZipStream str = new GZipStream(outFile, CompressionMode.Compress))
            foreach (string file in files)
            {
                string sRelativePath = file.Substring(dirLen);
                CompressFile(folderPath, sRelativePath, str, encrypted);
            }
    }

    public static void ExtractBundle(string filePath, string folderPath, bool encrypted)
    {
        using (FileStream inFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                while (DecompressFile(folderPath, zipStream, encrypted)) ;
        }
    }
}