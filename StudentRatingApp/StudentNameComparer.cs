using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentRatingApp
{
    public class StudentNameComparer : IComparer<Student>
    {
        public int Compare(Student? x, Student? y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
