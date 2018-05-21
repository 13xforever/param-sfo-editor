using System.Diagnostics;
using System.IO;
using System.Text;

namespace param.sfo.editor
{
    [DebuggerDisplay("{StringValue}", Name = "{Key}")]
    internal class ParamSfoEntry
    {
        private ParamSfoEntry() { }

        private ushort KeyOffset { get; set; }
        public EntryFormat ValueFormat { get; private set; }
        private int ValueLength { get; set; }
        public int ValueMaxLength { get; private set; }
        private int ValueOffset { get; set; }
        public string Key { get; set; }
        public string StringValue { get; set; }
        public byte[] BinaryValue { get; set; }

        public static ParamSfoEntry Read(BinaryReader reader, ParamSfo paramSfo, int itemNumber)
        {
            const int indexOffset = 0x14;
            const int indexEntryLength = 0x10;
            reader.BaseStream.Seek(indexOffset + indexEntryLength * itemNumber, SeekOrigin.Begin);
            var result = new ParamSfoEntry();
            result.KeyOffset = reader.ReadUInt16();
            result.ValueFormat = (EntryFormat)reader.ReadUInt16();
            result.ValueLength = reader.ReadInt32();
            result.ValueMaxLength = reader.ReadInt32();
            result.ValueOffset = reader.ReadInt32();

            reader.BaseStream.Seek(paramSfo.KeysOffset + result.KeyOffset, SeekOrigin.Begin);
            byte tmp;
            var sb = new StringBuilder(32);
            while ((tmp = reader.ReadByte()) != 0)
                sb.Append((char)tmp);
            result.Key = sb.ToString();

            reader.BaseStream.Seek(paramSfo.ValuesOffset + result.ValueOffset, SeekOrigin.Begin);
            result.BinaryValue = reader.ReadBytes(result.ValueLength);
            if (result.ValueFormat == EntryFormat.Utf8)
                result.StringValue = Encoding.UTF8.GetString(result.BinaryValue);
            else if (result.ValueFormat == EntryFormat.Utf8Null)
                result.StringValue = Encoding.UTF8.GetString(result.BinaryValue, 0, result.ValueLength - 1);

            return result;
        }
    }
}