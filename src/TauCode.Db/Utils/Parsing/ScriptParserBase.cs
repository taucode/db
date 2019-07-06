using System;
using System.Collections.Generic;
using TauCode.Db.Exceptions;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Parsing.Core;
using TauCode.Db.Utils.Parsing.Core.Fluent.Impl;
using TauCode.Db.Utils.Parsing.Core.Gallery;
using TauCode.Db.Utils.Parsing.Core.Nodes;

namespace TauCode.Db.Utils.Parsing
{
    public abstract class ScriptParserBase : IScriptParser
    {
        #region Fields

        private ParsingNode _root;
        private ParsingFlow _currentFlow;

        #endregion

        #region Constructor

        protected ScriptParserBase(IDialect dialect, ITokenizer tokenizer)
        {
            this.Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
            this.Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));

            if (this.Dialect != this.Tokenizer.Dialect)
            {
                throw new ArgumentException("Dialect of script parser differs from dialect of tokenizer. That's an internal error.", nameof(tokenizer));
            }
        }

        #endregion

        #region Protected

        protected ITokenizer Tokenizer { get; }

        protected ParsingNode Root => _root ?? (_root = this.CreateRoot());

        protected ParsingBlock CreateIdleBlock()
        {
            var syntax = new NodeSyntax();
            syntax
                .Idle()
                .Idle("end");

            return new ParsingBlock(syntax.Root, syntax.GetNode("end").Node);
        }

        protected ParsingException CreateInternalParserErrorException()
        {
            return new ParsingException("Internal parser error.");
        }

        #endregion

        #region Polymorph

        protected virtual ParsingBlock CreateIdentityBlock()
        {
            return this.CreateIdleBlock();
        }

        protected virtual ParsingBlock CreateTableCreationBlock()
        {
            var identityBlock = this.CreateIdentityBlock();

            var builder = new TableCreationBuilder(this.Dialect, identityBlock);
            return builder.Build();
        }

        protected virtual ParsingBlock CreateIndexCreationBlock()
        {
            var builder = new IndexCreationBuilder();
            return builder.Build();
        }

        protected virtual ParsingNode CreateRoot()
        {
            var syntax = new NodeSyntax();

            var createTableBlock = this.CreateTableCreationBlock();
            var createIndexBlock = this.CreateIndexCreationBlock();

            syntax
                .Split("root")
                    .Attach(createTableBlock.InputNode)
                .GetSplitter("root")
                    .Attach(createIndexBlock.InputNode);

            return syntax.Root;
        }

        protected virtual object ParseClause()
        {
            var currentNode = this.Root;

            while (true)
            {
                var acceptResult = currentNode.Accepts(_currentFlow, 0);

                if (acceptResult == null)
                {
                    throw this.CreateInternalParserErrorException();
                }

                if (acceptResult.NextNode == currentNode && !acceptResult.TokenIsConsumed)
                {
                    throw this.CreateInternalParserErrorException();
                }

                currentNode.Act(_currentFlow.CurrentToken, _currentFlow.Context);
                currentNode = acceptResult.NextNode;

                if (currentNode is EndNode)
                {
                    currentNode.Act(null, _currentFlow.Context); // no token for you pal.
                    break;
                }
                else
                {
                    if (acceptResult.TokenIsConsumed)
                    {
                        _currentFlow.Advance(1); // go on 
                    }
                }
            }

            var result = _currentFlow.Context.Result;

            if (result == null)
            {
                throw this.CreateInternalParserErrorException();
            }

            _currentFlow.Context.ResetResult();
            return result;
        }

        #endregion

        #region IScriptParser Members

        public IDialect Dialect { get; }

        public object[] Parse(string script)
        {
            var tokens = this.Tokenizer.Tokenize(script);
            var results = new List<object>();
            _currentFlow = new ParsingFlow(tokens);

            while (true)
            {
                if (_currentFlow.IsEnd)
                {
                    break;
                }

                var result = this.ParseClause();

                if (result != null)
                {
                    results.Add(result);
                }
            }

            if (_currentFlow.Context.GetPropertyCount() != 0)
            {
                throw this.CreateInternalParserErrorException();
            }

            _currentFlow = null;
            return results.ToArray();
        }

        #endregion
    }
}
