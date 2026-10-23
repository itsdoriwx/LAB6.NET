using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentRatingApp
{
    public class StudentManager
    {
        private List<Student> _students;
        private IFileLoader _fileLoader;
        private IFileSaver _fileSaver;

        public StudentManager(IFileLoader fileLoader, IFileSaver fileSaver)
        {
            if (fileLoader == null)
            {
                throw new ArgumentNullException(nameof(fileLoader), "Завантажувач файлів не може бути null.");
            }

            if (fileSaver == null)
            {
                throw new ArgumentNullException(nameof(fileSaver), "Сервіс збереження не може бути null.");
            }

            _fileLoader = fileLoader;
            _fileSaver = fileSaver;
            _students = new List<Student>();
        }

        public void AddStudent(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student), "Студент не може бути null.");
            }

            bool isDuplicate = _students.Any(existing => string.Equals(existing.FullName, student.FullName, StringComparison.OrdinalIgnoreCase) && existing.Course == student.Course && string.Equals(existing.Group, student.Group, StringComparison.OrdinalIgnoreCase));

            if (isDuplicate)
            {
                throw new InvalidOperationException($"Студент «{student.FullName}» (курс: {student.Course}, група: {student.Group}) вже є у списку.");
            }

            _students.Add(student);
        }

        public bool RemoveStudent(Student student)
        {
            if (student == null)
            {
                return false;
            }

            return _students.Remove(student);
        }

        public List<Student> GetAllStudents()
        {
            return new List<Student>(_students);
        }

        public List<Student> SortByRating()
        {
            return _students.OrderByDescending(s => s.RatingScore).ToList();
        }

        public List<Student> SortByName()
        {
            return _students.OrderBy(s => s.FullName).ToList();
        }

        public List<Student> FilterByGroup(string group)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                return GetAllStudents();
            }

            return _students.Where(s => string.Equals(s.Group, group.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Student> GetTopThreeStudents()
        {
            return _students
                .OrderByDescending(s => s.RatingScore)
                .Take(3)
                .ToList();
        }

        public double CalculateAverageScoreByGroup(string group)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                return 0.0;
            }

            var groupStudents = _students.Where(s => string.Equals(s.Group, group.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            if (groupStudents.Count == 0)
            {
                return 0.0;
            }

            return groupStudents.Average(s => s.RatingScore);
        }

        public (Student? MinStudent, Student? MaxStudent) GetMinAndMaxRatingStudents()
        {
            if (_students.Count == 0)
            {
                return (null, null);
            }

            var minStudent = _students.MinBy(s => s.RatingScore);
            var maxStudent = _students.MaxBy(s => s.RatingScore);

            return (minStudent, maxStudent);
        }

        public List<Student> FilterByExactScore(double targetScore)
        {
            return _students .Where(s => Math.Abs(s.RatingScore - targetScore) < 0.001).ToList();
        }

        public List<Student> SearchByName(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllStudents();
            }

            string cleanTerm = searchTerm.Trim().ToLowerInvariant();

            return _students.Where(s => s.FullName.ToLowerInvariant().Contains(cleanTerm)).ToList();
        }

        public List<GroupStatistics> GetStudentCountPerGroup()
        {
            return _students
                .GroupBy(s => s.Group)
                .OrderBy(g => g.Key)
                .Select(g => new GroupStatistics(g.Key, g.Count()))
                .ToList();
        }
        public async Task SaveToFileAsync(string filePath)
        {
            await _fileSaver.SaveAsync(filePath, _students);
        }

        public async Task LoadFromFileAsync(string filePath)
        {
            var loadedStudents = await _fileLoader.LoadAsync<Student>(filePath);
            if (loadedStudents != null)
            {
                _students.Clear();
                foreach (var student in loadedStudents)
                {
                    _students.Add(student);
                }
            }
        }
    }
    public record GroupStatistics(string GroupName, int StudentCount); // клас для зберігання результатів запиту групування
}