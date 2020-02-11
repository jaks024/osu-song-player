using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace osu_song_player
{
	public class UserConfigManager
	{
		private readonly string configFileName = "config.json";
		private UserConfig config;
		public UserConfig Config { get => config; }
		public bool IsConfigEmpty { get; private set; } = true;
		public void SetOutputDeviceId(string device)
		{
			config.outputDeviceId = device;
		}
		public void SerializeConfig(string path, string outputDevice)
		{
			config.folderPath = path;
			config.outputDeviceId = outputDevice;
			if (!config.folderPath.Equals(string.Empty))
				IsConfigEmpty = false;

			using(StreamWriter sw = File.CreateText(configFileName))
			{
				sw.Write(JsonConvert.SerializeObject(config, Formatting.Indented));
				Console.WriteLine("serialized config to config.json");
			}
		}

		public void DeserializeConfig()
		{
			if (!File.Exists(configFileName))
			{
				Console.WriteLine("doesn't exist, creating");
				File.Create(configFileName);
				IsConfigEmpty = true;
				return;
			}
			Console.WriteLine(File.Exists(configFileName));

			using (StreamReader sr = File.OpenText(configFileName))
			{
				string content = sr.ReadToEnd();
				if (content.Equals(string.Empty) || content == null)
				{
					IsConfigEmpty = true;
					return;
				}
				config = JsonConvert.DeserializeObject<UserConfig>(content);
				IsConfigEmpty = false;
			}
		}


	}

	public struct UserConfig
	{
		public string folderPath;
		public string outputDeviceId;
	}
}
