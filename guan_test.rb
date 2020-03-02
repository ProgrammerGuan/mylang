require "open3"

$lang_exe = "MyLang\\bin\\Debug\\MyLang"
def main_test(test_codes,mode)
    if mode!="-c"
        test_codes.length.times do |i|
            o,s = Open3.capture3($lang_exe,mode,test_codes[i][0])
            if(o==test_codes[i][1]+"\n") 
                print "."
            else 
                puts "\noutput : "+ o + "expect : " + test_codes[i][1]
            end
            
        end
    else
        test_codes.length.times do |i|
            File.write("code.txt",test_codes[i][0])
            o,s = Open3.capture3($lang_exe + " -c < code.txt")
            if(o==test_codes[i][1]+"\n") 
                print "."
            else 
                puts "\noutput : "+ o + "expect : " + test_codes[i][1]
            end
            
        end
    end
    print "\n"
end

def tokenize_test
    test_codes=[
        ["1+1","1 + 1 EOF"],
        ["8/4 - 1","8 / 4 - 1 EOF"],
        ["1+2*3","1 + 2 * 3 EOF"],
    ]
    main_test(test_codes,"-t")
end

def parser_test
    test_codes=[
        ["1+2","Add( 1 2 )"],
        ["8/4 - 1","Sub( Divide( 8 4 ) 1 )"],
        ["1+2*3","Add( 1 Multiply( 2 3 ) )"],
    ]
    main_test(test_codes,"-p")
end

def interpreter_test
    test_codes=[
        ["1+2","3"],
        ["1-1","0"],
        ["1+2*3","7"],
    ]
    main_test(test_codes,"")
end

def program_test
    test_codes=[
["
let leta=1;
print leta;        
-e
","1"],

["
function Add{
    return @0+@1;
}
print Add(3,5);
-e
","8"],

["
let a=1+4;
if(a>3){
    print a;
}
else{
    print 0;
}
-e
","5"],
["
function add{
    let a = @0 + @1;
    print @0 - @1;
    return @0 * @1;
}
function a{
    let aa = 1+5;
    print 600;
}
print add(3,4,5);
print add(1+9-3,9*9);
print a();
-e
",
"-1
12
-74
567
600
0"],
["
let a = 99;
if( a < 99 ){
let v = 445;
let s = 990;
print v;
}else{
let af = 8555;
let aa = 5451;
print af;
}
-e
",
"8555"]
    ]
    main_test(test_codes,"-c")
end

tokenize_test
parser_test
interpreter_test
program_test
