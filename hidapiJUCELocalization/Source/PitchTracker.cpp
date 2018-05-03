/*
==============================================================================

PitchTracker.cpp
Created: 12 Dec 2017 5:01:34pm
Author:  Lenovo

==============================================================================
*/

#include "../JuceLibraryCode/JuceHeader.h"
#include "PitchTracker.h"
#include "AcfPitchTracker.h"

using namespace std;

PitchTracker::PitchTracker(int window_size) : _window_size(window_size) {};

PitchTracker::~PitchTracker() {};

void PitchTracker::create(PitchTracker*& pitch_tracker, int type, double sample_rate, int window_size) {
	if (type == 0) {
		pitch_tracker = new AcfPitchTracker(window_size);
	}
	else {
		jassert("Invalid Pitch Tracker Type");
	}

	if (sample_rate <= 0)
		jassert("Sample Rate must be a positive integer");

	if (window_size <= 0)
		jassert("Window Size must be a positive integer");

	pitch_tracker->init(sample_rate);

}

void PitchTracker::destroy(PitchTracker*& pitch_tracker) {
	delete pitch_tracker;
}

void PitchTracker::init(double sample_rate) {
	setSampleRate(sample_rate);
}

int PitchTracker::getSampleRate() {
	return _sample_rate;
}

int PitchTracker::getWindowSize() {
	return _window_size;
}

int PitchTracker::getMinFreqInHz() {
	return _min_freq_in_hz;
}

int PitchTracker::getMaxFreqInHz() {
	return _max_freq_in_hz;
}

int PitchTracker::getLengthofPitchArray() {
	return _pitch_array_in_midi.size();
}

float PitchTracker::getPitchAtIndex(int index) {
	int size = getLengthofPitchArray();
	if (index > size - 1)
		jassert("Invalid index for pitch array");
	return _pitch_array_in_midi[index];
}

void PitchTracker::setSampleRate(double sample_rate) {
	_sample_rate = sample_rate;
}

void PitchTracker::setMinFreqInHz(int min_freq_in_hz) {
	_min_freq_in_hz = min_freq_in_hz;
}

void PitchTracker::setMaxFreqInHz(int max_freq_in_hz) {
	_max_freq_in_hz = max_freq_in_hz;
}

float PitchTracker::findPitchInMidi(RingBuffer *window) {
	float pitch_in_hz = findPitchInHz(window);
	float pitch_in_midi = 0;
	if (pitch_in_hz == 0)
	{
		pitch_in_midi = -1;
	}
	else
	{
		pitch_in_midi = 69 + 12 * log(pitch_in_hz / 440) / log(2);
	}
	_pitch_array_in_midi.push_back(pitch_in_midi);
	return pitch_in_hz;
}

int PitchTracker::quantizeMidiPitch(float pitch_in_midi)
{
	int quantized_pitch = (int)round(pitch_in_midi);
	return quantized_pitch;
}

void PitchTracker::clearPitchArray()
{
	_pitch_array_in_midi.clear();
	_pitch_array_in_hz.clear();
}

vector<float> PitchTracker::getNewPitches(int previous_pitch_idx) {
	vector<float> new_pitches;
	for (int copy_pitch_idx = previous_pitch_idx + 1;
		copy_pitch_idx < _pitch_array_in_midi.size();
		copy_pitch_idx++) {

		new_pitches.push_back(_pitch_array_in_midi[copy_pitch_idx]);
	}
	return new_pitches;
}
