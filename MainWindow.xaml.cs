using System;
using System.Collections.Generic;
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

namespace osu_song_player
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SongFolderCrawler songFolderCrawler;
		public string selectedFolderPath;
		public MainWindow()
		{
			InitializeComponent();
			songFolderCrawler = new SongFolderCrawler(songListBox);
			songListBox.DataContext = songFolderCrawler;

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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			songListBox.ItemsSource = songFolderCrawler.Songs;
			songListBox.Items.Refresh();
			Console.WriteLine("refereshed");
		}
	}
}
