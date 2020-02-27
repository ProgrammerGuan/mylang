using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyLang
{
    /// <summary>
    /// 単純なトークナイザ
    /// 
    /// トークンは、必ず一つ以上のスペースで区切られている必要がある
    /// </summary>
    class SpaceSeparatedTokenizer : ITokenizer
    {
        private readonly Regex space = new Regex(@"\s+");
        private readonly Regex number = new Regex(@"\d+");
        private readonly Regex single_word = new Regex(@"[a-zA-Z][a-zA-Z_0-9]*");
        private readonly Regex Operator = new Regex(@"[\+\-\*\/=]");
        private readonly Regex KeySymbol = new Regex(@"[\;\(\)\{\}]");
        public SpaceSeparatedTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {

            var space_cut = space.Split(src);
            var codes = new List<string>();
            foreach(string word in space_cut)
            {
                if (Operator.IsMatch(word))
                {
                    var Op = Operator.Match(word);
                    var nums = Operator.Split(word);
                    codes.Add(nums[0]);
                    codes.Add(Op.ToString());
                    codes.Add(nums[1]);
                }
                if (KeySymbol.IsMatch(word))
                {
                    var sym = KeySymbol.Match(word).ToString();
                    var num = KeySymbol.Split(word);
                    switch (sym)
                    {
                        case "(":
                        case "{":
                            if(num[0]!="")
                                codes.Add(num[0]);
                            codes.Add(sym);
                            break;
                        case ")":
                        case "}":
                        case ";":
                            codes.Add(sym);
                            if(num[0]!="")
                                codes.Add(num[0]);
                            break;
                    }
                }
                else if(word!="") codes.Add(word);
            }
            // TODO: 仮のダミー実装
            return codes.Select(c => GetToken(c)).Concat(new[] { new Token(TokenType.Terminate, "EOF") }).ToArray();
        }

        public Token GetToken(string str)
        {
            return null;
        }

    }
}
