using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_song_player
{
	public class PlaylistItemViewModel : ViewModelBase
	{
		private string _name;

		public string Name { get => _name; set => SetProperty(ref _name, value); }
		public string Path { get; set; }


		public PlaylistItemViewModel(string n, string p)
		{
			Name = n;
			Path = p;
		}
	}
}
