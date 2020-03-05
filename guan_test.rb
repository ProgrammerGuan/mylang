require "open3"

$lang_exe = "MyLang\\bin\\Debug\\MyLang"
$testcode_filename = "testcode.mylang"
def main_test(test_codes,mode)
    if mode!=""
        test_codes.length.times do |i|
            o,s = Open3.capture3($lang_exe,mode,test_codes[i][0])
            if(o==test_codes[i][1]+"\n" || o==test_codes[i][1]) 
                print "."
            else
                puts "\noutput : "+ o + "expect : " + test_codes[i][1]
            end
            
        end
    else
        test_codes.length.times do |i|
            File.write($testcode_filename,test_codes[i][0])
            o,s = Open3.capture3($lang_exe + " " + $testcode_filename)
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
        ["1+1;","1 + 1 ; EOF"],
        ["8/4 - 1 ; ","8 / 4 - 1 ; EOF"],
        ["1+2*3;","1 + 2 * 3 ; EOF"],
    ]
    main_test(test_codes,"-t")
end

def parser_test
    test_codes=[
        ["1+2;","Block( Expression( Add( 1 2 ) ) )"],
        ["8/4 - 1;","Block( Expression( Sub( Divide( 8 4 ) 1 ) ) )"],
        ["1+2*3;","Block( Expression( Add( 1 Multiply( 2 3 ) ) ) )"],
    ]
    main_test(test_codes,"-p")
end

def interpreter_test
    test_codes=[
        ["print 1+2;","3"],
        ["print 1-1;","0"],
        ["print 1+2*3;","7"],
        ["1+34;",""]
    ]
    main_test(test_codes,"-i")
end

def program_test
    test_codes=[
["
let leta=1;
print leta;        
","1"],
["
print 3+5;
","8"],
["
let a=1+4;
if(a>3){
    print a;
}
else{
    print 0;
}
","5"],
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
",
"8555"],
["

function add(a,b){
    let c = a+b;
    return c;
}
print add(1,5);
","6"],
["
function Sub(a,b){
    return a - b;
}
print 5 + Sub(4,2);
","7"],
["
let a=0;
while(a<8){
    print a;
    a=a+1;
}
","0
1
2
3
4
5
6
7"],
["
let a=0;
for(a=3;a<8;a=a+1){
    print a;
}
","3
4
5
6
7"],
["
function Bigger(a,b){
    if(a>b){
        print a;
    }
    else{
        print b;
    }
}
Bigger(5,6);
","6"],
["
function fib(n){
    if(n<3){
        return 1;
    }
    else{
        return fib(n-1) + fib(n-2);
    }
}
print fib(8);
","21"],
["
function fib(n){
    if(n<3){
        return 1;
    }
    else{
        return fib(n-1) + fib(n-2);
    }
}
print fib(9);
","34"]
    ]
    main_test(test_codes,"")
end

tokenize_test
parser_test
interpreter_test
program_test
