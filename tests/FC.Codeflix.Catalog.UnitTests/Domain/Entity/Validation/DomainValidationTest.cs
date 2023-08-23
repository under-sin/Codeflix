using Bogus;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainValidation = FC.Codeflix.Catalog.Domain.Validation.DomainValidation;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Validation;

public class DomainValidationTest
{
    public Faker Faker { get; set; } = new Faker();

    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOk()
    {
        var fieldnName = Faker.Commerce.ProductName();

        var value = Faker.Commerce.ProductName();
        Action action = () => DomainValidation.NotNull(value, fieldnName);
        action.Should().NotThrow();
    }
    
    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        var fieldnName = Faker.Commerce.ProductName();

        string? value = null;
        Action action = () => DomainValidation.NotNull(value, fieldnName);
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldnName} should not be null");
    }

    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        var fieldnName = Faker.Commerce.ProductName();

        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldnName);
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldnName} should not be empty or null");
    }
    
    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOk()
    {
        var fieldnName = Faker.Commerce.ProductName();

        var target = Faker.Commerce.ProductName();
        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldnName);
        action.Should().NotThrow();
    }

    [Theory(DisplayName = nameof(MinLengthThrowWhenLess))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValeusSmallerThanMin), parameters: 10)]
    public void MinLengthThrowWhenLess(string target, int minLength)
    {
        var fieldnName = Faker.Commerce.ProductName();

        Action actiopn = 
            () => DomainValidation.MinLength(target, minLength, fieldnName);
        
        actiopn.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldnName} should not be less than {minLength} characters long");
    }

    public static IEnumerable<object[]> GetValeusSmallerThanMin(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 10 };
        var faker = new Faker();
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var exemplo = faker.Commerce.ProductName();
            var minLength = exemplo.Length + (new Random().Next(1, 20));
            yield return new object[] { exemplo, minLength };
        }
    }

    [Theory(DisplayName = nameof(MinLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValeusGreaterThanMin), parameters: 10)]
    public void MinLengthOk(string target, int minLength)
    {
        var fieldnName = Faker.Commerce.ProductName();

        Action actiopn =
            () => DomainValidation.MinLength(target, minLength, fieldnName);

        actiopn.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValeusGreaterThanMin(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 6 };
        var faker = new Faker();
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var exemplo = faker.Commerce.ProductName();
            var minLength = exemplo.Length - (new Random().Next(1, 5));
            yield return new object[] { exemplo, minLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthThrowWhenGreater))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValeusGreaterThanMax), parameters: 10)]
    public void MaxLengthThrowWhenGreater(string target, int maxLength)
    {
        var fieldnName = Faker.Commerce.ProductName();

        Action actiopn =
            () => DomainValidation.MaxLength(target, maxLength, fieldnName);

        actiopn.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldnName} should not be grater than {maxLength} characters long");
    }

    public static IEnumerable<object[]> GetValeusGreaterThanMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 5 };
        var faker = new Faker();
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var exemplo = faker.Commerce.ProductName();
            var maxLength = exemplo.Length - (new Random().Next(1, 10));
            yield return new object[] { exemplo, maxLength };
        }
    }

    [Theory(DisplayName = nameof(MaxLengthOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValeusLessThanMax), parameters: 10)]
    public void MaxLengthOk(string target, int maxLength)
    {
        var fieldnName = Faker.Commerce.ProductName();

        Action actiopn =
            () => DomainValidation.MaxLength(target, maxLength, fieldnName);

        actiopn.Should().NotThrow();
    }

    public static IEnumerable<object[]> GetValeusLessThanMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 6 };
        var faker = new Faker();
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var exemplo = faker.Commerce.ProductName();
            var maxLength = exemplo.Length + (new Random().Next(0, 5));
            yield return new object[] { exemplo, maxLength };
        }
    }

}