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
    }
}