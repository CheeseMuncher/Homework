using System.Buffers;
using System.Buffers.Text;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.Utils;

public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Utf8Parser.TryParse(reader.ValueSpan, out DateTime value, out _, 'R'))
            return value;

        if (DateTime.TryParse(Encoding.UTF8.GetString(reader.ValueSpan.ToArray()), out value))
            return value;

        throw new FormatException();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // The "R" standard format will always be 29 bytes.
        Span<byte> utf8Date = new byte[29];
        bool result = Utf8Formatter.TryFormat(value, utf8Date, out _, new StandardFormat('R'));
        writer.WriteStringValue(utf8Date);
    }
}