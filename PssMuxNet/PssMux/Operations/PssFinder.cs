using System;
using System.Collections.Generic;
using System.IO;
using PssMux.Extensions;

namespace PssMux.Operations
{
    public static class PssFinder
    {
        private const int ReadSize = 0x10;

        public static IList<PssInfo> FindPss(string sourceFileName)
        {
            var binaryReader = new BinaryReader(File.OpenRead(sourceFileName));
            var pssFound = new List<PssInfo>();
            var fileIndex = 0;
            var fileFound = false;
            long fileStart = 0;
            
            var currentBytes = binaryReader.ReadBytes(ReadSize);

            while (currentBytes.Length == ReadSize)
            {
                var currentPosition = binaryReader.BaseStream.Position;

                if (currentBytes.StartsWith(PssConstants.PssHeader) && 
                    IsSectorStart(currentPosition - ReadSize) && !fileFound)
                {
                    fileStart = currentPosition - ReadSize;
                    fileFound = true;
                }
                else if (currentBytes.StartsWith(PssConstants.PssEnd) && fileFound)
                {
                    fileFound = false;
                    
                    pssFound.Add(new PssInfo
                    {
                        Id = fileIndex,
                        Start = fileStart,
                        End = currentPosition - 0xC
                    });
                    
                    Console.WriteLine($"File {fileIndex} found @0x{fileStart:x8}");

                    fileIndex += 1;
                }

                binaryReader.BaseStream.Seek(0x7F0, SeekOrigin.Current);
                currentBytes = binaryReader.ReadBytes(ReadSize);
            }
            
            binaryReader.Close();
            
            return pssFound;
        }

        private static bool IsSectorStart(long currentAddress)
        {
            return currentAddress % 0x800 == 0;
        }
    }
}