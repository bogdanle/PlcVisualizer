using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace PlcVisualizer.Views.Controls
{
    /// <summary>
    /// Interaction logic for TagsGrid.xaml.
    /// </summary>
    public partial class TagsGrid : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TagsGrid), new PropertyMetadata(null, OnItemsSourceChanged));

        public TagsGrid()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (TagsGrid)d;
            if (e.NewValue is IEnumerable itemsSource)
            {
                obj.dataGrid.ItemsSource = itemsSource;

                ICollectionView colView = CollectionViewSource.GetDefaultView(itemsSource);
                if (colView != null && colView.CanGroup)
                {
                    colView.GroupDescriptions.Clear();
                    colView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
                }
            }
        }
    }
}
