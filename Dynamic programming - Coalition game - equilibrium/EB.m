function res = EB(w,round,reward,max_rounds,player_index)

global r_array;
n = size(w,2);
fillMatrix(max_rounds,w,reward);


sum_R_of_coalition_members = sumRofMemebers(w,round,reward,max_rounds,player_index);
r_array(player_index,round+1) = 1/n * (reward - sum_R_of_coalition_members) + (n-1)/n * r_array(player_index,round+1)*calcM(w,round,reward,max_rounds,player_index);
end