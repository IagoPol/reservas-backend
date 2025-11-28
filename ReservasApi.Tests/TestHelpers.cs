using Microsoft.EntityFrameworkCore;
using ReservasApi.Data;

namespace ReservasApi.Tests
{
    /*
    // static because: it doesn't store any data; it only provides utility methods; 
    it's ideal for pure utility functions that do not depend on object instances; 
    you will never write sth like var helpers = new TestHelpers();
    static PREVENTS INSTANTIATION and (thus) AVOIDS UNNECESARY OBJECT CREATION;
    utility classes in C# are conventionally static, fex. Math.Sin(), Path.Combine() or File.ReadAllText();
    My test helpers belong to this category.
    */
    public static class TestHelpers // si la clase no fuese static, igualmente se podría hacer TestHelpers.GetInMemoryDbContext
    { // entonces por qué la hacemos static? pues to avoid unnecesary object creation i.e. to prevent unnecessary instantiation.
        public static AppDbContext GetInMemoryDbContext() // lo mismo que inicial y directamente yo antes ponía en ReservasControllerTests.cs, si bien en este caso ponía private en lugar de public static.
        {
            var opciones = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(opciones);
        } // final de la función pública y estática sin parámetros 'GetInMemoryDbContext' que retorna AppDbContext.

    } // final de la clase pública y estática TestHelpers.

} // end of namespace
