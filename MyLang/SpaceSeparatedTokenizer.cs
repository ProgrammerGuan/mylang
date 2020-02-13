﻿using System;
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
            foreach (char s in src)
                if(s!=' ')
                    now_character.Add(s.ToString());
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
                        if(now_character[i+1]!="/"&&now_character[i+1]!="*")
                            dummy.Add(new Token(TokenType.Slash, "/"));
                        else if (now_character[i + 1] == "/")
                        {
                            //  //の時に　後ろ全部無視する
                            dummy.Add(new Token(TokenType.Terminate, "[EOF]"));
                            return dummy;
                        }
                        else  // /* の時に
                        {
                            i += 2;
                            while (true)
                            {
                                if (now_character[i] == "/" && now_character[i-1] == "*")
                                    break;
                                else
                                    i++;
                            }

                        }
                        break;
                    default:
                        int num = 0;
                         is_number = int.TryParse(now_character[i], out num);
                        if(!is_number)//not number
                        {
                            while (i < now_character.Count && Char.IsLetter(now_character[i].ToCharArray()[0]))
                            {
                                single_word.Add(now_character[i++]);
                                if (i == now_character.Count)
                                    break;
                            }
                            if (i < now_character.Count - 1)
                                i -= 1;
                            dummy.Add(new Token(TokenType.Symbol, string.Join("", single_word.ToArray())));
                            single_word.Clear();
                            //dummy.Add(new Token(TokenType.Symbol, now_character[i]));
                        }
                        else // is number
                        {
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
