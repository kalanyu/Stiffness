from random import randint
from datetime import datetime

variation = ['0.1','0.3','0.4','0.5','0.6','0.7','1.0'];
trial_no = raw_input('Input total trial number for each stimulus value\n')
print 'total trial number = ' + str(int(trial_no) * len(variation))

filename = raw_input('Input file name\n')

count_list = [int(trial_no)] * len(variation);
print sum(count_list)
count = 1
# 1.000000e+00,-9.800000e+00,5.000000e-01,0.000000e-01,0.000000e-02,-10.000000e-01
with open(filename+'.csv','w') as f:
    while sum(count_list) != 0:
        index = randint(0,len(variation)-1)
        if count_list[index] == 0:
            continue
        else:
            count_list[index] -= 1
            order = True if randint(0,10) >= 5 else False

            f.write(str(count) + ',') #trial count
            f.write((variation[index] if order else '0.5') + ',') #stimulus weight
            f.write(('0.5' if order else variation[index])) #stimulus weight
            f.write('\n')
            count += 1
            order = not order
f.closed
