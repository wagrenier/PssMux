using System;
using System.IO;
using CommandLine;
using PssMux.Handler;
using PssMuxCmd.Options;

namespace PssMuxCmd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<MuxOptions>(args)
                .MapResult(
                    MuxFiles,
                    errs => 1);
        }
        
        public static int MuxFiles(MuxOptions muxOptions)
        {
            try
            {
                var sourceFileContent = File.ReadAllBytes(muxOptions.Source);
                var targetFileContent = File.ReadAllBytes(muxOptions.Target);

                var muxFileContent = PssHandler.SwitchPssAudio(sourceFileContent, targetFileContent);

                var directoryName = new FileInfo(muxOptions.Target).Directory;
                File.WriteAllBytes($"{directoryName.Name}/a.pss", muxFileContent);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);

                return -1;
            }
            
            return 0;
        }
    }
}