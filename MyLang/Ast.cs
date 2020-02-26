using System;
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

        /// <summary>
        /// 式(Expression) のベースクラス
        /// </summary>
        public abstract class Exp : Ast { }
        public abstract class Statement : Ast { }
        /// <summary>
        /// Blockは大きいクラス、色々Statementがある
        /// </summary>
        public class Block : Ast
        {
            public readonly List<Ast> StatementList;
            public readonly string FunctionName;
            /// <summary>
            /// Blockを作ると、名前を確認する
            /// </summary>
            public Block()
            {
                StatementList = new List<Ast>();
                FunctionName = "main";
            }
            public Block(string function_name)
            {
                StatementList = new List<Ast>();
                FunctionName = function_name;
            }

            public void AddStatement(Statement statement)
            {
                StatementList.Add(statement);
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Block", StatementList.ToArray());
            }
        }
        /// <summary>
        /// ２項演算子の種類
        /// </summary>
        public enum BinOpType
        {
            Add, // +
            Sub, // -
            Multiply, // *
            Divide, // /
        }
        /// <summary>
        /// キーワードの種類
        /// </summary>
        public enum KeywordType
        {
            Let,    // let or Let
            Equal,  // =
            End,    // ;
            Print,  // print
            Function,   //function
            Return, //  return
            Leftblock,  //  {
            Rightblock, //  }
            LeftBracket,    //  (
            RightBracket,   //  )
            Comma,  //  ,
            At, // @
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
        /// Assign Statementを表すAST
        /// </summary>
        public class AssignStatement : Statement
        {
            public readonly Symbol Lhs;
            public readonly Exp Rhs;
            public AssignStatement(Symbol lhs, Exp rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Assign", new Ast[] { Lhs,Rhs});
            }
        }
        /// <summary>
        /// Equal Statementを表すAST
        /// </summary>
        public class EqualStatement : Statement
        {
            public readonly Symbol Lhs;
            public readonly Ast Rhs;
            public EqualStatement(Symbol lhs,Ast rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                if (Rhs is DoFunctionStatement function)
                {
                    return Tuple.Create("Equal", new Ast[] { Lhs, function.FunctionName });
                }
                else if (Rhs is Exp exp)
                {
                    return Tuple.Create("Equal", new Ast[] { Lhs, exp });
                }
                else throw new Exception("Unknowed equal right hand side statement");
            }
        }
        /// <summary>
        /// Print Statementを表すAST
        /// </summary>
        public class PrintStatement : Statement
        {
            public readonly Exp Parameter;
            public PrintStatement(Exp parameter)
            {
                Parameter = parameter;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Print", new Ast[] { Parameter });
            }
        }
        /// <summary>
        /// Function Statementを表すAST
        /// </summary>
        public class FunctionStatement : Statement
        {
            public readonly Symbol FunctionName;
            public readonly Block Functionblock;
            public FunctionStatement(Symbol function_name,Block function_block)
            {
                FunctionName = function_name;
                Functionblock = function_block;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Function", new Ast[] { FunctionName, Functionblock });
            }
        }
        /// <summary>
        /// DoFunction Statementを表すAST
        /// </summary>
        public class DoFunctionStatement : Statement
        {
            public readonly Symbol FunctionName;
            List<Exp> Parameters;
            public DoFunctionStatement(string name,string functionOwner ,List<Exp> parameters)
            {
                FunctionName = new Symbol(name,functionOwner);
                Parameters = parameters;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Do", new Ast[] { FunctionName }.Concat(Parameters).ToArray());
            }
        }

        /// <summary>
        /// Return Statementを表すAST
        /// </summary>
        public class ReturnStatement : Statement
        {
            public readonly Ast Return_Value;
            public ReturnStatement(Ast exp)
            {
                Return_Value = exp;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Return", new Ast[] { Return_Value });
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
        /// Symbolを表すAST
        /// </summary>
        public class Symbol : Exp
        {
            public readonly string Value;
            public readonly string Owner;
            public Symbol(string value,string owner)
            {
                Value = value;
                Owner = owner;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Value, (Ast[])null);
            }
        }

        public class Locate : Exp
        {
            public readonly float Value;
            public readonly string value_string;
            public Locate(float value)
            {
                Value = value;
                value_string = "@" + Value.ToString();
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(value_string, (Ast[])null);
            }
        }
        /// <summary>
        /// キーワードを表すAST
        /// </summary>
        public class Keyword : Statement
        {
            public readonly string Value;
            public readonly KeywordType Type;
            public Keyword(string value, TokenType type)
            {
                Value = value;
                Type = Parser.KeywordMap[type];
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// ASTを文字列表現に変換するクラス
        /// </summary>
        public class AstDisplayer
        {
            List<Tuple<int, string>> list_;

            public AstDisplayer() { }

            /// <summary>
            /// ASTから、文字列表現に変換する.
            /// 
            /// prettyPrintにtrueを指定すると、改行やインデントを挟んだ読みやすい表現になる
            /// 
            /// BuildString(1 + 2 * 3 の AST, false) => "Add( 1 Multiply( 2 3 ) )"
            /// 
            /// BuildString(1 + 2 * 3 の AST, true) => 
            ///   "Add( 
            ///     1 
            ///     Multiply(
            ///       2
            ///       3
            ///     )
            ///    )"
            /// </summary>
            /// <param name="ast">対象のAST</param>
            /// <param name="prettyPrint">Pretty pring をするかどうか</param>
            /// <returns></returns>
            public string BuildString(Ast ast, bool prettyPrint = true)
            {
                list_ = new List<Tuple<int, string>>();
                build(0, ast);
                if( prettyPrint)
                {
                    return string.Join("\n", list_.Select(s => new string(' ', s.Item1 * 2) + s.Item2).ToArray());
                }
                else
                {
                    return string.Join(" ", list_.Select(s => s.Item2).ToArray());
                }
            }

            void build(int level, Ast ast)
            {
                var displayInfo = ast.GetDisplayInfo();
                if (displayInfo.Item2 == null)
                {
                    add(level, displayInfo.Item1);
                }
                else
                {
                    add(level, displayInfo.Item1 + "(");
                    foreach( var child in displayInfo.Item2)
                    {
                        build(level + 1, child);
                    }
                    add(level, ")");
                }
            }

            void add(int level, string text)
            {
                list_.Add(Tuple.Create(level, text));
            }
        }
    }

}
