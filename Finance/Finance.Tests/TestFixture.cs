using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Dsl;
using AutoFixture.Xunit2;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Finance.Tests;

public class TestFixture
{
    protected readonly Fixture _fixture = DateOnlyFixture.Create();

    protected TestFixture()
    {
        _fixture.Customize(new AutoMoqCustomization());
    }

    protected T Create<T>() => _fixture.Create<T>();

    protected IPostprocessComposer<T> Build<T>() => _fixture.Build<T>();

    protected T CreateWith<T>(Func<Fixture, T> builder) => builder.Invoke(_fixture);

    protected IEnumerable<T> CreateMany<T>(int count = 3) => _fixture.CreateMany<T>(count);

    protected Mock<TDependency> Mock<TDependency>() where TDependency : class => _fixture.Freeze<Mock<TDependency>>();

    protected T Create<T>(Expression<Func<T, object>> propertyPicker, object value) => _fixture.Build<T>().With(propertyPicker, value).Create();

    protected void Inject<T>(T injectedType) => _fixture.Inject(injectedType);
}

public class TestFixture<TSut> : TestFixture where TSut : class
{
    private TSut _sut = default!;
    protected TSut Sut => _sut ??= _fixture.Create<TSut>();
}

public class DateOnlyFixture
{
    public static Fixture Create()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
        return fixture;
    }
}

public class AutoDateOnlyAttribute : AutoDataAttribute
{
    public AutoDateOnlyAttribute() : base(() => DateOnlyFixture.Create()) {}
}