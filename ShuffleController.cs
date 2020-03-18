using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_song_player
{
	public class ShuffleController
	{
		public HashSet<int> pastValues = new HashSet<int>();

		public int GetNextValue(int maximum, int ignore)
		{
			if (pastValues.Count == maximum)
				ClearPastValues();

			pastValues.Add(ignore);
			Random rand = new Random();
			int num = rand.Next(0, maximum);
			while (IsDuplicate(num))
			{
				num = rand.Next(0, maximum);
			}
			pastValues.Add(num);
			return num;
		}

		private bool IsDuplicate(int val)
		{
			return pastValues.Contains(val);
		}

		public void ClearPastValues()
		{
			pastValues.Clear();
		}
	}
}
