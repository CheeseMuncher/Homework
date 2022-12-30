using System.Text.RegularExpressions;
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

        RuleFor(row => row)
            .Must(row => ValidateRow(row, 0))
            .WithMessage(Missing(nameof(LedgerInputRow.Date)));

        RuleFor(row => row)
            .Must(row => ValidateRow(row, 1))
            .WithMessage(Missing(nameof(LedgerInputRow.Currency)));

        RuleFor(row => row)
            .Must(row => ValidateRow(row, 2))
            .WithMessage(Missing(nameof(LedgerInputRow.Portfolio)));

        RuleFor(row => row)
            .Must(row => ValidateRow(row, 3))
            .WithMessage(Missing(nameof(LedgerInputRow.Code)));

        RuleFor(row => row)
            .Must(row => ValidateRow(row, 4))
            .WithMessage(Missing(nameof(LedgerInputRow.Consideration)));
    }

    private bool ValidateRow(IList<object> row, int index)
    {
        if (row.Count < index + 1)
            return false;

        return !string.IsNullOrWhiteSpace(row[index]?.ToString());
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

        RuleFor(row => row)
            .Must(row => ValidUnits(row))
            .WithMessage(row => Invalid(nameof(LedgerInputRow.Units), $"{row[5]}"));
    }

    private bool ValidUnits(IList<object> row)
    {
        if (row.Count < 6)
            return true;

        return int.TryParse(row[5].ToString(), out _);
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

public class LedgerValidator : PrefixErrorMessageValidator<LedgerCandidate>, IValidator<LedgerCandidate>
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

public abstract class PrefixErrorMessageValidator<T> : AbstractValidator<T>
{
    public override ValidationResult Validate(ValidationContext<T> context)
    {
        var result = base.Validate(context);
        PrefixErrorMessages(result);
        return result;
    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = default(CancellationToken))
    {
        var result = await base.ValidateAsync(context, cancellation);
        PrefixErrorMessages(result);
        return result;
    }

    protected void PrefixErrorMessages(ValidationResult result)
    {
        if (result.Errors?.Any() ?? false)
            foreach (var error in result.Errors)
                if (Regex.IsMatch(error.PropertyName, @"\[\d+\]"))
                    error.ErrorMessage = $"{error.PropertyName} error: {error.ErrorMessage}";
    }
}

public class LedgerCandidate
{
    public IList<object> HeaderRow { get; set;}
    public IList<IList<object>> DataRows { get; set;}
}