﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLang
{
    namespace Ast
    {
        /// <summary>
        /// AST(Abstract Syntax Tree)のベースクラス
        /// </summary>
        public abstract class Ast
        {
            /// <summary>
            /// 文字列表現を作成するための情報を取得する.
            /// 
            /// string は 文字列でのそのASTの種類を表す
            /// Ast[] は、子供のASTを返す。子供を取らないASTの場合は、nullが入る。
            /// </summary>
            /// <returns>文字列表現のための情報</returns>
            public abstract Tuple<string, Ast[]> GetDisplayInfo();
        }

        #region Expression

        /// <summary>
        /// 式(Expression) のベースクラス
        /// </summary>
        public abstract class Exp : Ast { }

        /// <summary>
        /// ２項演算子の種類
        /// </summary>
        public enum BinOpType
        {
            Add, // +
            Sub, // -
            Multiply, // *
            Divide, // /

            Equal, // ==
            NotEqual, // !=
            Less, // <
            LessEqual, // <=
            Greater, // >
            GreaterEqual, // >=
        }

        /// <summary>
        /// 二項演算子(Binary Operator)を表すAST
        /// </summary>
        public class BinOp : Exp
        {
            public readonly BinOpType Operator;
            public readonly Exp Lhs;
            public readonly Exp Rhs;
            public BinOp(BinOpType op, Exp lhs, Exp rhs)
            {
                Operator = op;
                Lhs = lhs;
                Rhs = rhs;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Operator.ToString(), new Ast[] { Lhs, Rhs });
            }
        }

        /// <summary>
        /// 数値を表すAST
        /// </summary>
        public class Number : Exp
        {
            public readonly float Value;
            public Number(float value)
            {
                Value = value;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Value.ToString(), (Ast[])null);
            }
        }

        /// <summary>
        /// 変数を表すAST
        /// </summary>
        public class Symbol : Exp
        {
            public readonly string Name;
            public Symbol(string name)
            {
                Name = name;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Name, (Ast[])null);
            }
        }

        /// <summary>
        /// 関数の適用(Apply Function)を表すAST
        /// </summary>
        public class ApplyFunction : Exp
        {
            public readonly Symbol Name;
            public readonly Exp[] Args;
            public ApplyFunction(Symbol name, Exp[] args)
            {
                Name = name;
                Args = args;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("ApplyFunction", new Ast[] { Name }.Concat(Args).ToArray());
            }
        }

        #endregion

        #region Statement
        /// <summary>
        /// 文(Satement) のベースクラス
        /// </summary>
        public class Program : Ast
        {
            public readonly Statement[] Statements;

            public Program(IList<Statement> statements)
            {
                Statements = statements.ToArray();
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Program", Statements.Select(s=>(Ast)s).ToArray());
            }
        }

        /// <summary>
        /// 文(Satement) のベースクラス
        /// </summary>
        public abstract class Statement : Ast { }

        public class AssignStatement : Statement
        {
            public readonly Symbol Variable;
            public readonly Exp Exp;
            public AssignStatement(Symbol variable, Exp exp)
            {
                Variable = variable;
                Exp = exp;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("let", new Ast[] { Variable, Exp });
            }

        }

        public class PrintStatement : Statement
        {
            public readonly Exp Exp;
            public readonly string Format;
            public PrintStatement(string format, Exp exp)
            {
                Format = format;
                Exp = exp;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("print " + Format, new Ast[] { Exp });
            }
        }

        public class ReturnStatement : Statement
        {
            public readonly Exp Exp;
            public ReturnStatement(Exp exp)
            {
                Exp = exp;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("return", new Ast[] { Exp });
            }
        }

        public class FunctionStatement : Statement
        {
            public readonly Symbol Name;
            public readonly Statement[] Body;
            public FunctionStatement(Symbol name, IList<Statement> body)
            {
                Name = name;
                Body = body.ToArray();
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("function", new Ast[] { Name, new AstList(Body) });
            }
        }

        public class IfStatement : Statement
        {
            public readonly Exp Exp;
            public readonly Statement[] ThenBody;
            public readonly Statement[] ElseBody;
            public IfStatement(Exp exp, IEnumerable<Statement> thenBody, IEnumerable<Statement> elseBody)
            {
                Exp = exp;
                ThenBody = thenBody.ToArray();
                if (elseBody != null)
                {
                    ElseBody = elseBody.ToArray();
                }
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("if", new Ast[] { Exp, new AstList(ThenBody), new AstList(ElseBody) });
            }
        }

        public class LoopStatement : Statement
        {
            public readonly Statement[] Body;
            public LoopStatement(IList<Statement> body)
            {
                Body = body.ToArray();
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("loop", new Ast[] { new AstList(Body) });
            }
        }

        public class BreakStatement : Statement
        {
            public BreakStatement()
            {
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("break", new Ast[] {});
            }
        }

        public class AstList : Ast
        {
            public Ast[] List;
            public AstList(Ast[] list)
            {
                List = list;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("", List);
            }
        }

        #endregion

    }

}
