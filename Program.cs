using dotnet_markdown_sample_code;
using System.Text.RegularExpressions;

int arg = 0;
if (args.Length > 0 && string.IsNullOrEmpty(args[0].TrimStart('-', '/'))) ++arg;

if (args.Length > arg && (args[arg].StartsWith('-') || args[arg].StartsWith('/')) && (args[arg].TrimStart('-', '/').ToLowerInvariant() == "help" || args[arg].TrimStart('-', '/') == "?"))
{
    Console.WriteLine("dotnet-markdown-sample-code usage:");
    Console.WriteLine("  dotnet-markdown-sample-code [source [dest [language [timeout]]]]");
    Console.WriteLine("    source: the relative or full path to the source code from which samples should be extracted (default is Samples.cs)");
    Console.WriteLine("      dest: the relative or full path to the destination markdown file into which the sample code should be placed (default is README.md)");
    Console.WriteLine("  language: the markdown language specifier so that markdown can colorize the code sample properly (default is csharp)");
    Console.WriteLine("   timeout: the maximum number of seconds to keep trying to update the destination file (uses expontential backoff with random offset starting at 250ms, default is 30)");
    Console.WriteLine("");
    return;
}
Regex RegionsRegex = new(@"(\s*#region\s+)([^\r\n]+)((.|\r|\n)+?(?=#endregion))", RegexOptions.Compiled);

string sourceFile = (args.Length > arg ? args[arg] : "Samples.cs");
string targetFile = (args.Length > arg + 1 ? args[arg + 1] : "README.md");
string markDownLanguageSpecifier = (args.Length > arg + 2 ? args[arg + 2] : "csharp");
string timeout = (args.Length > arg + 3 ? args[arg + 3] : "");
int timeoutSeconds = string.IsNullOrEmpty(timeout) ? 30 : int.Parse(timeout);
Console.Write($"Updating {targetFile} from {sourceFile}...");
string baseFolder = Directory.GetCurrentDirectory();
string sourceFileFullPath = Path.Combine(baseFolder, sourceFile);
string targetFileFullPath = Path.Combine(baseFolder, targetFile);
int attempt = 0;
DateTime startTime = DateTime.UtcNow;
do
{
    try
    {
        string sourceFileContents = File.ReadAllText(sourceFileFullPath);
        MatchCollection rawRegions = RegionsRegex.Matches(sourceFileContents);
        string targetFileContents = File.ReadAllText(targetFileFullPath);
        foreach (Match match in rawRegions.Cast<Match>())
        {
            string regionName = match.Groups[2].Value;
            string regionContents = match.Groups[3].Value.Trim();
            string targetInsert = $"[//]: # ({regionName})\r\n```{markDownLanguageSpecifier}\r\n{regionContents}\r\n```";
            string targetReplacePattern = @"\[\/\/\]\:\s*\#\s*\(" + regionName + @"\).*[\r\n]```[^\r\n]*(.|\r|\n)+?(?=```)```";
            targetFileContents = Regex.Replace(targetFileContents, targetReplacePattern, targetInsert);
        }
        targetFileContents = targetFileContents.TrimEnd();
        File.WriteAllText(targetFileFullPath, targetFileContents);
        Console.WriteLine($"Success!");
        break;
    }
    catch (Exception ex)
    {
        // timeout?
        if ((DateTime.UtcNow - startTime) > TimeSpan.FromSeconds(timeoutSeconds))
        {
            Console.WriteLine($"{Environment.NewLine}Timeout updating {targetFile} from {sourceFile}: {ex.Message}");
            break;
        }
        int waitMs = (1 << attempt) * 250 + (int)(TickBasedRandom.GetRandom() % 250);
        if (attempt > 2) Console.Write($"{Environment.NewLine}Error updating {targetFile} from {sourceFile}: {ex.Message}\r\nRetrying in {waitMs}ms...");
        System.Threading.Thread.Sleep(waitMs);
        // fall through and retry
    }
} while (true);
