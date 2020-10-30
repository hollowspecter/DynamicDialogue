﻿using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace DynamicDialogue.Compiler
{
	/// <summary>
	/// Compiles TALKING-Files into DynamicDialogueChunks.
	/// </summary>
	public class Compiler : BarkParserBaseListener
	{
		/// <summary>
		/// Specifies the result of compiled Yarn Code.
		/// Please not, compilation failures result in an <see cref="ParseException"/>,
		/// so they don't get a status.
		/// </summary>
		public enum Status
		{
			Success
		}

		//TODO compile talking-files to rule-response-packages

		/// <summary>
		/// The name of the file that is currently parsed.
		/// </summary>
		private readonly string FileName;

		/// <summary>
		/// Gets the pack being generated by this compiler
		/// </summary>
		internal Pack Pack
		{
			get; private set;
		}

		internal Compiler(string fileName)
		{
			Pack = new Pack();
			FileName = fileName;
		}

		/// <summary>
		/// Reads contents of a file and generates a pack from it.
		/// </summary>
		/// <param name="path">The path to the file to compile.</param>
		/// <param name="pack">On return, contains the compiled pack</param>
		/// <returns>Status of compilation</returns>
		/// <exception cref="ParseException">Thrown when a parse
		/// error occurs during compilation</exception>
		public static Status CompileFile(string path, out Pack pack)
		{
			var source = File.ReadAllText(path);
			var fileName = Path.GetFileNameWithoutExtension(path);
			return CompileString(source, fileName, out pack);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="text"></param>
		/// <param name="pack"></param>
		/// <returns></returns>
		public static Status CompileString(string text, string fileName, out Pack pack)
		{
			AntlrInputStream inputStream = new AntlrInputStream(text);
			BarkLexer lexer = new BarkLexer(inputStream);
			CommonTokenStream tokens = new CommonTokenStream(lexer);
			BarkParser parser = new BarkParser(tokens);

			//turning off the normal error listener and using ours
			lexer.RemoveErrorListeners();
			lexer.AddErrorListener(LexerErrorListener.Instance);
			parser.RemoveErrorListeners();
			parser.AddErrorListener(ParseErrorListener.Instance);

			IParseTree tree;
			try
			{
				tree = parser.talk();
			}
			catch (ParseException e)
			{
#if DEBUG
				var tokenStringList = new List<string>();
				tokens.Reset();
				foreach (var token in tokens.GetTokens())
				{
					tokenStringList.Add($"{token.Line}:{token.Column} {BarkLexer.DefaultVocabulary.GetDisplayName(token.Type)} \"{token.Text}\"");
				}

				throw new ParseException($"{e.Message}\n\nTokens:\n{string.Join("\n", tokenStringList)}");
#else
				throw new ParseException(e.Message);
#endif // DEBUG
			}

			Compiler compiler = new Compiler(fileName);
			compiler.Compile(tree);
			pack = compiler.Pack;
			return Status.Success;
		}
	}
}
