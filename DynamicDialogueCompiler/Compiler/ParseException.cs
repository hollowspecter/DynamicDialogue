using System;
using System.Globalization;

namespace DynamicDialogue.Compiler
{
	/// <summary>
	/// An exception representing something going wrong during parsing.
	/// 
	/// Source: https://github.com/YarnSpinnerTool/YarnSpinner/blob/master/YarnSpinner.Compiler/ParseException.cs
	/// Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.
	/// </summary>
	[Serializable]
	public sealed class ParseException : Exception
	{
		internal int lineNumber = 0;

		internal ParseException(string message) : base(message) { }

		internal static ParseException Make(Antlr4.Runtime.ParserRuleContext context, string message)
		{
			int line = context.Start.Line;

			// getting the text that has the issue inside
			int start = context.Start.StartIndex;
			int end = context.Stop.StopIndex;
			string body = context.Start.InputStream.GetText(new Antlr4.Runtime.Misc.Interval(start, end));

			string messageResult = string.Format(CultureInfo.CurrentCulture, "Error on line {0}\n{1}\n{2}", line, body, message);
			var e = new ParseException(messageResult) { lineNumber = line };
			return e;
		}
	}
}
