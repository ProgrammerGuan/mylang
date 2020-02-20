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
            {TokenType.Equal,Ast.BinOpType.Equal },
        };

        public static Dictionary<TokenType, Ast.KeywordType> KeywordMap = new Dictionary<TokenType, Ast.KeywordType>
        {
            {TokenType.Let,Ast.KeywordType.Let },
            {TokenType.Print,Ast.KeywordType.Print},
            {TokenType.End,Ast.KeywordType.End },
            {TokenType.RightBlock,Ast.KeywordType.Rightblock },
            {TokenType.LeftBlock,Ast.KeywordType.Leftblock },
            {TokenType.Return,Ast.KeywordType.Return }
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
            return Block(main_block);
        }

        private Ast.Block Block(Ast.Block block)
        {
            var statement = Statement();
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

        private Ast.Statement Statement()
        {
            var token = currentToken();
            if (token == null) return null;
            if (token.IsKeyWord)
            {
                progress();
                switch (token.Type)
                {
                    case TokenType.Let:
                        return AssignStatement();
                    case TokenType.Print:
                        return PrintStatement();
                    case TokenType.Function:
                        return FunctionStatement();
                    case TokenType.RightBlock:
                        pos_ -= 1;
                        return null;
                    case TokenType.Return:
                        return ReturnStatement();
                    case TokenType.End:
                        return null;
                    default:
                        throw new Exception("Unknowed Keyword");
                }
            }
            else if (token.IsSymbol)
            {
                progress();
                return new Ast.DoFunctionStatement(token.Text);
            }
            else throw new Exception("Error Statement");
        }

        private Ast.AssignStatement AssignStatement()
        {
            var lhs = Exp_Value();
            if (lhs is Ast.Symbol lhs_sym)
            {
                var token = currentToken();
                if (token.Type != TokenType.Equal) throw new Exception(string.Format("Let keyword need '=' to assign value to {0}", lhs_sym.Value));
                progress();
                var rhs = Exp1();
                if (rhs == null) throw new Exception(string.Format("Let keyword need right hand side value while assigning to {0}", lhs_sym.Value));
                else if (rhs is Ast.Exp exp)
                {
                    var end_sign = Statement_Keyword();
                    if (end_sign .Type == Ast.KeywordType.End)
                        return new Ast.AssignStatement(lhs_sym, rhs);
                    else throw new Exception("need ';' after statement");
                }
                else throw new Exception(string.Format("The right hand side value need to be exception while assigning"));
            }
            else throw new Exception("Error Left Hand Side Symbol while assigning");
        }

        private Ast.PrintStatement PrintStatement()
        {
            var parameter = Exp1();
            if (parameter == null) throw new Exception("print need parameter after it ");
            else
            {
                var end_sign = Statement_Keyword();
                if (end_sign.Type == Ast.KeywordType.End)
                    return new Ast.PrintStatement(parameter);
                else throw new Exception("need ';' after statement");
            }
        }

        private Ast.FunctionStatement FunctionStatement()
        {
            var function_name = Exp1();
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
            else throw new Exception("Invalid function name");
        }

        private Ast.ReturnStatement ReturnStatement()
        {
            var exp = Exp1();
            if (exp == null) return null;
            return new Ast.ReturnStatement(exp);
        }

        private Ast.Exp Exp1()
        {
            var lhs = Exp2();
            if (lhs == null) return null;
            else return Exp1_Rest(lhs);
        }

        private Ast.Exp Exp1_Rest(Ast.Exp lhs)
        {
            var token = currentToken();
            if (token == null)
                return lhs;
            if (token.Type == TokenType.Plus || token.Type == TokenType.Minus)
            {
                progress();
                var rhs = Exp2();
                var new_lhs = new Ast.BinOp(BinOpMap[token.Type], lhs, rhs);
                return Exp1_Rest(new_lhs);
            }
            else
                return lhs;
        }

        private Ast.Exp Exp2()
        {
            var lhs = Exp_Value();
            if (lhs == null) return null;
            else return Exp2_Rest(lhs);
        }

        private Ast.Exp Exp2_Rest(Ast.Exp lhs)
        {
            var token = currentToken();
            if (token == null)
                return lhs;
            if (token.Type == TokenType.Star || token.Type == TokenType.Slash)
            {
                progress();
                var rhs = Exp_Value();
                var tmp_lhs = new Ast.BinOp(BinOpMap[token.Type], lhs, rhs);
                return Exp2_Rest(tmp_lhs);
            }
            else
                return lhs;
        }

        private Ast.Exp Exp_Value()
        {
            var token = currentToken();
            progress();
            if (token.IsNumber)
                return new Ast.Number(Convert.ToSingle(token.Text));
            else if (token.IsSymbol)
                return new Ast.Symbol(token.Text);
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
