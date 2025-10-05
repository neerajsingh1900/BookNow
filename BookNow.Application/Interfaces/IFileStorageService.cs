using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary> Save stream and return the relative URL (e.g. /uploads/posters/abc.jpg) </summary>
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder);

        /// <summary> Delete a previously saved relative URL (no leading '/') if exists </summary>
        Task DeleteFileAsync(string relativeUrl);
    }
}
