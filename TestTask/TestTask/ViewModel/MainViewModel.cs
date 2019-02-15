using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using TestTask.Model;

namespace TestTask.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            GenerateCommand = new RelayCommand(GenerateMethod);
            CalculateCommand = new RelayCommand(CalculateMethod);
            F1 = F2 = new List<ObservablePoint>();
        }
        public SeriesCollection Series1 { get; set; }
        public ICommand GenerateCommand { get; private set; }
        public ICommand CalculateCommand { get; private set; }
        const int FCount = 20;
        const double step = 5;
        const int minY = -100;
        const int maxY = 100;
        List<ObservablePoint> F1, F2;
        public ObservableCollection<String> Report { get; set; }
        public void GenerateMethod()
        {
            Random random = new Random();
            F1 = GetNewRange(FCount, random);
            F2 = GetNewRange(FCount, random);

            Series1 = new SeriesCollection
            {
                new LineSeries()
                {
                    Values = new ChartValues<ObservablePoint>(F1),
                    Title = "F1",
                    LineSmoothness = 0,
                    Fill = Brushes.Transparent,
                },
                new LineSeries()
                {
                    Values = new ChartValues<ObservablePoint>(F2),
                    Title = "F2",
                    LineSmoothness = 0,
                    Fill = Brushes.Transparent,
                },
            };
            Report = new ObservableCollection<string>();
            this.RaisePropertyChanged(() => this.Report);
            this.RaisePropertyChanged(() => this.Series1);
        }
        List<ObservablePoint> GetNewRange(int pointCount, Random random)
        {
            List<ObservablePoint> range = new List<ObservablePoint>();
            
            range.Add(
                new ObservablePoint()
                {
                    X = step * random.NextDouble(),
                    Y = random.NextDouble() * random.Next(minY, maxY)
                }
            );
            for (int i = 1; i < pointCount; ++i)
            {
                var point = new ObservablePoint()
                {
                    X = (random.NextDouble() + 0.01) * step + range[i - 1].X,
                    Y = random.NextDouble() * random.Next(minY, maxY)
                };

                range.Add(point);
            }
            return range;
        }

        int FindFirstMax(List<ObservablePoint> points, double value)
        {
            int l = 1;
            int r = points.Count - 1;
            int ans = r;
            double currentvalue = points[points.Count - 1].X;
            while (l <= r)
            {
                int middle = (l + r) / 2;
                if (MathMethods.Comparator(points[middle].X, value) == 1)
                {
                    if (MathMethods.Comparator(points[middle].X, currentvalue) == -1)
                    {
                        currentvalue = points[middle].X;
                        ans = middle;
                    }
                    r = middle - 1;
                }
                else
                {
                    l = middle + 1;
                }
            }
            return ans;
        }
        private void CalculateMethod()
        {
            int F1Count = F1.Count - 1;
            int F2Count = F2.Count - 1;
            List<Segment> segments = new List<Segment>();
            ObservablePoint previousIntersectionPoint = new ObservablePoint(-1, -1);
            for (int i1 = 1; i1 <= F1Count; ++i1)
            {

                int l = FindFirstMax(F2, F1[i1 - 1].X);
                int r = FindFirstMax(F2, F1[i1].X);
                if (segments.Count != 0)
                {
                    previousIntersectionPoint = segments[segments.Count - 1].Right;
                }
                for (int i2 = l; i2 <= r; ++i2)
                {
                    var type = MathMethods.GetIntersectionType(F1[i1 - 1], F1[i1], F2[i2 - 1], F2[i2]);
                    if (type == IntersectionType.FirstSecondSegment)
                    {
                        segments.Add(new Segment(F2[i2 - 1], F1[i1], '='));
                        previousIntersectionPoint = F1[i1];
                    }
                    if (type == IntersectionType.SecondFirstSegment)
                    {
                        segments.Add(new Segment(F1[i1 - 1], F2[i2], '='));
                        previousIntersectionPoint = F2[i2];
                    }
                    if (type == IntersectionType.One)
                    {
                        var point = MathMethods.GetIntersectionPoint(F1[i1 - 1], F1[i1], F2[i2 - 1], F2[i2]);
                        if (MathMethods.IsInSegment(point, F1[i1 - 1], F1[i1]) &&
                            MathMethods.IsInSegment(point, F2[i2 - 1], F2[i2]))
                        {
                           
                            if (MathMethods.Comparator(previousIntersectionPoint.X, -1.0) == 1)
                            {
                                int id1 = FindFirstMax(F1, previousIntersectionPoint.X);
                                int id2 = FindFirstMax(F2, previousIntersectionPoint.X);
                                if (MathMethods.IsInSegment(F1[id1].X, previousIntersectionPoint.X, point.X))
                                {
                                    if (MathMethods.IsInSegment(F2[id2].X, previousIntersectionPoint.X, point.X))
                                    {
                                        if (MathMethods.Comparator(F1[id1].X, F2[id2].X) == 1)
                                        {
                                            double delta = F1[id1].Y - MathMethods.GetY(F1[id1].X, F2[id2 - 1], F2[id2]);
                                            segments.Add(new Segment(previousIntersectionPoint, point, delta));
                                        }
                                        else
                                        {
                                            double delta = MathMethods.GetY(F2[id2].X, F1[id1 - 1], F1[id1]) - F2[id2].Y;
                                            segments.Add(new Segment(previousIntersectionPoint, point, delta));
                                        }
                                    }
                                    else
                                    {
                                        double delta = F1[id1].Y - MathMethods.GetY(F1[id1].X, F2[id2-1], F2[id2]);
                                        segments.Add(new Segment(previousIntersectionPoint, point, delta));
                                    }
                                        
                                }
                                else
                                {
                                    double delta = MathMethods.GetY(F2[id2].X, F1[id1 - 1], F1[id1]) - F2[id2].Y;
                                    segments.Add(new Segment(previousIntersectionPoint, point, delta));
                                }
                            }
                            previousIntersectionPoint = point;
                        }

                    }
                }
            }
            if (segments.Count > 0 || MathMethods.Comparator(previousIntersectionPoint.X, -1) == 1)
            {
                ObservablePoint firstpoint, lastpoint;
                double leftdelta, rightdelta;
                if (segments.Count > 0)
                {
                    firstpoint = segments[0].Left;
                    lastpoint = segments[segments.Count - 1].Right;
                }
                else
                {
                    lastpoint = firstpoint = previousIntersectionPoint;
                }

                if (!(MathMethods.IsEqual(F1[0], F2[0]) || MathMethods.IsEqual(previousIntersectionPoint, new ObservablePoint(-1, -1)) &&
                    (MathMethods.IsEqual(previousIntersectionPoint, F1[0]) || MathMethods.IsEqual(previousIntersectionPoint, F2[0]))))
                {
                    int leftId1 = FindFirstMax(F1, firstpoint.X);
                    int leftId2 = FindFirstMax(F2, firstpoint.X);
                    var leftpoint = new ObservablePoint(Math.Max(F1[0].X, F2[0].X), 0);
                    
                    if (MathMethods.Comparator(F1[leftId1 - 1].X, F2[leftId2 - 1].X) == 1)
                    {
                        leftdelta = F1[leftId1 - 1].Y - MathMethods.GetY(F1[leftId1 - 1].X, F2[leftId2 - 1], F2[leftId2]);

                    }
                    else
                    {
                        leftdelta = MathMethods.GetY(F2[leftId2 - 1].X, F1[leftId1 - 1], F1[leftId1]) - F2[leftId2 - 1].Y;
                    }
                    segments.Insert(0, new Segment(leftpoint, firstpoint, leftdelta));
                }

                if (!(MathMethods.IsEqual(F1[F1Count], F2[F2Count]) || MathMethods.IsEqual(previousIntersectionPoint, new ObservablePoint(-1, -1)) &&
                    (MathMethods.IsEqual(previousIntersectionPoint, F1[F1Count]) || MathMethods.IsEqual(previousIntersectionPoint, F2[F2Count]))))
                {
                    int rightId1 = FindFirstMax(F1, lastpoint.X);
                    int rightId2 = FindFirstMax(F2, lastpoint.X);
                    var rightpoint = new ObservablePoint(Math.Min(F1[F1Count].X, F2[F2Count].X), 0);
                    if (MathMethods.Comparator(F1[rightId1].X, F2[rightId2].X) == -1)
                    {
                        rightdelta = F1[rightId1].Y - MathMethods.GetY(F1[rightId1].X, F2[rightId2 - 1], F2[rightId2]);
                    }
                    else
                    {
                        rightdelta = MathMethods.GetY(F2[rightId2].X, F1[rightId1 - 1], F1[rightId1]) - F2[rightId2].Y;
                    }
                    segments.Add(new Segment(lastpoint, rightpoint, rightdelta));
                }
            }
            else
            {
                double middleX = (F1[0].X + F1[F1Count].X) / 2;
                int id1 = FindFirstMax(F1, middleX);
                int id2 = FindFirstMax(F2, middleX);
                ObservablePoint left = new ObservablePoint(Math.Max(F1[0].X, F2[0].X), 0);
                ObservablePoint right = new ObservablePoint(Math.Min(F1[F1Count].X, F2[F2Count].X), 0);
                segments.Add(new Segment(left, right, MathMethods.GetY(middleX, F1[id1 - 1], F1[id1]) - MathMethods.GetY(middleX, F2[id2 - 1], F2[id2])));
            }

            UpdateReport(segments);
            this.RaisePropertyChanged(() => this.Report);
        }
        void UpdateReport(List<Segment> segments)
        {
            Report = new ObservableCollection<string>();
            for (int i = 0; i < segments.Count; ++i)
            {
                string line = Math.Round(segments[i].Left.X, 2).ToString() + "; " + Math.Round(segments[i].Right.X, 2).ToString();
                if (segments[i].Sign == '=')
                {
                    line = "[" + line + "]: ";
                }
                else
                {
                    line = "(" + line + "): ";
                }
                line += "F1 " + segments[i].Sign + " F2";
                Report.Add(line);
            }
        }
        
    }
}