function get_coalitions_with_min_number_of_players = coalitions_with_min_number_of_players(w)
min_number_of_players_in_coalition = calc_min_num_of_players_in_succ_coalition(w);    
get_coalitions_with_min_number_of_players=[];
coal = combnk(w,min_number_of_players_in_coalition);
for j=1:size(coal,1)
  if (sum(coal(j,:)) >= 10)
      get_coalitions_with_min_number_of_players(end+1,:) = coal(j,:); 
  end
end
end