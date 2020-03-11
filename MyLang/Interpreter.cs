﻿using System;
using System.Collections.Generic;
using System.Text;
using MyLang.Ast;
using System.Linq;
namespace MyLang
{
    /// <summary>
    /// 変数と方法の名前を保存する
    /// </summary>
    static class VariablesWareHouse
    {
        //全ての方法は変数と自分のBlockを持つ
        static public Dictionary<string, Ast.Ast> Global = new Dictionary<string, Ast.Ast>();
        static public Stack<MyLangStack> Stacks = new Stack<MyLangStack>();
    }

    public class MyLangStack
    {
        public readonly string Name;
        public Dictionary<string, float> SymboToValue;
        public MyLangStack(string name)
        {
            SymboToValue = new Dictionary<string, float>();
            Name = name;
        }
    }

    public static class ReaderPassword
    {
        public static float Password = 985093846;
    }

    public class Interpreter
    {
        public Counter ReadCounter = new Counter();
        public Interpreter()
        {
        }

        public float REPLRun(Ast.Ast ast)
        {
            if (ast is Ast.Exp exp)
                return Answer(exp);
            else if (ast is Ast.Block block)
            {
                foreach (Statement statement in block.StatementList)
                {
                    var run_ans = ReadCounter.REPLReadCode(this, block, statement);
                    if (run_ans != ReaderPassword.Password)
                    {
                        if (VariablesWareHouse.Stacks.Count > 0) VariablesWareHouse.Stacks.Pop();
                        return run_ans;
                    }
                }
                if (VariablesWareHouse.Stacks.Count > 0) VariablesWareHouse.Stacks.Pop();
                return ReaderPassword.Password;
            }
            else
                throw new Exception("Havn't got any block");
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
                    var run_ans = ReadCounter.RunCode(this, block, statement);
                    if (run_ans != ReaderPassword.Password)
                    {
                        if (VariablesWareHouse.Stacks.Count > 0) VariablesWareHouse.Stacks.Pop();
                        return run_ans;
                    }
                }
                if (VariablesWareHouse.Stacks.Count > 0) VariablesWareHouse.Stacks.Pop();
                return ReaderPassword.Password;
            }
            else 
                throw new Exception("Havn't got any block");
        }

        public float Answer(Ast.Exp exp)
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
                foreach(var my_stack in VariablesWareHouse.Stacks)
                {
                    if (my_stack.SymboToValue.ContainsKey(ast_symbol.Value)) return my_stack.SymboToValue[ast_symbol.Value];
                }
                if (VariablesWareHouse.Global.ContainsKey(ast_symbol.Value)) return Run(VariablesWareHouse.Global[ast_symbol.Value]);
                else throw new Exception("Unknowed Variable");
                
            }
            else if(exp is FunctionCall function_call)
            {
                var para_value_list = new List<float>();
                foreach (Exp para in function_call.Parameters)
                {
                    if (para is Number num) para_value_list.Add(num.Value);
                    else if (para is Symbol sym) para_value_list.Add(Answer(sym));
                    else if(para is BinOp bin) para_value_list.Add(Answer(bin));
                    else throw new Exception("unknowed something");
                }
                if (VariablesWareHouse.Global.ContainsKey(function_call.FunctionName.Value))
                {
                    if (VariablesWareHouse.Global[function_call.FunctionName.Value] is FunctionStatement function_statement)
                    {
                        VariablesWareHouse.Stacks.Push(new MyLangStack(function_call.FunctionName.Value));
                        Enumerable.Range(0, function_call.Parameters.Count).ToList().ForEach(i =>
                         VariablesWareHouse.Stacks.Peek().SymboToValue.Add((function_statement.Parameters[i]).Value, Answer(function_call.Parameters[i])));
                        return Run(function_statement.Functionblock);
                    }
                    else throw new Exception("error function Name");
                }
                else throw new Exception("Unknowed function");
            }
            else if(exp is EqualExp equalExp)
            {
                foreach(var variable in VariablesWareHouse.Stacks)
                {
                    if (variable.SymboToValue.ContainsKey(equalExp.Lhs.Value))
                    {
                        variable.SymboToValue[equalExp.Lhs.Value] = Answer(equalExp.Rhs);
                        return 0;
                    }
                }
                if (VariablesWareHouse.Global.ContainsKey(equalExp.Lhs.Value))
                    VariablesWareHouse.Global[equalExp.Lhs.Value] = new Number( Answer(equalExp.Rhs));
                else throw new Exception("Unknowed variable");
                return 0;
            }
            else return 0;
        }

        public bool Compare(Compression compression)
        {
            var lhs = Answer(compression.Lhs);
            var rhs = Answer(compression.Rhs);
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
