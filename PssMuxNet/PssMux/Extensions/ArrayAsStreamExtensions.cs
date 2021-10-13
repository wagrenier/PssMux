using System.Buffers.Binary;
using System.Linq;

namespace PssMux.Extensions
{
    public static class ArrayAsStreamExtensions
    {
        public static bool SeekNextAudioBlock(this ArrayAsStream pssBuffer)
        {
            while (true)
            {
                var blockId = pssBuffer.Read(0x4);

                if (blockId.SequenceEqual(PssConstants.PackStart))
                {
                    pssBuffer.SeekRelative(0xa);
                }
                else if (blockId.SequenceEqual(PssConstants.AudioSegment))
                {
                    return false;
                }
                else if (blockId.SequenceEqual(PssConstants.EndFile))
                {
                    return true;
                }
                else
                {
                    var blockSize = BinaryPrimitives.ReadUInt16BigEndian(pssBuffer.Read(0x2));
                    pssBuffer.SeekRelative(blockSize);
                }
            }
        }

        public static int InitialAudioBlock(this ArrayAsStream pssBuffer, out uint totalSize)
        {
            var blockSize = BinaryPrimitives.ReadUInt16BigEndian(pssBuffer.Read(0x2));
            pssBuffer.SeekRelative(0x3b - 0x6);

            totalSize = BinaryPrimitives.ReadUInt32LittleEndian(pssBuffer.Read(0x4));
            var dataSize = blockSize - PssConstants.FirstHeaderSize + 0x6;
            return dataSize;
        }

        public static int AudioBlock(this ArrayAsStream pssBuffer)
        {
            var blockSize = BinaryPrimitives.ReadUInt16BigEndian(pssBuffer.Read(0x2));
            
            pssBuffer.SeekRelative(-0x6);
            pssBuffer.SeekRelative(PssConstants.HeaderSize);
            
            var dataSize = blockSize - PssConstants.HeaderSize + 0x6;
            return dataSize;
        }

        public static ArrayAsStream BuildFullAudioBuffer(this ArrayAsStream pssBuffer)
        {
            pssBuffer.SeekNextAudioBlock();
            var currentBlockSize = pssBuffer.InitialAudioBlock(out var totalSize);

            var fullAudio = new ArrayAsStream(new byte[totalSize]);
            
            fullAudio.Write(pssBuffer.Read(currentBlockSize));

            while (true)
            {
                var isDone = pssBuffer.SeekNextAudioBlock();

                if (isDone)
                {
                    break;
                }

                currentBlockSize = pssBuffer.AudioBlock();
                
                fullAudio.Write(pssBuffer.Read(currentBlockSize));
            }
            
            fullAudio.SeekAbsolute(0);
            pssBuffer.SeekAbsolute(0);
            
            return fullAudio;
        }
        
    }
}