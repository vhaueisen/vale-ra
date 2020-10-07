using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

public class PackageHandler
{
    private static ValeRAPackage Load(string filepath)
    {
        ValeRAPackage package = new ValeRAPackage();
        using (FileStream fs = new FileStream(filepath, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                package = formatter.Deserialize(fs) as ValeRAPackage;
                if (package.Encrypted)
                {
                    int position = package.Content.Length;
                    while (position > 0 && package.Content[position - 1] == 0)
                        position--;
                    byte[] bytes = DataEncrypt.Decrypt(package.Content, BundleHandler.BundleKey, BundleHandler.BundleSalt);
                    Array.Copy(bytes, package.Content, position);
                }
            }
            catch
            {
            }
        }
        return package;
    }

    private static void Save(string filepath, ref ValeRAPackage package)
    {
        package.CompressedSize = package.CompressedSize + package.Content.Length * (1.0f / (1024.0f * 1024.0f));
        using (FileStream fs = new FileStream(filepath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                package.Content = package.Encrypted ? DataEncrypt.Encrypt(package.Content, BundleHandler.BundleKey, BundleHandler.BundleSalt) : package.Content;
                formatter.Serialize(fs, package);
            }
            catch
            {
            }
        }
    }

    private static MemoryStream CompressPackage(string filePath, string folderPath, MemoryStream memoryStream, ref ValeRAPackage package)
    {
        string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
        using (GZipStream zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
            package.Files = new ValeRAPackage.PackageFile[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                byte[] bytes = File.ReadAllBytes(Path.Combine(folderPath, files[i]));
                package.UncompressedSize = package.UncompressedSize + bytes.Length * (1.0f / (1024.0f * 1024.0f));
                package.Files[i] = new ValeRAPackage.PackageFile(Path.GetFileName(files[i]), bytes.Length);
                zipStream.Write(bytes, 0, bytes.Length);
            }
        }
        return memoryStream;
    }

    private static void DecompressPackage(MemoryStream memoryStream, GZipStream zipStream, string folderPath, ref ValeRAPackage package)
    {
        for (int i = 0; i < package.Files.Length; i++)
        {
            string outFilePath = Path.Combine(folderPath, package.Files[i].FileName);
            using (FileStream outFile = new FileStream(outFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] bytes = new byte[package.Files[i].Size];
                zipStream.Read(bytes, 0, bytes.Length);
                outFile.Write(bytes, 0, bytes.Length);
            }
        }
    }
    public static void CreateBundle(string filePath, string folderPath, bool encrypted = false)
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        ValeRAPackage package = new ValeRAPackage();
        package.Encrypted = encrypted;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            CompressPackage(filePath, folderPath, memoryStream, ref package);
            package.Content = memoryStream.ToArray();
        }
        Save(filePath, ref package);
#if UNITY_EDITOR
        UnityEngine.Debug.Log(string.Format("Compress Time: {0}, Speed(Mb/s): {1}", watch.ElapsedMilliseconds, (package.UncompressedSize / (watch.ElapsedMilliseconds / 1000.0f))));
#endif
        watch.Stop();
    }

    public static void ExtractBundle(string filePath, string folderPath)
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        ValeRAPackage package = Load(filePath);
        using (MemoryStream memoryStream = new MemoryStream(package.Content))
        {
            using (GZipStream zipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                DecompressPackage(memoryStream, zipStream, folderPath, ref package);
            }
        }
#if UNITY_EDITOR
        UnityEngine.Debug.Log(string.Format("Extract Time: {0}, Speed(Mb/s): {1}", watch.ElapsedMilliseconds, (package.CompressedSize / (watch.ElapsedMilliseconds / 1000.0f))));
#endif
        watch.Stop();
    }
}