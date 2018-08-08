using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SevenZip.SDK;
using SevenZip.SDK.Compress.LZMA;

namespace AtriumWebApp.Web.Base.Library
{
    /// <summary> 
    /// Copied from Survey. Probably need to make sure it doesn't break any team projects.
    ///  http://stackoverflow.com/questions/222030/how-do-i-create-7-zip-archives-with-net
    /// </summary>
    public class SevenZipHelper
    {
        public static MemoryStream CompressStreamLZMA(byte[] bytes)
        {
            return CompressStreamLZMA(new MemoryStream(bytes));
        }

        public static MemoryStream CompressStreamLZMA(MemoryStream inStream)
        {
            Int32 dictionary = 1 << 23;
            Int32 posStateBits = 2;
            Int32 litContextBits = 3; // for normal files
            // UInt32 litContextBits = 0; // for 32-bit data
            Int32 litPosBits = 0;
            // UInt32 litPosBits = 2; // for 32-bit data
            Int32 algorithm = 2;
            Int32 numFastBytes = 128;

            string mf = "bt4";
            bool eos = true;
            bool stdInMode = false;

            // SevenZip.SDK.
            CoderPropID[] propIDs =  {
                CoderPropID.DictionarySize,
                CoderPropID.PosStateBits,
                CoderPropID.LitContextBits,
                CoderPropID.LitPosBits,
                CoderPropID.Algorithm,
                CoderPropID.NumFastBytes,
                CoderPropID.MatchFinder,
                CoderPropID.EndMarker
            };

            object[] properties = {
                (Int32)(dictionary),
                (Int32)(posStateBits),
                (Int32)(litContextBits),
                (Int32)(litPosBits),
                (Int32)(algorithm),
                (Int32)(numFastBytes),
                mf,
                eos
            };

            MemoryStream outStream = new MemoryStream();

            // SevenZip.SDK.Compress.LZMA.
            Encoder encoder = new Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            Int64 fileSize;
            if (eos || stdInMode)
                fileSize = -1;
            else
                fileSize = inStream.Length;
            for (int i = 0; i < 8; i++)
                outStream.WriteByte((Byte)(fileSize >> (8 * i)));
            encoder.Code(inStream, outStream, -1, -1, null);
            encoder = null;

            return outStream;
        }

        public static MemoryStream DecompressStreamLZMA(byte[] bytes)
        {
            return DecompressStreamLZMA(new MemoryStream(bytes));
        }

        public static MemoryStream DecompressStreamLZMA(MemoryStream inStream)
        {
            MemoryStream outStream = new MemoryStream();
            // SevenZip.SDK.Compress.LZMA.
            Decoder decoder = new Decoder();

            byte[] properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            decoder.SetDecoderProperties(properties);

            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = inStream.ReadByte();
                if (v < 0)
                    throw (new Exception("Can't Read 1"));
                outSize |= ((long)(byte)v) << (8 * i);
            }
            long compressedSize = inStream.Length - inStream.Position;

            decoder.Code(inStream, outStream, compressedSize, outSize, null);
            decoder = null;
            return outStream;
        }
    }
}
