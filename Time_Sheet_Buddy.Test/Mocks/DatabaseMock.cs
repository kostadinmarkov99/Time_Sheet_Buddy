namespace Time_Sheet_Buddy.Test.Mocks
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using Time_Sheet_Buddy.Data;

    public class DatabaseMock
    {
        public static ApplicationDbContext Instance
        {
            get
            {
                var dbContextoptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

                return new ApplicationDbContext(dbContextoptions);

            }
        }
    }
}
