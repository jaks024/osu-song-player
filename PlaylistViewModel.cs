using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_song_player
{
	public class PlaylistViewModel : ViewModelBase
	{
		private ObservableCollection<SongViewModel> _songs = new ObservableCollection<SongViewModel>();
		private string _name;
		private int _songCount;

		public ObservableCollection<SongViewModel> Songs { get => _songs; set => SetProperty(ref _songs, value); }
		public string Name { get => _name; set => SetProperty(ref _name, value); }
		public int SongCount { get => _songCount; set => SetProperty(ref _songCount, value); }

		public void UpdateProperties(PlaylistViewModel playlist)
		{
			Songs = playlist.Songs;
			Name = playlist.Name;
			SongCount = playlist.SongCount;
		}

	}
}
