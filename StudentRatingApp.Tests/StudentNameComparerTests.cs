using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentRatingApp.Tests
{
    [TestClass]
    public class StudentNameComparerTests
    {
        [TestMethod]
        public void Compare_WhenNamesAreDifferent_ReturnsCorrectOrder()   // тестування порівняння імен
        {
            var student1 = new Student("Андрій Шевченко", 2, "ПП-21", 85.0);
            var student2 = new Student("Богдан Коновалець", 2, "ПП-21", 90.0);
            var comparer = new StudentNameComparer();

            Assert.IsTrue(comparer.Compare(student1, student2) < 0);
            Assert.IsTrue(comparer.Compare(student2, student1) > 0);
        }

        [TestMethod]
        public void Compare_WhenNamesAreIdentical_ReturnsZero()
        {
            var student1 = new Student("Іваненко Іван Іванович", 1, "КН-11", 75.0);
            var student2 = new Student("Іваненко Іван Іванович", 3, "КН-31", 92.0);
            var comparer = new StudentNameComparer();

            Assert.AreEqual(0, comparer.Compare(student1, student2), "Студенти з однаковими іменами мають бути рівними.");
        }

        [TestMethod]
        public void Compare_WhenNamesHaveDifferentCase_ReturnsZero()
        {
            var student1 = new Student("леся миколайчук", 2, "АнД-21", 88.0);
            var student2 = new Student("Леся Миколайчук", 2, "АнД-21", 88.0);
            var comparer = new StudentNameComparer();

            Assert.AreEqual(0, comparer.Compare(student1, student2), "Порівняння має бути нечутливим до регістру.");
        }

        [TestMethod]
        public void Compare_WhenFirstStudentIsNull_ReturnsZero()
        {
            var student2 = new Student("Олександр Довженко", 2, "АнД-21", 80.0);
            var comparer = new StudentNameComparer();

            Assert.AreEqual(0, comparer.Compare(null, student2), "Порівняння null-об'єкта з іншим має повертати 0.");
        }

        [TestMethod]
        public void Compare_WhenSecondStudentIsNull_ReturnsZero()
        {
            var student1 = new Student("Олександр Довженко", 2, "АнД-21", 80.0);
            var comparer = new StudentNameComparer();

            Assert.AreEqual(0, comparer.Compare(student1, null), "Порівняння об'єкта з null-об'єктом має повертати 0.");
        }

        [TestMethod]
        public void Compare_WhenBothStudentsAreNull_ReturnsZero()
        {
            var comparer = new StudentNameComparer();

            Assert.AreEqual(0, comparer.Compare(null, null), "Порівняння двох null-об'єктів має повертати 0.");
        }
    }
}
