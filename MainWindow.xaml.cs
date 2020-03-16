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
		private readonly PlaylistSerializer serializer = new PlaylistSerializer();
		private readonly PlaylistCreator creator = new PlaylistCreator();
		public string selectedFolderPath;
		private readonly ObservableCollection<MMDevice> devices = new ObservableCollection<MMDevice>();
		private ObservableCollection<PlaylistItemViewModel> playlistItems = new ObservableCollection<PlaylistItemViewModel>();
		System.Windows.Threading.DispatcherTimer dispatcherTimer;
		private SongViewModel currentSong;
		public PlaylistViewModel currentPlaylist = new PlaylistViewModel();
		public MainWindow()
		{
			InitializeComponent();
			LoadDevices(); //cscore

			playlistItems = new ObservableCollection<PlaylistItemViewModel>(serializer.GetAllPlaylists());
			playlistListBox.ItemsSource = playlistItems;

			userConfigManager.DeserializeConfig();
			if (!userConfigManager.IsConfigEmpty)
			{
				//setting osu folder path
				if(playlistItems.Count == 0)
					LoadSongsFromPath(userConfigManager.Config.folderPath);
				else
				{
					selectedFolderPath = userConfigManager.Config.folderPath;
					selectedPathTextBlock.Text = userConfigManager.Config.folderPath;
				}

				if(userConfigManager.Config.outputDeviceId != null)
				{
					for(int i = 0; i < devices.Count; i++)
					{
						if (userConfigManager.Config.outputDeviceId.Equals(devices[i].DeviceID))
						{
							audioOutputComboBox.SelectedIndex = i;
							Console.WriteLine("audio device loaded from config " + devices[i]);
						}
					}
				}

				musicPlayer.Volume = userConfigManager.Config.volume;
			}

			playlistInfoPanel.DataContext = currentPlaylist;
			songListBox.DataContext = currentPlaylist;
			songInfoGrid.DataContext = musicPlayer;	//also sets the time slider and volume slider data context

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

			//songFolderCrawler.SearchThreaded(path);
		}

		private void SerializeConfig()
		{
			userConfigManager.SetConfigValues(selectedFolderPath, ((MMDevice)audioOutputComboBox.SelectedItem).DeviceID, musicPlayer.Volume);
			userConfigManager.SerializeConfig();
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
		
			//get selected song source from the currently selected tab. "all song" and "search" tab
			SongViewModel selected = songListTabControl.SelectedIndex == 1 ? 
					(SongViewModel)searchListbox.SelectedItem : (SongViewModel)songListBox.SelectedItem;
			if (selected == null)
				return;
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
				userConfigManager.SetConfigValues(((MMDevice)audioOutputComboBox.SelectedItem).DeviceID);
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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			PlaylistSerializer serializer = new PlaylistSerializer();
			PlaylistViewModel playlist = new PlaylistViewModel();
			playlist.Songs = songFolderCrawler.Songs;
			playlist.SongCount = songFolderCrawler.Songs.Count;
			playlist.Name = "all_songs";
			serializer.Serialize(playlist);
		}

		private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PlaylistItemViewModel item = playlistItems[playlistListBox.SelectedIndex];
			PlaylistViewModel playlist = serializer.DeserializePlaylist(item.Path);
			if(playlist != null)
				currentPlaylist.UpdateProperties(playlist);
		}

		private void VolumeSlider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			//volumeValueLabel.Content = musicPlayer.Volume.ToString("F0");
		}

		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			SongSearcher searcher = new SongSearcher();
			searchListbox.ItemsSource = searcher.Search(searchTextBox.Text, new List<SongViewModel>(currentPlaylist.Songs), (bool)ignoreTxSizeCheckBox.IsChecked);
			searchFoundCountLabel.Content = searchListbox.Items.Count;
		}

		private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
		{
			if(newPlaylistTextbox.Text.Length == 0)
			{
				MessageBox.Show("Playlist must have a name");
				return;
			}

			if (creator.inProgress && (bool)createConditionCheckBox.IsChecked )
			{
				MessageBox.Show("Cannot create playlist due to ongoing operation");
				return;
			}


			if ((bool)createConditionCheckBox.IsChecked )
			{
				creator.events += AddToPlaylistItemListFromCreator;
				creator.CreatePlaylist(newPlaylistTextbox.Text, selectedFolderPath);
				Console.WriteLine("called");
			}
			else
			{
				PlaylistViewModel playlist = creator.CreatePlaylist(newPlaylistTextbox.Text);
				playlistItems.Add(new PlaylistItemViewModel(playlist.Name, serializer.GetPlaylistPath(playlist)));
			}
			newPlaylistTextbox.Text = "";
			createConditionCheckBox.IsChecked = false;
		}

		public void AddToPlaylistItemListFromCreator(object sender, EventArgs e)
		{
			if (creator.tempPlaylist == null)
				return;
			this.Dispatcher.Invoke(() =>
			{
				playlistItems.Add(new PlaylistItemViewModel(creator.tempPlaylist.Name, serializer.GetPlaylistPath(creator.tempPlaylist)));
			});

			Console.WriteLine("added to list");
			creator.tempPlaylist = null;
		}
	}
}
