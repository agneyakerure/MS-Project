/*
  ==============================================================================

    This file was auto-generated!

  ==============================================================================
*/

#include "../JuceLibraryCode/JuceHeader.h"
#include "RingBuffer.h"
#include "PitchTracker.h"

//==============================================================================
/*
    This component lives inside our window, and this is where you should put all
    your controls and content.
*/
class MainContentComponent   : public AudioAppComponent
{
public:
    //==============================================================================
	MainContentComponent()
		:_window_size(1024), _num_input_channels(3), _num_output_channels(2)
    {
        setSize (800, 600);

		_ring_buffer1 = new RingBuffer();
		_ring_buffer2 = new RingBuffer();
		_ring_buffer3 = new RingBuffer();

		current_frame1 = new float[_window_size];
		current_frame2 = new float[_window_size];
		current_frame3 = new float[_window_size];

		channel_data = new float*[_num_input_channels];

		_pitch_tracker = 0;

        // specify the number of input and output channels that we want to open
        setAudioChannels (3, 2);


		if (!sender.connect("127.0.0.1", 9001))
			showConnectionErrorMessage("Error: could not connect to UDP port 9001.");
		if (!sender2.connect("127.0.0.1", 9001))
			showConnectionErrorMessage("Error: could not connect to UDP port 9001.");
		if (!sender3.connect("127.0.0.1", 9001))
			showConnectionErrorMessage("Error: could not connect to UDP port 9001.");
    }

    ~MainContentComponent()
    {
		delete _ring_buffer1;
		delete _ring_buffer2;
		delete _ring_buffer3;

		for (int i = 0; i < _num_input_channels; i++)
		{
			delete channel_data[i];
		}

		delete current_frame1;
		delete current_frame2;
		delete current_frame3;

		delete channel_data;

		PitchTracker::destroy(_pitch_tracker);

        shutdownAudio();
    }

    //==============================================================================
    void prepareToPlay (int samplesPerBlockExpected, double sampleRate) override
    {
        // This function will be called when the audio device is started, or when
        // its settings (i.e. sample rate, block size, etc) are changed.

        // You can use this function to initialise any resources you might need,
        // but be careful - it will be called on the audio thread, not the GUI thread.

        // For more details, see the help for AudioProcessor::prepareToPlay()
		_sample_rate = sampleRate;
		_num_samples_per_block = samplesPerBlockExpected;

		if (_pitch_tracker != NULL) {
			PitchTracker::destroy(_pitch_tracker);
		}
		PitchTracker::create(_pitch_tracker, 0, _sample_rate, _window_size);
		

    }

	double getHopSize(RingBuffer* _ring_buffer) {
		return _ring_buffer->getHopSize();
	}

    void getNextAudioBlock (const AudioSourceChannelInfo& bufferToFill) override
    {
        // Your audio-processing code goes here!

        // For more details, see the help for AudioProcessor::getNextAudioBlock()

		if (bufferToFill.buffer->getNumChannels() <= 0) {
			Logger::getCurrentLogger()->writeToLog("Error: No Input Audio Channel");
		}
		else {

			
			int hop_size = _ring_buffer1->getHopSize();
			int buffer_size = bufferToFill.numSamples;
			int index;

			for (int i = 0; i < _num_input_channels; i++)
			{
				channel_data[i] = bufferToFill.buffer->getWritePointer(i, bufferToFill.startSample);
			}

			float data1 = 0;
			float data2 = 0;
			float data3 = 0;

			float midi_pitch_of_window = 0;
			for (int i = 0; i < buffer_size; i++)
			{
				data1 = 0;
				data2 = 0;
				data1 = data1 + channel_data[0][i];
				data2 = data2 + channel_data[1][i];
				data3 = data3 + channel_data[2][i];

				_channel1_data.push_back(data1);
				_channel2_data.push_back(data2);
				_channel3_data.push_back(data3);
			}
			int x = _channel1_data.size();
			while ((_channel1_data.size() >= hop_size) ) { //&& (_channel2_data.size() >= hop_size)
				_ring_buffer1->addNextBufferToFrame(_channel1_data);
				_ring_buffer2->addNextBufferToFrame(_channel2_data);
				_ring_buffer3->addNextBufferToFrame(_channel3_data);

				
				float rms1 = _ring_buffer1->rmsOfWindow();
				float rms2 = _ring_buffer2->rmsOfWindow();
				float rms3 = _ring_buffer3->rmsOfWindow();
				

				if ((rms1 > rms3) && (rms2 > rms3))
				{
					micPairNumber = 1;
					index = crossCorrelation(_ring_buffer1, _ring_buffer2);
					midi_pitch_of_window = _pitch_tracker->findPitchInMidi(_ring_buffer1);
					Logger::getCurrentLogger()->writeToLog("1 and 2");
				}
				else if ((rms1 > rms2) && (rms3 > rms2))
				{
					micPairNumber = 2;
					index = crossCorrelation(_ring_buffer1, _ring_buffer3);
					midi_pitch_of_window = _pitch_tracker->findPitchInMidi(_ring_buffer2);
					Logger::getCurrentLogger()->writeToLog("1 and 3");
				}
				else if ((rms2 > rms1) && (rms3 > rms1))
				{
					micPairNumber = 3;
					index = crossCorrelation(_ring_buffer2, _ring_buffer3);
					midi_pitch_of_window = _pitch_tracker->findPitchInMidi(_ring_buffer3);
					Logger::getCurrentLogger()->writeToLog("2 and 3");
				}

				
				
				Logger::getCurrentLogger()->writeToLog(String(index));

				_channel1_data.erase(_channel1_data.begin(), _channel1_data.begin() + hop_size);
				_channel2_data.erase(_channel2_data.begin(), _channel2_data.begin() + hop_size);
				_channel3_data.erase(_channel3_data.begin(), _channel3_data.begin() + hop_size);
			}

			/*AudioIODevice* device = deviceManager.getCurrentAudioDevice();
			const BigInteger activeInputChannels = device->getActiveInputChannels();

			const float* intBuffer1 = bufferToFill.buffer->getReadPointer(0);
			const float* intBuffer2 = bufferToFill.buffer->getReadPointer(1);
	*/
			//if (index < 1000)
			//{
			//	index = 1023;
			//}
			//Logger::getCurrentLogger()->writeToLog(String(midi_pitch_of_window));
			//float arr1 = 1.34;

			// Right now we are not producing any data, in which case we need to clear the buffer
			// (to prevent the output of random noise)

			if (!sender.send("/juce/channel1", (float)index))
				showConnectionErrorMessage("Error: could not send OSC message 1.");

			if (!sender2.send("/juce/channel2", (float)midi_pitch_of_window))
				showConnectionErrorMessage("Error: could not send OSC message 2.");

			if (!sender3.send("/juce/channel3", (int)micPairNumber))
				showConnectionErrorMessage("Error: could not send OSC message 3.");


			bufferToFill.clearActiveBufferRegion();
		}

    }

	int crossCorrelation(RingBuffer *curr_frame1, RingBuffer *curr_frame2)
	{
		int read_position1 = curr_frame1->getReadPosition();
		//(float*)malloc(_window_size * sizeof(float));
		int read_position2 = curr_frame2->getReadPosition();
		//float* current_frame2 = new float[_window_size];//(float*)malloc(_window_size * sizeof(float));
		for (int i = 0; i < _window_size; i++)
		{
			current_frame1[i] = curr_frame1->getSample(0, (read_position1 + i) % _window_size);
			current_frame2[i] = curr_frame2->getSample(0, (read_position2 + i) % _window_size);
		}

		vector<float> auto_corr_array(_window_size*2-1, 0);
		//const int N = sizeof(auto_corr_array) / sizeof(float);
		int j, jmin, jmax, index;
		for (int i = 0; i < _window_size * 2; i++)
		{
			if (i >= _window_size)
			{
				jmin = i - _window_size + 1;
			}
			else
			{
				jmin = 1;
			}

			if (i < _window_size)
			{
				jmax = i;
			}
			else
			{
				jmax = _window_size;
			}

			for (int j = jmin; j <= jmax; j++)
			{
				index = _window_size - i + j;
				auto_corr_array[i -1 ] = auto_corr_array[i - 1] + (current_frame1[j - 1] * current_frame2[index - 1]);
			}
		}

		int indexMax = std::max_element(auto_corr_array.begin(), auto_corr_array.end()) - auto_corr_array.begin();
		return indexMax;
	}

	//float *conj(float *arr)
	//{
	//	for (int i = 1; i < sizeof(arr) / sizeof(arr[0]); i++)
	//	{
	//		if (i % 2 == 1)
	//		{
	//			arr[i] = -arr[i];
	//		}
	//	}
	//	return arr;
	//}

	//float *complexMult(float * arr1, float *arr2)
	//{
	//	float* answer = new float[sizeof(arr1)/sizeof(arr1[0])];
	//	for (int i = 0; i < (sizeof(arr1) / sizeof(arr1[0])) - 2; i = i + 2)
	//	{
	//		answer[i] = ((arr1[i] * arr2[i]) - (arr1[i + 1] * arr2[i + 1]));
	//		answer[i + 1] = (arr1[i] * arr2[i + 1] + (arr1[i + 1] * arr2[i + 1]));
	//	}
	//	return answer;
	//}

    void releaseResources() override
    {
        // This will be called when the audio device stops, or when it is being
        // restarted due to a setting change.

        // For more details, see the help for AudioProcessor::releaseResources()
    }

    //==============================================================================
    void paint (Graphics& g) override
    {
        // (Our component is opaque, so we must completely fill the background with a solid colour)
        g.fillAll (getLookAndFeel().findColour (ResizableWindow::backgroundColourId));

        // You can add your drawing code here!
    }

    void resized() override
    {
        // This is called when the MainContentComponent is resized.
        // If you add any child components, this is where you should
        // update their positions.
    }


private:
    //==============================================================================

    // Your private member variables go here...

	void showConnectionErrorMessage(const String& messageText)
	{
		AlertWindow::showMessageBoxAsync(
			AlertWindow::WarningIcon,
			"Connection error",
			messageText,
			"OK");
	}

	RingBuffer* _ring_buffer1;
	RingBuffer* _ring_buffer2;
	RingBuffer* _ring_buffer3;

	float* current_frame1;
	float* current_frame2;
	float* current_frame3;

	float** channel_data = 0;

	int _num_input_channels, _num_output_channels, _num_samples_per_block;
	int micPairNumber = 0;
	double _sample_rate;
	vector<float> auto_corr_array;

	PitchTracker* _pitch_tracker;

	vector<float> _channel1_data;
	vector<float> _channel2_data;
	vector<float> _channel3_data;

	OSCSender sender;
	OSCSender sender2;
	OSCSender sender3;

	const int _window_size;
    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR (MainContentComponent)
};


// (This function is called by the app startup code to create our main component)
Component* createMainContentComponent()     { return new MainContentComponent(); }
