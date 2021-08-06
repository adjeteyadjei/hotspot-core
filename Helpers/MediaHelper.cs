using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Hotvenues.Helpers
{
    public class MediaHelper
    { 
        private readonly string _rootPath;
        public MediaHelper(string rootPath)
        {
            _rootPath = rootPath;
        }

        public void SaveDataMedia(string imgData, string fileName)
        {
            var folder = Path.Combine(_rootPath, "App_Data", "Media");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            
            var base64Data = Regex.Match(imgData, @"data:(?<type>)/(?<extension>.+?),(?<data>.+)");
            var binData = Convert.FromBase64String(base64Data.Groups["data"].Value);

            var extension = base64Data.Groups["extension"].Value;
            var filePath = Path.Combine(folder, $"{fileName}.{extension}");
            if (File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllBytes(filePath, binData);
        }

        public Tuple<string, string> SaveDataMedia(string imgData)
        {
            //Medias Folder
            if (string.IsNullOrEmpty(imgData)) return new Tuple<string, string>("", "");
            var folder = Path.Combine(_rootPath, "App_Data", "Media");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);


            var base64Data = Regex.Match(imgData, @"data:(?<type>.+?);base64,(?<data>.+)");
            var binData = Convert.FromBase64String(base64Data.Groups["data"].Value);

            var extension = base64Data.Groups["type"].Value;

            var fileName = $"gi-{GeneralHelpers.TokenCode(10)}.{extension.Split('/')[1]}";
            var filePath = Path.Combine(folder, $"{fileName}");

            if (File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllBytes(filePath, binData);
            return new Tuple<string, string>(fileName, extension);
        }

        public FileStream GetMedia(string fileName)
        {
            var folder = Path.Combine(_rootPath, "App_Data", "Media");
            var path = Path.Combine(folder, $"{fileName}");
            return new FileStream(path, FileMode.Open);
        }

    }
}