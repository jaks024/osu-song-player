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
		private string _length;
		private string _path;
		public int Order { get => _order; set => SetProperty(ref _order, value); }
		public string Name { get => _name; set => SetProperty(ref _name, value); }
		public string Length { get => _length; set => SetProperty(ref _name, value); }
		public string Path { get => _path; set => SetProperty(ref _path, value); }

		public SongViewModel(int order, string name, string length, string path)
		{
			_order = order;
			_name = name;
			_length = length;
			_path = path;
		}
	}
}
