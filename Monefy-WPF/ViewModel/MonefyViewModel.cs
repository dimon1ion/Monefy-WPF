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
        DateTime dateTimeUser;
        DateTime DateTimeUser { get => dateTimeUser; set { dateTimeUser = value; OnPropertyChanged(nameof(UserDateTimeLabel)); } }
        string filterName;
        private double expens;
        private double result;
        private double profit;
        string fileName;
        bool emptyExpensData;
        private double pieChartSize;
        private readonly double center;
        private readonly double radius;
        Command showCalculate;
        Command changeListVisible;
        Command changeUserDateTime;
        Command filter;
        bool plusShow;
        string buttonResultBackground;
        string buttonResultBorderBrush;
        string listVisible;
        public string UserDateTimeLabel
        {
            get => $"{dateTimeUser.Day} {GetMonthName(dateTimeUser.Month)} {dateTimeUser.Year}";
        }
        public string ListVisible
        {
            get => listVisible;
            set
            {
                if (!Equals(listVisible, value))
                {
                    listVisible = value;
                    OnPropertyChanged(nameof(ListVisible));
                }
            }
        }
        public string BRBackground
        {
            get => buttonResultBackground;
            set
            {
                if (!Equals(buttonResultBackground, value))
                {
                    buttonResultBackground = value;
                    OnPropertyChanged(nameof(BRBackground));
                }
            }
        }
        public string BRBorderBrush
        {
            get => buttonResultBorderBrush;
            set
            {
                if (!Equals(buttonResultBorderBrush, value))
                {
                    buttonResultBorderBrush = value;
                    OnPropertyChanged(nameof(BRBorderBrush));
                }
            }
        }
        public string Expens
        {
            get
            {
                int num = Convert.ToInt32(expens * 100);
                return (num / 100).ToString() + ',' + ((num % 100).ToString().Length == 2 ? (num % 100).ToString() : '0' + (num % 100).ToString());
            }
            set
            {
                if (!Equals(expens.ToString(), value))
                {
                    expens = Double.Parse(value);
                    Result = (profit - expens).ToString();
                    OnPropertyChanged(nameof(Expens));
                }
            }
        }
        public string Result
        {
            get
            {
                int num = Convert.ToInt32(result * 100);
                return "Balance: " + (num / 100).ToString() + ',' + (Math.Abs(num % 100).ToString().Length == 2 ? Math.Abs(num % 100).ToString() : '0' + Math.Abs(num % 100).ToString()) + '$';
            }
            set
            {
                if (!Equals(result.ToString(), value))
                {
                    result = Double.Parse(value);
                    if (result >= 0)
                    {
                        BRBackground = "#7ac795";
                        BRBorderBrush = "#5aa377";
                    }
                    else
                    {
                        BRBackground = "#fc8181";
                        BRBorderBrush = "#a24445";
                    }
                    OnPropertyChanged(nameof(Result));
                }
            }
        }
        public string Profit
        {
            get
            {
                int num = Convert.ToInt32(profit * 100);
                return (num / 100).ToString() + ',' + ((num % 100).ToString().Length == 2 ? (num % 100).ToString() : '0' + (num % 100).ToString());
            }
            set
            {
                if (!Equals(profit.ToString(), value))
                {
                    profit = Double.Parse(value);
                    Result = (profit - expens).ToString();
                    OnPropertyChanged(nameof(Profit));
                }
            }
        }
        public ObservableCollection<Data> Categories { get; set; }
        public ObservableCollection<Data> DataFilter { get; set; }
        public ObservableCollection<Canvas> PieChart { get; set; }
        public ICommand ShowCalculate => showCalculate;
        public ICommand Filter => filter;
        public ICommand ChangeListVisible => changeListVisible;
        public ICommand ChangeUserDateTime => changeUserDateTime;

        #endregion

        ////////////////////////////////////////////////////////////////////////

        #region MonefyCalculate Variables/Prop

        private string noteVisibility;
        private string minusCategoriesView;
        private string plusCategoriesView;
        private string labelValue;
        private string noteValue;
        private bool dotPressed;
        Command addDigit;
        Command removeDigit;
        Command enterCategorie;
        Command addCategorie;

        public string NoteVisibility
        {
            get => noteVisibility;
            set
            {
                if (!Equals(noteVisibility, value))
                {
                    noteVisibility = value;
                    OnPropertyChanged(nameof(NoteVisibility));
                }
            }
        }
        public string MinusCategoriesView
        {
            get => minusCategoriesView;
            set
            {
                if (!Equals(minusCategoriesView, value))
                {
                    minusCategoriesView = value;
                    OnPropertyChanged(nameof(MinusCategoriesView));
                }
            }
        }
        public string PlusCategoriesView
        {
            get => plusCategoriesView;
            set
            {
                if (!Equals(plusCategoriesView, value))
                {
                    plusCategoriesView = value;
                    OnPropertyChanged(nameof(PlusCategoriesView));
                }
            }
        }
        public string LabelValue
        {
            get { return labelValue; }
            set
            {
                if (!Equals(labelValue, value))
                {
                    labelValue = value;
                    OnPropertyChanged(nameof(LabelValue));
                }
            }
        }
        public string NoteValue
        {
            get { return noteValue; }
            set
            {
                if (!Equals(noteValue, value))
                {
                    noteValue = value;
                    OnPropertyChanged(nameof(NoteValue));
                }
            }
        }
        public ICommand AddDigit => addDigit;
        public ICommand RemoveDigit => removeDigit;
        public ICommand EnterCategorie => enterCategorie;
        public ICommand AddCategorie => addCategorie;

        #endregion

        #region Constructor

        public MonefyViewModel(IFileService _fileService)
        {
            #region MainWindow Initialize

            Categories = new ObservableCollection<Data>();
            PieChart = new ObservableCollection<Canvas>() { new Canvas() };
            DataFilter = new ObservableCollection<Data>();
            buttonResultBackground = "#7ac795";
            buttonResultBorderBrush = "#5aa377";
            expens = 0;
            listVisible = "Hidden";
            filterName = "Day";
            pieChartSize = 200;
            center = pieChartSize / 2;
            radius = pieChartSize / 2;
            PieChart[0].Width = pieChartSize;
            PieChart[0].Height = pieChartSize;
            dateTimeUser = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            fileName = "Data.json";
            fileService = _fileService;
            data = fileService.Open(fileName);
            //data.Add(new Data() { Color = "Yellow", Cotegorie = "Mamix", Money = 2500.00, TimeCreate = dateTimeUser });
            //data.Add(new Data() { Color = "Red", Cotegorie = "Marmok", Money = 5000.00, TimeCreate = dateTimeUser });
            //data.Add(new Data() { Color = "Orange", Cotegorie = "COFFI", Money = 200.00, TimeCreate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1) });


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
                plusCategoriesView = "Hidden";
                minusCategoriesView = "Hidden";
                NoteVisibility = "Visible";
            });
            filter = new Command(obj =>
            {
                if (obj is MenuItem)
                {
                    filterName = (obj as MenuItem).Header.ToString();
                    UpdateFilter();
                }
                filter.Check();
            }, obj =>
            {
                if (obj is MenuItem)
                {
                    return filterName != (obj as MenuItem).Header.ToString();
                }
                return false;
            });
            changeListVisible = new Command(obj =>
            {
                if (ListVisible == "Hidden")
                {
                    ListVisible = "Visible";
                }
                else { ListVisible = "Hidden"; }
            });
            changeUserDateTime = new Command(obj =>
            {
                if (obj is Button)
                {
                    int year = dateTimeUser.Year;
                    int month = dateTimeUser.Month;
                    int day = dateTimeUser.Day;
                    switch ((obj as Button).Content)
                    {
                        case "<":
                            day--;
                            break;
                        case "<<":
                            month--;
                            break;
                        case ">":
                            DateTimeUser = dateTimeUser.AddDays(1);
                            UpdateFilter();
                            return;
                        case ">>":
                            DateTimeUser = dateTimeUser.AddMonths(1);
                            UpdateFilter();
                            return;
                    }
                    if (day <= 0)
                    {
                        month--;
                        day = DateTime.DaysInMonth(year, month);
                    }
                    if (month <= 0)
                    {
                        year--;
                        month = 12;
                    }
                    if (day > DateTime.DaysInMonth(year, month))
                    {
                        day = DateTime.DaysInMonth(year, month);
                    }
                    DateTimeUser = new DateTime(year, month, day);
                    UpdateFilter();
                }
            });

            UpdateFilter();


            #endregion

            ////////////////////////////////////////////////////////////////////////

            #region Calculate Initialize

            LabelValue = "0";
            NoteVisibility = "Visible";
            PlusCategoriesView = "Hidden";
            MinusCategoriesView = "Hidden";

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
                if (labelValue.Length > 1)
                {
                    if (labelValue[labelValue.Length - 1] == ',')
                    {
                        dotPressed = false;
                    }
                    LabelValue = labelValue.Remove(labelValue.Length - 1);
                }
                else
                {
                    LabelValue = "0";
                }
            });

            enterCategorie = new Command(obj =>
            {
                if (obj is Button)
                {
                    if ((obj as Button).Content.ToString().StartsWith("Enter"))
                    {
                        (obj as Button).Content = "Close Categories";
                    }
                    else
                    {
                        (obj as Button).Content = "Enter Categorie";
                    }
                    if (plusShow)
                    {
                        if (plusCategoriesView == "Hidden")
                        {
                            PlusCategoriesView = "Visible";
                            NoteVisibility = "Hidden";
                        }
                        else
                        {
                            PlusCategoriesView = "Hidden";
                            NoteVisibility = "Visible";
                        }
                    }
                    else
                    {
                        if (minusCategoriesView == "Hidden")
                        {
                            MinusCategoriesView = "Visible";
                            NoteVisibility = "Hidden";
                        }
                        else
                        {
                            MinusCategoriesView = "Hidden";
                            NoteVisibility = "Visible";
                        }
                    }
                }
            });
            addCategorie = new Command(obj =>
            {
                if (obj is Button)
                {
                    data.Add(new Data()
                    {
                        Color = (obj as Button).Foreground.ToString(),
                        Cotegorie = (obj as Button).Content.ToString(),
                        Note = noteValue,
                        Income = plusShow,
                        TimeCreate = dateTimeUser,
                        Money = Double.Parse(labelValue)
                    });
                    DataFilter.Add(data[data.Count - 1]);
                    if (!data[data.Count - 1].Income)
                    {
                        if (emptyExpensData)
                        {
                            Categories.Clear();
                            emptyExpensData = false;
                        }
                        if (Categories.Count(x => x.Cotegorie == data[data.Count - 1].Cotegorie) == 0)
                        {
                            Categories.Add(new Data() { Cotegorie = data[data.Count - 1].Cotegorie, Color = data[data.Count - 1].Color, Money = data[data.Count - 1].Money, Income = data[data.Count - 1].Income, TimeCreate = data[data.Count - 1].TimeCreate });
                        }
                        else
                        {
                            Categories.Where(x => x.Cotegorie == data[data.Count - 1].Cotegorie).ToList().ElementAt(0).Money += data[data.Count - 1].Money;
                        }
                        Expens = (expens + data[data.Count - 1].Money).ToString();
                        UpdatePieChart();
                        int max = Categories.Count;
                        for (int i = 0; i < max; i++)
                        {
                            Categories.Add(Categories[0]);
                            Categories.RemoveAt(0);
                        }
                    }
                    else
                    {
                        Profit = (profit + data[data.Count - 1].Money).ToString();
                    }
                    calculateView.Close();
                }
            }, obj =>
            {
                return Double.Parse(labelValue) > 0f;
            });


            //Если хотим чтобы кнопки были недоступны когда CanExecute = false в Calculate.xaml + убрать начальное условие в AddDigit.CanExecute

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

            PropertyChanged += (sender, e) =>
            {
                if (calculateView != null)
                {
                    if (calculateView.IsActive && e.PropertyName == nameof(LabelValue))
                    {
                        (addCategorie as Command).Check();
                    }
                }
            };
            #endregion
        }

        #endregion

        #region MonefyMainWindow Methods

        public void Save()
        {
            fileService.Save(fileName, data);
        }
        private void UpdatePieChart()
        {
            PieChart[0].Children.Clear();
            float angle = 0, prevAngle = 0;
            foreach (var category in Categories)
            {
                float precentAge = category.PrecentAge;
                if (category.Cotegorie != "Empty")
                {
                    precentAge = Convert.ToSingle(((category.Money * 100) / Convert.ToInt32(expens * 100)) * 100);
                    category.PrecentAge = Convert.ToInt16(precentAge);
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
        private void UpdateFilter()
        {
            DataFilter.Clear();
            Categories.Clear();
            Expens = "0";
            Profit = "0";
            emptyExpensData = true;
            if (data.Count != 0)
            {
                bool today = false, month = false, year = false;
                switch (filterName)
                {
                    default:
                    case "Day":
                        today = true;
                        month = true;
                        year = true;
                        break;
                    case "Month":
                        year = true;
                        month = true;
                        break;
                    case "Year":
                        year = true;
                        break;
                }
                foreach (Data item in data)
                {
                    if (year)
                    {
                        if (dateTimeUser.Year == item.TimeCreate.Year)
                        {
                            if (month)
                            {
                                if (dateTimeUser.Month == item.TimeCreate.Month)
                                {
                                    if (today)
                                    {
                                        if (dateTimeUser.Day != item.TimeCreate.Day) { continue; }
                                    }
                                }
                                else { continue; }
                            }
                        }
                        else { continue; }
                    }
                    DataFilter.Add(item);
                    if (!item.Income)
                    {
                        emptyExpensData = false;
                        if (Categories.Count(x => x.Cotegorie == item.Cotegorie) == 0)
                        {
                            Categories.Add(new Data() { Cotegorie = item.Cotegorie, Color = item.Color, Money = item.Money });
                        }
                        else
                        {
                            Categories.Where(x => x.Cotegorie == item.Cotegorie).ToList().ElementAt(0).Money += item.Money;
                        }
                        Expens = (expens + item.Money).ToString();
                    }
                    else
                    {
                        Profit = (profit + item.Money).ToString();
                    }
                }
            }
            if (emptyExpensData)
            {
                Categories.Add(new Data() { Cotegorie = "Empty", Color = "Gray", PrecentAge = 100 });
            }
            UpdatePieChart();
        }
        private string GetMonthName(int numOfMonth)
        {
            switch (numOfMonth)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
                default:
                    return "";

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
