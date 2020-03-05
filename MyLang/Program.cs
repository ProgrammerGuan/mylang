using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using MyLang;

class Program
{
    /// <summary>
    /// コマンド の entry point
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    static void Main(string[] args)
    {
        bool tokenizeOnly = false; // tokenize だけで終わるかどうか
        bool parseOnly = false; // parse だけで終わるかどうか
        bool isREPL = false;  //Coding Mode
        bool CompileFile = true;
        // 引数をparseする
        var rest = new List<string>();
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            switch (arg)
            {
                case "-h":
                case "--help":
                    showHelpAndExit();
                    break;
                case "-t":
                case "--tokenize":
                    tokenizeOnly = true;
                    CompileFile = false;
                    break;
                case "-p":
                case "--parse":
                    parseOnly = true;
                    CompileFile = false;
                    break;
                case "-d":
                case "--debug":
                    Logger.LogEnabled = true;
                    CompileFile = false;
                    break;
                case "-c":
                case "--coding":
                    isREPL = true;
                    CompileFile = false;
                    break;
                case "-i":
                case "--interpreter":
                    CompileFile = false;
                    break;
                default:
                    rest.Add(arg);
                    break;
            }
        }
        var codeList = new List<string>();
        if (CompileFile)
        {
            string path = Directory.GetCurrentDirectory() + @"\" + rest[0];
            codeList = File.ReadAllLines(path).ToList();
            Execute_Program(codeList);
            exit(0);
        }
        // 各実行器を用意する
        ITokenizer tokenizer = new SpaceSeparatedTokenizer();
        var parser = new Parser();
        var interpreter = new Interpreter();
        while (isREPL)
        {
            Console.Write(">>> ");
            var codeLine = Console.ReadLine();
            if (codeLine != "!q" && codeLine != "!Q")
            {
                var tok = tokenizer.Tokenize(codeLine);
                var par = parser.ProgramingParse(tok);
                Console.WriteLine(par.GetDisplayInfo());
                Console.Write("> ");
                Console.WriteLine(interpreter.Run(par));
            }
            else exit(0);

        }

        // 引数がないなら、ヘルプを表示して終わる
        if( rest.Count <= 0)
        {
            showHelpAndExit();
        }

        // Tokenize を行う
        var tokens = tokenizer.Tokenize(string.Join(" ", rest.ToArray()));

        if( tokenizeOnly)
        {
            Console.WriteLine(string.Join(" ", tokens.Select(t => t.Text).ToArray()));
            exit(0);
        }

        // Parse を行う
        var ast = parser.ProgramingParse(tokens);

        if( parseOnly)
        {
            Console.WriteLine(new MyLang.Ast.AstDisplayer().BuildString(ast, false));
            exit(0);
        }

        // Interpreter で実行する
        var result = interpreter.Run(ast);

        // 答えを出力する
        //Console.WriteLine(result);

        exit(0);
    }

    /// <summary>
    /// ヘルプを表示して終わる
    /// </summary>
    static void showHelpAndExit()
    {
        Console.WriteLine(@"
My Small Language.

Usage:
    > MyLang.exe [options...] ""program""

Options:
    -t, --tokenize : Show token list.
    -p, --parse    : Show parsed abstract syntax tree.
    -d, --debug    : Print debug log (for debug).
    -h, --help     : Show help.

Example:
    > MyLang.exe ""1 + 2""
    > MyLang.exe --debug ""1 + 2 * 3""
    > MyLang.exe --tokenize ""1 + 2 * 3""
    > MyLang.exe --parse ""1 + 2 * 3""
");
        exit(0);
    }

    /// <summary>
    /// デバッガがアタッチされている場合は、キーの入力を待つ
    ///
    /// Visual Studioの開津環境で、コンソールがすぐに閉じてしまうのの対策として使用している
    /// </summary>
    static void waitKey()
    {
        if (Debugger.IsAttached)
        {
            Console.ReadKey();
        }
    }

    /// <summary>
    /// 終了する
    /// </summary>
    /// <param name="resultCode"></param>
    static void exit(int resultCode) 
    {
        waitKey();
        Environment.Exit(resultCode);
    }

    static void Execute_Program(List<String> codeList)
    {
        // 各実行器を用意する
        ITokenizer tokenizer = new SpaceSeparatedTokenizer();
        var parser = new Parser();
        var interpreter = new Interpreter();
        var codes = codeList.ToArray().ToString();
        // Tokenize を行う
        var tokens = tokenizer.Tokenize(string.Join(" ", codeList));
        //Console.WriteLine(string.Join(" ", tokens.Select(t => t.Text).ToArray()));
        var ast = parser.ProgramingParse(tokens);
        //Console.WriteLine(new MyLang.Ast.AstDisplayer().BuildString(ast, false));
        interpreter.Run(ast);

    }

}

