namespace FizzBuzz.Tests;

public class TwistedFizzBuzzTests
{
    private readonly TwistedFizzBuzz.TwistedFizzBuzz _sut = new ();

    [Fact]
    public void GetRangeOutput_ReturnsFizzBuzzOutput_ForSimpleRange()
    {
        var result = _sut.GetRangeOutput(0, 50).ToArray();

        result.Count().Should().Be(51);
        result[1].Should().Be("1");
        result[3].Should().Be("Fizz");
        result[10].Should().Be("Buzz");
        result[11].Should().Be("11");
        result[15].Should().Be("FizzBuzz");
        result[0].Should().Be("FizzBuzz");
    }

    [Fact]
    public void GetRangeOutput_Handles_LargeRange()
    {
        var result = _sut.GetRangeOutput(1, 2000000000);

        result.Count().Should().Be(2000000000);
        result.Last().Should().Be("Buzz");
    }

    [Fact]
    public void GetRangeOutput_Handles_DescendingInput()
    {
        var result = _sut.GetRangeOutput(-2, -37).ToArray();

        result.Count().Should().Be(36);
        result[0].Should().Be("-2");
        result[1].Should().Be("Fizz");
        result.Last().Should().Be("-37");
    }

    [Fact]
    public void GetSetOutput_ReturnsFizzBuzzOutput_InCorrectOrder()
    {
        var input = new [] { -5, 6, 300, 12, 15, 4 };

        var result = _sut.GetSetOutput(input).ToArray();

        result.Count().Should().Be(6);
        result[0].Should().Be("Buzz");
        result[1].Should().Be("Fizz");
        result[2].Should().Be("FizzBuzz");
        result[3].Should().Be("Fizz");
        result[4].Should().Be("FizzBuzz");
        result[5].Should().Be("4");
    }

    [Fact]
    public void SetRules_ResultsInDifferentOutputs()
    {
        var rules = new [] 
        {
            new FizzBuzzRule { Multiple = 7, Word = "Poem" },
            new FizzBuzzRule { Multiple = 17, Word = "Writer" },
            new FizzBuzzRule { Multiple = 3, Word = "College" }
        };

        _sut.SetRules(rules);
        var rangeResult = _sut.GetRangeOutput(357, 357);
        var setResult = _sut.GetSetOutput(new [] { 119, 51, 21, 357 }).ToArray();

        rangeResult.Single().Should().Be("PoemWriterCollege");
        setResult.Should().HaveCount(4);
        setResult[0].Should().Be("PoemWriter");
        setResult[1].Should().Be("WriterCollege");
        setResult[2].Should().Be("PoemCollege");
        setResult[3].Should().Be("PoemWriterCollege");
    }

    [Fact]
    public void SetRules_HandlesInvalidJson()
    {
        var json = "{\"multiple\":\"cheese\",\"word\":\"music\"}";

        var result = _sut.SetRules(json);

        result.Should().BeFalse();
    }

    [Fact]
    public void SetRules_ResultsInDifferentOutputs_IfJsonValid()
    {
        var json = "{\"multiple\":4,\"word\":\"music\"}";

        var result = _sut.SetRules(json);
        var setResult = _sut.GetSetOutput(new [] { 4 });

        result.Should().BeTrue();
        setResult.Single().Should().Be("music");
    }
}