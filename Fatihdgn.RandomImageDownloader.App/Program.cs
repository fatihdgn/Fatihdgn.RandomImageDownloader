using CommandLine;
using System.Net;
using System.Text.Json;

Console.WriteLine("fatihdgn Random Image Downloader");

var result = Parser.Default.ParseArguments<CommandLineOptions>(args);
var options = result.Value;

if (!string.IsNullOrEmpty(options.InputFile))
{
    if (!File.Exists(options.InputFile)) throw new ArgumentException("Input file doesn't exists. Please provide a valid file path.");

    CommandLineOptions? fileOptions;
    try
    {
        fileOptions = JsonSerializer.Deserialize<CommandLineOptions>(File.ReadAllText(options.InputFile));

    }
    catch
    {
        Console.WriteLine("Input file was not valid. Please provide a valid file.");
        return;
    }
    if (fileOptions == null) throw new ArgumentException("Input file was not valid. Please provide a valid file.");

    options.Count = fileOptions.Count;
    options.Parallelism = fileOptions.Parallelism;
    options.SavePath = fileOptions.SavePath;
}

if (options.Count <= 0)
{
    do
    {
        Console.Write("Enter the number of images to download: ");
        var sCount = Console.ReadLine();
        if (!int.TryParse(sCount, out int count))
            Console.WriteLine("The number you entered was not valid. Please enter again.");
        options.Count = count;
    }
    while (options.Count <= 0);
}

if (options.Parallelism <= 0)
{
    do
    {
        Console.Write("Enter the maximum parallel download limit: ");
        var sParallelism = Console.ReadLine();
        if (!int.TryParse(sParallelism, out int parallelism))
            Console.WriteLine("The number you entered was not valid. Please enter again.");
        options.Parallelism = parallelism;
    }
    while (options.Parallelism <= 0);
}

if (string.IsNullOrEmpty(options.SavePath))
{
    Console.Write("Enter the save path (default: ./outputs): ");
    var savepath = Console.ReadLine();
    if (string.IsNullOrEmpty(savepath)) savepath = "./outputs";
    options.SavePath = savepath;
}

var isCancelled = false;

Console.CancelKeyPress += (sender, e) =>
{
    isCancelled = true;
    e.Cancel = true;
};

if (!Directory.Exists(options.SavePath))
    Directory.CreateDirectory(options.SavePath);

Console.WriteLine($"Downloading {options.Count} images ({options.Parallelism} parallel downloads at most)");
Console.WriteLine();
Console.Write($"Progress: 0/{options.Count}");

var counter = new Counter();
counter.CountIncremented += (sender, count) =>
{
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write($"Progress: {count}/{options.Count}");
};


var loop = Parallel.For(1, options.Count + 1, new ParallelOptions { MaxDegreeOfParallelism = options.Parallelism }, i =>
{
    //using var client = new HttpClient();
    //using var response = client.Get("https://picsum.photos/200/300");
    //response.EnsureSuccessStatusCode();
    //using var contentStream = await response.Content.ReadAsStreamAsync();
    //using var fileStream = File.Create($"{options.SavePath}/{i}.jpg");
    //contentStream.CopyTo(fileStream);
    if (isCancelled) return;
    using WebClient client = new WebClient();
    client.DownloadFile("https://picsum.photos/200/300", $"{options.SavePath}/{i}.jpg");
    counter.Increment();
});

while (!loop.IsCompleted) { }

if (isCancelled)
{
    if (Directory.Exists(options.SavePath))
        Directory.Delete(options.SavePath, true);
}



public class CommandLineOptions
{
    [Option('i', "input", Required = false, HelpText = "Input file path.")]
    public string InputFile { get; set; } = string.Empty;

    [Option('c', "count", Required = false, HelpText = "Number of images to download.")]
    public int Count { get; set; }

    [Option('p', "parallelism", Required = false, HelpText = "Maximum parallel download limit.")]
    public int Parallelism { get; set; }

    [Option('s', "savepath", Required = false, HelpText = "Path for the output directory.")]
    public string SavePath { get; set; } = string.Empty;
}

public class Counter
{
    public delegate void CountIncrementedEventArgs(object sender, int count);
    public event CountIncrementedEventArgs CountIncremented;

    private volatile int count;
    public int Count => count;

    public void Increment()
    {
        Interlocked.Increment(ref count);
        CountIncremented?.Invoke(this, count);
    }
}