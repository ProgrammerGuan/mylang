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
        public abstract float REPLReadCode(Interpreter interpreter, Ast.Block block, Ast.Statement statement);
    }

    public class Counter : StatementReader
    {
        public override float REPLReadCode(Interpreter interpreter, Block block, Statement statement)
        {
            return NextReader.REPLReadCode(interpreter, block, statement);
        }

        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class AssignReader : StatementReader
    {
        public override float REPLReadCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is Ast.AssignStatement assign_statement)
            {
                if (VariablesWareHouse.Stacks.Count > 0)
                    VariablesWareHouse.Stacks.Peek().SymboToValue.Add(assign_statement.Lhs.Value, interpreter.REPLRun(assign_statement.Rhs));
                else VariablesWareHouse.Global.Add(assign_statement.Lhs.Value, assign_statement.Rhs);
                return interpreter.REPLRun(assign_statement.Rhs);
            }
            else return NextReader.REPLReadCode(interpreter, block, statement);
        }

        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is Ast.AssignStatement assign_statement)
            {
                if (VariablesWareHouse.Stacks.Count > 0)
                    VariablesWareHouse.Stacks.Peek().SymboToValue.Add(assign_statement.Lhs.Value, interpreter.Run(assign_statement.Rhs));
                else VariablesWareHouse.Global.Add(assign_statement.Lhs.Value, assign_statement.Rhs);
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class PrintReader : StatementReader
    {
        public override float REPLReadCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is PrintStatement print_statement)
            {
                var parameter = print_statement.Parameter;
                if (parameter is Symbol symbol_parameter)
                    Console.WriteLine(interpreter.REPLRun(symbol_parameter));
                else if (parameter is FunctionCall do_function)
                    Console.WriteLine(interpreter.REPLRun(do_function));
                else Console.WriteLine(interpreter.REPLRun(print_statement.Parameter));
                return ReaderPassword.Password;
            }
            return NextReader.REPLReadCode(interpreter, block, statement);
        }

        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if(statement is PrintStatement print_statement)
            {
                var parameter = print_statement.Parameter;
                if (parameter is Symbol symbol_parameter)
                    Console.WriteLine(interpreter.Run(symbol_parameter));
                else if (parameter is FunctionCall do_function)
                    Console.WriteLine(interpreter.Run(do_function));
                else Console.WriteLine(interpreter.Run(print_statement.Parameter));
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class FunctionReader : StatementReader
    {
        public override float REPLReadCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is Ast.FunctionStatement function_statement)
            {
                if (VariablesWareHouse.Global.ContainsKey(function_statement.FunctionName.Value)) throw new Exception("Exist function name");
                VariablesWareHouse.Global.Add(function_statement.FunctionName.Value, function_statement);
                return ReaderPassword.Password;
            }
            return NextReader.REPLReadCode(interpreter, block, statement);
        }
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if(statement is Ast.FunctionStatement function_statement)
            {
                if(VariablesWareHouse.Global.ContainsKey(function_statement.FunctionName.Value)) throw new Exception("Exist function name");
                VariablesWareHouse.Global.Add(function_statement.FunctionName.Value, function_statement);
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }

    public class ExpressionReader : StatementReader
    {
        public override float REPLReadCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is Ast.ExpresstionStatement exp_statement)
            {
                return interpreter.REPLRun(exp_statement.Expression);
            }
            else return NextReader.REPLReadCode(interpreter, block, statement);
        }

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
        public override float REPLReadCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is ReturnStatement return_statement)
            {
                return interpreter.REPLRun(return_statement.Return_Value);
            }
            return NextReader.REPLReadCode(interpreter, block, statement);
        }
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
        public override float REPLReadCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is IfStatement if_statement)
            {
                foreach (KeyValuePair<Exp, Block> condition in if_statement.Condition_Mission)
                {
                    if (condition.Key is Compression compression)
                    {
                        if (interpreter.Compare(compression))
                        {
                            VariablesWareHouse.Stacks.Push(new MyLangStack(condition.Value.FunctionName));
                            var ans = interpreter.REPLRun(condition.Value);
                            if (ans != ReaderPassword.Password) return ans;
                            break;
                        }
                    }
                    else if (condition.Key is Number number)
                    {
                        if (number.Value != 0)
                        {
                            VariablesWareHouse.Stacks.Push(new MyLangStack(condition.Value.FunctionName));
                            var ans = interpreter.REPLRun(condition.Value);
                            if (ans != ReaderPassword.Password) return ans;
                            break;
                        }
                    }
                    else if (condition.Key is Symbol symbol)
                    {
                        if (symbol.Value == "Else")
                        {
                            VariablesWareHouse.Stacks.Push(new MyLangStack(condition.Value.FunctionName));
                            var ans = interpreter.REPLRun(condition.Value);
                            if (ans != ReaderPassword.Password) return ans;
                            break;
                        }
                    }
                }
                return ReaderPassword.Password;
            }
            return NextReader.REPLReadCode(interpreter, block, statement);
        }
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
                            VariablesWareHouse.Stacks.Push(new MyLangStack(condition.Value.FunctionName));
                            var ans = interpreter.Run(condition.Value);
                            if (ans != ReaderPassword.Password) return ans;
                            break;
                        }
                    }
                    else if (condition.Key is Number number)
                    {
                        if (number.Value != 0)
                        {
                            VariablesWareHouse.Stacks.Push(new MyLangStack(condition.Value.FunctionName));
                            var ans = interpreter.Run(condition.Value);
                            if (ans != ReaderPassword.Password) return ans;
                            break;
                        }
                    }
                    else if (condition.Key is Symbol symbol)
                    {
                        if (symbol.Value == "Else")
                        {
                            VariablesWareHouse.Stacks.Push(new MyLangStack(condition.Value.FunctionName));
                            var ans = interpreter.Run(condition.Value);
                            if (ans != ReaderPassword.Password) return ans;
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
        public override float REPLReadCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is WhileStatement while_statement)
            {
                if (while_statement.Condition is Compression compression)
                {
                    while (interpreter.Compare(compression))
                    {
                        VariablesWareHouse.Stacks.Push(new MyLangStack(while_statement.Mission.FunctionName));

                        interpreter.REPLRun(while_statement.Mission);
                    }
                }
                else if (while_statement.Condition is Number number)
                {
                    while (number.Value != 0)
                    {
                        VariablesWareHouse.Stacks.Push(new MyLangStack(while_statement.Mission.FunctionName));

                        interpreter.REPLRun(while_statement.Mission);
                    }
                }
                else if (while_statement.Condition is Symbol symbol)
                {
                    foreach (var stack in VariablesWareHouse.Stacks)
                    {
                        if (stack.SymboToValue.ContainsKey(symbol.Value))
                        {
                            while (stack.SymboToValue[symbol.Value] != 0)
                            {
                                VariablesWareHouse.Stacks.Push(new MyLangStack(while_statement.Mission.FunctionName));

                                interpreter.REPLRun(while_statement.Mission);
                            }
                            return ReaderPassword.Password;
                        }
                    }
                    if (VariablesWareHouse.Global.ContainsKey(symbol.Value))
                    {
                        while (interpreter.REPLRun(VariablesWareHouse.Global[symbol.Value]) != 0)
                        {
                            VariablesWareHouse.Stacks.Push(new MyLangStack(while_statement.Mission.FunctionName));
                            interpreter.REPLRun(while_statement.Mission);
                        }
                        return ReaderPassword.Password;
                    }
                }
                return ReaderPassword.Password;
            }
            return NextReader.REPLReadCode(interpreter, block, statement);
        }
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is WhileStatement while_statement)
            {
                if (while_statement.Condition is Compression compression)
                {
                    while (interpreter.Compare(compression))
                    {
                        VariablesWareHouse.Stacks.Push(new MyLangStack(while_statement.Mission.FunctionName));

                        interpreter.Run(while_statement.Mission);
                    }
                }
                else if (while_statement.Condition is Number number)
                {
                    while (number.Value != 0)
                    {
                        VariablesWareHouse.Stacks.Push(new MyLangStack(while_statement.Mission.FunctionName));

                        interpreter.Run(while_statement.Mission);
                    }
                }
                else if (while_statement.Condition is Symbol symbol)
                {
                    foreach(var stack in VariablesWareHouse.Stacks)
                    {
                        if (stack.SymboToValue.ContainsKey(symbol.Value))
                        {
                            while (stack.SymboToValue[symbol.Value] != 0)
                            {
                                VariablesWareHouse.Stacks.Push(new MyLangStack(while_statement.Mission.FunctionName));

                                interpreter.Run(while_statement.Mission);
                            }
                            return ReaderPassword.Password;
                        }
                    }
                    if (VariablesWareHouse.Global.ContainsKey(symbol.Value))
                    {
                        while (interpreter.Run(VariablesWareHouse.Global[symbol.Value]) != 0)
                        {
                            VariablesWareHouse.Stacks.Push(new MyLangStack(while_statement.Mission.FunctionName));
                            interpreter.Run(while_statement.Mission);
                        }
                        return ReaderPassword.Password;
                    }
                }
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter,block,statement);
        }
    }

    public class ForReader : StatementReader
    {
        public override float REPLReadCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is ForStatement for_statement)
            {
                if (for_statement.Initialize is ExpresstionStatement initial_exp)
                    interpreter.ExpressionReader.RunCode(interpreter, block, initial_exp);
                else throw new Exception("Invailed initial exp");
                if (for_statement.Condition.Expression is Compression compression)
                {
                    while (interpreter.Compare(compression))
                    {
                        VariablesWareHouse.Stacks.Push(new MyLangStack(for_statement.Mission.FunctionName));

                        interpreter.REPLRun(for_statement.Mission);
                        interpreter.REPLRun(for_statement.DoItEverytime);
                    }
                }
                else if (for_statement.Condition.Expression is Number number)
                {
                    while (number.Value != 0)
                    {
                        VariablesWareHouse.Stacks.Push(new MyLangStack(for_statement.Mission.FunctionName));

                        interpreter.REPLRun(for_statement.Mission);
                        interpreter.REPLRun(for_statement.DoItEverytime);
                    }
                }
                else if (for_statement.Condition.Expression is Symbol symbol)
                {
                    foreach (var stack in VariablesWareHouse.Stacks)
                    {
                        if (stack.SymboToValue.ContainsKey(symbol.Value))
                        {
                            while (stack.SymboToValue[symbol.Value] != 0)
                            {
                                VariablesWareHouse.Stacks.Push(new MyLangStack(for_statement.Mission.FunctionName));

                                interpreter.REPLRun(for_statement.Mission);
                            }
                            return ReaderPassword.Password;
                        }
                    }
                    if (VariablesWareHouse.Global.ContainsKey(symbol.Value))
                    {
                        while (interpreter.REPLRun(VariablesWareHouse.Global[symbol.Value]) != 0)
                        {
                            VariablesWareHouse.Stacks.Push(new MyLangStack(for_statement.Mission.FunctionName));

                            interpreter.REPLRun(for_statement.Mission);
                        }
                        return ReaderPassword.Password;
                    }
                }
                return ReaderPassword.Password;
            }
            return NextReader.REPLReadCode(interpreter, block, statement);
        }
        public override float RunCode(Interpreter interpreter, Block block, Statement statement)
        {
            if (statement is ForStatement for_statement)
            {
                if (for_statement.Initialize is ExpresstionStatement initial_exp)
                    interpreter.ExpressionReader.RunCode(interpreter, block, initial_exp);
                else throw new Exception("Invailed initial exp");
                if (for_statement.Condition.Expression is Compression compression)
                {
                    while (interpreter.Compare(compression))
                    {
                        VariablesWareHouse.Stacks.Push(new MyLangStack(for_statement.Mission.FunctionName));

                        interpreter.Run(for_statement.Mission);
                        interpreter.Run(for_statement.DoItEverytime);
                    }
                }
                else if (for_statement.Condition.Expression is Number number)
                {
                    while (number.Value != 0)
                    {
                        VariablesWareHouse.Stacks.Push(new MyLangStack(for_statement.Mission.FunctionName));

                        interpreter.Run(for_statement.Mission);
                        interpreter.Run(for_statement.DoItEverytime);
                    }
                }
                else if (for_statement.Condition.Expression is Symbol symbol)
                {
                    foreach (var stack in VariablesWareHouse.Stacks)
                    {
                        if (stack.SymboToValue.ContainsKey(symbol.Value))
                        {
                            while (stack.SymboToValue[symbol.Value] != 0)
                            {
                                VariablesWareHouse.Stacks.Push(new MyLangStack(for_statement.Mission.FunctionName));

                                interpreter.Run(for_statement.Mission);
                            }
                            return ReaderPassword.Password;
                        }
                    }
                    if (VariablesWareHouse.Global.ContainsKey(symbol.Value))
                    {
                        while (interpreter.Run(VariablesWareHouse.Global[symbol.Value]) != 0)
                        {
                            VariablesWareHouse.Stacks.Push(new MyLangStack(for_statement.Mission.FunctionName));

                            interpreter.Run(for_statement.Mission);
                        }
                        return ReaderPassword.Password;
                    }
                }
                return ReaderPassword.Password;
            }
            else return NextReader.RunCode(interpreter, block, statement);
        }
    }
}
