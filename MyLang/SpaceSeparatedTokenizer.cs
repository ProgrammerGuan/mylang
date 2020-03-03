using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
namespace MyLang
{
    public static class StringArrayExtension
    {
        public static List<string> AllToList(this System.Collections.Generic.IEnumerable<string> words)
        {
            var output = new List<string>();
            foreach (var s in words)
                if(!new Regex(@"\s+").IsMatch(s) && s!="") output.Add(s);
            return output;
        }
    }
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
        While,  // while
        For,    // for
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
    /// 単純なトークナイザ
    /// 
    /// トークンは、必ず一つ以上のスペースで区切られている必要がある
    /// </summary>
    class SpaceSeparatedTokenizer : ITokenizer {
        static Dictionary<string, TokenType> TokenTypeMap = new Dictionary<string, TokenType>
        {
            { "EOF",TokenType.Terminate},
            { "+",TokenType.Plus},
            { "-",TokenType.Minus},
            { "*",TokenType.Star},
            { "/",TokenType.Slash},
            { ">",TokenType.Larger},
            { ">=",TokenType.LargerEqual},
            { "<",TokenType.Smaller},
            { "<=",TokenType.SmallerEqual},
            { "!=",TokenType.NotEqual},
            { "==",TokenType.DoubleEqual},
            { "=",TokenType.Equal},
            { ";",TokenType.End},
            { "{",TokenType.LeftBlock},
            { "}",TokenType.RightBlock},
            { "(",TokenType.LeftBracket},
            { ")",TokenType.RightBracket},
            { "@",TokenType.At},
            { ",",TokenType.Comma},
            { "let",TokenType.Let},
            { "print",TokenType.Print},
            {"function",TokenType.Function },
            { "return",TokenType.Return},
            { "if",TokenType.If},
            { "elif",TokenType.Elif},
            { "else",TokenType.Else},
            {"while",TokenType.While },
            {"for",TokenType.For },
        };
        private readonly Regex split_word = new Regex(@"(\s+|\+|\-|\*|\/|;|\(|\)|\{|\}|\>=|\<=|==|!=|\>|\<|!|=|,)");
        public SpaceSeparatedTokenizer()
        {
        }

        public IList<Token> Tokenize(string src)
        {
            var codes = split_word.Split(src).AllToList();
            // TODO: 仮のダミー実装
            return codes.Select(c => GetToken(c)).Concat(new[] { GetToken("EOF") }).ToList();
        }

        private Token GetToken(string str)
        {
            float num;
            if (float.TryParse(str, out num)) return new Token(str, TokenType.Number);
            else if (TokenTypeMap.ContainsKey(str)) return new Token(str, TokenTypeMap[str]);
            else return new Token(str, TokenType.Symbol);
        }
    }

    
}
