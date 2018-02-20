%w=[8,6,2,2]
function fillMatrix(max_rounds,w,reward)
global r_array;
global T;
T = 10;
n = size(w,2);

r_array = zeros(n,max_rounds);
r_array(:,max_rounds-1) = reward/n;

for round=max_rounds-2:-1:1
    for player_index=1:n
        sum_R_of_coalition_members = sumRofMemebers(w,round,reward,max_rounds,player_index);
        r_array(player_index,round) = 1/n *( (reward - sum_R_of_coalition_members) + (r_array(player_index,round+1)*calcM(w,round+1,reward,max_rounds,player_index)));
    end
end
r_array(2,1)
hold on
for i=1:n
    plot(r_array(i,:),'DisplayName',num2str(w(i)));
end
set(gca, 'XTick', 1:n);
legend('show');
hold off
r_array
end