using CommandLine;

namespace PssMuxCmd.Options
{
    public class MuxOptions
    {
        [Option('s', "source", Required = true, HelpText = "PSS source file from which the audio will be taken")]
        public string Source { get; set; }
        
        [Option('t', "target", Required = true, HelpText = "PSS target file to which the audio will be injected")]
        public string Target { get; set; }
    }
}