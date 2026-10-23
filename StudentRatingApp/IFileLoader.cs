using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StudentRatingApp
{
    public interface IFileLoader
    {
            Task<List<T>?> LoadAsync<T>(string filePath);
    }
}
