using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentRatingApp
{
    public class Student : IComparable<Student>
    {
        private string _fullName = string.Empty;
        private int _course;
        private string _group = string.Empty;
        private double _ratingScore;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("ПІБ студента не може бути порожнім.", nameof(FullName));
                }
                _fullName = value.Trim();
            }
        }

        public int Course
        {
            get { return _course; }
            set
            {
                if (value < 1 || value > 4)
                {
                    throw new ArgumentException("Курс навчання має бути в діапазоні від 1 до 4.", nameof(Course));
                }
                _course = value;
            }
        }

        public string Group
        {
            get { return _group; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Назва групи не може бути порожньою.", nameof(Group));
                }
                _group = value.Trim();
            }
        }

        public double RatingScore
        {
            get { return _ratingScore; }
            set
            {
                if (value < 60.0 || value > 100.0)
                {
                    throw new ArgumentException("Рейтинговий бал має бути в межах від 60.00 до 100.00.", nameof(RatingScore));
                }
                _ratingScore = value;
            }
        }

        public Student() {}

        public Student(string fullName, int course, string group, double ratingScore)
        {
            FullName = fullName;
            Course = course;
            Group = group;
            RatingScore = ratingScore;
        }

        public int CompareTo(Student? other)
        {
            if (other == null)
            {
                return 1;
            }

            return other.RatingScore.CompareTo(this.RatingScore);
        }

        public override string ToString()
        {
            return $"{FullName}, {Course} курс, група {Group}, рейтинг: {RatingScore:F2}";
        }
    }
}
