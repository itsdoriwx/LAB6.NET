using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentRatingApp
{
    public class StudentDisplayItem
    {
        public Student Student { get; }
        public string FullName => Student.FullName;
        public int Course => Student.Course;
        public string Group => Student.Group;
        public double RatingScore => Student.RatingScore;

        public bool IsMaxRating { get; set; }
        public bool IsMinRating { get; set; }

        public StudentDisplayItem(Student student, bool isMax = false, bool isMin = false)
        {
            Student = student;
            IsMaxRating = isMax;
            IsMinRating = isMin;
        }
    }

    public partial class MainWindow : Window
    {
        private readonly StudentManager _manager;

        public MainWindow()
        {
            InitializeComponent();

            var fileReader = new JsonFileReader();
            var fileSaver = new JsonFileWriter();
            _manager = new StudentManager(fileReader, fileSaver);

            RefreshView(_manager.GetAllStudents());
        }

        private void RefreshView(IEnumerable<Student> students, string? activeBadgeText = null)
        {
            var displayItems = students.Select(s => new StudentDisplayItem(s)).ToList();
            LstStudents.ItemsSource = null;
            LstStudents.ItemsSource = displayItems;

            UpdateBadge(activeBadgeText);
        }

        private void RefreshViewWithCustomItems(IEnumerable<StudentDisplayItem> items, string? activeBadgeText = null)
        {
            LstStudents.ItemsSource = null;
            LstStudents.ItemsSource = items.ToList();

            UpdateBadge(activeBadgeText);
        }

        private void UpdateBadge(string? text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                TxtActiveViewInfo.Text = text;
                BorderActiveView.Visibility = Visibility.Visible;
            }
            else
            {
                BorderActiveView.Visibility = Visibility.Collapsed;
            }
        }

        private void ClearInputFields()
        {
            TxtFullName.Clear();
            TxtCourse.Clear();
            TxtGroup.Clear();
            TxtRating.Clear();
            TxtFullName.Focus();
        }

        private void BtnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fullName = TxtFullName.Text.Trim();
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    MessageBox.Show("ПІБ студента має бути вказаним.", "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtFullName.Focus();
                    return;
                }

                if (!int.TryParse(TxtCourse.Text.Trim(), out int course) || course < 1 || course > 4)
                {
                    MessageBox.Show("Курс має бути цілим значенням від 1 до 4.", "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtCourse.Focus();
                    return;
                }

                string group = TxtGroup.Text.Trim();
                if (string.IsNullOrWhiteSpace(group))
                {
                    MessageBox.Show("Назва групи має бути вказаною.", "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtGroup.Focus();
                    return;
                }

                string ratingInput = TxtRating.Text.Trim().Replace(',', '.');
                if (!double.TryParse(ratingInput, NumberStyles.Any, CultureInfo.InvariantCulture, out double ratingScore) || ratingScore < 60.0 || ratingScore > 100.0)
                {
                    MessageBox.Show("Рейтинговий бал має бути числом у межах від 60 до 100 балів.", "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtRating.Focus();
                    return;
                }

                var student = new Student(fullName, course, group, ratingScore);
                _manager.AddStudent(student);

                RefreshView(_manager.GetAllStudents());
                ClearInputFields();
            }
            catch (ArgumentException ex)
            {
                string cleanMsg = ex.Message.Split(new[] { " (Parameter" }, StringSplitOptions.None)[0];
                MessageBox.Show(cleanMsg, "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Помилка додавання", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudents.SelectedItem is StudentDisplayItem selectedItem)
            {
                var result = MessageBox.Show($"Ви дійсно бажаєте видалити студента:\n{selectedItem.FullName}?",
                    "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _manager.RemoveStudent(selectedItem.Student);
                    RefreshView(_manager.GetAllStudents());
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть студента зі списку для видалення.", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string term = TxtSearch.Text.Trim();
            var matches = _manager.SearchByName(term);
            RefreshView(matches, string.IsNullOrWhiteSpace(term) ? null : $"Пошук: {term}");
        }

        private void BtnSortByRating_Click(object sender, RoutedEventArgs e)
        {
            if (_manager.GetAllStudents().Count == 0)
            {
                MessageBox.Show("Список студентів порожній. Немає даних для сортування.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            RefreshView(_manager.SortByRating(), "Сортування: за рейтингом");
        }

        private void BtnSortByName_Click(object sender, RoutedEventArgs e)
        {
            if (_manager.GetAllStudents().Count == 0)
            {
                MessageBox.Show("Список студентів порожній. Немає даних для сортування.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            RefreshView(_manager.SortByName(), "Сортування: за алфавітом");
        }

        private void BtnTopThree_Click(object sender, RoutedEventArgs e)
        {
            if (_manager.GetAllStudents().Count == 0)
            {
                MessageBox.Show("Список студентів порожній.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var top = _manager.GetTopThreeStudents();
            RefreshView(top, "Топ-3 студенти");
        }

        private void BtnMinMax_Click(object sender, RoutedEventArgs e)
        {
            if (_manager.GetAllStudents().Count == 0)
            {
                MessageBox.Show("Список студентів порожній. Немає даних для аналізу.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var (min, max) = _manager.GetMinAndMaxRatingStudents();
            if (min == null || max == null)
            {
                return;
            }

            var items = new List<StudentDisplayItem>
            {
                new StudentDisplayItem(max, isMax: true),
                new StudentDisplayItem(min, isMin: true)
            };

            RefreshViewWithCustomItems(items, "Мінімум та максимум");
        }

        private void BtnAverageByGroup_Click(object sender, RoutedEventArgs e)
        {
            if (_manager.GetAllStudents().Count == 0)
            {
                MessageBox.Show("Список студентів порожній.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string group = TxtGroupInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(group))
            {
                MessageBox.Show("Введіть назву академічної групи у відповідне поле.", "Підказка", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtGroupInput.Focus();
                return;
            }

            double avg = _manager.CalculateAverageScoreByGroup(group);
            if (avg == 0.0)
            {
                MessageBox.Show($"Групу «{group}» не знайдено серед збережених студентів.", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Середній рейтинговий бал групи «{group}» = {avg:F2} балів.", "Розрахунки", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnFilterScore_Click(object sender, RoutedEventArgs e)
        {
            if (_manager.GetAllStudents().Count == 0)
            {
                MessageBox.Show("Список студентів порожній.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string scoreStr = TxtScoreInput.Text.Trim().Replace(',', '.');
            if (!double.TryParse(scoreStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double targetScore) || targetScore < 60.0 || targetScore > 100.0)
            {
                MessageBox.Show("Введіть коректне числове значення рейтингового балу в межах від 60 до 100 балів.", "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtScoreInput.Focus();
                return;
            }

            var matches = _manager.FilterByExactScore(targetScore);
            if (matches.Count == 0)
            {
                MessageBox.Show($"Студентів із рейтинговим балом {targetScore:F2} не знайдено.", "Результат пошуку", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            RefreshView(matches, $"Бал = {targetScore:F2}");
        }

        private void BtnGroupStats_Click(object sender, RoutedEventArgs e)
        {
            if (_manager.GetAllStudents().Count == 0)
            {
                MessageBox.Show("Список студентів порожній.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var stats = _manager.GetStudentCountPerGroup();
            string report = "Кількість студентів за академічними групами:\n\n" +
                            string.Join("\n", stats.Select(s => $"Група {s.GroupName}: {s.StudentCount} студент(ів)"));

            MessageBox.Show(report, "Статистика по групах", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnResetView_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Clear();
            TxtGroupInput.Clear();
            TxtScoreInput.Clear();
            RefreshView(_manager.GetAllStudents());
        }

        private async void BtnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_manager.GetAllStudents().Count == 0)
                {
                    throw new InvalidOperationException("Список студентів порожній. Немає даних для збереження.");
                }

                var saveDialog = new SaveFileDialog
                {
                    Title = "Збереження рейтингового списку",
                    Filter = "Файли JSON (*.json)|*.json|Усі файли (*.*)|*.*",
                    DefaultExt = ".json",
                    FileName = "students.json"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    await _manager.SaveToFileAsync(saveDialog.FileName);
                    MessageBox.Show("Дані успішно збережено у файл!", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося зберегти файл:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Вибір файлу для завантаження",
                Filter = "Файли JSON (*.json)|*.json|Усі файли (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    await _manager.LoadFromFileAsync(openDialog.FileName);
                    BtnResetView_Click(sender, e);
                    MessageBox.Show("Дані успішно завантажено!", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося завантажити файл:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}