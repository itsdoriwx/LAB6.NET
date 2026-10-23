using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentRatingApp
{
    public interface IFileSaver
    {
        Task SaveAsync<T>(string filePath, List<T> data); // таск = асинхронний метод, який зберігає дані у файл
    }
}
