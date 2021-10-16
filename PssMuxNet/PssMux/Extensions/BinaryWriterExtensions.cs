using System.IO;

namespace PssMux.Utils
{
    public static class BinaryWriterUtils
    {
        public static void WriteContentAtAddress(this BinaryWriter binWriter, long address, byte[] content)
        {
            binWriter.BaseStream.Seek(address, SeekOrigin.Begin);
            
            binWriter.Write(content);
        }
    }
}