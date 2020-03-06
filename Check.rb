sum = 1
s = 3
d = 2
now_num = 1
while(s<=1001)
    4.times do
        now_num +=d        
        sum+=now_num
    end
    d+=2
    s+=2
end

puts sum