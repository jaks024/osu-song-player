using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_song_player
{
	public class SongSearcher
	{
		private const string ignorePhrase = "tv size";
		public List<SongViewModel> Search(string searchFor, List<SongViewModel> original, bool ignoreTvSize)
		{
			string[] keys = searchFor.ToLower().Trim().Split(' ');
			List<SongViewModel> songs = new List<SongViewModel>();
			for (int i = 0; i < original.Count; i++)
			{
				string searchParam = original[i].SearchParams().ToLower();

				if (ignoreTvSize && searchParam.Contains(ignorePhrase))
					continue;

				int count = 0;
				for(int x = 0; x < keys.Length; x++)
				{
					if (searchParam.Contains(keys[x]))
						count++;
				}
				if(count == keys.Length)
				{
					Console.WriteLine("found: " + searchParam);
					songs.Add(original[i]);
				}
			}
			return songs;
		}
	}
}
