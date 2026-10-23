using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentRatingApp.Tests
{
    internal class DummyFileLoader : IFileLoader
    {
        public Task<List<T>?> LoadAsync<T>(string filePath) => Task.FromResult<List<T>?>(new List<T>());
    }

    internal class DummyFileSaver : IFileSaver
    {
        public Task SaveAsync<T>(string filePath, List<T> data) => Task.CompletedTask;
    }

    [TestClass]
    public class StudentManagerLinqTests
    {
        private StudentManager _manager = null!;
        private Student _student1 = null!;
        private Student _student2 = null!;
        private Student _student3 = null!;
        private Student _student4 = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _manager = new StudentManager(new DummyFileLoader(), new DummyFileSaver());

            _student1 = new Student("Нікішина Валерія Матвіївна", 2, "КН-22", 95.5);
            _student2 = new Student("Потоцька Олеся Олександрівна", 2, "КН-22", 88.0);
            _student3 = new Student("Ломкач Любава Вікторівна", 2, "ПП-21", 90.0);
            _student4 = new Student("Іванов Іван Іванович", 3, "АнД-31", 88.0);

            _manager.AddStudent(_student1);
            _manager.AddStudent(_student2);
            _manager.AddStudent(_student3);
            _manager.AddStudent(_student4);
        }

        [TestMethod]
        public void GetTopThreeStudents_ReturnsThreeStudentsWithHighestScores()
        {
            var topStudents = _manager.GetTopThreeStudents();

            Assert.AreEqual(3, topStudents.Count);
            Assert.AreEqual("Нікішина Валерія Матвіївна", topStudents[0].FullName);
            Assert.AreEqual("Ломкач Любава Вікторівна", topStudents[1].FullName);
            Assert.AreEqual(88.0, topStudents[2].RatingScore);
        }

        [TestMethod]
        public void CalculateAverageScoreByGroup_WhenGroupExists_ReturnsCorrectAverage()
        {
            double avg = _manager.CalculateAverageScoreByGroup("КН-22");

            Assert.AreEqual(91.75, avg, 0.001);
        }

        [TestMethod]
        public void CalculateAverageScoreByGroup_WhenGroupDoesNotExist_ReturnsZero()
        {
            double avg = _manager.CalculateAverageScoreByGroup("КН-32");

            Assert.AreEqual(0.0, avg);
        }

        [TestMethod]
        public void GetMinAndMaxRatingStudents_ReturnsCorrectExtremes()
        {
            var (minStudent, maxStudent) = _manager.GetMinAndMaxRatingStudents();

            Assert.IsNotNull(minStudent);
            Assert.IsNotNull(maxStudent);
            Assert.AreEqual(88.0, minStudent.RatingScore);
            Assert.AreEqual("Нікішина Валерія Матвіївна", maxStudent.FullName);
            Assert.AreEqual(95.5, maxStudent.RatingScore);
        }

        [TestMethod]
        public void FilterByExactScore_ReturnsAllStudentsWithMatchingScore()
        {
            var studentsWith88 = _manager.FilterByExactScore(88.0);

            Assert.AreEqual(2, studentsWith88.Count);
            CollectionAssert.Contains(studentsWith88, _student2);
            CollectionAssert.Contains(studentsWith88, _student4);
        }

        [TestMethod]
        public void SearchByName_WithFullMatch_ReturnsMatchingStudent()
        {
            var results = _manager.SearchByName("Валерія");

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Нікішина Валерія Матвіївна", results[0].FullName);
        }

        [TestMethod]
        public void SearchByName_CaseInsensitiveAndFragment_ReturnsCorrectList()
        {
            var results = _manager.SearchByName("ов");

            Assert.AreEqual(3, results.Count);
        }

        [TestMethod]
        public void SearchByName_WithEmptyTerm_ReturnsAllStudents()
        {
            var results = _manager.SearchByName("   ");

            Assert.AreEqual(4, results.Count);
        }

        [TestMethod]
        public void GetStudentCountPerGroup_ReturnsGroupNamesWithCorrectCounts()
        {

            var stats = _manager.GetStudentCountPerGroup();

            Assert.AreEqual(3, stats.Count);

            Assert.AreEqual("АнД-31", stats[0].GroupName);
            Assert.AreEqual(1, stats[0].StudentCount);

            Assert.AreEqual("КН-22", stats[1].GroupName);
            Assert.AreEqual(2, stats[1].StudentCount);

            Assert.AreEqual("ПП-21", stats[2].GroupName);
            Assert.AreEqual(1, stats[2].StudentCount);
        }
    }
}
