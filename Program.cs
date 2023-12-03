using LogAnalyzer;

var path = args.Length > 0 ? args[0] : string.Empty;

if (string.IsNullOrEmpty(path))
{
    Console.WriteLine("No path argument supplied");
    return;
}

List<string> logFiles = FileHelper.GetLogFiles(path);

foreach (string file in logFiles)
{
    Console.WriteLine($"------ Detail for log file: {file} -------");
    await LogProcessor.ProcessLogFile(file);
}
