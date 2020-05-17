using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace BlogML
{
	public class BlogMLResource
	{
		public static Stream GetSchemaStream()
		{
			Stream stream = typeof(BlogMLResource).Assembly.GetManifestResourceStream("BlogML.BlogML.xsd");
			if (stream == null)
				throw new InvalidOperationException("Schema not found");
			return stream;
		}

		public static XmlSchema GetSchema()
		{
			return XmlSchema.Read(
				GetSchemaStream(),
				new ValidationEventHandler(ValidationEvent)
			);
		}

		private static void ValidationEvent(object sender, ValidationEventArgs e)
		{
			string message = string.Format("Validation {0}: {1}", e.Severity, e.Message);
			throw new InvalidOperationException(message);
		}


		public static void Validate(string inputUri)
		{
			Validate(inputUri, null);
		}


		public static void Validate(
			string inputUri,
			ValidationEventHandler validationHandler
			)
		{
			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader(inputUri);
				Validate(reader, validationHandler);
			}
			finally
			{
				if (reader != null)
					reader.Close();

			}
		}

		public static void Validate(XmlTextReader reader)
		{
			Validate(reader, null);
		}


		public static void Validate(
			XmlTextReader treader,
			ValidationEventHandler validationHandler
			)
		{
			XmlReaderSettings validator = null;
			try
			{
				validator = new XmlReaderSettings();
				XmlSchema schema = GetSchema();
				validator.Schemas.Add(schema);
				validator.ValidationType = ValidationType.Schema;


				if (validationHandler != null)
					validator.ValidationEventHandler += validationHandler;
				else
					validator.ValidationEventHandler += new ValidationEventHandler(ValidationEvent);

				XmlReader objXmlReader = XmlReader.Create(treader, validator);


				while (objXmlReader.Read()) { }
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
			finally
			{
				if (validationHandler != null)
					validator.ValidationEventHandler -= validationHandler;
				else
					validator.ValidationEventHandler -= new ValidationEventHandler(ValidationEvent);
			}
		}
	}
}
