from random import randint
from datetime import datetime
import sys

variation = ['1','1.3','1.45','1.5','1.55','1.7','2'];
trial_no = raw_input('Input total trial number for each stimulus value\n')
print 'total trial number = ' + str(int(trial_no) * len(variation))

filename = raw_input('Input file name\n')
allowed_names = ['lowlow','highhigh','lowhigh','highlow']
if filename not in allowed_names: #check for eiligible file names
    print "file name not eiligible, support file names: lowlow, highhigh, lowhigh, highlow"
    sys.exit(0)

count_list = [int(trial_no)] * len(variation);
count = 1

with open(filename+'.csv','w') as f:
    while sum(count_list) != 0:
        index = randint(0,len(variation)-1)
        if count_list[index] == 0:
            continue
        else:
            trial_info = ""
            count_list[index] -= 1
            order = True if randint(0,10) >= 5 else False

            f.write(str(count) + ',') #trial count
            if filename != "lowhigh" and filename != "highlow":
                trial_info += (variation[index] if order else variation[3]) + ','
                trial_info += (variation[3] if order else variation[index])
            else:
                trial_info += (variation[3] + ',') #stimulus weight
                trial_info += variation[index] #comparison weight

            print "line " + str(count) + ":" + trial_info
            trial_info += '\n'
            f.write(trial_info)
            count += 1
            order = not order
f.closed

print "successfully created experiment file " + filename
