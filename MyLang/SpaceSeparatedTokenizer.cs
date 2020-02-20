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
            var now_character = new List<string>();
            bool is_Single_Word = false;
            foreach (char s in src)
            {
                if (s == ' ')
                {
                    is_Single_Word = true;
                    continue;
                }
                else if (s == '=' || s=='+' || s=='-' || s=='*' || s=='/') is_Single_Word = true;
                if (is_Single_Word)
                {
                    is_Single_Word = false;
                    now_character.Add(" ");
                }
                if (s.ToString() == ";") now_character.Add(" ");
                now_character.Add(s.ToString());
                if (s.ToString() == "=" || s.ToString() == "+" || s.ToString() == "-" || s.ToString() == "*" || s.ToString() == "/" ) now_character.Add(" ");
            }

            // TODO: 仮のダミー実装
            var dummy = new List<Token>();
            bool is_number = false;
            var single_word = new List<string>();
            for (int i = 0; i <now_character.Count; i++)
            {
                switch (now_character[i])
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
                        //　// と /*　どっちらもない
                        if (i + 1 >= now_character.Count)
                        {
                            dummy.Add(new Token(TokenType.Slash, "/"));
                            dummy.Add(new Token(TokenType.Terminate, "[EOF]"));
                            return dummy;
                        }
                        if (now_character[i+1]!="/"&&now_character[i+1]!="*")
                            dummy.Add(new Token(TokenType.Slash, "/"));
                        //　　//の時
                        else if (now_character[i + 1] == "/")
                        {
                            //後ろ全部無視する
                            dummy.Add(new Token(TokenType.Terminate, "[EOF]"));
                            return dummy;
                        }
                        // /* の時に
                        else
                        {
                            i += 2;
                            while (true)
                            {
                                //　もし今の字は "/" それて次の言葉は "*" の時に　コメントが終了
                                if (now_character[i] == "/" && now_character[i-1] == "*")
                                    break;
                                else  //iは今の字のインデックスです、ずっと後ろへ進む
                                    i++;
                            }
                        }
                        break;
                    case "=":
                        dummy.Add(new Token(TokenType.Equal, "="));
                        break;
                    case ";":
                        dummy.Add(new Token(TokenType.End, ";"));
                        break;
                    case "{":
                        dummy.Add(new Token(TokenType.LeftBlock, "{"));
                        break;
                    case "}":
                        dummy.Add(new Token(TokenType.RightBlock, "}"));
                        break;
                    default:
                        // Symbolと数字の時
                        int num = 0;
                         is_number = int.TryParse(now_character[i], out num);
                        if(!is_number)//Symbolの時
                        {
                            //もし今の字はSymbolれば　どんどん次の言葉をチェックします
                            if (now_character[i] == " ") break;
                            while (i < now_character.Count && Char.IsLetter(now_character[i].ToCharArray()[0]))
                            {
                                single_word.Add(now_character[i++]);
                                if (i == now_character.Count || now_character[i]==" ")
                                    break;
                            }
                            //その先にインデックスをプラスしたから、今の字をチェックしたいければ、インデックスをマイナスします
                            if (single_word.Count > 0)
                            {
                                switch (string.Join("", single_word.ToArray()))
                                {
                                    case "let":
                                    case "Let":
                                        dummy.Add(new Token(TokenType.Let, "Let"));
                                        break;
                                    case "print":
                                        dummy.Add(new Token(TokenType.Print, "Print"));
                                        break;
                                    case "function":
                                        dummy.Add(new Token(TokenType.Function, "Function"));
                                        break;
                                    case "return":
                                        dummy.Add(new Token(TokenType.Return, "Return"));
                                        break;
                                    default:
                                        dummy.Add(new Token(TokenType.Symbol, string.Join("", single_word.ToArray())));
                                        break;
                                }
                            }
                            single_word.Clear();
                        }
                        else // 数字の時
                        {
                            //同じ方法をしましたけと、TokenTypeに注意してください。
                            while (i<now_character.Count && int.TryParse(now_character[i], out num))
                            {
                                single_word.Add(now_character[i++]);
                                if (i == now_character.Count)
                                    break;
                            }
                            if(i<now_character.Count-1)
                                i -= 1;
                            dummy.Add(new Token(TokenType.Number, string.Join("",single_word.ToArray())));
                            single_word.Clear(); 
                        }
                        break;
                }
            }
            
            dummy.Add(new Token(TokenType.Terminate, "[EOF]"));
            return dummy;
        }

    }
}
