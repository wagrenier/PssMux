using CommandLine;

namespace PssMuxCmd.Options
{
    [Verb("mux", HelpText = "Mux the target's audio with the source's audio. The target will have the audio of the source.")]
    public sealed class MuxOptions
    {
        [Option('s', "source", Required = true, HelpText = "PSS source file from which the audio will be taken.")]
        public string Source { get; set; }
        
        [Option('t', "target", Required = true, HelpText = "PSS target file to which the audio will be injected.")]
        public string Target { get; set; }
    }
}