/*
  ==============================================================================

    PitchTracker.h
    Created: 12 Dec 2017 5:01:34pm
    Author:  Lenovo

  ==============================================================================
*/

#ifndef PITCHTRACKER_H_INCLUDED
#define PITCHTRACKER_H_INCLUDED

#include <vector>

#include "../JuceLibraryCode/JuceHeader.h"
#include "RingBuffer.h"

using namespace std;

class PitchTracker
{
protected:
	PitchTracker(int window_Size);
	virtual ~PitchTracker();

	int _sample_rate;
	const int _window_size;
	int _min_freq_in_hz;
	int _max_freq_in_hz;
	vector<float> _pitch_array_in_midi;
	vector<float> _pitch_array_in_hz;

	virtual float findPitchInHz(RingBuffer* window) = 0;

public:
	static void create(PitchTracker*& pitch_tracker, int type, double sample_rate, int window_size);
	static void destroy(PitchTracker*& pitch_tracker);

	void init(double sample_rate);

	//getters and setters
	int getSampleRate();
	int getWindowSize();
	int getMinFreqInHz();
	int getMaxFreqInHz();
	int getLengthofPitchArray();
	float getPitchAtIndex(int index);
	void setSampleRate(double sample_rate);
	void setMinFreqInHz(int min_freq_in_hz);
	void setMaxFreqInHz(int max_freq_in_hz);

	float findPitchInMidi(RingBuffer* window);
	int quantizeMidiPitch(float pitch_in_midi);

	void clearPitchArray();

	vector<float> getNewPitches(int previous_pitch_idx);
};

#endif  // PITCHTRACKER_H_INCLUDED
