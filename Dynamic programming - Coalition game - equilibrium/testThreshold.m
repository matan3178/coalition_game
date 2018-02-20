global r_array;
max_rounds = 7;
reward = 100;

w = randi([1 9],1,5);
w=sort(w);
fillMatrix(max_rounds,w,reward);
        