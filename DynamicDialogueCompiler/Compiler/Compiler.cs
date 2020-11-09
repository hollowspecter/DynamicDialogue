﻿using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DynamicDialogue.Core;

namespace DynamicDialogue.Compiler
{
	#region Compiler

	/// <summary>
	/// Compiles Bark-Files into DynamicDialogueChunks.
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

		private RuleVisitor ruleVisitor = new RuleVisitor();
		private ResponseVisitor responseVisitor = new ResponseVisitor();

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
		/// Compiles text as a string into a <see cref="Pack"/>
		/// </summary>
		/// <param name="text">The sourcecode to get compiled</param>
		/// <param name="pack">On return contains the compiled pack</param>
		/// <returns>Status of the compilation.</returns>
		/// <exception cref="ParseException">Throws parse exception if something
		/// goes wrong in the parsing step.</exception>
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

		internal void Compile(IParseTree tree)
		{
			ParseTreeWalker walker = new ParseTreeWalker();
			walker.Walk(this, tree);
		}

		public override void EnterRule_body([NotNull] BarkParser.Rule_bodyContext context)
		{
			Pack.AddRule(context.Accept(ruleVisitor));
		}

		public override void EnterResponse([NotNull] BarkParser.ResponseContext context)
		{
			Pack.AddResponse(context.Accept(responseVisitor));
		}
	}

	#endregion //Compiler

	#region Rule Visitor

	/// <summary>
	/// Visitor for a rule.
	/// </summary>
	internal class RuleVisitor : BarkParserBaseVisitor<Rule>
	{
		public override Rule VisitRule_body([NotNull] BarkParser.Rule_bodyContext context)
		{
			Rule rule = new Rule();

			// Parse Conditions
			{
				ConditionVisitor conditionVisitor = new ConditionVisitor();
				for (int i = 0; i < context.conditions().condition_statement().Length; ++i)
				{
					rule.AddCondition(context.conditions().condition_statement(i).Accept(conditionVisitor));
				}
			}

			// Parse RuleResponse
			rule.AddConsequence(context.rule_response().Accept(new RuleResponseVisitor()));

			// Parse Remember (optional)
			if (context.remember() != null)
			{
				StorageChange storageChange = new StorageChange();
				RememberStatementVisitor rememberVisitor = new RememberStatementVisitor(storageChange);
				for (int i = 0; i < context.remember().equals_statement().Length; ++i)
				{
					context.remember().equals_statement()[i].Accept(rememberVisitor);
				}
				rule.AddConsequence(storageChange);
			}

			// Parse Trigger (optional)
			if (context.trigger() != null)
			{
				rule.AddConsequence(context.trigger().Accept(new TriggerVisitor()));
			}

			return rule;
		}
	}

	/// <summary>
	/// Visitor for conditions
	/// </summary>
	internal class ConditionVisitor : BarkParserBaseVisitor<Clause>
	{
		public override Clause VisitCondition_statement([NotNull] BarkParser.Condition_statementContext context)
		{
			// Check what kind of condition statement it is
			if (context.WORD() != null)
			{
				return HandleWORD(context.WORD());
			}
			else
			{
				return HandleEquationStatement(context.equals_statement());
			}
		}

		private Clause HandleWORD([NotNull] ITerminalNode _word)
		{
			return new ExistsClause(_word.GetText());
		}

		private Clause HandleEquationStatement([NotNull] BarkParser.Equals_statementContext context)
		{
			EqualsStatement equalsStatement = new EqualsStatement(context);
			switch (equalsStatement.ThisMode)
			{
				case EqualsStatement.Mode.Bool:
					return new BoolClause(equalsStatement.Key, equalsStatement.BoolValue);
				case EqualsStatement.Mode.Float:
					return new FloatClause(equalsStatement.Key, FloatClause.CompareMode.EQUAL_TO, equalsStatement.FloatValue);
				case EqualsStatement.Mode.String:
					return new StringClause(equalsStatement.Key, equalsStatement.StringValue);
				default:
					throw new NotImplementedException();
			}
		}
	}

	/// <summary>
	/// Visitor for a reponse consequence in a rule
	/// </summary>
	internal class RuleResponseVisitor : BarkParserBaseVisitor<Consequence>
	{
		public override Consequence VisitRule_response([NotNull] BarkParser.Rule_responseContext context)
		{
			return new TextResponse(context.WORD().GetText());
		}
	}

	/// <summary>
	/// Visitor for a remember consequence in a rule
	/// Doesn't really need to return int
	/// </summary>
	internal class RememberStatementVisitor : BarkParserBaseVisitor<int>
	{
		private StorageChange storageChange;

		public RememberStatementVisitor(StorageChange _storageChange)
		{
			storageChange = _storageChange;
		}

		public override int VisitEquals_statement([NotNull] BarkParser.Equals_statementContext context)
		{
			EqualsStatement equalsStatement = new EqualsStatement(context);
			switch (equalsStatement.ThisMode)
			{
				case EqualsStatement.Mode.Bool:
					storageChange.AddChange(equalsStatement.Key, equalsStatement.BoolValue);
					break;
				case EqualsStatement.Mode.Float:
					storageChange.AddChange(equalsStatement.Key, equalsStatement.FloatValue);
					break;
				case EqualsStatement.Mode.String:
					storageChange.AddChange(equalsStatement.Key, equalsStatement.StringValue);
					break;
				default:
					throw new NotImplementedException();
			}
			return 0;
		}
	}

	/// <summary>
	/// Visitor for triggers in a rule
	/// </summary>
	internal class TriggerVisitor : BarkParserBaseVisitor<Consequence>
	{
		public override Consequence VisitTrigger([NotNull] BarkParser.TriggerContext context)
		{
			return new TriggerResponse(context.MENTION().GetText(), context.WORD().GetText());
		}
	}

	/// <summary>
	/// Struct for parsing and storing data about an EqualsStatement
	/// </summary>
	internal struct EqualsStatement
	{
		public string Key
		{
			private set; get;
		}
		public bool BoolValue
		{
			private set; get;
		}
		public string StringValue
		{
			private set; get;
		}
		public float FloatValue
		{
			private set; get;
		}
		public Mode ThisMode
		{
			private set; get;
		}

		public EqualsStatement([NotNull] BarkParser.Equals_statementContext context)
		{
			if (context.NUMBER() != null)
				ThisMode = Mode.Float;
			else if (context.BOOLEAN() != null)
				ThisMode = Mode.Bool;
			else
				ThisMode = Mode.String;

			BoolValue = false;
			StringValue = "";
			FloatValue = 0f;
			Key = context.WORD()[0].GetText();
			switch (ThisMode)
			{
				case Mode.Bool:
					BoolValue = bool.Parse(context.GetChild(2).GetText());
					break;
				case Mode.Float:
					FloatValue = float.Parse(context.GetChild(2).GetText());
					break;
				case Mode.String:
					StringValue = context.GetChild(2).GetText();
					break;
			}
		}

		public enum Mode
		{
			Bool, String, Float
		}
	}

	#endregion //RuleVisitor

	#region ResponseVisitor

	/// <summary>
	/// Visitor for a response
	/// </summary>
	internal class ResponseVisitor : BarkParserBaseVisitor<Response>
	{
		public override Response VisitResponse([NotNull] BarkParser.ResponseContext context)
		{
			Response response = new Response(context.WORD().GetText());
			LineVisitor lineVisitor = new LineVisitor();
			for (int i = 0; i < context.response_body().line().Length; ++i)
			{
				response.AddLine(context.response_body().line()[i].Accept(lineVisitor));
			}
			return response;
		}
	}

	/// <summary>
	/// Visitor for a line
	/// </summary>
	internal class LineVisitor : BarkParserBaseVisitor<string>
	{
		public override string VisitLine([NotNull] BarkParser.LineContext context)
		{
			string result = context.GetText();
			return result.Substring(1, result.Length - 2);
		}
	}

	#endregion //ReponseVisitor
}
