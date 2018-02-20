function m = calcM(w,round,reward,max_rounds,player_index)
n = size(w,2);
arr = zeros(1,n);
for i=1:n
    if (i ~= player_index)
        coalitions = sumRofMemebers_returnPossibleCoal(w,round,reward,max_rounds,i);
        count = 0;
        for j=1:size(coalitions,1)
            count = count + sum(coalitions(j,:)==player_index);
        end
        arr(i) = (1/size(coalitions,1))*count;
    end
end
m = sum(arr(1,:));
end