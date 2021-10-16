using CommandLine;

namespace PssMuxCmd.Options
{
    [Verb("find-mux", HelpText = "Finds PSS from the source and target and then mux the audio of the source into " +
                                 "the target. The target will have the audio of the source. This will do a 1-1 mapping, " +
                                 "meaning that the first file found in source will be handled with the first file found " +
                                 "in the target.")]
    public sealed class FindAndMuxOptions
    {
        [Option('s', "source", Required = true, HelpText = "File in which you wish to find PSS and from which the audio will be taken.")]
        public string Source { get; set; }
        
        [Option('t', "target", Required = true, HelpText = "File in which you wish to find PSS and which the audio will be injected. This file will be duplicated with a _mux appended.")]
        public string Target { get; set; }
    }
}