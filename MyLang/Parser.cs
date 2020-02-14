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

        public Parser()
        {

        }

        /// <summary>
        /// 現在のトークンを取得する
        /// </summary>
        /// <returns></returns>
        Token currentToken()
        {
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
            
            //var lhs = new Ast.Number(1);
            //var rhs = new Ast.Number(2);
            //var ast = new Ast.BinOp(Ast.BinOpType.Add, lhs, rhs);

            return Exp1();
        }
        
        private Ast.Exp Exp1()
        {
            var lhs = Exp2();
            if (lhs == null)
                return null;
            var token = currentToken();
            if (token.Type == TokenType.Plus || token.Type == TokenType.Minus)
            {
                var binOpType = BinOpMap[token.Type];
                progress();
                var rhs = Exp1();
                var ast = new Ast.BinOp(binOpType, lhs, rhs);
                return ast;
            }
            else
            {
                return lhs;
            }
            
        }

        private Ast.Exp Exp2()
        {
            var lhs = Exp_Value();
            if (lhs == null)
                return null;
            if (pos_ == tokens_.Count - 1)
                return lhs;
            progress();
            var token = currentToken();
            if (token.Type == TokenType.Star || token.Type == TokenType.Slash)
            {
                var binOpType = BinOpMap[token.Type];
                progress();
                var rhs = Exp2();
                var ast = new Ast.BinOp(binOpType, lhs, rhs);
                return ast;
            }
            else
            {
                return lhs;
            }
        }

        private Ast.Exp Exp_Value()
        {
            var token = currentToken();
            if (token.IsNumber)
                return new Ast.Number(Convert.ToSingle(token.Text));
            else if (token.IsSymbol)
                return new Ast.Symbol(token.Text);
            else throw new Exception(string.Format("Invalid input {0}", token.Text));

        }

    }
}
