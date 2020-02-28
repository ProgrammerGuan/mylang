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
        protected StatementReader NextReader = null;
        public abstract float ReadCode(Interpreter interpreter, Ast.Ast code, Ast.Block block);
    }

    public class CodeCounter : StatementReader
    {
        public CodeCounter()
        {
            this.NextReader = new AssignReader();
        }

        public override float ReadCode(Interpreter interpreter, Ast.Ast code, Block block)
        {
            return NextReader.ReadCode(interpreter, code,block);
        }
    }

    public class AssignReader : StatementReader
    {
        public AssignReader()
        {
            this.NextReader = new PrintReader();
        }
        public override float ReadCode(Interpreter interpreter, Ast.Ast code,Ast.Block block)
        {
            if (code is AssignStatement assign_statement)
            {
                if (!VariablesOwners.Dic[block.FunctionName].Variable.ContainsKey(assign_statement.Lhs.Value)) throw new Exception("Unknowed variable");
                VariablesOwners.Dic[block.FunctionName].Variable[assign_statement.Lhs.Value] = interpreter.Run(assign_statement.Rhs);
            }
            else NextReader.ReadCode(interpreter, code,block);
            return 0;
        }
    }

    public class PrintReader : StatementReader
    {
        public PrintReader()
        {
            this.NextReader = new FunctionReader();
        }
        public override float ReadCode(Interpreter interpreter, Ast.Ast code, Block block)
        {
            if (code is PrintStatement print_statement)
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
                        else throw new Exception("unknowed something");
                    }
                    Enumerable.Range(0, do_function.Parameters.Count).ToList().ForEach(i =>
                        VariablesOwners.Dic[do_function.FunctionName.Value].Variable["@" + i.ToString()] = para_value_list[i]
                    );
                    Console.WriteLine(interpreter.Run(VariablesOwners.Dic[block.FunctionName].Function[do_function.FunctionName.Value]));
                }
                else Console.WriteLine(interpreter.Run(print_statement.Parameter));
            }
            else NextReader.ReadCode(interpreter, code, block);
            return 0;
        }
    }

    public class FunctionReader : StatementReader
    {
        public FunctionReader()
        {
            NextReader = new ExpressionReader();
        }
        public override float ReadCode(Interpreter interpreter, Ast.Ast code, Block block)
        {
            if (code is FunctionStatement function_statement)
            {
                if (!VariablesOwners.Dic[block.FunctionName].Function.ContainsKey(function_statement.FunctionName.Value)) throw new Exception("Unknowed function name");
                VariablesOwners.Dic[block.FunctionName].Function[function_statement.FunctionName.Value] = function_statement.Functionblock;
            }
            else NextReader.ReadCode(interpreter, code, block);
            return 0;
        }
    }

    public class ExpressionReader : StatementReader
    {
        public ExpressionReader()
        {
            NextReader = new ReturnReader();
        }

        public override float ReadCode(Interpreter interpreter, Ast.Ast code, Block block)
        {
            if (code is ExpresstionStatement exp_statement)
            {
                interpreter.Run(exp_statement.Expression);
            }
            else NextReader.ReadCode(interpreter, code, block);
            return 0;
        }
    }

    public class ReturnReader : StatementReader
    {
        public ReturnReader()
        {
            this.NextReader = new EqualReader();
        }

        public override float ReadCode(Interpreter interpreter, Ast.Ast code, Block block)
        {
            if (code is ReturnStatement return_statement)
            {
                return interpreter.Run(return_statement.Return_Value);
            }
            else NextReader.ReadCode(interpreter, code, block);
            return 0;
        }
    }

    public class EqualReader : StatementReader
    {
        public EqualReader()
        {
            this.NextReader = new IfReader();
        }

        public override float ReadCode(Interpreter interpreter, Ast.Ast code, Block block)
        {
            if (code is EqualStatement equal_statement)
            {
                if (!VariablesOwners.Dic[block.FunctionName].Variable.ContainsKey(equal_statement.Lhs.Value)) throw new Exception("Unknowed variable");
                VariablesOwners.Dic[block.FunctionName].Variable[equal_statement.Lhs.Value] = interpreter.Run(equal_statement.Rhs);
            }
            else NextReader.ReadCode(interpreter, code, block);
            return 0;
        }
    }

    public class IfReader : StatementReader
    {
        public IfReader()
        {
            this.NextReader = null;
        }

        public override float ReadCode(Interpreter interpreter, Ast.Ast code, Block block)
        {
            if(code is IfStatement if_statement)
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
            }
            return 0;
        }
    }
}
