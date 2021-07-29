using Monefy_WPF.Model;
using Monefy_WPF.Service;
using Monefy_WPF.Service.Command;
using Monefy_WPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Monefy_WPF.ViewModel
{
    public class MonefyViewModel : INotifyPropertyChanged
    {
        #region MonefyMainWindow Variables/Prop

        Calculate calculateView;
        private List<Data> data;
        private IFileService fileService;
        DateTime dateTime;
        long expens;
        string fileName;
        bool emptyData;
        private double pieChartSize;
        private readonly double center;
        private readonly double radius;
        Command showCalculate;
        bool plusShow;
        public ObservableCollection<Data> Categories { get; set; }
        public ObservableCollection<Canvas> PieChart { get; set; }
        public ICommand ShowCalculate => showCalculate;

        #endregion

        ////////////////////////////////////////////////////////////////////////

        #region MonefyCalculate Variables/Prop

        private string labelValue = "0";
        Command addDigit;
        Command removeDigit;
        bool dotPressed;



        public string LabelValue
        {
            get { return labelValue; }
            set
            {
                if (!labelValue.Equals(value))
                {
                    labelValue = value;
                    OnPropertyChanged(nameof(LabelValue));
                }
            }
        }
        public ICommand AddDigit => addDigit;
        public ICommand RemoveDigit => removeDigit;

        #endregion

        #region Constructor

        public MonefyViewModel(IFileService _fileService)
        {
            #region MainWindow Initialize

            Categories = new ObservableCollection<Data>();
            PieChart = new ObservableCollection<Canvas>() { new Canvas() };
            expens = 0;
            pieChartSize = 200;
            center = pieChartSize / 2;
            radius = pieChartSize / 2;
            PieChart[0].Width = pieChartSize;
            PieChart[0].Height = pieChartSize;
            dateTime = DateTime.Now;
            fileName = "Data.json";
            fileService = _fileService;
            data = fileService.Open(fileName);
            data.Add(new Data() { Color = "Yellow", Cotegorie = "Mamix", Money = 2500.00 });
            data.Add(new Data() { Color = "Red", Cotegorie = "Marmok", Money = 5000.00 });
            data.Add(new Data() { Color = "Orange", Cotegorie = "COFFI", Money = 200.00 });


            showCalculate = new Command(obj =>
            {
                if (obj is Button)
                {
                    if ((obj as Button).Name.StartsWith("plus"))
                    {
                        plusShow = true;
                    }
                    else
                    {
                        plusShow = false;
                    }
                }
                calculateView = new Calculate(this);
                calculateView.ShowDialog();
                labelValue = "0";
                dotPressed = false;
            });

            if (data.Count != 0)
            {
                emptyData = false;
                foreach (Data item in data)
                {
                    if (Categories.Count(x => x.Cotegorie == item.Cotegorie) == 0)
                    {
                        Categories.Add(new Data() { Cotegorie = item.Cotegorie, Color = item.Color, Money = item.Money });
                    }
                    else
                    {
                        Categories.Where(x => x.Cotegorie == item.Cotegorie).ToList().ElementAt(0).Money += item.Money;
                    }
                    expens += Convert.ToInt64(item.Money);
                }
            }
            else
            {
                Categories.Add(new Data() { Cotegorie = "Empty", Color = "Gray", PrecentAge = 100 });
                emptyData = true;
            }
            UpdatePieChart();

            #endregion

            ////////////////////////////////////////////////////////////////////////

            #region Calculate Initialize

            addDigit = new Command(obj => //Execute
            {
                if (obj is Button)
                {
                    if ((obj as Button).Content.ToString() == ",")
                    {
                        dotPressed = true;
                    }
                    else if (LabelValue == "0")
                    {
                        labelValue = String.Empty;
                    }
                    LabelValue += (obj as Button).Content.ToString();
                }
            }, obj => //CanExecute
            {
                if (calculateView.IsActive)
                {
                    if (dotPressed)
                    {
                        if (obj is Button)
                        {
                            if (labelValue.Split(',')[1].Length == 2)
                            {
                                return false;
                            }
                            return (obj as Button).Content.ToString() != ",";
                        }
                    }
                    else if (LabelValue == "0")
                    {
                        if (obj is Button)
                        {
                            return (obj as Button).Content.ToString() != "0";
                        }
                    }
                }
                return true;
            });

            removeDigit = new Command(obj =>
            {
                if (labelValue.Length > 0)
                {
                    if (labelValue[labelValue.Length - 1] == ',')
                    {
                        dotPressed = false;
                    }
                    LabelValue = labelValue.Remove(labelValue.Length - 1);
                    if (labelValue == String.Empty)
                    {
                        LabelValue = "0";
                    }
                }
            });


            //Если хотим чтобы кнопки были недоступны когда CanExecute = false в Calculate.xaml + убрать условие в AddDigit.CanExecute

            //PropertyChanged += (sender, e) =>
            //{
            //    if (calculateView != null)
            //    {
            //        if (calculateView.IsActive && e.PropertyName == nameof(LabelValue))
            //        {
            //            (addDigit as Command).Check();
            //        }
            //    }
            //};
            #endregion
        }

        #endregion

        #region MonefyMainWindow Methods

        private void UpdatePieChart()
        {
            PieChart[0].Children.Clear();
            float angle = 0, prevAngle = 0;
            foreach (var category in Categories)
            {
                float precentAge = category.PrecentAge;
                if (category.Cotegorie != "Empty")
                {
                    precentAge = Convert.ToSingle((category.Money * 100) / expens);
                    category.PrecentAge = precentAge;
                }
                if (precentAge > 99.999f)
                {
                    precentAge = 99.999f;
                }
                double line1X = (radius * Math.Cos(angle * Math.PI / 180)) + center;
                double line1Y = (radius * Math.Sin(angle * Math.PI / 180)) + center;

                angle = precentAge * (float)360 / 100 + prevAngle;

                double arcX = (radius * Math.Cos(angle * Math.PI / 180)) + center;
                double arcY = (radius * Math.Sin(angle * Math.PI / 180)) + center;

                var line1Segment = new LineSegment(new Point(line1X, line1Y), false);
                double arcWidth = radius, arcHeight = radius;
                bool isLargeArc = precentAge > 50;
                var arcSegment = new ArcSegment()
                {
                    Size = new Size(arcWidth, arcHeight),
                    Point = new Point(arcX, arcY),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = isLargeArc,
                };
                var line2Segment = new LineSegment(new Point(center, center), false);

                var pathFigure = new PathFigure(
                    new Point(center, center),
                    new List<PathSegment>()
                    {
                        line1Segment,
                        arcSegment,
                        line2Segment,
                    },
                    true);

                var pathFigures = new List<PathFigure>() { pathFigure, };
                var pathGeometry = new PathGeometry(pathFigures);
                var path = new Path()
                {
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(category.Color)),
                    Data = pathGeometry,
                };
                prevAngle = angle;
                PieChart[0].Children.Add(path);
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
