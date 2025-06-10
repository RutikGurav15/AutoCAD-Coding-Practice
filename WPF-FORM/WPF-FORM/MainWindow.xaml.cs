using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_FORM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateLine_Click(object sender, RoutedEventArgs e)
        {
            string start = LineLengthInput.Text;
            string end = LineEndInput.Text;

            if (!string.IsNullOrWhiteSpace(start) && !string.IsNullOrWhiteSpace(end))
            {
                Entities.DrawLine(start, end);
            }

            LineLengthInput.Text = "";
            LineEndInput.Text = "";
        }

        private void CreatePolyline_Click(object sender, RoutedEventArgs e)
        {
            string start = PolylineStartInput.Text;
            string mid = PolylineMidInput.Text;
            string end = PolylineEndInput.Text;

            if (!string.IsNullOrWhiteSpace(start) &&
                !string.IsNullOrWhiteSpace(mid) &&
                !string.IsNullOrWhiteSpace(end))
            {
                Entities.DrawPolyline(start, mid, end);
            }

            PolylineStartInput.Text = "";
            PolylineMidInput.Text = "";
            PolylineEndInput.Text = "";
        }

        private void CreateCircle_Click(object sender, RoutedEventArgs e)
        {
            string start = CircleStartInput.Text;

            if (double.TryParse(CircleRadiusInput.Text, out double radius) &&
                !string.IsNullOrWhiteSpace(start))
            {
                Entities.DrawCircle(start, radius);
            }

            CircleStartInput.Text = "";
            CircleRadiusInput.Text = "";
        }

        private void CreateRectangle_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(RectHeightInput.Text, out double height) &&
                double.TryParse(RectWidthInput.Text, out double width))
            {
                Entities.DrawRectangle(height, width);
            }

            RectHeightInput.Text = "";
            RectWidthInput.Text = "";
        }

        private void CreateArc_Click(object sender, RoutedEventArgs e)
        {
            string center = ArcCenterInput.Text.Trim();
            bool radiusValid = double.TryParse(ArcRadiusInput.Text.Trim(), out double radius);
            bool startAngleValid = double.TryParse(ArcStartAngleInput.Text.Trim(), out double startAngle);
            bool endAngleValid = double.TryParse(ArcEndAngleInput.Text.Trim(), out double endAngle);

            if (!string.IsNullOrWhiteSpace(center) && radiusValid && startAngleValid && endAngleValid)
            {
                Entities.DrawArc(center, radius, startAngle, endAngle);
            }

            ArcCenterInput.Text = "";
            ArcRadiusInput.Text = "";
            ArcStartAngleInput.Text = "";
            ArcEndAngleInput.Text = "";
        }


        private void EntitySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hide all groups
            LineGroup.Visibility = Visibility.Collapsed;
            PolylineGroup.Visibility = Visibility.Collapsed;
            CircleGroup.Visibility = Visibility.Collapsed;
            RectangleGroup.Visibility = Visibility.Collapsed;
            ArcGroup.Visibility = Visibility.Collapsed;

            // Show selected
            if (EntitySelector.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Line":
                        LineGroup.Visibility = Visibility.Visible;
                        break;
                    case "Polyline":
                        PolylineGroup.Visibility = Visibility.Visible;
                        break;
                    case "Circle":
                        CircleGroup.Visibility = Visibility.Visible;
                        break;
                    case "Rectangle":
                        RectangleGroup.Visibility = Visibility.Visible;
                        break;
                    case "Arc":
                        ArcGroup.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void AddDimension_Click(object sender, RoutedEventArgs e)
        {
            Entities.FindEntityGiveDimensionCommand();
        }

        private void ShowLayerInputs_Click(object sender, RoutedEventArgs e)
        {
            LayerInputPanel.Visibility = Visibility.Visible;
        }

        private void CreateLayer_Click(object sender, RoutedEventArgs e)
        {
            string layerName = LayerNameInput.Text.Trim();
            bool isValidColor = short.TryParse(LayerColorInput.Text.Trim(), out short colorIndex);

            string selectedEntityType = (LayerEntitySelector.SelectedItem as ComboBoxItem)?.Content?.ToString();

            Entities.CreateLayer(layerName, colorIndex, selectedEntityType);

            LayerNameInput.Text = "";
            LayerColorInput.Text = "";
            LayerEntitySelector.SelectedIndex = -1;
            LayerInputPanel.Visibility = Visibility.Collapsed;
        }
        private void LayerEntitySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (LayerEntitySelector.SelectedItem as ComboBoxItem)?.Content?.ToString();
        }
        private void RemoveLayerButton_Click(object sender, RoutedEventArgs e)
        {
            Entities.RemoveLayerCommand();
        }

    }
}
