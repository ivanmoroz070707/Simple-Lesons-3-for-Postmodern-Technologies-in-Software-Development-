using NUnit.Framework;
using System;
using System.IO;

namespace my_head.Tests
{
    public class AppTests
    {
        [Test]
        public void App_ReadsFromStdin_WhenNoFileProvided()
        {
            var input = new StringReader("line1\nline2\nline3\nline4\n");
            var output = new StringWriter();
            var error = new StringWriter();

            int exitCode = App.Run(new[] { "-n", "2" }, input, output, error);

            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(output.ToString(), Is.EqualTo("line1\r\nline2\r\n").Or.EqualTo("line1\nline2\n"));
            Assert.That(error.ToString(), Is.Empty);
        }

        [Test]
        public void App_UnknownOption_ReturnsExitCode2()
        {
            var input = new StringReader("");
            var output = new StringWriter();
            var error = new StringWriter();

            int exitCode = App.Run(new[] { "--unknown" }, input, output, error);

            Assert.That(exitCode, Is.EqualTo(2));
            Assert.That(output.ToString(), Is.Empty);
            Assert.That(error.ToString(), Does.Contain("Unknown option").IgnoreCase);
        }

        [Test]
        public void App_InvalidNumberForN_ReturnsExitCode2()
        {
            var input = new StringReader("");
            var output = new StringWriter();
            var error = new StringWriter();

            int exitCode = App.Run(new[] { "-n", "abc" }, input, output, error);

            Assert.That(exitCode, Is.EqualTo(2));
            Assert.That(output.ToString(), Is.Empty);
            Assert.That(error.ToString(), Does.Contain("Invalid").IgnoreCase);
        }

        [Test]
        public void App_FileNotFound_ReturnsExitCode1()
        {
            var input = new StringReader("");
            var output = new StringWriter();
            var error = new StringWriter();

            int exitCode = App.Run(new[] { "nonexistent_file.txt" }, input, output, error);

            Assert.That(exitCode, Is.EqualTo(1));
            Assert.That(output.ToString(), Is.Empty);
            Assert.That(error.ToString(), Does.Contain("not found").IgnoreCase);
        }
        [Test]
        public void App_ReadsFromFile_Successfully()
        {
            // 1. Тест, який читає з файлу
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, "file_line1\nfile_line2\n");

            var input = new StringReader(""); // Порожній stdin
            var output = new StringWriter();
            var error = new StringWriter();

            try
            {
                int exitCode = App.Run(new[] { tempFilePath }, input, output, error);

                Assert.That(exitCode, Is.EqualTo(0));
                Assert.That(output.ToString(), Does.Contain("file_line1"));
                Assert.That(error.ToString(), Is.Empty);
            }
            finally
            {
                
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            }
        }

        [Test]
        public void App_PrioritizesFile_OverStdin()
        {
            
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, "data_from_file\n");

            var input = new StringReader("data_from_stdin\n"); 
            var output = new StringWriter();
            var error = new StringWriter();

            try
            {
                int exitCode = App.Run(new[] { tempFilePath }, input, output, error);

                Assert.That(exitCode, Is.EqualTo(0));
                // Перевіряємо, що вивелися дані з файлу, а не з stdin
                Assert.That(output.ToString(), Does.Contain("data_from_file"));
                Assert.That(output.ToString(), Does.Not.Contain("data_from_stdin"));
            }
            finally
            {
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            }
        }

        [Test]
        public void App_OutputsTenLines_ByDefault()
        {
            
            var sb = new System.Text.StringBuilder();
            for (int i = 1; i <= 15; i++)
            {
                sb.AppendLine($"line{i}");
            }

            var input = new StringReader(sb.ToString());
            var output = new StringWriter();
            var error = new StringWriter();

            
            int exitCode = App.Run(Array.Empty<string>(), input, output, error);

            Assert.That(exitCode, Is.EqualTo(0));

            
            var outputLines = output.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            Assert.That(outputLines.Length, Is.EqualTo(10)); 
            Assert.That(outputLines[0], Is.EqualTo("line1")); 
            Assert.That(outputLines[9], Is.EqualTo("line10")); 
        }
    }
}