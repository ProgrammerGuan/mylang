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
        private readonly Regex space = new Regex(@"/s+");
        private readonly Regex number = new Regex(@"/d+");
        private readonly Regex single_word = new Regex(@"[a-zA-Z][a-zA-Z_0-9]*");
        private readonly Regex Operator = new Regex(@"[+-*/]");


        public SpaceSeparatedTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {
            var codes = space.Split(src);
            // TODO: 仮のダミー実装
            var dummy = new List<Token>();
            
            dummy.Concat(new IEnumerable<Token>(TokenType.Terminate, "[EOF]"));
            return dummy;
        }

        public Token GetToken(string str)
        {

        }

    }
}
