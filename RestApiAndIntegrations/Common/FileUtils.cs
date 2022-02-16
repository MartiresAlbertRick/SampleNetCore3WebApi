using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AD.CAAPS.Common
{
    public static class FileUtils
    {
        public static void IsDirectoryNotExistThrowAnException(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"Unable to locate directory {directory}");
            }
        }

        public static void IsFileNotExistThrowAnException(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"Unable to locate file {filename}");
            }
        }

        public static bool IsCsvFile(string filename)
        {
            Utils.CheckStringIsNullOrWhiteSpaceThrowException(filename, () => new ArgumentException($"No value assigned for {filename}"));
            return Path.GetExtension(filename).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) || Path.GetExtension(filename).Equals(".txt", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsExcelFile(string filename)
        {
            Utils.CheckStringIsNullOrWhiteSpaceThrowException(filename, () => new ArgumentException($"No value assigned for {filename}"));
            if (Path.GetExtension(filename).Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else if (Path.GetExtension(filename).Equals(".xls", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;
        }

        public static bool MatchCurrentDateAndFileCreationDate(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return file.CreationTime.Date.CompareTo(DateTime.Today.Date) == 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        public static bool MatchCurrentDateAndFileNameDate(FileInfo file, string fileNamePattern, string dateTimePattern)
        {
            Utils.CheckObjectIsNullThrowException(file, () => new ArgumentNullException(nameof(file)));
            string regexResult = Regex.Match(file.Name, @fileNamePattern).Groups[1].Value;
            if (!Utils.DateTimeTryParseExact(regexResult, dateTimePattern, out DateTime dateTimeFromResult))
                throw new FormatException($"File datetime stamp {regexResult} did not matched with pattern {dateTimePattern}");

            return DateTime.Compare(dateTimeFromResult.Date, DateTime.Today.Date) == 0;
        }

        public static void EnsureDirectoryExists(string directory)
        {
            CheckPathNotExistThenCreate(directory);
        }

        public static async Task SaveJSONToFile(string filePath, string jsonSerialized)
        {
            EnsureDirectoryExists(Path.GetDirectoryName(filePath));
            using var file = new StreamWriter(filePath);
            await file.WriteAsync(jsonSerialized).ConfigureAwait(false);
            return;
        }

        public static string CheckPathNotExistThenCreate(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static long GetFileSize(string filePath)
        {
            return new FileInfo(filePath).Length;
        }

        public static async Task DeleteFile(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
                if (File.Exists(filePath))
                    await Task.Run(() => { File.Delete(filePath); }).ConfigureAwait(false);
        }

        public static async Task MoveFile(string sourceFile, string destinationFile)
        {
            try
            {
                await Task.Run(() => { File.Move(sourceFile, destinationFile); }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new MoveFileException($"Unable to move a file from \"{sourceFile}\" to from configuration for \"{sourceFile}\".", e);
            }

        }
    }
}