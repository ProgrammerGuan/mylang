using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLang
{
    /// <summary>
    /// トークン
    /// </summary>
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Text;

        public Token(string text , TokenType type)
        {
            Text = text;
            Type = type;
        }

        public bool IsNumber => (Type == TokenType.Number);

        //public bool IsNumber
        //{
        //    get { return (Type == TokenType.Number); }
        //}

        
        public bool IsSymbol => (Type == TokenType.Symbol);
        public bool IsAt => (Type == TokenType.At);
        public bool IsBinaryOperator => (Type == TokenType.Plus || Type == TokenType.Minus || Type == TokenType.Star || Type == TokenType.Slash);
        public bool IsCompareOperator => (Type == TokenType.Larger || Type == TokenType.Smaller || Type == TokenType.DoubleEqual
            || Type == TokenType.LargerEqual || Type == TokenType.SmallerEqual || Type == TokenType.NotEqual);
        public bool IsKeyWord => (Type == TokenType.Let || Type==TokenType.Print || Type==TokenType.Function 
            || Type == TokenType.Return || Type==TokenType.If || Type==TokenType.Elif || Type==TokenType.Else 
            || Type==TokenType.While || Type==TokenType.For
            || Type==TokenType.End || Type == TokenType.LeftBlock || Type == TokenType.RightBlock
            || Type==TokenType.LeftBracket || Type==TokenType.RightBracket || Type==TokenType.Equal );
        
    }

    public interface ITokenizer
    {
        /// <summary>
        /// ソースコードをトークンに分割する
        /// </summary>
        /// <param name="src">ソースコード</param>
        /// <returns>トークンのリスト</returns>
        IList<Token> Tokenize(string src);
    }
}
