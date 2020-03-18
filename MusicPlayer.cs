using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using System.Windows;
namespace osu_song_player
{
	public class MusicPlayer : ViewModelBase
	{
		private ISoundOut soundOut;
		private IWaveSource waveSource;
		private float volumeBuffer = 100;
		private SongViewModel song;
		public bool HasAudio { get; private set; }
		public bool IsPlaying { get; private set; }
		public bool manualStop { get; private set; }
		public SongViewModel SongInfo
		{
			get => song == null ? new SongViewModel(0, "", "", "") : song;
		}
		public double TimeInSeconds
		{
			get
			{
				if (waveSource != null)
				{
					return waveSource.GetLength().TotalSeconds;
				}
				return 0;
			}
		}
		public double Position
		{
			get
			{
				if (waveSource != null)
				{
					Console.WriteLine("seconds " + waveSource.GetPosition().TotalSeconds);
					return waveSource.GetPosition().TotalSeconds;
				}
				return 0;
			}
			set
			{
				waveSource.SetPosition(TimeSpan.FromSeconds(value));
			}
		}
		public string PositionInTime
		{
			get
			{
				if(waveSource != null)
					return waveSource.GetPosition().ToString(@"mm\:ss");
				return "00:00";
			}
		}
		public float Volume
		{
			get
			{
				if (soundOut != null && soundOut.WaveSource != null)
					return Math.Min(soundOut.Volume * 100, 100f);
				return volumeBuffer;
			}
			set
			{
				if (soundOut != null && soundOut.WaveSource != null)
					soundOut.Volume = Math.Min(value / 100, 100f);
				volumeBuffer = value;
				NotifyPropertyChanged("VolumeRounded");
				Console.WriteLine("Volume: " + volumeBuffer);
			}
		}
		public double VolumeRounded
		{
			get { return Math.Round(volumeBuffer, 1); }
		}
		public void Update()
		{
			NotifyPropertyChanged("Position");
			NotifyPropertyChanged("PositionInTime");
			if (soundOut == null)
				return;
			Console.WriteLine("state " + soundOut.PlaybackState);
			if (soundOut.PlaybackState == PlaybackState.Stopped)
				IsPlaying = false;
		}

		public void Open(SongViewModel songViewModel, string name, MMDevice device)
		{
			CleanUp();

			if (!System.IO.File.Exists(name))
			{
				MessageBox.Show("Cannot find the song file at " + name, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			waveSource = CodecFactory.Instance.GetCodec(name).ToSampleSource().ToMono().ToWaveSource();
			soundOut = new WasapiOut() { Latency = 100, Device = device };
			soundOut.Initialize(waveSource);

			song = songViewModel;
			song.Length = waveSource.GetLength().ToString(@"mm\:ss");
			Console.WriteLine("selected " + song);

			NotifyPropertyChanged("SongInfo");
			NotifyPropertyChanged("TimeInSeconds");
			NotifyPropertyChanged("Position");
			NotifyPropertyChanged("PositionInTime");

			HasAudio = true;
		}
		public void SetDevice(MMDevice device)
		{
			soundOut = new WasapiOut { Latency = 100, Device = device };
			if(waveSource != null)
				soundOut.Initialize(waveSource);
			Console.WriteLine("changed device to " + device);
		}
		public void Play()
		{
			if (soundOut != null && HasAudio)
			{
				soundOut.Play();
				Console.WriteLine("volume buffer " + volumeBuffer);
				UpdateVolume();
				IsPlaying = true;
				manualStop = false;
			}
		}
		public void Pause()
		{
			if (soundOut != null && HasAudio)
			{
				soundOut.Pause();
				IsPlaying = false;
				manualStop = true;
			}
		}
		public void Stop()
		{
			if (soundOut != null && HasAudio)
			{
				soundOut.Stop();
				IsPlaying = false;
				manualStop = true;
			}
		}
		public void ResetCurrentTime()
		{
			Position = 0;
		}

		//cleans up everything
		public void End()
		{
			manualStop = false;
			HasAudio = false;
			IsPlaying = false;
			Stop();
			CleanUp();
		}

		public void UpdateVolume()
		{
			Volume = volumeBuffer;
		}

		private void CleanUp()
		{
			if(soundOut!= null)
			{
				soundOut.Dispose();
				soundOut = null;
			}
			if(waveSource != null)
			{
				waveSource.Dispose();
				waveSource = null;
			}
		}
	}
}
