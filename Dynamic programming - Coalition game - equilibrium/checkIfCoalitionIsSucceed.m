function b = checkIfCoalitionIsSucceed(coal,w)
global T;
b = 0;
sum = 0;
for i=1:size(coal,2)
    sum = sum + w(coal(i));
end
if (sum >= T)
    b = 1;
end
end