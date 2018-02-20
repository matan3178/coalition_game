function min_number_of_players_in_coalition = calc_min_num_of_players_in_succ_coalition(w)
    min_number_of_players_in_coalition = 0;    
    n = size(w,2);
    b = 0;
    for i=1:n
      coal = combnk(w,i);
      for j=1:size(coal,1)
          if (sum(coal(j,:)) >= 10)
              min_number_of_players_in_coalition = i;
              b = 1;
              break;
          end
      end
      if (b == 1)
          break;
      end
    end
end