using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace osu_song_player
{
	public class SongFolderCrawler : ViewModelBase
	{
		public ObservableCollection<SongViewModel> Songs { get; private set; }

		private void Search(string path)
		{
			ObservableCollection<SongViewModel> songs = new ObservableCollection<SongViewModel>();
			int songCount = 0;
			string[] directories = Directory.GetDirectories(path);
			foreach (var d in directories)
			{
				string[] meta = Directory.GetFiles(d, "*.osu", SearchOption.TopDirectoryOnly);
				if (meta.Length == 0)
					continue;
				using(StreamReader s = new StreamReader(meta[0])){
					int lineCount = 0;
					string line, audioName, title, artist;
					line = audioName = title = artist = "";
					while((line = s.ReadLine()) != null && lineCount < 40)
					{
						if (line.Contains("AudioFilename:"))
						{
							audioName = line.Substring(("AudioFilename:").Length + 1);
						}
						else if (line.Contains("Title:"))
						{
							title = line.Substring(("Title:").Length);
						}
						else if (line.Contains("Artist:"))
						{
							artist = line.Substring(("Artist:").Length);
							break;
						}
						lineCount++;
					}
					SongViewModel song = new SongViewModel(songCount, title, artist, new DirectoryInfo(d).Name + "\\"+ audioName);
					Console.WriteLine(song);
					songs.Add(song);
					songCount++;
				}
			}
			Songs = songs;
			NotifyPropertyChanged("Songs");
			Console.WriteLine("*********************" + songs.Count);
		}

		public void SearchThreaded(string path)
		{
			Thread t = new Thread(() => Search(path));
			t.Start();
		}

	}
}
