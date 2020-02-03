using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public List<SongViewModel> songs { get; } = new List<SongViewModel>();
		public string selectedFolderPath;
		public MainWindow()
		{
			InitializeComponent();
			songListBox.DataContext = this;

		}

		private void SelectPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog();
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (!dialog.SelectedPath.Equals(selectedFolderPath))
				{
					selectedFolderPath = dialog.SelectedPath;
					selectedPathTextBlock.Text = selectedFolderPath;

					List<string> folders = SongFolderCrawler.Search(selectedFolderPath);
					for(int i = 0; i < folders.Count; i++)
					{
						songs.Add(new SongViewModel(i, folders[i], folders[i].Length.ToString(), folders[i])); 
					}
					songListBox.Items.Refresh();
				}
				
			}
		}
	}
}
