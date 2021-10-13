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
                var targetFileInfo = new FileInfo(muxOptions.Target);
                File.WriteAllBytes($"{targetFileInfo.DirectoryName}/{targetFileInfo.Name}_mux.pss", muxFileContent);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.White;

                return -1;
            }
            
            return 0;
        }
    }
}