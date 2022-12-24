using FluentValidation;
using FluentValidation.Results;

namespace Finance.Domain.GoogleSheets.Models;

public interface IValidator<T>
{
    public ValidationResult Validate(T candidate);
}

public class LedgerRowValidator : AbstractValidator<IList<object>>, IValidator<IList<object>>
{
    private string Missing(string field) => $"{field} is missing";

    public LedgerRowValidator()
    {
        RuleFor(row => row)
            .NotEmpty()
            .WithMessage("Input is empty");

        RuleFor(row => row[0])
            .NotNull()
            .WithMessage(Missing(nameof(LedgerInputRow.Date)))
            .NotEmpty()
            .WithMessage(Missing(nameof(LedgerInputRow.Date)));

        RuleFor(row => row[1])
            .NotNull()
            .WithMessage(Missing(nameof(LedgerInputRow.Currency)))
            .Must(obj => !string.IsNullOrWhiteSpace(obj.ToString()))
            .WithMessage(Missing(nameof(LedgerInputRow.Currency)));

        RuleFor(row => row[2])
            .NotNull()
            .WithMessage(Missing(nameof(LedgerInputRow.Portfolio)))
            .Must(obj => !string.IsNullOrWhiteSpace(obj.ToString()))
            .WithMessage(Missing(nameof(LedgerInputRow.Portfolio)));

        RuleFor(row => row[3])
            .NotNull()
            .WithMessage(Missing(nameof(LedgerInputRow.Code)))
            .Must(obj => !string.IsNullOrWhiteSpace(obj.ToString()))
            .WithMessage(Missing(nameof(LedgerInputRow.Code)));

        RuleFor(row => row[4])
            .NotNull()
            .WithMessage(Missing(nameof(LedgerInputRow.Consideration)))
            .NotEmpty()
            .WithMessage(Missing(nameof(LedgerInputRow.Consideration)));
    }
}

public class LedgerDataRowValidator : LedgerRowValidator
{
    private string Invalid(string field, string value) => $"{field} value '{value}' is invalid";

    public LedgerDataRowValidator()
    {
        RuleFor(row => row[0])
            .Must(obj => DateOnly.TryParse(obj.ToString(), out _))
            .WithMessage(obj => Invalid(nameof(LedgerInputRow.Date), $"{obj[0]}"));

        RuleFor(row => row[4])
            .Must(obj => decimal.TryParse(obj.ToString(), out _))
            .WithMessage(obj => Invalid(nameof(LedgerInputRow.Consideration), $"{obj[4]}"));

        RuleFor(row => row[5])
            .Must(obj => int.TryParse(obj.ToString(), out _))
            .WithMessage(obj => Invalid(nameof(LedgerInputRow.Units), $"{obj[5]}"));
    }
}

public class LedgerHeaderRowValidator : AbstractValidator<IList<object>>, IValidator<IList<object>>
{
    public LedgerHeaderRowValidator()
    {
        RuleFor(row => row)
            .Must(row => IsValid(row))
            .WithMessage("Header row invalid");
    }

    private bool IsValid(IList<object> input)
    {
        if (input.Count < LedgerInputRow.HeaderRow.Length)
            return false;
        
        return input.Take(LedgerInputRow.HeaderRow.Length)
            .Select(i => i.ToString())
            .SequenceEqual(LedgerInputRow.HeaderRow);
    }
}

public class LedgerValidator : AbstractValidator<LedgerCandidate>, IValidator<LedgerCandidate>
{
    public LedgerValidator()
    {
        RuleFor(candidate => candidate)
            .Must(lc => !IsEmpty(lc))
            .WithMessage("Ledger is empty");

        RuleFor(candidate => candidate.HeaderRow)
            .Cascade(CascadeMode.Continue)
            .SetValidator(new LedgerHeaderRowValidator());
            
        RuleForEach(candidate => candidate.DataRows)
            .Cascade(CascadeMode.Continue)
            .SetValidator(new LedgerDataRowValidator());
    }

    private bool IsEmpty(LedgerCandidate lc) =>
        (lc.HeaderRow == null || !lc.HeaderRow.Any())
        && (lc.DataRows == null || !lc.DataRows.Any());
}

public class LedgerCandidate
{
    public IList<object> HeaderRow { get; set;}
    public IList<IList<object>> DataRows { get; set;}
}
