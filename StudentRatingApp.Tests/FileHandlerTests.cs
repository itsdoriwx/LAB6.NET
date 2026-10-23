using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentRatingApp.Tests
{
    [TestClass]
    public class FileHandlerTests
    {
        private string _testFilePath = string.Empty;

        [TestInitialize]
        public void TestInitialize()  // метод перед кожним тестом
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
        }

        [TestCleanup]
        public void TestCleanup()    // метод після кожного тесту
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestMethod]
        public async Task SaveAsync_ShouldWriteDataToFile()
        {
            var students = new List<Student>
            {
                new Student("Нікішина Валерія Матвіївна", 2, "КН-21", 95.5),
                new Student("Потоцька Олеся Олександрівна", 2, "КН-21", 88.0)
            };

            var fileSaver = new JsonFileWriter();
            await fileSaver.SaveAsync(_testFilePath, students);

            Assert.IsTrue(File.Exists(_testFilePath), "Файл не було створено.");
            string fileContent = await File.ReadAllTextAsync(_testFilePath);
            Assert.IsFalse(string.IsNullOrWhiteSpace(fileContent), "Файл порожній.");

            var fileReader = new JsonFileReader();   // чи можна прочитати файл назад
            var loadedStudents = await fileReader.LoadAsync<Student>(_testFilePath);

            Assert.IsNotNull(loadedStudents, "Десеріалізація не вдалася.");
            Assert.AreEqual(2, loadedStudents.Count, "Кількість десеріалізованих об'єктів не збігається.");
        }

        [TestMethod]
        public async Task LoadAsync_ShouldReadDataFromFile()
        {
            var jsonContent = "[{\"FullName\":\"Нікішина Валерія Матвіївна\",\"Course\":2,\"Group\":\"КН-21\",\"RatingScore\":95.5}]";
            await File.WriteAllTextAsync(_testFilePath, jsonContent);

            var fileReader = new JsonFileReader();
            var loadedStudents = await fileReader.LoadAsync<Student>(_testFilePath);

            Assert.IsNotNull(loadedStudents);
            Assert.AreEqual(1, loadedStudents.Count);
            Assert.AreEqual("Нікішина Валерія Матвіївна", loadedStudents[0].FullName);
            Assert.AreEqual(95.5, loadedStudents[0].RatingScore);
        }

        [TestMethod]
        public async Task LoadAsync_WhenFileDoesNotExist_ReturnsNull()
        {
            var fileReader = new JsonFileReader();
            var loadedStudents = await fileReader.LoadAsync<Student>(_testFilePath);

            Assert.IsNull(loadedStudents, "Повинен повертати null, якщо файл не існує.");
        }
    }
}
