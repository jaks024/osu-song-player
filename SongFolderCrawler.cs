using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_song_player
{
	public class SongFolderCrawler
	{
		public static List<string> Search(string path)
		{
			List<String> folders = new List<string>();
			foreach(var d in Directory.GetDirectories(path))
			{
				folders.Add(new DirectoryInfo(d).Name);
			}

			return folders;
		}


	}
}
