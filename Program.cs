using System.Security.Cryptography;

Console.Write("Enter folder path: ");
string path = Console.ReadLine()!;

if (!Directory.Exists(path))
{
    Console.WriteLine("Folder does not exist.");
    return;
}

string[] files = Directory.GetFiles(
    path,
    "*",
    SearchOption.AllDirectories
);

Console.WriteLine($"Found {files.Length} files.");

Dictionary<long, List<string>> filesBySize = new();

foreach (string file in files)
{
    long size = new FileInfo(file).Length;

    if (!filesBySize.ContainsKey(size))
    {
        filesBySize[size] = new List<string>();
    }

    filesBySize[size].Add(file);
}

Console.WriteLine("\nDuplicate files:");

foreach (var sizeGroup in filesBySize)
{
    if (sizeGroup.Value.Count < 2)
    {
        continue;
    }

    Dictionary<string, List<string>> filesByHash = new();

    foreach (string file in sizeGroup.Value)
    {
        string hash = GetFileHash(file);

        if (!filesByHash.ContainsKey(hash))
        {
            filesByHash[hash] = new List<string>();
        }

        filesByHash[hash].Add(file);
    }

    foreach (var hashGroup in filesByHash)
    {
        if (hashGroup.Value.Count < 2)
        {
            continue;
        }

        Console.WriteLine("\nDuplicate group:");

        foreach (string file in hashGroup.Value)
        {
            Console.WriteLine(Path.GetFileName(file));
        }
    }
}

static string GetFileHash(string file)
{
    using SHA256 sha256 = SHA256.Create();
    using FileStream stream = File.OpenRead(file);

    byte[] hash = sha256.ComputeHash(stream);

    return Convert.ToHexString(hash);
}