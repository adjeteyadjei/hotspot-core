using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Hotvenues.Helpers
{
    public class ImageHelpers
    {
        private readonly string _rootPath;
        public ImageHelpers(string rootPath)
        {
            _rootPath = rootPath;
        }

        public void SaveDataImage(string imgData, string fileName)
        {
            var folder = Path.Combine(_rootPath, "App_Data", "Images");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, $"{fileName}.png");
            var base64Data = Regex.Match(imgData, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);
            if (File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllBytes(filePath, binData);
        }

        public string SaveDataImage(string imgData)
        {
            //Images Folder
            if (string.IsNullOrEmpty(imgData)) return "";
            var folder = Path.Combine(_rootPath, "App_Data", "Images");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var fileName = $"gi-{GeneralHelpers.TokenCode(10)}";
            var filePath = Path.Combine(folder, $"{fileName}.png");
            var base64Data = Regex.Match(imgData, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);
            if (File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllBytes(filePath, binData);
            return fileName;
        }

        public FileStream GetImage(string fileName)
        {
            var folder = Path.Combine(_rootPath, "App_Data", "Images");
            var path = Path.Combine(folder, $"{fileName}.png");
            return new FileStream(path, FileMode.Open);
        }

    }
}