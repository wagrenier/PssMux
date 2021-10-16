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
            Parser.Default.ParseArguments<MuxOptions, FindOptions>(args)
                .MapResult(
                    (MuxOptions opts) => MuxFiles(opts),
                    (FindOptions opts) => FindPss(opts),
                    errs => 1);
        }

        private static int MuxFiles(MuxOptions muxOptions)
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
                DisplayErrorMessage(ex.Message);

                return -1;
            }
            
            DisplaySuccessMessage("All Done!");
            return 0;
        }

        private static int FindPss(FindOptions options)
        {
            try
            {
                PssHandler.FindPss(options.File, options.Extract);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex.Message);

                return -1;
            }

            DisplaySuccessMessage("All Done!");
            
            return 0;
        }

        private static void DisplaySuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void DisplayErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}