using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLang.Ast;

namespace MyLang
{
     public abstract class StatementReader
    {
        protected StatementReader NextReader;
        public virtual void SetNextReader(StatementReader reader)
        {
            NextReader = reader;
        }
        public abstract float RunCode(Interpreter interpreter, Ast.Block block, Ast.Statement statement);
    }

    public class Counter : StatementReader
    {
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class AssignReader : StatementReader
    {
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is Ast.AssignStatement assign_statement)
            {
                if (!VariablesOwners.Dic[block.FunctionName].Variable.ContainsKey(assign_statement.Lhs.Value)) throw new Exception("Unknowed variable");
                VariablesOwners.Dic[block.FunctionName].Variable[assign_statement.Lhs.Value] = interpreter.Run(assign_statement.Rhs);
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class PrintReader : StatementReader
    {
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if(statement is PrintStatement print_statement)
            {
                var parameter = print_statement.Parameter;
                if (parameter is Symbol symbol_parameter)
                {
                    if (VariablesOwners.Dic[block.FunctionName].Variable.ContainsKey(symbol_parameter.Value)) Console.WriteLine(VariablesOwners.Dic[block.FunctionName].Variable[symbol_parameter.Value]);
                    else if (VariablesOwners.Dic["main"].Variable.ContainsKey(symbol_parameter.Value)) Console.WriteLine(VariablesOwners.Dic["main"].Variable[symbol_parameter.Value]);
                    else throw new Exception(string.Format("Undefinded variable {0}", symbol_parameter.Value));
                }
                else if (parameter is FunctionCall do_function)
                {
                    if (!VariablesOwners.Dic[block.FunctionName].Function.ContainsKey(do_function.FunctionName.Value)) throw new Exception("Unknowed function");
                    var para_value_list = new List<float>();
                    foreach (Exp para in do_function.Parameters)
                    {
                        if (para is Number num) para_value_list.Add(num.Value);
                        else if (para is Symbol sym) para_value_list.Add(VariablesOwners.Dic[do_function.Owner].Variable[sym.Value]);
                        else if (para is BinOp op) para_value_list.Add(interpreter.Run(op));
                        else throw new Exception("unknowed something");
                    }
                    Enumerable.Range(0, do_function.Parameters.Count).ToList().ForEach(i =>
                        VariablesOwners.Dic[do_function.FunctionName.Value].Variable[VariablesOwners.Dic[do_function.FunctionName.Value].ParameterList[i]] = para_value_list[i]
                    );
                    Console.WriteLine(interpreter.Run(VariablesOwners.Dic[block.FunctionName].Function[do_function.FunctionName.Value]));
                }
                else Console.WriteLine(interpreter.Run(print_statement.Parameter));
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class FunctionReader : StatementReader
    {
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if(statement is Ast.FunctionStatement function_statement)
            {
                if (!VariablesOwners.Dic[block.FunctionName].Function.ContainsKey(function_statement.FunctionName.Value)) throw new Exception("Unknowed function name");
                VariablesOwners.Dic[block.FunctionName].Function[function_statement.FunctionName.Value] = function_statement.Functionblock;
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class ExpressionReader : StatementReader
    {
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if(statement is Ast.ExpresstionStatement exp_statement)
            {
                interpreter.Run(exp_statement.Expression);
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class ReturnReader : StatementReader
    {
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if(statement is ReturnStatement return_statement)
            {
                return interpreter.Run(return_statement.Return_Value);
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class IfReader : StatementReader
    {
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if(statement is IfStatement if_statement)
            {
                foreach (KeyValuePair<Exp, Block> condition in if_statement.Condition_Mission)
                {
                    if (condition.Key is Compression compression)
                    {
                        if (interpreter.Compare(compression))
                        {
                            interpreter.Run(condition.Value);
                            break;
                        }
                    }
                    else if (condition.Key is Number number)
                    {
                        if (number.Value != 0)
                        {
                            interpreter.Run(condition.Value);
                            break;
                        }
                    }
                    else if (condition.Key is Symbol symbol)
                    {
                        if (symbol.Value == "Else")
                        {
                            interpreter.Run(condition.Value);
                            break;
                        }
                    }
                }
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class WhileReader : StatementReader
    {
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is WhileStatement while_statement)
            {
                if (while_statement.Condition is Compression compression)
                {
                    while (interpreter.Compare(compression))
                    {
                        interpreter.Run(while_statement.Mission);
                    }
                }
                else if (while_statement.Condition is Number number)
                {
                    while (number.Value != 0)
                    {
                        interpreter.Run(while_statement.Mission);
                    }
                }
                else if (while_statement.Condition is Symbol symbol)
                {
                    while (VariablesOwners.Dic[block.FunctionName].Variable[symbol.Value] != 0)
                    {
                        interpreter.Run(while_statement.Mission);
                    }
                }
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter,block,statement);
        }
    }
}
