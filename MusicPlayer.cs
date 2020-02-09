using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
namespace osu_song_player
{
	public class MusicPlayer : ViewModelBase
	{
		private ISoundOut soundOut;
		private IWaveSource waveSource;
		private SongViewModel song;

		public SongViewModel SongInfo
		{
			get => song;
		}
		public double TimeInSeconds
		{
			get
			{
				if (waveSource != null)
				{
					//NotifyPropertyChanged("TimeInSeconds");
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
			get => waveSource.GetPosition().ToString(@"mm\:ss");
		}

		public void Update()
		{
			NotifyPropertyChanged("Position");
			NotifyPropertyChanged("PositionInTime");
		}

		public void Open(SongViewModel songViewModel, string name, MMDevice device)
		{
			CleanUp();
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
		}

		public void Play()
		{
			if (soundOut != null)
				soundOut.Play();
		}
		public void Pause()
		{
			if (soundOut != null)
				soundOut.Pause();
		}
		public void Stop()
		{
			if (soundOut != null)
				soundOut.Stop();
		}

		public void Change()
		{
			Stop();
			CleanUp();
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
