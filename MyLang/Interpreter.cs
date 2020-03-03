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
        IfReader IfReader = new IfReader();
        ReturnReader ReturnReader = new ReturnReader();
        ExpressionReader ExpressionReader = new ExpressionReader();
        FunctionReader FunctionReader = new FunctionReader();
        PrintReader PrintReader = new PrintReader();
        AssignReader AssignReader = new AssignReader();
        Counter ReadCounter = new Counter();
        public Interpreter()
        {
            ReadCounter.SetNextReader(AssignReader);
            AssignReader.SetNextReader(PrintReader);
            PrintReader.SetNextReader(FunctionReader);
            FunctionReader.SetNextReader(ExpressionReader);
            ExpressionReader.SetNextReader(ReturnReader);
            ReturnReader.SetNextReader(IfReader);
        }

        public float Run(Ast.Ast ast)
        {
            // TODO: 仮のダミー実装
            if (ast is Ast.Exp exp)
                return Answer(exp);
            else if(ast is Ast.Block block)
            {
                #region tmpCommand
                //foreach (Ast.Ast statement in block.StatementList)
                //{
                //    if (statement is AssignStatement assign_statement)
                //    {
                //        if (!VariablesOwners.Dic[block.FunctionName].Variable.ContainsKey(assign_statement.Lhs.Value)) throw new Exception("Unknowed variable");
                //        VariablesOwners.Dic[block.FunctionName].Variable[assign_statement.Lhs.Value] = Run(assign_statement.Rhs);
                //    }
                //    else if (statement is PrintStatement print_statement)
                //    {
                //        var parameter = print_statement.Parameter;
                //        if (parameter is Symbol symbol_parameter)
                //        {
                //            if (VariablesOwners.Dic[block.FunctionName].Variable.ContainsKey(symbol_parameter.Value)) Console.WriteLine(VariablesOwners.Dic[block.FunctionName].Variable[symbol_parameter.Value]);
                //            else if (VariablesOwners.Dic["main"].Variable.ContainsKey(symbol_parameter.Value)) Console.WriteLine(VariablesOwners.Dic["main"].Variable[symbol_parameter.Value]);
                //            else throw new Exception(string.Format("Undefinded variable {0}", symbol_parameter.Value));
                //        }
                //        else if (parameter is FunctionCall do_function)
                //        {
                //            if (!VariablesOwners.Dic[block.FunctionName].Function.ContainsKey(do_function.FunctionName.Value)) throw new Exception("Unknowed function");
                //            var para_value_list = new List<float>();
                //            foreach (Exp para in do_function.Parameters)
                //            {
                //                if (para is Number num) para_value_list.Add(num.Value);
                //                else if (para is Symbol sym) para_value_list.Add(VariablesOwners.Dic[do_function.Owner].Variable[sym.Value]);
                //                else if (para is BinOp op) para_value_list.Add(Run(op));
                //                else throw new Exception("unknowed something");
                //            }
                //            Enumerable.Range(0, do_function.Parameters.Count).ToList().ForEach(i =>
                //                VariablesOwners.Dic[do_function.FunctionName.Value].Variable[VariablesOwners.Dic[do_function.FunctionName.Value].ParameterList[i]] = para_value_list[i]
                //            );
                //            Console.WriteLine(Run(VariablesOwners.Dic[block.FunctionName].Function[do_function.FunctionName.Value]));
                //        }
                //        else Console.WriteLine(Run(print_statement.Parameter));
                //    }
                //    else if (statement is FunctionStatement function_statement)
                //    {
                //        if (!VariablesOwners.Dic[block.FunctionName].Function.ContainsKey(function_statement.FunctionName.Value)) throw new Exception("Unknowed function name");
                //        VariablesOwners.Dic[block.FunctionName].Function[function_statement.FunctionName.Value] = function_statement.Functionblock;
                //    }
                //    else if (statement is ExpresstionStatement exp_statement)
                //    {
                //        Run(exp_statement.Expression);
                //    }
                //    else if (statement is ReturnStatement return_statement)
                //    {
                //        return Run(return_statement.Return_Value);
                //    }
                //    else if (statement is EqualStatement equal_statement)
                //    {
                //        if (!VariablesOwners.Dic[block.FunctionName].Variable.ContainsKey(equal_statement.Lhs.Value)) throw new Exception("Unknowed variable");
                //        VariablesOwners.Dic[block.FunctionName].Variable[equal_statement.Lhs.Value] = Run(equal_statement.Rhs);
                //    }
                //    else if (statement is IfStatement if_statement)
                //    {
                //        foreach (KeyValuePair<Exp, Block> condition in if_statement.Condition_Mission)
                //        {
                //            if (condition.Key is Compression compression)
                //            {
                //                if (Compare(compression))
                //                {
                //                    Run(condition.Value);
                //                    break;
                //                }
                //            }
                //            else if (condition.Key is Number number)
                //            {
                //                if (number.Value != 0)
                //                {
                //                    Run(condition.Value);
                //                    break;
                //                }
                //            }
                //            else if (condition.Key is Symbol symbol)
                //            {
                //                if (symbol.Value == "Else")
                //                {
                //                    Run(condition.Value);
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //    else throw new Exception("Unknowed statement");
                //}
                #endregion
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
                    VariablesOwners.Dic[function_call.FunctionName.Value].Variable["@" + i.ToString()] = para_value_list[i]
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
