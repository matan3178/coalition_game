function [sum_general] = sumRofMemebers(w,round,reward,max_rounds,player_index)
global r_array;
sum_general = 1000;
numOfPlayers = size(w,2);
grandCoalition = zeros(1,numOfPlayers);
for i=1:numOfPlayers
    grandCoalition(1,i) = i;
end

for i=1:numOfPlayers
    possibleCoalitions = [];
    coal = combnk(grandCoalition,i);
    for j=1:size(coal,1)
        if (sum(coal(j,:)==player_index) > 0 && checkIfCoalitionIsSucceed(coal(j,:),w)==1)
            possibleCoalitions(end+1,:) = coal(j,:);
        end
    end
    for k=1:size(possibleCoalitions,1)
        sum1 = 0;
        for l=1:size(possibleCoalitions(k,:),2)
            if (possibleCoalitions(k,l) ~= player_index)
                sum1 = sum1 + r_array(possibleCoalitions(k,l),round+1);
            end
        end
        if (sum1 < sum_general)
            sum_general = sum1;
        end
    end
end
