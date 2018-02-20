
function R_res = R(w,round,reward,max_rounds,player_index)
    global r_array;
    %epsilon = 1;
    n = size(w,2);
    if (round == max_rounds - 1)
        R_res = 1/n * reward;
        %R_res = 1/n * (reward - epsilon) + ((n-1)/n) * (epsilon / (n-1));
    else
        sum_R_of_coalition_members = sumRofMemebers(w,round+1,reward,max_rounds,player_index);
        R_res = 1/n * (reward - sum_R_of_coalition_members) + (n-1)/n * r_array(player_index,round+1)*calcM(w,round,reward,max_rounds,player_index);
    end
end
