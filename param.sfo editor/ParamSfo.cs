using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace param.sfo.editor
{
    internal class ParamSfo
    {
        private ParamSfo() { }

        public string Magic { get; private set; }
        public byte MajorVersion { get; private set; }
        public byte MinorVersion { get; private set; }
        public short Reserved1 { get; private set; }
        public int KeysOffset { get; private set; }
        public int ValuesOffset { get; private set; }
        private int ItemCount { get; set; }
        public List<ParamSfoEntry> Items { get; private set; }

        public static ParamSfo Read(Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Stream must be seekable", nameof(stream));

            stream.Seek(0, SeekOrigin.Begin);
            var result = new ParamSfo();
            using (var reader = new BinaryReader(stream, new UTF8Encoding(false), true))
            {
                result.Magic = new string(reader.ReadChars(4));
                if (result.Magic != "\0PSF")
                    throw new FormatException("Not a valid SFO file");

                result.MajorVersion = reader.ReadByte();
                result.MinorVersion = reader.ReadByte();
                result.Reserved1 = reader.ReadInt16();
                result.KeysOffset = reader.ReadInt32();
                result.ValuesOffset = reader.ReadInt32();
                result.ItemCount = reader.ReadInt32();
                result.Items = new List<ParamSfoEntry>(result.ItemCount);

                for (var i = 0; i < result.ItemCount; i++)
                    result.Items.Add(ParamSfoEntry.Read(reader, result, i));
            }

            return result;
        }
    }
}