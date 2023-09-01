using System.Linq;
using System.Xml.Linq;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Catagory;

// Collection que fiz para o test qual Fixture ele deve receber
[Collection(nameof(CategoryTestFixture))]
public class CategoryTest {
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
        => _categoryTestFixture = categoryTestFixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domian", "Category - Aggregates")]
    public void Instantiate() {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var datetimeBefore = DateTime.Now;
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        // refatorando os testes para deixa-los mais legiveis
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (datetimeBefore <= category.CreatedAt).Should().BeTrue();
        (datetimeAfter >= category.CreatedAt).Should().BeTrue();
        category.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domian", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive) {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var datetimeBefore = DateTime.Now;
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (datetimeBefore <= category.CreatedAt).Should().BeTrue();
        (datetimeAfter >= category.CreatedAt).Should().BeTrue();
        category.IsActive.Should().Be(isActive);
    }
    
    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domian", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name) {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(name!, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domian", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull() {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(validCategory.Name, null!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be null");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domian", "Category - Aggregates")]
    [MemberData(nameof(GetNamesWithLessThan3Characteres), parameters: 10)]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string invalidName) {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action =
            () => new DomainEntity.Category(invalidName, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be less than 3 characters long");
    }

    public static IEnumerable<object[]> GetNamesWithLessThan3Characteres(int numberOfTests = 6) {
        // gera dados randomicos para o teste acima.
        var fixture = new CategoryTestFixture();
        for (int i = 0; i < numberOfTests; i++) {
            var isOdd = i % 2 == 1;
            yield return new object[]
            {
                fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)]
            };
        }
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domian", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters() {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "A").ToArray());

        Action action =
            () => new DomainEntity.Category(invalidName, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be grater than 255 characters long");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domian", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters() {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidDescription = String
            .Join(null, Enumerable.Range(1, 10_001).Select(_ => "A").ToArray());

        Action action =
            () => new DomainEntity.Category(validCategory.Name, invalidDescription);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be grater than 10000 characters long");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domian", "Category - Aggregates")]
    public void Activate() {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var category = new DomainEntity
            .Category(validCategory.Name, validCategory.Description, false);

        category.Activate();

        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domian", "Category - Aggregates")]
    public void Deactivate() {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var category = new DomainEntity.
            Category(validCategory.Name, validCategory.Description, true);

        category.Deactivate();

        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domian", "Category - Aggregates")]
    public void Update() {
        var category = _categoryTestFixture.GetValidCategory();
        var categoryWithNewValeus = _categoryTestFixture.GetValidCategory();

        category.Update(categoryWithNewValeus.Name, categoryWithNewValeus.Description);

        category.Name.Should().Be(categoryWithNewValeus.Name);
        category.Description.Should().Be(categoryWithNewValeus.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domian", "Category - Aggregates")]
    public void UpdateOnlyName() {
        var category = _categoryTestFixture.GetValidCategory();
        var currentDescription = category.Description;

        var categoryName = _categoryTestFixture.GetValidCategoryName();
        category.Update(categoryName);

        category.Name.Should().Be(categoryName);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domian", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void UpdateErrorWhenNameIsEmpty(string? name) {
        var category = _categoryTestFixture.GetValidCategory();

        Action action =
            () => category.Update(name!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domian", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("1")]
    [InlineData("12")]
    public void UpdateErrorWhenNameIsLessThan3Characters(string invalidName) {
        var category = _categoryTestFixture.GetValidCategory();

        Action action =
            () => category.Update(invalidName!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be less than 3 characters long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domian", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255Characters() {
        var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);
        var category = _categoryTestFixture.GetValidCategory();

        Action action =
            () => category.Update(invalidName!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be grater than 255 characters long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domian", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000Characters() {
        var newCategoryName = _categoryTestFixture.GetValidCategoryName();
        var category = _categoryTestFixture.GetValidCategory();
        var invalidDescription = _categoryTestFixture.Faker.Commerce.ProductDescription();
        while (invalidDescription.Length <= 10_000)
            invalidDescription = $"{invalidDescription} {_categoryTestFixture.Faker.Commerce.ProductDescription()}";

        Action action =
            () => category.Update(newCategoryName, invalidDescription);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be grater than 10000 characters long");
    }
}