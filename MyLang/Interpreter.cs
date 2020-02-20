using System;
using System.Collections.Generic;
using System.Text;
using MyLang.Ast;

namespace MyLang
{
    static class InterpreterPassword
    {
        public const int MainFunction = 985093846;
        public const int NotFloatAnswer = 981296739;
    }

    static class UserDictionary
    {
        static public Dictionary<string, float> Variable = new Dictionary<string, float> { };
        static public Dictionary<string, Block> Function = new Dictionary<string, Block> { };
    }

    public class Interpreter
    {
        
        public Interpreter()
        {
        }

        public float Run(Ast.Ast ast)
        {
            // TODO: 仮のダミー実装
            if (ast is Ast.Exp exp)
                return Answer(exp);
            else if(ast is Ast.Block block)
            {
                foreach (Statement statement in block.StatementList)
                {
                    if (statement is AssignStatement assign_statement)
                    {
                        if (!UserDictionary.Variable.ContainsKey(assign_statement.Lhs.Value)) throw new Exception("Unknowed variable");
                        UserDictionary.Variable[assign_statement.Lhs.Value] = Run(assign_statement.Rhs);
                    }
                    else if (statement is PrintStatement print_statement)
                    {
                        var parameter = print_statement.Parameter;
                        if (parameter is Symbol symbol_parameter)
                        {
                            if (UserDictionary.Variable.ContainsKey(symbol_parameter.Value)) Console.WriteLine(UserDictionary.Variable[symbol_parameter.Value]);
                            else throw new Exception(string.Format("Undefinded variable {0}", symbol_parameter.Value));
                        }
                        else Console.WriteLine(Run(print_statement.Parameter));
                    }
                    else if (statement is FunctionStatement function_statement)
                        {
                        if (UserDictionary.Function.ContainsKey(function_statement.FunctionName.Value)) throw new Exception("Existed function name");
                        UserDictionary.Function.Add(function_statement.FunctionName.Value, function_statement.Functionblock);
                    }
                    else if (statement is DoFunctionStatement do_function)
                        {
                            if (UserDictionary.Function.ContainsKey(do_function.FunctionName.Value))
                                Run(UserDictionary.Function[do_function.FunctionName.Value]);
                        }
                    else if(statement is ReturnStatement return_statement)
                    {
                        return Run(return_statement.Return_Value);
                    }
                    else if(statement is EqualStatement equal_statement)
                    {
                        if (!UserDictionary.Variable.ContainsKey(equal_statement.Lhs.Value)) throw new Exception("Unknowed variable");
                        UserDictionary.Variable[equal_statement.Lhs.Value] = Run(equal_statement.Rhs);
                    }
                    else throw new Exception("Unknowed statement");
                }
                return InterpreterPassword.MainFunction;

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
                if (UserDictionary.Variable.ContainsKey(ast_symbol.Value)) return UserDictionary.Variable[ast_symbol.Value];
                else if (UserDictionary.Function.ContainsKey(ast_symbol.Value)) return Run(UserDictionary.Function[ast_symbol.Value]);
                else throw new Exception("Unknowed variable or function");
            }
            else return 0;
        }

        
    }
}
