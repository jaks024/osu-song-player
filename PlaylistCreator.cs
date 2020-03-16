using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace osu_song_player
{
	public class PlaylistCreator
	{
		private SongFolderCrawler crawler;
		private string tempName;
		public PlaylistViewModel tempPlaylist;
		public event EventHandler events;
		public bool inProgress;
		public PlaylistViewModel CreatePlaylist(string name)
		{
			PlaylistSerializer serializer = new PlaylistSerializer();
			PlaylistViewModel playlist = new PlaylistViewModel();
			playlist.Songs = new System.Collections.ObjectModel.ObservableCollection<SongViewModel>();
			playlist.SongCount = 0;
			playlist.Name = name;
			serializer.Serialize(playlist);
			return playlist;
		}

		public void CreatePlaylist(string name, string path)
		{
			Console.WriteLine("search sequence started: " + name);
			crawler = new SongFolderCrawler();
			crawler.SearchThreaded(path);

			Thread checkThread = new Thread(CheckComplete);
			checkThread.Start();
			tempName = name;
			inProgress = true;
		}

		public void CheckComplete()
		{
			while (true)
			{
				Thread.Sleep(1000);
				Console.WriteLine("checked compeltion");
				if (crawler.searchCompleted)
				{
					Console.WriteLine("search completed, " + tempName);
					CreatePlaylist();
					events?.Invoke(this, EventArgs.Empty);
					inProgress = false;
					break;
				}
			}
		}

		private void CreatePlaylist()
		{
			PlaylistSerializer serializer = new PlaylistSerializer();
			PlaylistViewModel playlist = new PlaylistViewModel();

			if(crawler.Songs == null)
			{
				CreatePlaylist(tempName);
				return;
			}

			playlist.Songs = crawler.Songs;
			playlist.SongCount = crawler.Songs.Count;
			playlist.Name = tempName;
			serializer.Serialize(playlist);
			Console.WriteLine("sequence compelted, playlist: " + tempName + ", song count: " + playlist.SongCount);
			tempPlaylist = playlist;
		}
	}
}
