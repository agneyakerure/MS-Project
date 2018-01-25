Rate = 16000;
Microphone = dsp.AudioRecorder('SampleRate',Rate,'SamplesPerFrame',3200,'DeviceName','Primary Sound Capture Driver','NumChannels',2);%Mic/Inst/Line In 1/2 (Studio 68)
tic;
while (toc)
    
    % Microphone Input
    audio = step(Microphone);
    signal1 = audio(:,1);
    signal2 = audio(:,2);

    signalLength=3200; % SamplesperFrame
    noise= 0.00; % noise floor
    
    thre1=max(signal1); % For noise
    thre2=max(signal2);

    if (thre1>noise && thre2>noise)
        
        % Cross Correlating Signals from both channels
        crossCorr=xcorr(signal1,signal2); 
        [ccmaximum, cctime]=max(crossCorr);
        ccestimation=signalLength-cctime; 
        
        % Applying Normalizing Factor
        gcc=zeros((signalLength*2-1),1);
        phatfilter=zeros((signalLength*2-1),1); 
        crossspectrum=fft(crossCorr); 
        for n=1:(signalLength*2-1)
            phatfilter(n)=abs(crossspectrum(n)); 
            gcc(n)=crossspectrum(n)/phatfilter(n); 
        end
        gcccorrelation=ifft(gcc);
        for n=1:(signalLength*2-1)
            gcccorrelation(n)=abs(gcccorrelation(n)); % Getting absolute value
        end 
        
        [gccmaximum,gcctime]=max(gcccorrelation); 
        gccestimation=signalLength-gcctime; 
        
        maxDelay = 7;
        gcoco=((gccestimation+maxDelay)*180)/(2*maxDelay); % Assuming 20 samples per delay value, spread to 180

        lag=zeros((signalLength*2-1),1);
        for n=1:(signalLength*2-1)
            lag(n)= signalLength-n; 
        end 

    end

    subplot(2,1,1); 
    plot(lag,gcccorrelation,'r') ;
    legend(sprintf('PHAT delay max = %f  ',gccestimation ));
    ylabel('cross-correlation'); 
    subplot(2,1,2);

    gx=gcoco;
    gy=90;
    ang=0:0.1:2*pi; 
    r=2;
    gccxp=r*cos(ang);
    gccyp=r*sin(ang);

    plot(gx+gccxp,gy+gccyp,'r');
    axis([-0 180 -0 180]); 
    drawnow;
end