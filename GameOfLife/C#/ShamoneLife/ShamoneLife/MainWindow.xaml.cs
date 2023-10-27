using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timer = System.Threading.Timer;

namespace ShamoneLife;

public partial class MainWindow : Window
{
    const int DesiredWidth = 900;
    const int Dimension = 100;
    bool ShouldSimulate = true;
    public Dictionary<(int x, int y), Rectangle> BoardMap { get; set; }
    public (State State, Rectangle Rect)[,] Board { get; set; }
    public MainWindow()
    {
        BoardMap = new Dictionary<(int x, int y), Rectangle>();
        Board = new (State, Rectangle)[Dimension, Dimension];
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Width = DesiredWidth;
        Height = DesiredWidth;
        Draw(DesiredWidth, Dimension);
    }

    async void StartSimulation(object sender, EventArgs args)
    {
        ShouldSimulate = true;

        while (ShouldSimulate)
        {
            BoardHelper.UpdateNextState(BoardMap.Keys.ToList(), Board);
            BoardHelper.SetState(BoardMap.Keys.ToList(), Board);
            await Task.Delay(25);
            InvalidateVisual();
        }
    }

    private void DrawCell((int x, int y) coordinate, object sender, EventArgs args)
    {
        if (Board[coordinate.x, coordinate.y].State.IsActive)
        {
            BoardHelper.SetCoordinateState(coordinate, Board, kill: true);
            return;
        }

        BoardHelper.SetCoordinateState(coordinate, Board);
    }

    void Reset(object sender, EventArgs args)
    {
        BoardMap.Keys.ToList()
            .ForEach(coordinate => BoardHelper.SetCoordinateState(coordinate, Board, kill: true));
        InvalidateVisual();
    }

    void StopSimulation(object sender, EventArgs args)
    {
        ShouldSimulate = false;
    }
    public void Draw(int width, int dimension)
    {
        var layout = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        var buttonGrid = new Grid
        {
            FlowDirection = FlowDirection.LeftToRight,
        };

        buttonGrid.RowDefinitions.Add(new RowDefinition());
        buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
        buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
        buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
        buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());


        var startButton = new Button
        {
            Content = "Start",
            Width = 100,
            Height = 20,
            Margin = new Thickness(0, 0, 0, 5)
        };

        var stopButton = new Button
        {
            Content = "Stop",
            Width = 100,
            Height = 20,
            Margin = new Thickness(0, 0, 0, 5)
        };

        var resetButton = new Button
        {
            Content = "Reset",
            Width = 100,
            Height = 20,
            Margin = new Thickness(0, 0, 0, 5)
        };

        startButton.Click += StartSimulation;
        stopButton.Click += StopSimulation;
        resetButton.Click += Reset;

        Grid.SetColumn(startButton, 0);
        Grid.SetColumn(stopButton, 1);
        Grid.SetColumn(resetButton, 2);

        buttonGrid.Children.Add(startButton);
        buttonGrid.Children.Add(stopButton);
        buttonGrid.Children.Add(resetButton);



        var combobox = new ComboBox
        {
            Width = 100,
            Height = 20,
            Margin = new Thickness(0, 0, 0, 15)
        };

        combobox.Items.Add("Glider");
        combobox.Items.Add("Glider Gun");
        combobox.Items.Add("Spinner");
        combobox.Items.Add("Pulsar");
        combobox.Items.Add("Simpkin Glider Gun");
        combobox.Items.Add("Puffer Train");
        combobox.Items.Add("Random");
        combobox.SelectedIndex = 0;

        Grid.SetColumn(combobox, 3);

        combobox.SelectionChanged += DrawFigure;

        buttonGrid.Children.Add(combobox);

        layout.Children.Add(buttonGrid);

        var grid = new Grid
        {
            Width = width * 0.9,
            Height = width * 0.9,
            Background = Brushes.White
        };

        for (var i = 0; i < dimension; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
        }

        for (var i = 0; i < dimension; i++)
        {
            for (var j = 0; j < dimension; j++)
            {
                var rect = new Rectangle
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.1,
                    Fill = Brushes.White
                };

                var x = i;
                var y = j;

                rect.MouseLeftButtonDown += (sender, e) => DrawCell((x, y), sender, e);

                var state = new State
                {
                    IsActive = false,
                    ShouldBeActive = false
                };

                Grid.SetColumn(rect, i);
                Grid.SetRow(rect, j);


                Board[i, j] = (state, rect);
                BoardMap.Add((i, j), rect);
                grid.Children.Add(rect);
            }
        }

        layout.Children.Add(grid);

        Content = layout;
    }

    private void DrawFigure(object sender, SelectionChangedEventArgs e) 
    {
        switch (((ComboBox)sender).SelectedItem.ToString())
        {
            case "Glider": 
                BoardHelper.MakeGlider(Board);
                break;

            case "Glider Gun":
                BoardHelper.MakeGliderGun(Board);
                break;

            case "Spinner":
                BoardHelper.MakeSpinner(Board);
                break;

            case "Pulsar": 
                BoardHelper.MakePulsar(Board);
                break;

            case "Simpkin Glider Gun":
                BoardHelper.MakeSimpkinGliderGun(Board);
                break;

            case "Puffer Train":
                BoardHelper.MakePufferTrain(Board);
                break;

            default:
                break;
        }
    }
}

public class State
{
    public bool IsActive { get; set; }
    public bool ShouldBeActive { get; set; }
}