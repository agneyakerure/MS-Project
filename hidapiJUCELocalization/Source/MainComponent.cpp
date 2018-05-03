/*
  ==============================================================================

    This file was auto-generated!

  ==============================================================================
*/

#include "../JuceLibraryCode/JuceHeader.h"
#include "hidapi.h"
#include "RingBuffer.h"
#include "PitchTracker.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <Windows.h>

#define OUTPUT_BUFFER_SIZE 1024
#define MAX_STR 255

class MainContentComponent   : public AudioAppComponent
{
public:
    //==============================================================================
    MainContentComponent()
		:_window_size(1024), _num_input_channels(1), _num_output_channels(2)
    {
        setSize (800, 600);

        // specify the number of input and output channels that we want to open
        setAudioChannels (1, 2);

		_ring_buffer1 = new RingBuffer();
		current_frame1 = new float[_window_size];
		channel_data = new float*[_num_input_channels];
		//_pitch_tracker = 0;

		if (!sender.connect("127.0.0.1", 9001))
			showConnectionErrorMessage("Error: could not connect to UDP port 9001.");
		/*if (!sender.connect("127.0.0.1", 9001))
			showConnectionErrorMessage("Error: could not connect to UDP port 9001.");*/
		
    }

    ~MainContentComponent()
    {
		delete _ring_buffer1;
		for (int i = 0; i < _num_input_channels; i++)
		{
			delete channel_data[i];
		}
		delete channel_data;
		delete current_frame1;
		//PitchTracker::destroy(_pitch_tracker);
        shutdownAudio();
    }

    //==============================================================================
    void prepareToPlay (int samplesPerBlockExpected, double sampleRate) override
    {

		_sample_rate = sampleRate;
		_num_samples_per_block = samplesPerBlockExpected;

		/*if (_pitch_tracker != NULL) {
			PitchTracker::destroy(_pitch_tracker);
		}*/
		//PitchTracker::create(_pitch_tracker, 0, _sample_rate, _window_size);

		handle = hid_open(0x2886, 0x07, NULL);
		hid_set_nonblocking(handle, 1);
		read_register(handle, 0x10, buf, 1);
		buf[0] = 35;
		write_register(handle, 0x10, buf, 1);

		buf[0] = 0; // spk proc bypass
		write_register(handle, 0x13, buf, 1);

		buf[0] = 0; // no AGC
		write_register(handle, 0x2A, buf, 1);

		char buffer[OUTPUT_BUFFER_SIZE];
    }

	double getHopSize(RingBuffer* _ring_buffer) {
		return _ring_buffer->getHopSize();
	}

    void getNextAudioBlock (const AudioSourceChannelInfo& bufferToFill) override
    {
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
			float midi_pitch_of_window = 0;
			for (int i = 0; i < buffer_size; i++)
			{
				data1 = 0;
				data1 = data1 + channel_data[0][i];
				_channel1_data.push_back(data1);
			}
			//Logger::getCurrentLogger()->writeToLog(String(_channel1_data.size()));
			while ((_channel1_data.size() >= hop_size)) {
				_ring_buffer1->addNextBufferToFrame(_channel1_data);
				//midi_pitch_of_window = _pitch_tracker->findPitchInMidi(_ring_buffer1);
				
				_channel1_data.erase(_channel1_data.begin(), _channel1_data.begin() + hop_size);
			}
			
			if (read_auto_report(handle, &angle, &vad_activity))
			{
				Logger::getCurrentLogger()->writeToLog(String(angle));
				if (!sender.send("/juce/channel1", (float)angle)) 
					showConnectionErrorMessage("Error: could not send OSC message 1.");
				
				/*if (!sender2.send("/juce/channel2", (float)midi_pitch_of_window))
					showConnectionErrorMessage("Error: could not send OSC message 2.");*/
			}
			//Sleep(10);
			bufferToFill.clearActiveBufferRegion();
		}
    }

    void releaseResources() override
    {
    }

    //==============================================================================
    void paint (Graphics& g) override
    {
        // (Our component is opaque, so we must completely fill the background with a solid colour)
        g.fillAll (getLookAndFeel().findColour (ResizableWindow::backgroundColourId));

    }

    void resized() override
    {

    }

	int write_register(hid_device *handle, unsigned char reg, unsigned char * data, unsigned char len) {
		unsigned char buf[6];
		int res;
		// Set a register on the mic -- 
		buf[0] = 0; // First byte is report number
		buf[1] = reg; // register #
		buf[2] = 0; // space
		buf[3] = len; // length
		buf[4] = 0; // space
		for (int i = 0; i<len; i++) buf[5 + i] = data[i];
		res = hid_write(handle, buf, len + 5);
		return res;
	}

	// Read data of size len into ret at register reg, return success
	int read_register(hid_device *handle, unsigned char reg, unsigned char * ret, unsigned char len) {
		int res;
		unsigned char buf[9];
		buf[0] = 0;
		buf[1] = reg;
		buf[2] = 0x80;
		buf[3] = len;
		buf[4] = 0;
		buf[5] = 0;
		buf[6] = 0;
		res = hid_write(handle, buf, 7);
		res = hid_read(handle, buf, 7 + len);
		if (buf[0] == reg) {
			for (int i = 0; i<len; i++) {
				ret[i] = buf[4 + i];
			}
			return 1;
		}
		return 0;

	}

	// Returns 1 if auto report was read, 0 otherwise
	int read_auto_report(hid_device *handle, unsigned short * angle, unsigned char *vad_activity) {
		int res;
		unsigned char buf[9];
		angle[0] = 0;
		vad_activity[0] = 0;

		res = hid_read(handle, buf, 9);
		if (buf[0] == 0xFF) 
		{
			angle[0] = buf[6] * 256 + buf[5];
			vad_activity[0] = buf[4];
			return 1;
		}
		return 0;

	}


private:
    //==============================================================================
	RingBuffer* _ring_buffer1;


	void showConnectionErrorMessage(const String& messageText)
	{
		AlertWindow::showMessageBoxAsync(
			AlertWindow::WarningIcon,
			"Connection error",
			messageText,
			"OK");
	}

	OSCSender sender;
	//OSCSender sender2;

	hid_device *handle;

	int res;
	wchar_t wstr[MAX_STR];
	unsigned char buf[4];

	const int _window_size;
	vector<float> _channel1_data;
	double _sample_rate;
	int _num_input_channels, _num_output_channels, _num_samples_per_block;
	float** channel_data = 0;
	float* current_frame1;
	//PitchTracker* _pitch_tracker;
	unsigned short angle;
	unsigned char vad_activity;

    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR (MainContentComponent)
};


// (This function is called by the app startup code to create our main component)
Component* createMainContentComponent()     { return new MainContentComponent(); }
