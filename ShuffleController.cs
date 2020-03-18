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
			Console.WriteLine("past value: " + pastValues.Count + " maximum: " + maximum);
			if (pastValues.Count == maximum)
			{
				Console.WriteLine("cleared past values *****");
				ClearPastValues();
			}

			int loopBreaker = 0;
			pastValues.Add(ignore);
			Random rand = new Random();
			int num = rand.Next(0, maximum);
			while (IsDuplicate(num))
			{
				num = rand.Next(0, maximum);
				loopBreaker++;
				if (loopBreaker > 10000)
					break;
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
			Console.WriteLine("cleared past values");
			pastValues.Clear();
		}
	}
}
