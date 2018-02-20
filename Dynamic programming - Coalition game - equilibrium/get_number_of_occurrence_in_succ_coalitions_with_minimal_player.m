function m = get_number_of_occurrence_in_succ_coalitions_with_minimal_players(w)
coalitions_with_min_number_of_players = get_coalitions_with_min_number_of_players(w);
n = size(w,2);

m=zeros(1,n);
for i=1:size(w,2)
    for j=1:size(coalitions_with_min_number_of_players,1)
        m(i) = m(i) + sum(coalitions_with_min_number_of_players(j,:)==w(i));
    end
end
for i=1:size(m,2)
    m(i) = m(i) / sum(w(1,:)==w(i));
end
end
