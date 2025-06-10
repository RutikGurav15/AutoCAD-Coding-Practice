using AutoCAD_Project.Core;
using System.Data;
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

namespace AutoCAD_Project
{
    public partial class MainWindow : Window
    {
        BlockCounter blockCounter;
        LayerCleaner layerCleaner;

        public MainWindow()
        {
            InitializeComponent();
            blockCounter = new BlockCounter();
            layerCleaner = new LayerCleaner();
        }

        private void BlockCounterBtn_Click(object sender, RoutedEventArgs e)
        {
            RightPanel.Children.Clear();

            List<BlockData> blockList = blockCounter.GetBlockCounts();

            Grid table = new Grid();
            table.ColumnDefinitions.Add(new ColumnDefinition());
            table.ColumnDefinitions.Add(new ColumnDefinition());
            table.ColumnDefinitions.Add(new ColumnDefinition());
            table.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            table.Children.Add(CreateCell("Sr.No", 0, 0, true));
            table.Children.Add(CreateCell("Block Name", 0, 1, true));
            table.Children.Add(CreateCell("Count", 0, 2, true));

            for (int i = 0; i < blockList.Count; i++)
            {
                table.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                table.Children.Add(CreateCell((i + 1).ToString(), i + 1, 0));
                table.Children.Add(CreateCell(blockList[i].Name, i + 1, 1));
                table.Children.Add(CreateCell(blockList[i].Count.ToString(), i + 1, 2));
            }

            RightPanel.Children.Add(table);

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            buttonPanel.Children.Add(CreateActionButton("Cancel", (s, e) => RightPanel.Children.Clear(), "Cancel the operation"));
            buttonPanel.Children.Add(CreateActionButton("Generate Excel", (s, e) => GenerateExcel(blockList), "Export block data to Excel"));
            buttonPanel.Children.Add(CreateActionButton("Clean Block", (s, e) => blockCounter.CleanUnusedBlocks(), "Remove unused blocks from drawing"));
            RightPanel.Children.Add(new Separator { Margin = new Thickness(0, 15, 0, 15) });
            RightPanel.Children.Add(buttonPanel);
        }
        private UIElement CreateCell(string content, int row, int column, bool isHeader = false)
        {
            TextBlock tb = new TextBlock
            {
                Text = content,
                Padding = new Thickness(10, 4, 10, 4), // Reduced padding = shorter height
                FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal,
                FontSize = isHeader ? 14 : 13,
                TextAlignment = TextAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Foreground = isHeader
                    ? new SolidColorBrush(Color.FromRgb(30, 30, 30))
                    : new SolidColorBrush(Color.FromRgb(60, 60, 60))
            };

            Color borderColor = Color.FromRgb(200, 200, 200);
            Color headerBackground = Color.FromRgb(225, 230, 240);
            Color altRowColor = Color.FromRgb(248, 248, 248);  // Optional alternating row color

            Border border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(borderColor),
                Background = isHeader
                    ? new SolidColorBrush(headerBackground)
                    : (row % 2 == 0
                        ? new SolidColorBrush(Colors.White)
                        : new SolidColorBrush(altRowColor)),
                Child = tb
            };

            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);

            return border;
        }



        private UIElement CreateActionButton(string content, RoutedEventHandler onClick, string tooltip)
        {
            var btn = new Button
            {
                Content = content,
                Margin = new Thickness(0),
                Padding = new Thickness(15, 8, 15, 8),
                Background = new SolidColorBrush(Color.FromRgb(60, 90, 153)),
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                ToolTip = tooltip,
                Width = 140,
                Height = 38,
            };

            btn.Click += onClick;

            // Hover effect
            btn.MouseEnter += (s, e) =>
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(50, 80, 140));
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(60, 90, 153));
            };

            // Wrap in Border to apply rounded corners
            var border = new Border
            {
                CornerRadius = new CornerRadius(5),
                Child = btn,
                Margin = new Thickness(5)
            };

            return border;
        }


        private void GenerateExcel(List<BlockData> blockList)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "BlockCount",
                DefaultExt = ".xlsx",
                Filter = "Excel Files (*.xlsx)|*.xlsx"
            };

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                blockCounter.GenerateExcel(blockList, dlg.FileName);
                MessageBox.Show("Excel file generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CleanBlocks()
        {
            var res = MessageBox.Show("Are you sure you want to clean unused blocks?", "Confirm Cleanup", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                blockCounter.CleanUnusedBlocks();
                MessageBox.Show("Unused blocks cleaned!", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LayerCleanerBtn_Click(object sender, RoutedEventArgs e)
        {
            RightPanel.Children.Clear();

            List<LayerData> layers = layerCleaner.GetLayerUsageData();

            Grid table = new Grid();
            table.ColumnDefinitions.Add(new ColumnDefinition());
            table.ColumnDefinitions.Add(new ColumnDefinition());
            table.ColumnDefinitions.Add(new ColumnDefinition());
            table.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            table.Children.Add(CreateCell("Sr.No", 0, 0, true));
            table.Children.Add(CreateCell("Layer Name", 0, 1, true));
            table.Children.Add(CreateCell("Count", 0, 2, true));

            for (int i = 0; i < layers.Count; i++)
            {
                table.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                table.Children.Add(CreateCell((i + 1).ToString(), i + 1, 0));
                table.Children.Add(CreateCell(layers[i].Name, i + 1, 1));
                table.Children.Add(CreateCell(layers[i].Count.ToString(), i + 1, 2));
            }

            RightPanel.Children.Add(table);

            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            buttonPanel.Children.Add(CreateActionButton("Cancel", (s, e) => RightPanel.Children.Clear(), "Cancel the operation"));
            buttonPanel.Children.Add(CreateActionButton("Clean Layer", (s, e) => CleanLayers(), "Remove unused and frozen layers"));

            RightPanel.Children.Add(new Separator { Margin = new Thickness(0, 15, 0, 15) });
            RightPanel.Children.Add(buttonPanel);
        }

        private void CleanLayers()
        {
            var res = MessageBox.Show("Are you sure you want to clean unused and frozen layers?", "Confirm Cleanup", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                layerCleaner.CleanUnusedAndFrozenLayers();
                MessageBox.Show("Unused and frozen layers cleaned!", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EntityConverterBtn_Click(object sender, RoutedEventArgs e)
        {
            RightPanel.Children.Clear();

            WrapPanel entityPanel = new WrapPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10),
            };

            entityPanel.Children.Add(CreateRoundedButton("Text to MText", (s, e1) => new TextToMTextConverter().Convert(), "Convert Text to MText"));
            entityPanel.Children.Add(CreateRoundedButton("MText to Text", (s, e1) => new MTextToTextConverter().Convert(), "Convert MText to Text"));
            entityPanel.Children.Add(CreateRoundedButton("Line to Polyline", (s, e1) => new LineToPolylineConverter().Convert(), "Convert Line to Polyline"));
            entityPanel.Children.Add(CreateRoundedButton("Polyline to Line", (s, e1) => new PolylineToLineConverter().Convert(), "Convert Polyline to Line"));
            entityPanel.Children.Add(CreateRoundedButton("Block to Entity", (s, e1) => new BlockToEntityConverter().Convert(), "Explode Block to Entities"));

            RightPanel.Children.Add(entityPanel);
        }

        private Button CreateRoundedButton(string text, RoutedEventHandler clickEvent, string tooltip)
        {
            Button button = new Button
            {
                Content = text,
                ToolTip = tooltip,
                Margin = new Thickness(8),
                Padding = new Thickness(14, 8, 14, 8),
                Background = new SolidColorBrush(Color.FromRgb(45, 49, 66)), // Default background
                Foreground = Brushes.White,
                Cursor = Cursors.Hand,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                Width = 150,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Create a custom control template with rounded corners
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            borderFactory.SetValue(Border.SnapsToDevicePixelsProperty, true);

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenter);

            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = borderFactory;
            button.Template = template;

            // Hover effects
            button.MouseEnter += (s, e) =>
            {
                button.Background = new SolidColorBrush(Color.FromRgb(70, 130, 180)); // Steel blue
            };
            button.MouseLeave += (s, e) =>
            {
                button.Background = new SolidColorBrush(Color.FromRgb(45, 49, 66)); // Original color
            };

            button.Click += clickEvent;

            return button;
        }



    }
}