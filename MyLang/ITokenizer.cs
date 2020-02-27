﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLang
{
    /// <summary>
    /// トークンの種類
    /// </summary>
    public enum TokenType
    {
        Terminate, // ソースの終わりを表す

        Number, // 数値
        Symbol, // 識別子

        Plus, // "+"
        Minus, // "-"
        Star, // "*"
        Slash, // "/"
        Equal, // "="

        //Compare Operator
        Larger, //  ">"
        Smaller,    // "<"
        DoubleEqual,    // "=="
        LargerEqual,    //  ">="
        SmallerEqual,   //  "<="
        NotEqual,   //  "!="
        //keyword
        Let,    //"let"
        Print,  // "print"
        Function,   //function
        Return, // return
        If, //if
        Elif,   //  elif
        Else,  //   else
        // keyword symbol
        End,    // ";"
        LeftBlock,  // "{"
        RightBlock, // "}"
        LeftBracket,    // "("
        RightBracket,   // ")"
        Comma,  //  ,
        At, // "@"
    }

    /// <summary>
    /// トークン
    /// </summary>
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Text;

        public Token(TokenType type, string text)
        {
            Type = type;
            Text = text;
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
            || Type==TokenType.End || Type == TokenType.LeftBlock || Type == TokenType.RightBlock
            || Type==TokenType.LeftBracket || Type==TokenType.RightBracket );
        
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
