using FluentAssertions;
using MetaEdit;
using MetaEdit.Conventions;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace MetaEditTests
{
    public class FileOperationsTests : TestFixture<FileOperations>
    {
        private const string FileName = "Test String";
        private const string FileExt = "amr";

        public FileOperationsTests()
        {
            Inject(new TotalRecallConvention() as IDecodeConvention);
        }

        [Fact]
        public void Sut_ImplementsContract()
        {
            (Sut is IFileOperations).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abc")]
        public void TryExtractFileName_ReturnsFalse_IfInputContainsNoSlashes(string input)
        {
            // Act
            var result = Sut.TryExtractFileName(input, out var _);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> DirectorySeparatorTestData => new List<object[]>
            {
                new object[] { $"{Path.DirectorySeparatorChar}" } ,
                new object[] { $"abc{Path.DirectorySeparatorChar}" } ,
                new object[] { $"{Path.DirectorySeparatorChar}def" } ,
                new object[] { $"abc{Path.DirectorySeparatorChar}def" }
            };

        [Theory]
        [MemberData(nameof(DirectorySeparatorTestData))]
        public void TryExtractFileName_ReturnsFalse_IfInputDoesNotContainAmrFileExtension(string input)
        {
            // Act
            var result = Sut.TryExtractFileName(input, out var _);

            // Assert
            result.Should().BeFalse();
        }

        public static IEnumerable<object[]> ValidPathTestData => new List<object[]>
            {
                new object[] { $".{Path.DirectorySeparatorChar}{FileName}.{FileExt}" } ,
                new object[] { $".{Path.DirectorySeparatorChar}pathto{Path.DirectorySeparatorChar}{FileName}.{FileExt}" }
            };

        [Theory]
        [MemberData(nameof(ValidPathTestData))]
        public void TryExtractFileName_ReturnsTrueWithOutput_IfInputAsExpected(string input)
        {
            // Act
            var result = Sut.TryExtractFileName(input, out var file);

            // Assert
            result.Should().BeTrue();
            file.Should().Be($"{FileName}.{FileExt}");
        }
    }
}