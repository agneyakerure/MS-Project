function z = myCrossCorr( x,y )
%UNTITLED Summary of this function goes here
%   Detailed explanation goes here
    lx = length(x);
    ly = length(y);
    lconv = lx + ly - 1;
    z = zeros(lconv,1);
    for i = 1:lconv
    
        if i >= ly
            jmin = i - ly + 1;
        else 
            jmin = 1;
        end
        if i < lx
            jmax = i;
        else
            jmax = lx;
        end

        for j = jmin : jmax
            index = ly - i + j;
            z(i) = z(i) + x(j) * y(index);
        end
    end

end