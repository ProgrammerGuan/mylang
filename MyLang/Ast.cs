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

            public void AddStatement(Ast statement)
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
            If, //if
            Elif,//  elif
            Else,   //  else
            While,
            Leftblock,  //  {
            Rightblock, //  }
            LeftBracket,    //  (
            RightBracket,   //  )
            Comma,  //  ,
            At, // @
        }

        public enum CompareOpType
        {
            Larger,     // >
            Smaller,    // <
            DoubleEqual,    // ==
            LargerEqual,    //  >=
            SmallerEqual,   //  <=
            NotEqual,   //  !=
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
        /// Equalを表すAST
        /// </summary>
        public class EqualStatement : Exp
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
                if (Rhs is FunctionCall function)
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
            public readonly Ast Parameter;
            public PrintStatement(Ast parameter)
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
        /// Return Statementを表すAST
        /// </summary>
        public class ReturnStatement : Statement
        {
            public readonly Exp Return_Value;
            public ReturnStatement(Exp exp)
            {
                Return_Value = exp;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Return", new Ast[] { Return_Value });
            }
        }

        public class ExpresstionStatement : Statement
        {
            public readonly Exp Expression;
            public ExpresstionStatement(Exp exp)
            {
                Expression = exp;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Expression", new Ast[] { Expression });
            }
        }

        public class IfStatement : Statement
        {
            public static int IfCount = 0;
            public Dictionary<Exp, Block> Condition_Mission;
            public IfStatement()
            {
                Condition_Mission = new Dictionary<Exp, Block>();
            }
            public void AddCondition(Exp condition,Block block)
            {
                Condition_Mission.Add(condition, block);
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                var ReturnList = new List<Ast>();
                foreach(KeyValuePair<Exp,Block> item in Condition_Mission)
                {
                    ReturnList.Add(item.Key);
                    ReturnList.Add(item.Value);
                }
                return Tuple.Create("If", ReturnList.ToArray());
            }
        }

        public class WhileStatement : Statement
        {
            public readonly Exp Condition;
            public readonly Block Mission;
            public static int WhileCount;
            public WhileStatement(Exp condition, Block mission)
            {
                Condition = condition;
                Mission = mission;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("While", new Ast[] { Condition,Mission });
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

        public class FunctionCall : Exp
        {
            public readonly Symbol FunctionName;
            public readonly List<Exp> Parameters;
            public readonly string Owner;
            public FunctionCall(string name, string functionOwner, List<Exp> parameters)
            {
                FunctionName = new Symbol(name, functionOwner);
                Parameters = parameters;
                Owner = functionOwner;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Do", new Ast[] { FunctionName }.Concat(Parameters).ToArray());
            }
        }

        public class LocateSymbol : Exp
        {
            public readonly float Value;
            public readonly string value_string;
            public readonly string Owner;
            public LocateSymbol(float value,string owner)
            {
                Value = value;
                Owner = owner;
                value_string = "@" + Value.ToString();
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(value_string, (Ast[])null);
            }
        }
       
        public class Compression : Exp
        {
            public readonly CompareOpType CompareOp;
            public readonly Exp Lhs, Rhs;
            public Compression(CompareOpType compareOp,Exp lhs,Exp rhs)
            {
                CompareOp = compareOp;
                Lhs = lhs;
                Rhs = rhs;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(CompareOp.ToString(), new Ast[] { Lhs, Rhs });
            }
        }

        public class EqualExp : Exp
        {
            public readonly Symbol Lhs;
            public readonly Ast Rhs;
            public EqualExp(Symbol lhs,Ast rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                if(Rhs is FunctionCall functionCall)
                    return Tuple.Create("Equal", new Ast[] { Lhs, functionCall.FunctionName });
                else return Tuple.Create("Equal", new Ast[] { Lhs, Rhs });
            }
        }

        /// <summary>
        /// キーワードを表すAST
        /// </summary>
        public class Keyword : Ast
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
