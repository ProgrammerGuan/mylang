require 'pp'
require 'fileutils'
require 'open3'

MY_LANG_EXE = "MyLang/bin/Debug/MyLang.exe"

def run_test(testcases, cmd)
  total = 0
  success = 0

  testcases.each do |input, expected_output|
    total += 1
    output, status = Open3.capture2e(*cmd, input)
    output.strip!

    if status.exitstatus != 0
      puts "ERROR: #{input}"
      next
    end
    
    if output != expected_output.to_s
      puts "NG: #{input} => #{output}, but expect #{expected_output}"
    else
      success += 1
      puts "OK: #{input} => #{output}"
    end
  end
  if total == success
    puts "OK: #{total} testcases passed"
  else
    puts "ERR: #{total-success} testcases failed"
    raise
  end
end

def test_tokenizer
  testcases = [
    ["1", "1 EOF"],
    ["1 + 2", "1 + 2 EOF"],
    ["1   +   2", "1 + 2 EOF"],
    ["1   +  2 * 3", "1 + 2 * 3 EOF"],
    #["1+2", "1 + 2 [EOF]"], # スペースがなくても、Tokenizeできるようにする
    #["a + b", "a + b [EOF]"], # Symbolも対応する
    #["(1 + 2) * 3", "( 1 + 2 ) [EOF]"], # "(", ")" に対応する
  ]
  puts "** Testing Tokenizer ..."
  run_test(testcases, [MY_LANG_EXE, '--tokenize'])
end


def test_parser
  testcases = [
    ["1", "1"],
    ["1 + 2", "Add( 1 2 )"],
    ["2 * 3", "Multiply( 2 3 )"],
    ["1 + 2 * 3", "Add( 1 Multiply( 2 3 ) )"],
    ["1 + 2 + 3", "Add( Add( 1 2 ) 3 )"],
    ["1 * 2 * 3", "Multiply( Multiply( 1 2 ) 3 )"],
  ]
  puts "** Testing Parser ..."
  run_test(testcases, [MY_LANG_EXE, '--parse'])
end

def test_interpreter
  testcases = [
    ["1", 1],
    ["1 + 2", 3],
    ["2 * 3", 6],
    ["1 + 2 * 3", 7],
    ["1 + 2 + 3", 6],
    ["1 * 2 * 3", 6],
  ]
  puts "** Testing Interpreter ..."
  run_test(testcases, [MY_LANG_EXE])
end

test_tokenizer
test_parser
test_interpreter