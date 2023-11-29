using System.Collections.Generic;

[System.Serializable]
public class MusicData
{
    public BeatData beat_data;
    public EnergyData energy_data;
    public List<float> smoothed_volume;
    public FrequencyData frequency_data;
    public ChromaData chroma_data;
    public Transients transients;
    public MelodyData melody_data;
}

// Beat data structure
[System.Serializable]
public class BeatData
{
    public int tempo;
    public List<float> beat_times;
}

// Energy data structure
[System.Serializable]
public class EnergyData
{
    public List<float> rms_values;
    public float rms_threshold;
    public List<float> high_energy_beats;
    public List<float> low_energy_beats;
}

// // Smoothed volumes structure
// [System.Serializable]
// public class SmoothedVolume
// {
//     public List<float> original;
//     public List<float> t_1_s;
//     public List<float> t_3_s;
//     public List<float> t_9_s;
// }

// Frequency data structure
[System.Serializable]
public class FrequencyData
{
    public FreqBand bass;
    public FreqBand mid;
    public FreqBand treble;
}

// Individual frequency band
[System.Serializable]
public class FreqBand
{
    public List<int> freq_range;
    public List<float> amplitude_values;
}

// Chroma data structure
[System.Serializable]
public class ChromaData
{
    public List<List<float>> chroma; // A list of 12 lists (chromagrams)
    public List<float> chroma_avg;
    public int key_idx;
    public string key;
}

// Transients data structure
[System.Serializable]
public class Transients
{
    public List<float> onset_times;
    public List<int> labels;
}

// Melody data structure
[System.Serializable]
public class MelodyData
{
    public List<float> melody_line;
    public List<int> quantized_midi_numbers;
    public List<float> time_vector;
}
