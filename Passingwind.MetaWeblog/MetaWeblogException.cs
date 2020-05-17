using System;

namespace Passingwind.MetaWeblog
{
	/// <summary>
	/// Exception specifically for MetaWeblog API.  Error (or fault) responses
	///     request a code value.  This is our chance to add one to the exceptions
	///     which can be used to produce a proper fault.
	/// </summary>
	public class MetaWeblogException : Exception
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaWeblogException"/> class.
		/// Constructor to load properties
		/// </summary>
		/// <param name="code">
		/// Fault code to be returned in Fault Response
		/// </param>
		/// <param name="message">
		/// Message to be returned in Fault Response
		/// </param>
		public MetaWeblogException(string code, string message)
			: base(message)
		{
			this.Code = code;
		}

		public MetaWeblogException()
		{
		}

		public MetaWeblogException(string message) : base(message)
		{
		}

		public MetaWeblogException(string message, Exception innerException) : base(message, innerException)
		{
		}

		#endregion Constructors and Destructors

		#region Properties

		/// <summary>
		///     Gets code is actually for Fault Code.  It will be passed back in the
		///     response along with the error message.
		/// </summary>
		public string Code { get; private set; }

		#endregion Properties
	}
}
