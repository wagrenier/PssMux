using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PssMux.Operations;
using PssMux.Utils;

namespace PssMux.Handler
{
    public static class PssHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="targetFileName"></param>
        public static void FindAndMux(string sourceFileName, string targetFileName)
        {
            ConsoleUtils.DisplayWarningMessage("Your source ISO will now be scanned, this may take a while...");
            var sourcePssFiles = FindPss(sourceFileName, false);
            
            ConsoleUtils.DisplayWarningMessage("Your target ISO will now be scanned, this may take a while...");
            var targetPssFiles = FindPss(targetFileName, false);

            ConsoleUtils.DisplayWarningMessage("Your target ISO will now be copied, this may take a while...");
            var targetMuxFile = FileUtils.FileNameAppend(targetFileName, "mux");
            
            File.Copy(targetFileName, targetMuxFile);
            
            using var sourceBinaryReader = new BinaryReader(File.OpenRead(sourceFileName));
            using var targetBinaryReader = new BinaryReader(File.OpenRead(targetFileName));
            using var targetBinaryWriter = new BinaryWriter(File.OpenWrite(targetMuxFile));

            for (var i = 0; i < sourcePssFiles.Count && i < targetPssFiles.Count; i++)
            {
                try
                {
                    var sourcePssContent = ExtractPssFileToByteArray(sourcePssFiles[i], sourceBinaryReader);
                    var targetPssContent = ExtractPssFileToByteArray(targetPssFiles[i], targetBinaryReader);

                    var muxPss = SwitchPssAudio(sourcePssContent, targetPssContent);

                    targetBinaryWriter.WriteContentAtAddress(targetPssFiles[i].Start, muxPss);
                }
                catch (Exception ex)
                {
                    ConsoleUtils.DisplayErrorMessage($"Unable to undub PSS file {i}. Reason given: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Switches the target's audio to the source's audio.
        /// </summary>
        /// <param name="source">The source PSS file with the full path</param>
        /// <param name="target">The target PSS file with the full path</param>
        /// <returns>Byte array representing the new mux file</returns>
        public static byte[] SwitchPssAudio(byte[] source, byte[] target)
        {
            var pssHandler = new PssAudioMux(source, target);
            return pssHandler.TransferAudio();
        }
        
        /// <summary>
        /// Finds all PSS files contained within a file. Scan an ISO to find all the PSS within a game!
        /// </summary>
        /// <param name="fileName">Name of the file to scan.</param>
        /// <param name="extractFiles">Will extract the PSS files from the scanned file. Will be saved in the same directory.</param>
        /// <returns>Returns a list of PSS files found within the scanned file.</returns>
        public static IList<PssInfo> FindPss(string fileName, bool extractFiles)
        {
            var pssFiles = PssFinder.FindPss(fileName);

            if (extractFiles)
            {
                ExtractPssFilesToFiles(pssFiles, fileName);
            }

            return pssFiles;
        }

        /// <summary>
        /// Will extract the PSS files from the scanned file. Will be saved in the same directory as fileName.
        /// </summary>
        /// <param name="pssInfos">List of PSS files.</param>
        /// <param name="fileName">Name of the file to scan.</param>
        public static void ExtractPssFilesToFiles(IEnumerable<PssInfo> pssInfos, string fileName)
        {
            using var binaryReader = new BinaryReader(File.OpenRead(fileName));
            var directory = new FileInfo(fileName).DirectoryName;

            foreach (var pssInfo in pssInfos)
            {
                var pssContent = ExtractPssFileToByteArray(pssInfo, binaryReader);
                
                File.WriteAllBytes($"{directory}/{pssInfo.Id}.pss", pssContent);
            }
        }
        
        /// <summary>
        /// Extract the PSS file from a target file and returns the content in a byte array form.
        /// </summary>
        /// <param name="pssInfo">List of PSS files.</param>
        /// <param name="binaryReader">A BinaryRead object of your file.</param>
        /// <returns></returns>
        public static byte[] ExtractPssFileToByteArray(PssInfo pssInfo, BinaryReader binaryReader)
        {
            var fileSize = pssInfo.End - pssInfo.Start;

            if (fileSize <= 0)
            {
                return System.Array.Empty<byte>();
            }

            binaryReader.BaseStream.Seek(pssInfo.Start, SeekOrigin.Begin);

            // TODO: Handle possible overflow from long to int cast
            return binaryReader.ReadBytes((int) fileSize);
        }
    }
}