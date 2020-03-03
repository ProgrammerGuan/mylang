using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLang
{
    public class Parser
    {
        IList<Token> tokens_;
        int pos_ = 0;

        static Dictionary<TokenType, Ast.BinOpType> BinOpMap = new Dictionary<TokenType, Ast.BinOpType>
        {
            {TokenType.Plus, Ast.BinOpType.Add },
            {TokenType.Minus, Ast.BinOpType.Sub },
            {TokenType.Star, Ast.BinOpType.Multiply },
            {TokenType.Slash, Ast.BinOpType.Divide },
        };

        public static Dictionary<TokenType, Ast.KeywordType> KeywordMap = new Dictionary<TokenType, Ast.KeywordType>
        {
            {TokenType.Let,Ast.KeywordType.Let },
            {TokenType.Equal,Ast.KeywordType.Equal },
            {TokenType.Print,Ast.KeywordType.Print},
            {TokenType.End,Ast.KeywordType.End },
            {TokenType.RightBlock,Ast.KeywordType.Rightblock },
            {TokenType.LeftBlock,Ast.KeywordType.Leftblock },
            {TokenType.RightBracket,Ast.KeywordType.RightBracket},
            {TokenType.LeftBracket,Ast.KeywordType.LeftBracket},
            {TokenType.Return,Ast.KeywordType.Return },
            {TokenType.At,Ast.KeywordType.At },
            {TokenType.Comma,Ast.KeywordType.Comma },
            {TokenType.If,Ast.KeywordType.If },
            {TokenType.Elif,Ast.KeywordType.Elif },
            {TokenType.Else,Ast.KeywordType.Else },
            {TokenType.While,Ast.KeywordType.While },
        };

        static Dictionary<TokenType, Ast.CompareOpType> CompareOpMap = new Dictionary<TokenType, Ast.CompareOpType>
        {
            {TokenType.Larger,Ast.CompareOpType.Larger },
            {TokenType.Smaller,Ast.CompareOpType.Smaller },
            {TokenType.DoubleEqual,Ast.CompareOpType.DoubleEqual},
            {TokenType.LargerEqual,Ast.CompareOpType.LargerEqual },
            {TokenType.SmallerEqual,Ast.CompareOpType.SmallerEqual },
            {TokenType.NotEqual,Ast.CompareOpType.NotEqual},
        };
        public Parser()
        {

        }

        /// <summary>
        /// 現在のトークンを取得する
        /// </summary>
        /// <returns></returns>
        Token currentToken()
        {
            if (pos_ >= tokens_.Count) return null;
            return tokens_[pos_];
        }

        /// <summary>
        /// 次のトークンに進む
        /// </summary>
        void progress()
        {
            Logger.Trace($"progress {currentToken().Text}");
            pos_++;
        }

        public Ast.Ast Parse(IList<Token> tokens)
        {
            tokens_ = tokens;
            tokens_.RemoveAt(tokens_.Count - 1);
            pos_ = 0;

            // TODO: 仮のダミー実装
            var main_block = new Ast.Block();
            return Exp1(main_block);
        }

        public Ast.Ast ProgramingParse(IList<Token> tokens)
        {
            tokens_ = tokens;
            tokens_.RemoveAt(tokens_.Count - 1);
            pos_ = 0;
            var main_block = new Ast.Block();
            return Block(main_block);
        }

        /// <summary>
        /// Blockを作る
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private Ast.Block Block(Ast.Block block)
        {
            if (!VariablesOwners.Dic.ContainsKey(block.FunctionName))
            {
                var newDic = new VariablesOwners.UserDictionary();
                VariablesOwners.Dic.Add(block.FunctionName, newDic);
            }
            var statement = Statement(block);
            if (statement == null) return block;
            else
            {
                block.AddStatement(statement);
                return Block_Rest(block);
            }
        }

        private Ast.Block Block_Rest(Ast.Block block)
        {
            var next_block = Block(block);
            return block;
        }
        /// <summary>
        /// どのStatementを確認すると作る
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private Ast.Ast Statement(Ast.Block block)
        {
            var token = currentToken();
            if (token == null) return null;
            if (token.IsKeyWord)
            {
                progress();

                switch (token.Type)
                {
                    case TokenType.Let:
                        return AssignStatement(block);
                    case TokenType.Print:
                        return PrintStatement(block);
                    case TokenType.Function:
                        return FunctionStatement(block);
                    case TokenType.RightBlock:
                        pos_ -= 1;
                        return null;
                    case TokenType.Return:
                        return ReturnStatement(block);
                    case TokenType.End:
                        return null;
                    case TokenType.If:
                        return IfStatement(block);
                    case TokenType.While:
                        return WhileStatement(block);
                    default:
                        throw new Exception("Unknowed Keyword");
                }
            }
            else
                return ExpressionStatement(block);
            
        }

        private Ast.AssignStatement AssignStatement(Ast.Block block)
        {
            var lhs = Exp_Value(block);
            if (lhs is Ast.Symbol lhs_sym)
            {
                var token = currentToken();
                if (token.Type != TokenType.Equal) throw new Exception(string.Format("Let keyword need '=' to assign value to {0}", lhs_sym.Value));
                progress();
                var minus_symbol = currentToken();
                var IsMinus = false;
                if (minus_symbol.Type == TokenType.Minus)
                {
                    progress();
                    IsMinus = true;
                }
                var rhs = Exp1(block);
                if (rhs == null) throw new Exception(string.Format("Let keyword need right hand side value while assigning to {0}", lhs_sym.Value));
                else if (rhs is Ast.Exp exp)
                {
                    var end_sign = Statement_Keyword();
                    if (end_sign .Type == Ast.KeywordType.End)
                    {
                        if (VariablesOwners.Dic[block.FunctionName].Variable.ContainsKey(lhs_sym.Value)) throw new Exception("Existed Variable");
                        VariablesOwners.Dic[block.FunctionName].Variable.Add(lhs_sym.Value, 0);
                        if(IsMinus) return new Ast.AssignStatement(lhs_sym, new Ast.BinOp(Ast.BinOpType.Sub ,new Ast.Number(0),rhs));
                        return new Ast.AssignStatement(lhs_sym, rhs);
                    }
                    else throw new Exception("need ';' after statement");
                }
                else throw new Exception(string.Format("The right hand side value need to be exception while assigning"));
            }
            else throw new Exception("Error Left Hand Side Symbol while assigning");
        }

        private Ast.PrintStatement PrintStatement(Ast.Block block)
        {
            var exp = Exp1(block);
            if (exp == null) throw new Exception("print need parameter after it ");
            var end_sign = Statement_Keyword();
            if (end_sign.Type != Ast.KeywordType.End) throw new Exception("need ';' after statement");
            return new Ast.PrintStatement(exp);
        }

        private Ast.FunctionStatement FunctionStatement(Ast.Block block)
        {
            var token = currentToken();
            progress();
            var function_name = VariableOrFunctionCall(token,block,true);
            if (function_name is Ast.Symbol name)
            {
                var left_block = Statement_Keyword();
                if (left_block.Type != Ast.KeywordType.Leftblock) throw new Exception("Need a '{' ");
                var function_block = new Ast.Block(name.Value);
                var function_statement = new Ast.FunctionStatement(name, Block(function_block));
                var right_block = Statement_Keyword();
                if (right_block.Type != Ast.KeywordType.Rightblock) throw new Exception("Need a '}' ");
                return function_statement;
            }
            else if(function_name is Ast.FunctionCall function_call)
            {
                var left_block = Statement_Keyword();
                if (left_block.Type != Ast.KeywordType.Leftblock) throw new Exception("Need a '{' ");
                var function_block = new Ast.Block(function_call.FunctionName.Value);
                var function_statement = new Ast.FunctionStatement(function_call.FunctionName, Block(function_block));
                var right_block = Statement_Keyword();
                if (right_block.Type != Ast.KeywordType.Rightblock) throw new Exception("Need a '}' ");
                return function_statement;
            }
            else throw new Exception("Invalid function name");
        }

        private Ast.ReturnStatement ReturnStatement(Ast.Block block)
        {
            var exp = Exp1(block);
            if (exp == null) return null;
            return new Ast.ReturnStatement(exp);
        }

        private Ast.ExpresstionStatement ExpressionStatement(Ast.Block block)
        {
            var exp = Exp(block);
            if (exp == null) throw new Exception("None expression");
            var end_sign = Statement_Keyword();
            if (end_sign.Type != Ast.KeywordType.End) throw new Exception("need ';' after statement");
            return new Ast.ExpresstionStatement(exp);
        }

        private Ast.IfStatement IfStatement(Ast.Block block)
        {
            var left_barcket = Statement_Keyword();
            if (left_barcket.Type != Ast.KeywordType.LeftBracket) throw new Exception("Need '(' ");
            var condition = Exp(block);
            var right_barcket = Statement_Keyword();
            if (right_barcket.Type != Ast.KeywordType.RightBracket) throw new Exception("Need '(' ");
            var left_block = Statement_Keyword();
            if (left_block.Type != Ast.KeywordType.Leftblock) throw new Exception("Need '{' ");
            var if_block = Block(new Ast.Block("if"+Ast.IfStatement.IfCount));
            var right_block = Statement_Keyword();
            if (right_block.Type != Ast.KeywordType.Rightblock) throw new Exception("Need '}' ");
            var if_statement = new Ast.IfStatement();
            if_statement.AddCondition(condition, if_block);
            ElifStatement(if_statement, block);
            return ElseStatement(if_statement, block);
        }

        private void ElifStatement(Ast.IfStatement ifStatement, Ast.Block block)
        {
            var elif = currentToken();
            if (elif.Type != TokenType.Elif) return;
            progress();
            var left_barcket = Statement_Keyword();
            if (left_barcket.Type != Ast.KeywordType.LeftBracket) throw new Exception("Need '(' ");
            var condition = Exp(block);
            var right_barcket = Statement_Keyword();
            if (right_barcket.Type != Ast.KeywordType.RightBracket) throw new Exception("Need '(' ");
            var left_block = Statement_Keyword();
            if (left_block.Type != Ast.KeywordType.Leftblock) throw new Exception("Need '{' ");
            var if_block = Block(new Ast.Block("if" + Ast.IfStatement.IfCount));
            var right_block = Statement_Keyword();
            if (right_block.Type != Ast.KeywordType.Rightblock) throw new Exception("Need '}' ");
            ifStatement.AddCondition(condition, if_block);
            ElifStatement(ifStatement, block);
        }

        private Ast.IfStatement ElseStatement(Ast.IfStatement ifStatement, Ast.Block block)
        {
            var els = currentToken();
            if (els.Type != TokenType.Else) return ifStatement;
            progress();
            var left_block = Statement_Keyword();
            if (left_block.Type != Ast.KeywordType.Leftblock) throw new Exception("Need '{' ");
            var if_block = Block(new Ast.Block("if" + Ast.IfStatement.IfCount));
            var right_block = Statement_Keyword();
            if (right_block.Type != Ast.KeywordType.Rightblock) throw new Exception("Need '}' ");
            ifStatement.AddCondition(new Ast.Symbol("Else", if_block.FunctionName), if_block);
            return ifStatement;
        }

        private Ast.WhileStatement WhileStatement(Ast.Block block)
        {
            var left_barcket = Statement_Keyword();
            if (left_barcket.Type != Ast.KeywordType.LeftBracket) throw new Exception("Need '(' ");
            var condition = Exp(block);
            var right_barcket = Statement_Keyword();
            if (right_barcket.Type != Ast.KeywordType.RightBracket) throw new Exception("Need '(' ");
            var left_block = Statement_Keyword();
            if (left_block.Type != Ast.KeywordType.Leftblock) throw new Exception("Need '{' ");
            var while_block = Block(new Ast.Block("while" + Ast.WhileStatement.WhileCount++));
            var right_block = Statement_Keyword();
            if (right_block.Type != Ast.KeywordType.Rightblock) throw new Exception("Need '}' ");
            return new Ast.WhileStatement(condition, while_block);
        }

        private Ast.Exp Exp(Ast.Block block)
        {
            var lhs = Exp1(block);
            if (lhs == null) return null;
            var compareOp = currentToken();
            if(compareOp.Type == TokenType.Equal)
            {
                progress();
                var equal_rhs = Exp(block);
                if (lhs is Ast.Symbol lhs_sym)
                {
                    return new Ast.EqualExp(lhs_sym, equal_rhs);
                }
                else throw new Exception("Unknowed symbol");
            }
            if (!compareOp.IsCompareOperator) return lhs;
            progress();
            var rhs = Exp1(block);
            return new Ast.Compression(CompareOpMap[compareOp.Type], lhs, rhs);
        }

        private Ast.Exp Exp1(Ast.Block block)
        {
            var lhs = Exp2(block);
            if (lhs == null) return null;
            else return Exp1_Rest(lhs,block);
        }

        private Ast.Exp Exp1_Rest(Ast.Exp lhs,Ast.Block block)
        {
            var token = currentToken();
            if (token == null)
                return lhs;
            if (token.Type == TokenType.Plus || token.Type == TokenType.Minus)
            {
                progress();
                var rhs = Exp2(block);
                var new_lhs = new Ast.BinOp(BinOpMap[token.Type], lhs, rhs);
                return Exp1_Rest(new_lhs,block);
            }
            else
                return lhs;
        }

        private Ast.Exp Exp2(Ast.Block block)
        {
            var lhs = Exp_Value(block);
            if (lhs == null) return null;
            else return Exp2_Rest(lhs,block);
        }

        private Ast.Exp Exp2_Rest(Ast.Exp lhs,Ast.Block block)
        {
            var token = currentToken();
            if (token == null)
                return lhs;
            if (token.Type == TokenType.Star || token.Type == TokenType.Slash)
            {
                progress();
                var rhs = Exp_Value(block);
                var tmp_lhs = new Ast.BinOp(BinOpMap[token.Type], lhs, rhs);
                return Exp2_Rest(tmp_lhs,block);
            }
            else
                return lhs;
        }

        private Ast.Exp Exp_Value(Ast.Block block)
        {
            var token = currentToken();
            progress();
            switch (token.Type)
            {
                case TokenType.Number:
                    return new Ast.Number(Convert.ToSingle(token.Text));
                case TokenType.Symbol:
                    return VariableOrFunctionCall(token,block,false);
                case TokenType.At:
                    var index = currentToken();
                    if (!index.IsNumber) throw new Exception("Index must be number");
                    progress();
                    return new Ast.LocateSymbol(Convert.ToSingle(index.Text), block.FunctionName);
                default:
                    pos_ -= 1;
                    return null;//(string.Format("Invalid input {0}", token.Text));
            }
        }

        private Ast.Exp VariableOrFunctionCall(Token token, Ast.Block block,bool make_function)
        {
            var next_token = currentToken();
            if (next_token.Type == TokenType.LeftBracket) return FunctionCall(token, block, make_function);
            if (make_function)
            {
                if (VariablesOwners.Dic[block.FunctionName].Function.ContainsKey(token.Text)) throw new Exception("Existed Function name");
                VariablesOwners.Dic[block.FunctionName].Function.Add(token.Text, null);
            }
            return new Ast.Symbol(token.Text, block.FunctionName);
            
        }

        private Ast.FunctionCall FunctionCall(Token function, Ast.Block block,bool make_function)
        {
            var left_bracket = Statement_Keyword();
            if (left_bracket.Type != Ast.KeywordType.LeftBracket) throw new Exception("Need a '(' ");
            var parameters = new List<Ast.Exp>();
            Parameters(parameters, block);
            var right_brack = Statement_Keyword();
            if (right_brack.Type != Ast.KeywordType.RightBracket) throw new Exception("Need a ')' ");
            if (make_function)
            {
                if (VariablesOwners.Dic[block.FunctionName].Function.ContainsKey(function.Text)) throw new Exception("Existed Function name");
                VariablesOwners.Dic[block.FunctionName].Function.Add(function.Text, null);
                VariablesOwners.Dic.Add(function.Text, new VariablesOwners.UserDictionary());
                foreach(Ast.Exp par in parameters)
                {
                    if (par is Ast.Symbol function_variable)
                    {
                        VariablesOwners.Dic[function.Text].Variable.Add(function_variable.Value, 0);
                        VariablesOwners.Dic[function.Text].ParameterList.Add(function_variable.Value);
                    }
                    else throw new Exception("unknowed variable");
                }
            }
            return new Ast.FunctionCall(function.Text, block.FunctionName, parameters);
        }

        private List<Ast.Exp> Parameters(List<Ast.Exp> parameters, Ast.Block block)
        {
            var parameter = Exp1(block);
            if (parameter == null) return parameters;
            parameters.Add(parameter);
            return Parameter_rest(parameters, block);
        }

        private List<Ast.Exp> Parameter_rest(List<Ast.Exp> parameters, Ast.Block block)
        {
            var comma = currentToken();
            if (comma.Type == TokenType.RightBracket) return parameters;
            if (comma.Type != TokenType.Comma) throw new Exception("need a ',' ");
            progress();
            var next_parameter = Parameters(parameters, block);
            return next_parameter;
        }

        private Ast.Keyword Statement_Keyword()
        {
            var token = currentToken();
            progress();
            if (token.IsKeyWord)
                return new Ast.Keyword(token.Text, token.Type);
            else throw new Exception(string.Format("Unknowed sign in statement"));
        }


    }
}
