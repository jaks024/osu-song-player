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
				string name = GetFileNameNoExtension(p);
				Console.WriteLine(name);
				items.Add(new PlaylistItemViewModel(name, p));
			}

			return items;
		}
		private string GetFileNameNoExtension(string p)
		{
			int index = p.LastIndexOf("\\") + 1;
			return p.Substring(index, p.LastIndexOf(".") - index); //remove extensions
		}

		public string GetPlaylistPath(PlaylistViewModel playlist)
		{
			string path = Path.Combine(Directory.GetCurrentDirectory(), directoryName);
			return string.Format("{0}\\{1}{2}", path, playlist.Name, jsonExtension);
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
				Console.WriteLine("playlist deserialized: " + path);
				PlaylistViewModel playlist = JsonConvert.DeserializeObject<PlaylistViewModel>(content);
				playlist.Name = GetFileNameNoExtension(path);
				return playlist;
			}
		}

		public void DeletePlaylistFile(string path)
		{
			if (!File.Exists(path))
			{
				Console.WriteLine("Playlist doesnt exist");
				System.Windows.MessageBox.Show("Cannot delete a playlist that does not exist");
				return;
			}

			File.Delete(path);
			System.Windows.MessageBox.Show("Deleted playlist at " + path);
			Console.WriteLine("deleted " + path);
		}

		public string RenamePlaylist(string path, string newName)
		{
			if (!File.Exists(path))
			{
				Console.WriteLine("Playlist doesnt exist");
				System.Windows.MessageBox.Show("Cannot rename a playlist that does not exist");
				return "";
			}
			int index = path.LastIndexOf("\\") + 1;
			string changed = Path.Combine(path.Substring(0, index), newName + jsonExtension);
			File.Move(path, changed);
			Console.WriteLine("Renamed to: " + changed);
			return changed;
		}
	}
}
