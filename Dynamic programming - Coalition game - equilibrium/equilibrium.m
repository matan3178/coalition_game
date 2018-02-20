%w = [8,2,2,2];
reward = 100;
max_round = 3;
curr_round = 1;

res = zeros(1,5);
for i=1:size(weights,1)
    w=weights(i,:);
    for j=1:5
        player_index = j;
        value = reward - sumRofMemebers(w,curr_round,reward,max_round,player_index);
        coal = sumRofMemebers_returnPossibleCoal(w,curr_round,reward,max_round,player_index);
        r=zeros(1,5);
        r(player_index) = value;
        for k=1:size(coal,2)
            if (coal(1,k) ~= player_index)
                r(coal(1,k)) = R(w,curr_round,reward,max_round,coal(1,k));
            end
        end
        res(end+1,:)=r;
    end
    res(end+1,1)=0;
end

% for i=1:size(w,2)
%     player_index = i;
%     value = EB(w,curr_round,reward,max_round,player_index);
%     strcat("EB_",num2str(player_index),": ",num2str(value))
% end