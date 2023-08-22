using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Catagory;

public class CategoryTestFixture
{
    // As fixture são um forma de padrozinar as intancias dos objetos usados para fazer os teste.
    // nesse caso é uma category

    public DomainEntity.Category GetValidCategory()
        => new("Category name", "Category description");
}

// CollectionDefinition => com o collection podemos "injetar" o fixture na classe de teste
[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection :
    ICollectionFixture<CategoryTestFixture> { }