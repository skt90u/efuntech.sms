using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using ICSharpCode.NRefactory.CSharp;
using JSBeautifyLib;
using EFunTech.Sms.Portal;

namespace EFunTech.Sms.CodeGenerator
{
    public static class Utils
    {
        private static ILogService logService;

        static Utils()
        {
            logService = new SmartInspectLogService();
        }

        public static void HtmlToTemplate(string name, string inputPath, string outputPath)
        {
            var list = new List<string>();

            var lines = File.ReadAllLines(inputPath);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var prefix = "            ";

                string lineTrimStart = line.TrimStart();
                string lineTrimStartToken = line.Replace(lineTrimStart, string.Empty);

                list.Add(string.Format("{0}{1}'{2}',", prefix, lineTrimStartToken, lineTrimStart));
                
            }
            File.WriteAllLines(outputPath, list);
        }
        public static void MakeSureDirExist(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static void SearchDir(string dir, List<string> paths)
        {
            paths.AddRange(Directory.GetFiles(dir, "*.js"));
            foreach (string subdir in Directory.GetDirectories(dir))
                SearchDir(subdir, paths);
        }

        public static void WriteToCSharpFile(string filePath, List<string> lines, bool overwrite)
        {
            Utils.MakeSureDirExist(Path.GetDirectoryName(filePath));

            if (File.Exists(filePath))
            {
                if (!overwrite)
                {
                    logService.Error("檔案已存在 - {0}", filePath);
                    return;
                }

                if (overwrite)
                {
                    BackupFile(filePath);
                }
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);

            logService.Debug("寫入檔案 - {0}", filePath);

            FormattCSharpCode(filePath);
        }

        public static void AppenAddLineToFile(string inputPath, string outputPath)
        {
            var outputContent = new List<string>();

            var lines = File.ReadAllLines(inputPath, Encoding.UTF8);
            foreach (var line in lines)
            {
                string str = line.Replace("{", "{{")
                    .Replace("}", "}}")
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");

                // lines.Add(string.Format(""));
                outputContent.Add("            lines.Add(string.Format(\"" + str + "\"));");
            }

            MakeSureDirExist(Path.GetDirectoryName(outputPath));
            File.WriteAllLines(outputPath, outputContent, Encoding.UTF8);
        }

        public static void FormattCSharpCode(string filePath)
        {
            FormattCSharpCode(filePath, filePath);
        }

        public static void FormattCSharpCode(string inputPath, string outputPath)
        {
            string code = File.ReadAllText(inputPath, Encoding.UTF8);

            code = new CSharpFormatter(FormattingOptionsFactory.CreateAllman()).Format(code);

            File.WriteAllText(outputPath, code, Encoding.UTF8);
        }

        public static void FormattJsCode(string filePath)
        {
            FormattJsCode(filePath, filePath);
        }

        public static void FormattJsCode(string inputPath, string outputPath)
        {
            string code = File.ReadAllText(inputPath, Encoding.UTF8);

            var options = new JSBeautifyOptions();
            options.indent_size = 4;
            options.indent_char = ' ';
            options.indent_level = 0;
            options.preserve_newlines = true;
            var jsbeautify = new JSBeautify(code, options);

            code = jsbeautify.GetResult();

            File.WriteAllText(outputPath, code, Encoding.UTF8);
        }

        public static void WriteToJavascriptFile(string filePath, List<string> lines, bool overwrite, bool bFormattJsCode, bool bBackupFile)
        {
            Utils.MakeSureDirExist(Path.GetDirectoryName(filePath));

            if (File.Exists(filePath))
            {
                if (!overwrite)
                {
                    logService.Error("檔案已存在 - {0}", filePath);
                    return;
                }

                if (overwrite)
                {
                    BackupFile(filePath);
                }
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);

            logService.Debug("寫入檔案 - {0}", filePath);

            if (bFormattJsCode)
                FormattJsCode(filePath);
        }
        public static void WriteToJavascriptFile(string filePath, List<string> lines, bool overwrite)
        {
            WriteToJavascriptFile(filePath, lines, overwrite, true, true);
        }

        private static void BackupFile(string filePath)
        {
            string backupPath = string.Format("{0}.{1}", filePath, DateTime.UtcNow.ToString("yyyyMMdd_hhmmss"));
            File.Copy(filePath, backupPath);
        }

        public static void WriteToCshtmlFile(string filePath, List<string> lines, bool overwrite)
        {
            Utils.MakeSureDirExist(Path.GetDirectoryName(filePath));

            if (File.Exists(filePath))
            {
                if (!overwrite)
                {
                    logService.Error("檔案已存在 - {0}", filePath);
                    return;
                }

                if (overwrite)
                {
                    BackupFile(filePath);
                }
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);

            logService.Debug("寫入檔案 - {0}", filePath);
        }

        public static void OpenFolder(string outputDir)
        {
            var process = new System.Diagnostics.Process();

            process.StartInfo.UseShellExecute = true;

            process.StartInfo.FileName = @"explorer";

            process.StartInfo.Arguments = string.Format("\"{0}\"", outputDir); ;

            process.Start();
        }
    }
}
