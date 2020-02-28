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

        private readonly Regex split_word = new Regex(@"(\s+|\+|\-|\*|\/|;|\(|\)|\{|\}|\>=|\<=|==|!=|\>|\<|!|=|,)");
        public SpaceSeparatedTokenizer()
        {
        }

        public IList<Token> Tokenize(string src)
        {
            var codes = split_word.Split(src).AllToList();
            // TODO: 仮のダミー実装
            return codes.Select(c => new Token(c)).Concat(new[] { new Token( "EOF") }).ToList();
        }
    }

    
}
