using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Passingwind.Blog.Web.Mvc.Json
{
	public class DateTimeJsonConverter : JsonConverter<DateTime>
	{
		private readonly DateTimeKind _dateTimeKind;
		private readonly string _format;

		public DateTimeJsonConverter(DateTimeKind dateTimeKind)
		{
			_dateTimeKind = dateTimeKind;
		}

		public DateTimeJsonConverter(string format)
		{
			_format = format;
		}

		public DateTimeJsonConverter(DateTimeKind dateTimeKind, string format)
		{
			_dateTimeKind = dateTimeKind;
			_format = format;
		}

		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			Debug.Assert(typeToConvert == typeof(DateTime));

			return DateTime.Parse(reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			if (_dateTimeKind == DateTimeKind.Utc)
			{
				if (!string.IsNullOrEmpty(_format))
					writer.WriteStringValue(value.ToUniversalTime().ToString(_format));
				else
					writer.WriteStringValue(value.ToUniversalTime());
			}
			else if (_dateTimeKind == DateTimeKind.Local)
			{
				if (!string.IsNullOrEmpty(_format))
					writer.WriteStringValue(value.ToLocalTime().ToString(_format));
				else
					writer.WriteStringValue(value.ToLocalTime());
			}
			else
			{
				if (!string.IsNullOrEmpty(_format))
					writer.WriteStringValue(value.ToString(_format));
				else
					writer.WriteStringValue(value);
			}
		}
	}

	public class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
	{
		private readonly string _format;

		public DateTimeOffsetJsonConverter(string format)
		{
			_format = format;
		}

		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			Debug.Assert(typeToConvert == typeof(DateTime));

			return DateTime.Parse(reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
		{
			if (!string.IsNullOrEmpty(_format))
				writer.WriteStringValue(value.ToString(_format));
			else
				writer.WriteStringValue(value);
		}
	}
}
