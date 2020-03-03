using System;
using System.Collections.Generic;
using System.Text;
using MyLang.Ast;
using System.Linq;
namespace MyLang
{
    /// <summary>
    /// 変数と方法の名前を保存する
    /// </summary>
    static class VariablesOwners
    {
        //全ての方法は変数と自分のBlockを持つ
        static public Dictionary<string,UserDictionary> Dic = new Dictionary<string,UserDictionary>();
        public class UserDictionary
        {
            public Dictionary<string, float> Variable ;
            public Dictionary<string, Block> Function ;
            public List<string> ParameterList;
            public UserDictionary()
            {
                Variable = new Dictionary<string, float> { };
                Function = new Dictionary<string, Block> { };
                ParameterList = new List<string>();
            }
        }
    }

    public static class ReaderPassword
    {
        public static float Password = 985093846;
    }

    public class Interpreter
    {
        public ForReader ForReader = new ForReader();
        public WhileReader WhileReader = new WhileReader();
        public IfReader IfReader = new IfReader();
        public ReturnReader ReturnReader = new ReturnReader();
        public ExpressionReader ExpressionReader = new ExpressionReader();
        public FunctionReader FunctionReader = new FunctionReader();
        public PrintReader PrintReader = new PrintReader();
        public AssignReader AssignReader = new AssignReader();
        public Counter ReadCounter = new Counter();
        public Interpreter()
        {
            ReadCounter.SetNextReader(AssignReader);
            AssignReader.SetNextReader(PrintReader);
            PrintReader.SetNextReader(FunctionReader);
            FunctionReader.SetNextReader(ExpressionReader);
            ExpressionReader.SetNextReader(ReturnReader);
            ReturnReader.SetNextReader(IfReader);
            IfReader.SetNextReader(WhileReader);
            WhileReader.SetNextReader(ForReader);
        }

        public float Run(Ast.Ast ast)
        {
            // TODO: 仮のダミー実装
            if (ast is Ast.Exp exp)
                return Answer(exp);
            else if(ast is Ast.Block block)
            {
                foreach(Statement statement in block.StatementList)
                {
                    var run_ans = ReadCounter.RunCode(this,block,statement);
                    if (run_ans != ReaderPassword.Password) return run_ans;
                }
                return 0;
            }
            else 
                throw new Exception("Havn't got any block");
        }

        private float Answer(Ast.Exp exp)
        {
            if (exp is Ast.BinOp binOp)
            {
                var left_answer = Answer(binOp.Lhs);
                var right_answer = Answer(binOp.Rhs);
                switch (binOp.Operator)
                {
                    case BinOpType.Add:
                        return left_answer + right_answer;
                    case BinOpType.Sub:
                        return left_answer - right_answer;
                    case BinOpType.Multiply:
                        return left_answer * right_answer;
                    case BinOpType.Divide:
                        return left_answer / right_answer;
                    default:
                        return 0;
                }
            }
            else if (exp is Ast.Number ast_num)
                return ast_num.Value;
            else if(exp is Ast.Symbol ast_symbol)
            {
                if (VariablesOwners.Dic.ContainsKey(ast_symbol.Owner))
                {
                    if (VariablesOwners.Dic[ast_symbol.Owner].Variable.ContainsKey(ast_symbol.Value)) return VariablesOwners.Dic[ast_symbol.Owner].Variable[ast_symbol.Value];
                    else if (VariablesOwners.Dic["main"].Variable.ContainsKey(ast_symbol.Value)) return VariablesOwners.Dic["main"].Variable[ast_symbol.Value];
                    else throw new Exception("Unknowned Variable");
                    
                }
                else throw new Exception("Unknowed variable or function");
            }
            else if(exp is FunctionCall function_call)
            {
                if (!VariablesOwners.Dic[function_call.Owner].Function.ContainsKey(function_call.FunctionName.Value)) throw new Exception("Unknowned function");
                var para_value_list = new List<float>();
                foreach (Exp para in function_call.Parameters)
                {
                    if (para is Number num) para_value_list.Add(num.Value);
                    else if (para is Symbol sym) para_value_list.Add(VariablesOwners.Dic[function_call.Owner].Variable[sym.Value]);
                    else throw new Exception("unknowed something");
                }
                Enumerable.Range(0, function_call.Parameters.Count).ToList().ForEach(i =>
                    VariablesOwners.Dic[function_call.FunctionName.Value].Variable[VariablesOwners.Dic[function_call.FunctionName.Value].ParameterList[i]] = para_value_list[i]
                );
                return Run(VariablesOwners.Dic[function_call.Owner].Function[function_call.FunctionName.Value]);
            }
            else if(exp is LocateSymbol locate)
            {
                if (!VariablesOwners.Dic.ContainsKey(locate.Owner)) throw new Exception("Unknowed function");
                if (VariablesOwners.Dic[locate.Owner].Variable.ContainsKey(locate.value_string))
                    return VariablesOwners.Dic[locate.Owner].Variable[locate.value_string];
                else throw new Exception("Error Parameter");
            }
            else if(exp is EqualExp equalExp)
            {
                if (VariablesOwners.Dic[equalExp.Lhs.Owner].Variable.ContainsKey(equalExp.Lhs.Value))
                    VariablesOwners.Dic[equalExp.Lhs.Owner].Variable[equalExp.Lhs.Value] = Run(equalExp.Rhs);
                else if (VariablesOwners.Dic["main"].Variable.ContainsKey(equalExp.Lhs.Value))
                    VariablesOwners.Dic["main"].Variable[equalExp.Lhs.Value] = Run(equalExp.Rhs);
                else
                    throw new Exception("Unknowed variable");
                return 0;
            }
            else return 0;
        }

        public bool Compare(Compression compression)
        {
            var lhs = Run(compression.Lhs);
            var rhs = Run(compression.Rhs);
            switch (compression.CompareOp)
            {
                case CompareOpType.Larger:
                    return (lhs > rhs ? true : false);
                case CompareOpType.Smaller:
                    return (lhs < rhs ? true : false);
                case CompareOpType.DoubleEqual:
                    return (lhs == rhs ? true : false);
                case CompareOpType.LargerEqual:
                    return (lhs >= rhs ? true : false);
                case CompareOpType.SmallerEqual:
                    return (lhs <= rhs ? true : false);
                case CompareOpType.NotEqual:
                    return (lhs != rhs ? true : false);
                default:
                    throw new Exception("Error Compare");
            }
        }
    }
}
