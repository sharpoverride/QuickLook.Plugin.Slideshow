using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace QuickLook.Plugin.Slideshow
{
    /// <summary>
    /// Interaction logic for SlideshowPanel.xaml
    /// </summary>
    public partial class SlideshowPanel : UserControl
    {
        public SlideshowPanel()
        {
            InitializeComponent();
        }

        public event Action StartSlideshow;

        public int NumberOfSeconds
        {
            get => shortUpDownSeconds.Value.Value;
        }

        internal void ShowEntriesFound(List<string> entries)
        {
            lblNumberOfEntries.Content =
                $"Found {entries.Count} entries in directory";
        }

        private void ButtonStartSlideshow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StartSlideshow?.Invoke();
        }
    }
}
