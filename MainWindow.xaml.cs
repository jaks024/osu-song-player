using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using CSCore.Streams;
namespace osu_song_player
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MusicPlayer musicPlayer = new MusicPlayer();
		private readonly UserConfigManager userConfigManager = new UserConfigManager();
		private readonly SongFolderCrawler songFolderCrawler = new SongFolderCrawler();
		public string selectedFolderPath;
		private readonly ObservableCollection<MMDevice> devices = new ObservableCollection<MMDevice>();
		System.Windows.Threading.DispatcherTimer dispatcherTimer;
		private SongViewModel currentSong;
		public MainWindow()
		{
			InitializeComponent();
			LoadDevices(); //cscore

			userConfigManager.DeserializeConfig();
			if (!userConfigManager.IsConfigEmpty)
			{
				LoadSongsFromPath(userConfigManager.Config.folderPath);

				if(userConfigManager.Config.outputDeviceId != null)
				{
					for(int i = 0; i < devices.Count; i++)
					{
						if (userConfigManager.Config.outputDeviceId.Equals(devices[i].DeviceID))
						{
							audioOutputComboBox.SelectedIndex = i;
							Console.WriteLine("audio device loaded from config");
						}
					}
				}
			}
			songListBox.DataContext = songFolderCrawler;
			songInfoGrid.DataContext = musicPlayer;

			dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += dispatcherTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

			this.Closed += new EventHandler(ClosingCleanUp);

		}

		private void ClosingCleanUp(object sender, EventArgs e)
		{
			musicPlayer.End();
			SerializeConfig();
		}

		List<SongViewModel> temp = new List<SongViewModel>();
		private void SelectPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog();
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (!dialog.SelectedPath.Equals(selectedFolderPath))
				{
					LoadSongsFromPath(dialog.SelectedPath);

					//if path changed, save it to config
					SerializeConfig();
				}
				
			}
		}

		private void LoadSongsFromPath(string path)
		{
			selectedFolderPath = path;
			selectedPathTextBlock.Text = path;

			songFolderCrawler.SearchThreaded(path);
		}

		private void SerializeConfig()
		{
			userConfigManager.SerializeConfig(selectedFolderPath, ((MMDevice)audioOutputComboBox.SelectedItem).DeviceID);
		}

		private void LoadDevices()
		{
			using (var mmdeviceEnumerator = new MMDeviceEnumerator())
			{
				using (
					var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
				{
					foreach (var device in mmdeviceCollection)
					{
						devices.Add(device);
					}
				}
			}
			audioOutputComboBox.ItemsSource = devices;
			audioOutputComboBox.SelectedIndex = 0;
		}

		private void CtrlPlayBtn_Click(object sender, RoutedEventArgs e) 
		{
			SongViewModel selected = (SongViewModel)songListBox.SelectedItem;
			if (!musicPlayer.HasAudio || !currentSong.CheckEquals(selected))
			{
				currentSong = selected;
				Console.WriteLine("playing: " + selectedFolderPath + "\\" + currentSong.Path);
				musicPlayer.Open(currentSong, selectedFolderPath + "\\" + currentSong.Path, (MMDevice)audioOutputComboBox.SelectedItem);
				Console.WriteLine((MMDevice)audioOutputComboBox.SelectedItem);
			}
		
			musicPlayer.Play();
			dispatcherTimer.Start();
		}
		private void CtrlPauseBtn_Click(object sender, RoutedEventArgs e)
		{
			musicPlayer.Pause();
			dispatcherTimer.Stop();
		}
		private void CtrlStopBtn_Click(object sender, RoutedEventArgs e)
		{
			musicPlayer.Stop();
			dispatcherTimer.Stop();
		}

		private void SongListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//musicPlayer.End();
			//maybe add currently selected name to top
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			musicPlayer.Update();
		}

		private void AudioOutputComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (audioOutputComboBox.SelectedItem != null)
			{
				musicPlayer.Stop();
				musicPlayer.SetDevice((MMDevice)audioOutputComboBox.SelectedItem);
				userConfigManager.SetOutputDeviceId(((MMDevice)audioOutputComboBox.SelectedItem).DeviceID);
			}
		}

		private bool tempIsPreviouslyPlaying = false;

		private void TimeSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			tempIsPreviouslyPlaying = musicPlayer.IsPlaying;
			musicPlayer.Pause();
			Console.WriteLine("started drag");
		}

		private void TimeSlider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			musicPlayer.Update();
			Console.WriteLine("dragging");
		}

		private void TimeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			if (tempIsPreviouslyPlaying)
				musicPlayer.Play();
			Console.WriteLine("finish drag");
		}
	}
}
