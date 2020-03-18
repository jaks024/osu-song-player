using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace osu_song_player
{
	public class PlaylistViewModel : ViewModelBase
	{
		private ObservableCollection<SongViewModel> _songs = new ObservableCollection<SongViewModel>();
		private string _name;
		private int _songCount;

		public ObservableCollection<SongViewModel> Songs { get => _songs; set => SetProperty(ref _songs, value); }
		[Newtonsoft.Json.JsonIgnore]
		public string Name { get => _name; set => SetProperty(ref _name, value); }

		[Newtonsoft.Json.JsonIgnore]
		public int SongCount { get => _songCount; set => SetProperty(ref _songCount, value); }

		[Newtonsoft.Json.JsonIgnore]
		public bool changed;
		public void UpdateProperties(PlaylistViewModel playlist)
		{
			Songs = playlist.Songs;
			Name = playlist.Name;
			SongCount = playlist.Songs.Count;
		}

	}
}
