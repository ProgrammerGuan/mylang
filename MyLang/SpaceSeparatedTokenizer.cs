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
        public SpaceSeparatedTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {
            var test = new List<string>();
            foreach (char s in src)
                if(s!=' ')
                    test.Add(s.ToString());
            // TODO: 仮のダミー実装
            var dummy = new List<Token>();
            bool is_number = false;
            var numList = new List<string>();
            for (int i = 0; i <test.Count; i++)
            {
                switch (test[i])
                {
                    case "+":
                        dummy.Add(new Token(TokenType.Plus, "+"));
                        break;
                    case "-":
                        dummy.Add(new Token(TokenType.Minus, "-"));
                        break;
                    case "*":
                        dummy.Add(new Token(TokenType.Star, "*"));
                        break;
                    case "/":
                        dummy.Add(new Token(TokenType.Slash, "/"));
                        break;
                    default:
                        int num = 0;
                         is_number = int.TryParse(test[i], out num);
                        if(!is_number)//not number
                        {
                            dummy.Add(new Token(TokenType.Symbol, test[i]));
                        }
                        else // is number
                        {
                            while (i<test.Count && int.TryParse(test[i], out num))
                            {
                                numList.Add(test[i++]);
                                if (i == test.Count)
                                    break;
                            }
                            if(i<test.Count-1)
                                i -= 1;
                            dummy.Add(new Token(TokenType.Number, string.Join("",numList.ToArray())));
                            numList.Clear(); 
                        }
                        break;
                }
            }
            
            dummy.Add(new Token(TokenType.Terminate, "[EOF]"));
            return dummy;
        }

    }
}
