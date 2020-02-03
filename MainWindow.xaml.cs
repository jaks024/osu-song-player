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

		public MainWindow()
		{

			SongViewModel song = new SongViewModel(0, "song one", "5:00", "path/123");
			songs.Add(song);

			InitializeComponent();
			songListBox.DataContext = this;

		}
	}
}
