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
    class SimpleTokenizer
    {
        string src_;
        int pos_;
        int start_;
        List<Token> tokens_ = new List<Token>();

        public SimpleTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {
            src_ = src;
            pos_ = 0;
            while (pos_ < src_.Length)
            {
                start_ = pos_;
                var c = src_[pos_];
                if (isSpace(c))
                {
                    lexSeparator();
                }
                else if (isDigit(c))
                {
                    lexNumber();
                }
                else if (isAlphabet(c))
                {
                    lexSymbolOrKeyword();
                }
                else if (c == '"')
                {
                    lexString();
                }
                else
                {
                    lexOperator();
                }
            }

            tokens_.Add(Token.CreateTerminator());
            return tokens_;
        }

        bool isSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        bool isAlphabet(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || c == '@';
        }

        bool isAlphabetOrDigit(char c)
        {
            return isAlphabet(c) || isDigit(c);
        }

        void lexSeparator()
        {
            while (pos_ < src_.Length)
            {
                var c = src_[pos_];
                if (!isSpace(c))
                {
                    break;
                }
                pos_++;
            }
        }

        void lexNumber()
        {
            while (pos_ < src_.Length)
            {
                var c = src_[pos_];
                if (!isDigit(c))
                {
                    break;
                }
                pos_++;
            }
            tokens_.Add(new Token(TokenType.Number, src_.Substring(start_, pos_ - start_)));
        }

        void lexSymbolOrKeyword()
        {
            while (pos_ < src_.Length)
            {
                var c = src_[pos_];
                if (!isAlphabetOrDigit(c))
                {
                    break;
                }
                pos_++;
            }
            var str = src_.Substring(start_, pos_ - start_);
            var token = Token.FromString(str, false);
            if (token != null)
            {
                tokens_.Add(token);
            }
            else {
                tokens_.Add(new Token(TokenType.Symbol, str));
            }
        }

        void lexOperator()
        {
            var c = src_[pos_];
            pos_++;

            // ２文字以上になる可能性のある記号を扱う
            switch ( c)
            {
                case '<':
                case '>':
                case '!':
                case '=':
                    if (pos_ < src_.Length)
                    {
                        var c2 = src_[pos_];
                        if (c == '<' && c2 == '=')
                        {
                            pos_++;
                        }
                        else if (c == '>' && c2 == '=')
                        {
                            pos_++;
                        }
                        else if (c == '=' && c2 == '=')
                        {
                            pos_++;
                        }
                        else if (c == '!' && c2 == '=')
                        {
                            pos_++;
                        }
                    }
                    break;
                default:
                    // １文字の記号
                    break;
            }

            tokens_.Add(Token.FromString(src_.Substring(start_, pos_ - start_)));
        }

        void lexString()
        {
            pos_++; // '"' を読み飛ばす

            var sb = new StringBuilder();
            for (; ; )
            {
                var c = src_[pos_++];
                if( c == '"')
                {
                    break;
                }
                else
                {
                    sb.Append(c);
                }
            }

            tokens_.Add(new Token(TokenType.String, sb.ToString()));
        }

    }
}
