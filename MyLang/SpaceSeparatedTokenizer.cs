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
    /// 単純なトークナイザ
    /// 
    /// トークンは、必ず一つ以上のスペースで区切られている必要がある
    /// </summary>
    class SpaceSeparatedTokenizer : ITokenizer { 

        private readonly Regex split_word = new Regex(@"(\s+|\+|\-|\*|\/|;|\(|\)|\{|\}|\>=|\<=|==|!=|\>|\<|!|=)");
        private readonly Regex space = new Regex(@"\s+");
        private readonly Regex number = new Regex(@"\d+");
        private readonly Regex single_word = new Regex(@"[a-zA-Z][a-zA-Z_0-9]*");
        private readonly Regex Operator = new Regex(@"[\+\-\*\/]");
        private readonly Regex KeySymbol = new Regex(@"[=\;\(\)\{\}]");
        public SpaceSeparatedTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {
            var codes = split_word.Split(src).AllToList();
            // TODO: 仮のダミー実装
            return codes.Select(c => GetToken(c)).Concat(new[] { new Token(TokenType.Terminate, "EOF") }).ToList();
        }

        public Token GetToken(string str)
        {
            switch (str)
            {
                case "+":
                    return new Token(TokenType.Plus, str);
                case "-":
                    return new Token(TokenType.Minus, str);
                case "*":
                    return new Token(TokenType.Star, str);
                case "/":
                    return new Token(TokenType.Slash, str);
                case ">":
                    return new Token(TokenType.Larger, str);
                case ">=":
                    return new Token(TokenType.LargerEqual, str);
                case "<":
                    return new Token(TokenType.Smaller, str);
                case "<=":
                    return new Token(TokenType.SmallerEqual, str);
                case "!=":
                    return new Token(TokenType.NotEqual, str);
                case "==":
                    return new Token(TokenType.DoubleEqual, str);
                case "=":
                    return new Token(TokenType.Equal, str);
                case ";":
                    return new Token(TokenType.End, str);
                case "{":
                    return new Token(TokenType.LeftBlock, str);
                case "}":
                    return new Token(TokenType.RightBlock, str);
                case "(":
                    return new Token(TokenType.LeftBracket, "(");
                case ")":
                    return new Token(TokenType.RightBracket, ")");
                case "@":
                    return new Token(TokenType.At, "@");
                case ",":
                    return new Token(TokenType.Comma, ",");
                case "let":
                    return new Token(TokenType.Let, str);
                case "print":
                    return new Token(TokenType.Print, str);
                case "function":
                    return new Token(TokenType.Function, str);
                case "return":
                    return new Token(TokenType.Return, str);
                case "if":
                    return new Token(TokenType.If, "If");
                 case "elif":
                    return new Token(TokenType.Elif, "Elif");
                 case "else":
                    return new Token(TokenType.Else, "Else");
                default:
                    float num;
                    if (float.TryParse(str, out num)) return new Token(TokenType.Number, str);
                    return new Token(TokenType.Symbol, str);
        }
    }

    }
}
