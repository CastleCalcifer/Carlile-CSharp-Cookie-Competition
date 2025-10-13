using Carlile_Cookie_Competition.Models;
using FluentAssertions;
using Xunit;

namespace Carlile_Cookie_Competition.Tests.Models
{
    public class YearRecordTests
    {
        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            // Act
            var yearRecord = new YearRecord();

            // Assert
            yearRecord.Id.Should().Be(0);
            yearRecord.YearNumber.Should().Be(0);
            yearRecord.ResultsViewable.Should().BeFalse();
        }

        [Theory]
        [InlineData(2020)]
        [InlineData(2024)]
        [InlineData(2030)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void YearNumber_ShouldAcceptVariousValues(int yearNumber)
        {
            // Arrange
            var yearRecord = new YearRecord();

            // Act
            yearRecord.YearNumber = yearNumber;

            // Assert
            yearRecord.YearNumber.Should().Be(yearNumber);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ResultsViewable_ShouldAcceptBooleanValues(bool resultsViewable)
        {
            // Arrange
            var yearRecord = new YearRecord();

            // Act
            yearRecord.ResultsViewable = resultsViewable;

            // Assert
            yearRecord.ResultsViewable.Should().Be(resultsViewable);
        }

        [Fact]
        public void Id_ShouldBeSettable()
        {
            // Arrange
            var yearRecord = new YearRecord();
            var id = 123;

            // Act
            yearRecord.Id = id;

            // Assert
            yearRecord.Id.Should().Be(id);
        }
    }
}
