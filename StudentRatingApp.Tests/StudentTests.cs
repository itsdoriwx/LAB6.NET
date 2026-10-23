namespace StudentRatingApp.Tests
{
    [TestClass]
    public sealed class StudentTests
    {
        [TestMethod]
        public void Constructor_WithValidArguments_CreatesStudentObject()  // тест конструктора (на створення об'єкта)
        {
            // Arrange
            string fullName = "Коваленко Тарас Михайлович";
            int course = 2;
            string group = "КН-11";
            double ratingScore = 95.5;

            // Act
            var student = new Student(fullName, course, group, ratingScore);

            // Assert
            Assert.AreEqual(fullName, student.FullName);
            Assert.AreEqual(course, student.Course);
            Assert.AreEqual(group, student.Group);
            Assert.AreEqual(ratingScore, student.RatingScore);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Constructor_WithInvalidFullName_ThrowsArgumentException(string invalidFullName)
        {
            Assert.ThrowsException<ArgumentException>(() => new Student(invalidFullName, 2, "КН-21", 85.0));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(5)]
        [DataRow(7)]
        public void Constructor_WithInvalidCourse_ThrowsArgumentException(int invalidCourse)
        {
            Assert.ThrowsException<ArgumentException>(() => new Student("Іван Петренко", invalidCourse, "КН-21", 85.0));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void Constructor_WithInvalidGroup_ThrowsArgumentException(string invalidGroup)
        {
            Assert.ThrowsException<ArgumentException>(() => new Student("Іван Петренко", 2, invalidGroup, 85.0));
        }

        [TestMethod]
        [DataRow(59.9)]
        [DataRow(-10.0)]
        [DataRow(100.1)]
        [DataRow(150.0)]
        public void Constructor_WithInvalidRatingScore_ThrowsArgumentException(double invalidScore)
        {
            Assert.ThrowsException<ArgumentException>(() => new Student("Іван Петренко", 2, "КН-21", invalidScore));
        }

        [TestMethod]
        public void CompareTo_WithOtherStudent_ReturnsCorrectValue()  // тест реалізації порівняння IComparable
        {
            var student1 = new Student("Студент 1", 2, "КН-21", 90.0);
            var student2 = new Student("Студент 2", 2, "КН-21", 80.0);
            var student3 = new Student("Студент 3", 2, "КН-21", 90.0);

            Assert.IsTrue(student1.CompareTo(student2) < 0, "Студент з вищим рейтингом має бути перед студентом з меншим.");
            Assert.IsTrue(student2.CompareTo(student1) > 0, "Студент з нижчим рейтингом має бути після студента з вищим.");
            Assert.AreEqual(0, student1.CompareTo(student3), "Студенти з однаковим рейтингом мають бути рівними.");
        }

        [TestMethod]
        public void CompareTo_WithNull_ReturnsPositiveValue()
        {
            var student = new Student("Іван", 1, "ПП-11", 80.0);
            Assert.IsTrue(student.CompareTo(null) > 0, "Порівняння об'єкта з null має повертати додатне значення.");
        }

        [TestMethod]
        public void ToString_ReturnsCorrectlyFormattedString()   // тест ToString()
        {
            var student = new Student("Говін Тарас Маркович", 2, "АнД-21", 95.5);
            string expectedString = $"{student.FullName}, {student.Course} курс, група {student.Group}, рейтинг: {student.RatingScore:F2}";
            string actualString = student.ToString();
            Assert.AreEqual(expectedString, actualString);
        }
    }
}
