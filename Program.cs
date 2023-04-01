using dotnet_markdown_sample_code;
using System.Text.RegularExpressions;

Regex RegionsRegex = new(@"(\s*#region\s+)([^\r\n]+)((.|\r|\n)+?(?=#endregion))", RegexOptions.Compiled);

string sourceFile = (args.Length > 1 ? args[1] : "Samples.cs");
string targetFile = (args.Length > 2 ? args[2] : "README.md");
string markDownLanguageSpecifier = (args.Length > 3 ? args[3] : "csharp");
string timeout = (args.Length > 4 ? args[4] : "");
int timeoutSeconds = string.IsNullOrEmpty(timeout) ? 30 : int.Parse(timeout);
Console.WriteLine($"Updating {targetFile} from {sourceFile}...");
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
        Console.WriteLine($"Updated {targetFile} from {sourceFile}!");
        break;
    }
    catch (Exception ex)
    {
        // timeout?
        if ((DateTime.UtcNow - startTime) > TimeSpan.FromSeconds(timeoutSeconds))
        {
            Console.WriteLine($"Timeout updating {targetFile} from {sourceFile}: {ex.Message}");
            break;
        }
        else if (attempt > 2) Console.WriteLine($"Error updating {targetFile} from {sourceFile}: {ex.Message}\r\nRetrying in a bit...");
        System.Threading.Thread.Sleep((1 << attempt) * 250 + (int)(TickBasedRandom.GetRandom() % 250));
        // fall through and retry
    }
} while (true);
/* Source Powershell:
# NOTE: you may have to run this the first time you build:
# Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
"Updating README.md from Samples.cs..."
$scriptPath=If(!$PSCommandPath) { (Get - Location).Path }
Else { Split-Path -parent $PSCommandPath }
$samplesFile=$scriptPath+"\AmbientServicesSamples\Samples.cs"
$readmeFile=$scriptPath+"\README.md"
$samplesFileContents=Get-Content $samplesFile -Raw
$samplesRegionPattern="[\r\n]\s*#region\s+([^\r\n\s]*)(.|\r|\n)+?(?=#endregion)"
$rawRegions=[Regex]::Matches($samplesFileContents,"(\s*#region\s+)([^\r\n]+)((.|\r|\n)+?(?=#endregion))")
$readmeFileContents=Get-Content $readmeFile -Raw
ForEach($_ in $rawRegions)
{ 
    $regionName =$_.Groups[2].Value
    $regionContents =$_.Groups[3].Value.Trim()
    $sampleInsert = "[//]: # (" + $regionName + ")`r`n``````csharp`r`n" + $regionContents + "`r`n``````"
    $readmeReplacePattern = "\[\/\/\]\:\s*\#\s*\(" + $regionName + "\).*[\r\n]``````[^\r\n]*(.|\r|\n)+?(?=``````)``````"
    $readmeFileContents =[Regex]::Replace($readmeFileContents,$readmeReplacePattern,$sampleInsert)
    #($readmeFileContents -Replace $readmeReplacePattern,$sampleInsert)
}
$readmeFileContents=$readmeFileContents.TrimEnd();
Set-Content -Path $readmeFile -Value $readmeFileContents
"Updated README.md from Samples.cs!"
*/
