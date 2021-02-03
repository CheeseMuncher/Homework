using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MetaEditTests
{
    public class TestFixture
    {
        protected readonly Fixture _fixture;

        protected TestFixture()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        protected T Create<T>() => _fixture.Create<T>();

        protected object CreateType(Type type) => new SpecimenContext(_fixture).Resolve(type);

        protected IPostprocessComposer<T> Build<T>() => _fixture.Build<T>();

        protected T CreateWith<T>(Func<Fixture, T> builder) => builder.Invoke(_fixture);

        protected IEnumerable<T> CreateMany<T>(int count = 3) => _fixture.CreateMany<T>(count);

        protected Mock<TDependency> Mock<TDependency>() where TDependency : class => _fixture.Freeze<Mock<TDependency>>();

        protected T Create<T>(Expression<Func<T, object>> propertyPicker, object value) => _fixture.Build<T>().With(propertyPicker, value).Create();

        protected void Inject<T>(T injectedType) => _fixture.Inject(injectedType);

        protected T Not<T>(T valueToAvoid)
        {
            T newFake;

            do
            {
                newFake = Create<T>();
            } while (newFake.Equals(valueToAvoid));

            return newFake;
        }
    }

    public class TestFixture<TSut> : TestFixture where TSut : class
    {
        private TSut _sut;
        protected TSut Sut => _sut ??= _fixture.Create<TSut>();
    }
}