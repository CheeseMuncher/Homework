using System;
using Finance.Domain.Models;

namespace Finance.Tests.LedgerRowTests;

public class LedgerRowBuilder : TestFixture
{
    public LedgerRowBuilder()
    {
        date = Create<DateOnly>();
        currency = Create<string>();
        portfolio = Create<string>();
        code = Create<string>();
        consideration = Create<decimal>();
        units = Create<int>();
        price = Create<decimal>();
        localExposure = Create<decimal>();
        positionExposure = Create<decimal>();
        portfolioExposure = Create<decimal>();        
    }
    private DateOnly date;
    private string currency;
    private string portfolio;
    private string code;
    private decimal consideration;
    private int units;
    private decimal price;
    private decimal localExposure;
    private decimal positionExposure;
    private decimal portfolioExposure;

    public LedgerRowBuilder WithDate(DateOnly date)
    {
        this.date = date;
        return this;
    }
    public LedgerRowBuilder WithCurrency(string currency)
    {
        this.currency = currency;
        return this;
    }
    public LedgerRowBuilder WithPortfolio(string portfolio)
    {
        this.portfolio = portfolio;
        return this;
    }
    public LedgerRowBuilder WithCode(string code)
    {
        this.code = code;
        return this;
    }
    public LedgerRowBuilder WithConsideration(decimal consideration)
    {
        this.consideration = consideration;
        return this;
    }
    public LedgerRowBuilder WithUnits(int units)
    {
        this.units = units;
        return this;
    }
    public LedgerRowBuilder WithPrice(decimal price)
    {
        this.price = price;
        return this;
    }
    public LedgerRowBuilder WithExposures(decimal local, decimal position,decimal portfolio)
    {
        localExposure = local;
        positionExposure = position;
        portfolioExposure = portfolio;
        return this;
    }

    public LedgerRow Build() => new LedgerRow
    {
        Date = date,
        Currency = currency,
        Portfolio = portfolio,
        Code = code,
        Consideration = consideration,
        Units = units,
        Price = price,
        LocalExposure = localExposure,
        PositionExposure = positionExposure,
        PortfolioExposure = portfolioExposure
    };
}