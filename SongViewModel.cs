﻿using System;
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
		public int Order { get => _order; set => SetProperty(ref _order, value); }
		public string Name { get => _name; set => SetProperty(ref _name, value); }
		public string Artist { get => _artist; set => SetProperty(ref _artist, value); }
		public string Length { get => _length; set => SetProperty(ref _length, value); }
		public string Path { get => _audioName; set => SetProperty(ref _audioName, value); }
		public string NameAndArtist { get => _name + " - " + _artist;}
		public SongViewModel(int order, string name, string artist, string audioName)
		{
			_order = order;
			_name = name;
			_artist = artist;
			_audioName = audioName;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1} - {2}, {3}, {4}", _order, _name, _artist, _length, _audioName);
		}
	}
}
