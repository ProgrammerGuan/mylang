﻿using System;
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
        };

        //public Parser()
        //{

        //}

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
        private Ast.Statement Statement(Ast.Block block)
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
                    default:
                        throw new Exception("Unknowed Keyword");
                }
            }
            else if (token.IsSymbol)
            {
                progress();
                if (VariablesOwners.Dic[block.FunctionName].Function.ContainsKey(token.Text)) return DoFunctionStatement(token, block);
                else if (VariablesOwners.Dic[block.FunctionName].Variable.ContainsKey(token.Text))
                {
                    var operater = currentToken();
                    progress();
                    if (operater == null) throw new Exception("None active");
                    else if (operater.Type == TokenType.Equal)
                    {
                        var lhs = new Ast.Symbol(token.Text,block.FunctionName);
                        return EqualStatement(lhs,block);
                    }
                    else throw new Exception("Unknowed active");
                }
                else throw new Exception("Unknowed function");
            }
            else throw new Exception("Error Statement");
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

        private Ast.EqualStatement EqualStatement(Ast.Symbol lhs,Ast.Block block)
        {
            Ast.Ast rhs;
            var rhs_exp = Exp1(block);
            if(rhs_exp !=null)
            {
                rhs = rhs_exp;
            }
            else
            {
                var rhs_statement = Statement(block);
                if (rhs_statement == null) throw new Exception("Equal statement havn't got right hand side");
                else rhs = rhs_exp;
            }
            var end_sign = Statement_Keyword();
            if (end_sign.Type == Ast.KeywordType.End)
                return new Ast.EqualStatement(lhs, rhs);
            else throw new Exception("need ';' after statement");
        }

        private Ast.PrintStatement PrintStatement(Ast.Block block)
        {
            var parameter = Statement(block);
            if (parameter == null) throw new Exception("print need parameter after it ");
            else
            {
                if (parameter is Ast.DoFunctionStatement fun) return new Ast.PrintStatement(fun);
                var end_sign = Statement_Keyword();
                if (end_sign.Type == Ast.KeywordType.End)
                    return new Ast.PrintStatement(parameter);
                else throw new Exception("need ';' after statement");
            }
        }

        private Ast.FunctionStatement FunctionStatement(Ast.Block block)
        {
            var function_name = Exp1(block);
            if (function_name is Ast.Symbol name)
            {
                var left_block = Statement_Keyword();
                if (left_block.Type != Ast.KeywordType.Leftblock) throw new Exception("Need a '{' ");
                var function_block = new Ast.Block(name.Value);
                var function_statement = new Ast.FunctionStatement(name, Block(function_block));
                var right_block = Statement_Keyword();
                if (right_block.Type != Ast.KeywordType.Rightblock) throw new Exception("Need a '}' ");
                if (VariablesOwners.Dic[block.FunctionName].Function.ContainsKey(function_block.FunctionName)) throw new Exception("Existed Function name");
                VariablesOwners.Dic[block.FunctionName].Function.Add(function_block.FunctionName, null);
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

        private Ast.DoFunctionStatement DoFunctionStatement(Token function,Ast.Block block)
        {
            var left_bracket = Statement_Keyword();
            if (left_bracket.Type != Ast.KeywordType.LeftBracket) throw new Exception("Need a '(' ");
            var parameters = new List<Ast.Exp>();
            Parameters(parameters,block);
            var right_brack = Statement_Keyword();
            if (right_brack.Type != Ast.KeywordType.RightBracket) throw new Exception("Need a ')' ");
            var end_sign = Statement_Keyword();
            if (end_sign.Type != Ast.KeywordType.End) throw new Exception("Need ';' ");
            Enumerable.Range(0, parameters.Count).ToList().ForEach(i =>
                VariablesOwners.Dic[function.Text].Variable.Add("@" + i.ToString(), 0)
            );
            return new Ast.DoFunctionStatement(function.Text,block.FunctionName,parameters);
        }

        private  List<Ast.Exp> Parameters(List<Ast.Exp> parameters,Ast.Block block)
        {
            var parameter = Exp_Value(block);
            if (parameter == null) return parameters;
            parameters.Add(parameter);
            return Parameter_rest(parameters, block);
        } 

        private List<Ast.Exp> Parameter_rest(List<Ast.Exp> parameters,Ast.Block block)
        {
            var comma = currentToken();
            if (comma.Type == TokenType.RightBracket) return parameters;
            if (comma.Type != TokenType.Comma) throw new Exception("need a ',' ");
            progress();
            var next_parameter = Parameters(parameters,block);
            return next_parameter;
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
            if (token.IsNumber)
                return new Ast.Number(Convert.ToSingle(token.Text));
            else if (token.IsSymbol)
                return new Ast.Symbol(token.Text,block.FunctionName);
            else if (token.IsAt)
            {
                var index = currentToken();
                if (!index.IsNumber) throw new Exception("Index must be number");
                progress();
                return new Ast.Locate(Convert.ToSingle(index.Text),block.FunctionName);
            }
                
            else throw new Exception(string.Format("Invalid input {0}", token.Text));

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
