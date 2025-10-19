using System;
using System.Linq;
using System.Threading.Tasks;
using MatchaReviewApp.Data;
using MatchaReviewApp.Models;
using MatchaReviewApp.Repositories;
using MatchaReviewApp.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace MatchaReviewApp.Tests
{
    [TestFixture]
    public class StoreServiceTest
    {
        private ApplicationDbContext _context = null!;
        private Repository<Store> _repository = null!;
        private StoreService _service = null!;
        private SqliteConnection _connection = null!;

        [SetUp]
        public void SetUp()
        {
            // Use SQLite in-memory database as an alternative to UseInMemoryDatabase,
            // keep connection open for the lifetime of the context so the DB persists during the test.
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new ApplicationDbContext(options);

            // Create schema
            _context.Database.EnsureCreated();

            _repository = new Repository<Store>(_context);

            // Null logger to avoid external dependencies
            _service = new StoreService(_repository, NullLogger<StoreService>.Instance);
        }

        [TearDown]
        public async Task TearDown()
        {
            // Dispose context and close/dispose the shared connection after the test completes.
            if (_context != null)
            {
                await _context.DisposeAsync();
            }

            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        [Test]
        public async Task CreateUpdateDelete_UsingRepository_PersistsAndCleansUp()
        {
            // Arrange
            var store = new Store
            {
                Name = "EF Integration Store",
                Address = "1 Integration Way",
                Rating = 4.1m,
                Description = "Integration test using real Repository and ApplicationDbContext"
            };

            // Act - Create via StoreService (which uses Repository<T>)
            var created = await _service.CreateStoreAsync(store);

            // Assert - persisted
            Assert.IsNotNull(created);
            Assert.Greater(created.Id, 0);
            Assert.IsTrue(_context.Stores.Any(s => s.Id == created.Id && s.Name == "EF Integration Store"));

            // Act - Update
            created.Name = "EF Integration Store Updated";
            var updateResult = await _service.UpdateStoreAsync(created);

            // Assert - update successful and persisted
            Assert.IsTrue(updateResult);
            var fetched = await _repository.GetByIdAsync(created.Id);
            Assert.IsNotNull(fetched);
            Assert.AreEqual("EF Integration Store Updated", fetched!.Name);

            // Act - Delete via service
            var deleteResult = await _service.DeleteStoreAsync(created.Id);

            // Assert - deleted from database
            Assert.IsTrue(deleteResult);
            Assert.IsFalse(_context.Stores.Any(s => s.Id == created.Id));
        }
    }
}