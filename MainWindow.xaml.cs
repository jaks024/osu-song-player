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
		private SongFolderCrawler songFolderCrawler;
		public string selectedFolderPath;
		private readonly ObservableCollection<MMDevice> devices = new ObservableCollection<MMDevice>();
		System.Windows.Threading.DispatcherTimer dispatcherTimer;
		public MainWindow()
		{
			InitializeComponent();
			LoadDevices();
			songFolderCrawler = new SongFolderCrawler(songListBox);
			songListBox.DataContext = songFolderCrawler;
			songInfoGrid.DataContext = musicPlayer;

			dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += dispatcherTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
		}

		List<SongViewModel> temp = new List<SongViewModel>();
		private void SelectPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog();
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (!dialog.SelectedPath.Equals(selectedFolderPath))
				{
					selectedFolderPath = dialog.SelectedPath;
					selectedPathTextBlock.Text = selectedFolderPath;

					songFolderCrawler.SearchThreaded(selectedFolderPath);
				}
				
			}
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
			musicPlayer.Change();

			SongViewModel song = (SongViewModel)songListBox.SelectedItem;
			Console.WriteLine("playing: " + selectedFolderPath + "\\" + song.Path);
			musicPlayer.Open(song, selectedFolderPath + "\\" + song.Path, (MMDevice)audioOutputComboBox.SelectedItem);
			Console.WriteLine((MMDevice)audioOutputComboBox.SelectedItem);
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			musicPlayer.Update();
		}

	}
}
