data = humansData;

dataSize = size(data,1);
res = zeros(dataSize,10);
for i=1:dataSize
    round = data(i,1);
    proposerIndex = data(i,2);
    w = zeros(1,5);
    for j=1:5
        w(j) = data(i,j+2);
    end
    actualShares = zeros(1,5);
    for j=1:5
        actualShares(j) = data(i,j+7);
    end
    EB = zeros(1,5);
    for j=1:5
        temp = sumRofMemebers_returnPossibleCoal(w,round,100,3,proposerIndex);
    end
    EB(i,proposerIndex) = 100 - sum;

    res(i) = EB;
end