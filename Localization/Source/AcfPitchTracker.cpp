/*
  ==============================================================================

    AcfPitchTracker.cpp
    Created: 12 Dec 2017 5:01:43pm
    Author:  Lenovo

  ==============================================================================
*/

#include <vector>

#include "../JuceLibraryCode/JuceHeader.h"
#include "AcfPitchTracker.h"

using namespace std;

AcfPitchTracker::AcfPitchTracker(int window_size) : PitchTracker(window_size)
{
	setSampleRate(44100);
	setMinFreqInHz(80);
	setMaxFreqInHz(1000);
}

AcfPitchTracker::~AcfPitchTracker() {};

float AcfPitchTracker::findPitchInHz(RingBuffer* window)
{
	int read_position = window->getReadPosition();
	float* current_frame = (float*)malloc(_window_size * sizeof(float));
	float rms_value = window->rmsOfWindow();
	float energy_window = rms_value*rms_value*_window_size;
	if (rms_value < 0.005)
	{
		_pitch_array_in_hz.push_back(0);
		return 0;
	}
	else
	{
		for (int i = 0; i < _window_size; i++)
		{
			current_frame[i] = window->getSample(0, (read_position + i) % _window_size);
		}
		vector<float> auto_corr_array = autoCorrelation(current_frame, energy_window);
		int peak_location = findPeak(auto_corr_array);
		float pitch_in_hz = float(_sample_rate) / float(peak_location);
		_pitch_array_in_hz.push_back(pitch_in_hz);
		return pitch_in_hz;
	}

	delete current_frame;
}

vector<float> AcfPitchTracker::smoothAutoCorr(vector<float> auto_corr_array)
{
	vector<float> dummy(_window_size, 0);
	for (int i = 1; i < _window_size; i++)
	{
		if (i == 1 || i == _window_size - 1)
		{
			dummy[i] = auto_corr_array[i];
		}
		else if (i == 2 || i == _window_size - 2)
		{
			dummy[i] = (auto_corr_array[i - 1] + auto_corr_array[i] + auto_corr_array[i + 1]) / 3;
		}
		else
		{
			dummy[i] = (auto_corr_array[i - 2] + auto_corr_array[i - 1] + auto_corr_array[i] + auto_corr_array[i + 1] + auto_corr_array[i + 2]) / 5;
		}
	}

	return dummy;
}

vector<float> AcfPitchTracker::autoCorrelation(float *curr_frame, float energy_window)
{
	vector<float> auto_corr_array(_window_size, 0);
	for (int delay = 0; delay < _window_size; delay++)
	{
		float dummy(0);
		for (int i = 0; i < _window_size - delay; i++)
		{
			dummy = dummy + curr_frame[i] * curr_frame[delay + i];
		}
		auto_corr_array[delay] = dummy / energy_window;
	}
	vector<float> smoothed_auto_corr_array = smoothAutoCorr(auto_corr_array);
	return smoothed_auto_corr_array;
}

int AcfPitchTracker::maxIndexAutoCorr(vector<float> auto_corr_array, int start_index, int end_index)
{
	int max_index = start_index;
	for (int i = start_index + 1; i < end_index; i++)
	{
		if (auto_corr_array[i] > auto_corr_array[max_index])
		{
			max_index = i;
		}
	}
	return max_index;
}

int AcfPitchTracker::findPeak(vector<float> auto_corr_array)
{
	vector<float> peaks_location;
	vector<float> peak_value;
	int max_search_in_hz = 0;
	int min_search_in_hz = 0;
	int size = _pitch_array_in_hz.size();

	if (size == 0)
	{
		max_search_in_hz = _max_freq_in_hz;
		min_search_in_hz = _min_freq_in_hz;
	}
	else
	{
		if (_pitch_array_in_hz[size - 1] == 0)
		{
			max_search_in_hz = _max_freq_in_hz;
			min_search_in_hz = _min_freq_in_hz;
		}
		else
		{
			max_search_in_hz = _pitch_array_in_hz[size - 1] * 2;
			min_search_in_hz = _pitch_array_in_hz[size - 1] / 2;
		}
	}
	//Logger::getCurrentLogger()->writeToLog(String(sampleRate));
	int min_offset_sample = _sample_rate / _max_freq_in_hz;
	int max_offset_sample = _sample_rate / _min_freq_in_hz;
	if (_max_freq_in_hz > max_search_in_hz)
	{
		min_offset_sample = (int)_sample_rate / max_search_in_hz;
	}
	if (_min_freq_in_hz < min_search_in_hz)
	{
		max_offset_sample = (int)_sample_rate / min_search_in_hz;
	}


	int num_peaks(0);
	for (int i = min_offset_sample + 1; i < max_offset_sample - 1; i++)
	{
		float back_diff = auto_corr_array[i] - auto_corr_array[i - 1];
		float for_diff = auto_corr_array[i + 1] - auto_corr_array[i];
		if (back_diff*for_diff <= 0)
		{
			peaks_location.push_back(i);
			peak_value.push_back(auto_corr_array[i]);
			num_peaks++;
		}
	}
	if (peaks_location.size() == 0)
	{
		return maxIndexAutoCorr(auto_corr_array, min_offset_sample, max_offset_sample);
	}
	else
	{
		int max_index = 0;
		float max = peak_value[max_index];
		for (int i = 1; i < num_peaks; i++)
		{
			if (peak_value[i] > max)
			{
				max = peak_value[i];
				max_index = i;
			}
		}
		return peaks_location[max_index];
	}

}
