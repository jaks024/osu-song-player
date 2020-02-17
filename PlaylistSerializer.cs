using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace osu_song_player
{
	public class PlaylistSerializer
	{
		private const string jsonExtension = ".json";
		private const string directoryName = "playlists";
		public void Serialize(PlaylistViewModel playlist)
		{
			Directory.CreateDirectory(directoryName);

			using (StreamWriter sw = File.CreateText(Path.Combine(Directory.GetCurrentDirectory(), directoryName, playlist.Name + jsonExtension)))
			{
				sw.Write(JsonConvert.SerializeObject(playlist, Formatting.Indented));
				Console.WriteLine("serialized playlist " + playlist.Name + " to config.json");
			}
		}

		public List<PlaylistItemViewModel> GetAllPlaylists()
		{
			string path = Path.Combine(Directory.GetCurrentDirectory(), directoryName);
			IEnumerable<string> meta = Directory.EnumerateFiles(path, "*.json", SearchOption.TopDirectoryOnly);
			List<PlaylistItemViewModel> items = new List<PlaylistItemViewModel>();
			foreach (string p in meta)
			{
				int index = p.LastIndexOf("\\") + 1;
				string name = p.Substring(index, p.LastIndexOf(".") - index); //remove extensions
				Console.WriteLine(name);
				items.Add(new PlaylistItemViewModel(name, p));
			}

			return items;
		}

		public PlaylistViewModel DeserializePlaylist(string path)
		{
			//string path = Path.Combine(Directory.GetCurrentDirectory(), directoryName, name);
			if (!File.Exists(path))
			{
				Console.WriteLine("playlist doesnt exist: " + path);
				return null;
			}
			using (StreamReader sr = File.OpenText(path))
			{
				string content = sr.ReadToEnd();
				if (content.Equals(string.Empty) || content == null)
				{
					return null;
				}
				return JsonConvert.DeserializeObject<PlaylistViewModel>(content);
			}
		}

	}
}
