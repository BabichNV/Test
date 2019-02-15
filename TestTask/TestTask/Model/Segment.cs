using GalaSoft.MvvmLight;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask.Model;

namespace TestTask
{
    public class Segment : ObservableObject
    {
        public ObservablePoint Left { get; set; }
        public ObservablePoint Right { get; set; }
        public char Sign { get; set; }
        public Segment(ObservablePoint left, ObservablePoint right, char s)
        {
            Left = left;
            Right = right;
            Sign = s;
        }
        public Segment(ObservablePoint left, ObservablePoint right, double delta)
        {
            Left = left;
            Right = right;
            switch (MathMethods.Comparator(delta, 0.0))
            {
                case 1:
                    Sign = '>';
                    break;
                case 0:
                    Sign = '=';
                    break;
                case -1:
                    Sign = '<';
                    break;
            }
        }
    }
}
