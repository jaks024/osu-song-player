using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_song_player
{
	public class SongViewModel : ViewModelBase
	{
		private int _order;
		private string _name;
		private string _artist;
		private string _length = "00:00";
		private string _audioName;
		private string _playlistName;
		public int Order { get => _order; set => SetProperty(ref _order, value); }
		public string Name { get => _name; set => SetProperty(ref _name, value); }
		public string Artist { get => _artist; set => SetProperty(ref _artist, value); }

		[Newtonsoft.Json.JsonIgnore]
		public string Length { get => _length; set => SetProperty(ref _length, value); }
		public string Path { get => _audioName; set => SetProperty(ref _audioName, value); }
		[Newtonsoft.Json.JsonIgnore]
		public string PlaylistName { get => _playlistName; set => _playlistName = value; }

		[Newtonsoft.Json.JsonIgnore]
		public string NameAndArtist { get => _name + " - " + _artist;}
		[Newtonsoft.Json.JsonConstructor]
		public SongViewModel(int order, string name, string artist, string audioName)
		{
			_order = order;
			_name = name;
			_artist = artist;
			_audioName = audioName;
		}
		public SongViewModel(SongViewModel song)
		{
			_order = song.Order;
			_name = song.Name;
			_artist = song.Artist;
			_audioName = song.Path;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1} - {2}, {3}, {4}", _order, _name, _artist, _length, _audioName);
		}
		public string SearchParams()
		{
			return string.Format("{0} {1}", Name, Artist);
		}

		//not override because it throws castexception in listbox 
		public bool CheckEquals(object obj)
		{
			SongViewModel s = (SongViewModel)obj;
			if (s == null)
				return false;
			return _order == s._order && NameAndArtist.Equals(s.NameAndArtist) && _playlistName.Equals(s._playlistName);
		}
	}
}
