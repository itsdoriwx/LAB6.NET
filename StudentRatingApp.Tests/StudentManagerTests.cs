using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentRatingApp.Tests
{
    [TestClass]
    public class StudentManagerTests
    {
        private JsonFileReader _fileReader = null!;
        private JsonFileWriter _fileSaver = null!;
        private StudentManager _manager = null!;

        [TestInitialize]
        public void Setup()
        {
            _fileReader = new JsonFileReader();
            _fileSaver = new JsonFileWriter();
            _manager = new StudentManager(_fileReader, _fileSaver);
        }

        [TestMethod]
        public void Constructor_WithNullLoader_ThrowsArgumentNullException()  // валідація конструктора
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StudentManager(null!, _fileSaver));
        }

        [TestMethod]
        public void Constructor_WithNullSaver_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StudentManager(_fileReader, null!));
        }

        [TestMethod]    
        public void AddStudent_WithValidStudent_AddsToList()    // тест додавання студентів
        {
            var student = new Student("Матвійчук Іван Іванович", 1, "КН-11", 85.0);

            _manager.AddStudent(student);

            var allStudents = _manager.GetAllStudents();

            Assert.AreEqual(1, allStudents.Count); // к-сть змін
            Assert.AreEqual("Матвійчук Іван Іванович", allStudents[0].FullName); // контекст змін 
        }

        [TestMethod]
        public void AddStudent_WithNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _manager.AddStudent(null!));
        }

        [TestMethod]
        public void RemoveStudent_WhenStudentExists_ReturnsTrueAndRemoves()   // видалення студента
        {
            var student = new Student("Ломкач Любава Вікторівна", 2, "К-21", 90.0);
            _manager.AddStudent(student);

            bool result = _manager.RemoveStudent(student);

            Assert.IsTrue(result);  // звіт про успіх є/ні
            Assert.AreEqual(0, _manager.GetAllStudents().Count);   // фактичний результат звітування 
        } 

        [TestMethod]
        public void SortByRating_ReturnsStudentsOrderedByRatingDescending()    // сортування за рейтингом (за спаданням)
        {
            var studentLow = new Student("Богдан", 1, "КН-11", 70.0);
            var studentHigh = new Student("Анна", 1, "КН-11", 95.0);
            var studentMiddle = new Student("Василь", 1, "КН-11", 85.0);

            _manager.AddStudent(studentLow);
            _manager.AddStudent(studentHigh);
            _manager.AddStudent(studentMiddle);

            var sorted = _manager.SortByRating();

            Assert.AreEqual("Анна", sorted[0].FullName);
            Assert.AreEqual("Василь", sorted[1].FullName);
            Assert.AreEqual("Богдан", sorted[2].FullName);
        }

        [TestMethod]
        public void SortByName_ReturnsStudentsOrderedAlphabetically()           // сортування за алфавітом ПІБ
        {
            var student3 = new Student("Тарас", 2, "КН-21", 80.0);
            var student1 = new Student("Андрій", 2, "КН-21", 85.0);
            var student2 = new Student("Ольга", 2,  "ІПЗ-31", 90.0);

            _manager.AddStudent(student3);
            _manager.AddStudent(student1);
            _manager.AddStudent(student2);

            var sorted = _manager.SortByName();

            Assert.AreEqual("Андрій", sorted[0].FullName);
            Assert.AreEqual("Ольга", sorted[1].FullName);
            Assert.AreEqual("Тарас", sorted[2].FullName);
        }

        [TestMethod]
        public void FilterByGroup_ReturnsOnlyMatchingGroup()
        {
            var student1 = new Student("Студент 1", 1, "КН-11", 80.0);
            var student2 = new Student("Студент 2", 2, "КН-21", 85.0);
            var student3 = new Student("Студент 3", 1, "КН-11", 90.0);

            _manager.AddStudent(student1);
            _manager.AddStudent(student2);
            _manager.AddStudent(student3);

            var filtered = _manager.FilterByGroup("КН-11");

            Assert.AreEqual(2, filtered.Count);
            Assert.AreEqual("КН-11", filtered[0].Group);
            Assert.AreEqual("КН-11", filtered[1].Group);
        }
    }
}
