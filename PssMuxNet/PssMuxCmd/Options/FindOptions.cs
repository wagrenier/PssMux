using CommandLine;

namespace PssMuxCmd.Options
{
    // ReSharper disable once ClassNeverInstantiated.Global
    [Verb("find")]
    public sealed class FindOptions
    {
        [Option('f', "file", Required = true, HelpText = "File in which you wish to find PSS files.")]
        public string File { get; set; }
        
        [Option( 'e', "extract", HelpText = "Will extract the PSS found in the same directory as the given file.", Default = false)]
        public bool Extract { get; set; }
    }
}