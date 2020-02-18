using System;
using System.Collections.Generic;
using System.Text;
using MyLang.Ast;

namespace MyLang
{
    public class Interpreter
    {

        public Dictionary<string, float> UserVariableList = new Dictionary<string, float>{};

        public Interpreter()
        {
        }

        public float Run(Ast.Ast ast)
        {
            // TODO: 仮のダミー実装
            if (ast is Ast.Exp exp)
                return Answer(exp);
            else if(ast is Ast.AssignStatement assign_statement)
            {
                UserVariableList.Add(assign_statement.Lhs.Value, Run(assign_statement.Rhs));
                return 0;
            }
            else if (ast is Ast.PrintStatement print_statement)
            {
                var parameter = print_statement.Parameter;
                if(parameter is Ast.Symbol symbol_parameter)
                {
                    if (UserVariableList.ContainsKey(symbol_parameter.Value)) Console.WriteLine(UserVariableList[symbol_parameter.Value]);
                    else throw new Exception(string.Format("Undefinded variable {0}",symbol_parameter.Value));
                }
                else Console.WriteLine(Run(ast));
                return 0;
            }
            else
                return 0;
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
            {
                return ast_num.Value;
            }
            else return 0;
        }

        
    }
}
