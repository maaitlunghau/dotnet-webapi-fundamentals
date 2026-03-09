using loyal_service.Models;
using MongoDB.Driver;

namespace LoyalService.Services
{
    public class LoyaltyCustomerService
    {
        private readonly IMongoCollection<Customer> _customers;
        private readonly IMongoCollection<PointHistory> _pointHistory;

        public LoyaltyCustomerService(MongoDbService mongoDbService)
        {
            _customers = mongoDbService.GetCollection<Customer>();
            _pointHistory = mongoDbService.GetCollection<PointHistory>();
        }

        // Ensure customer exists
        private async Task EnsureCustomerExists(int userId)
        {
            var customer = await _customers
                .Find(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                var newCustomer = new Customer
                {
                    UserId = userId,
                    Points = 0,
                    Tier = "Silver"
                };

                await _customers.InsertOneAsync(newCustomer);
            }
        }

        // Get all customers
        public async Task<List<Customer>> GetAllCustomers()
        {
            return await _customers.Find(_ => true).ToListAsync();
        }

        // Get customer by UserId
        public async Task<Customer> GetCustomer(int userId)
        {
            return await _customers
                .Find(x => x.UserId == userId)
                .FirstOrDefaultAsync();
        }

        // Add points
        public async Task AddPoints(int userId, int points,
            string description)
        {
            await EnsureCustomerExists(userId);

            var update = Builders<Customer>.Update.Inc(x => x.Points, points);

            await _customers.UpdateOneAsync(
                x => x.UserId == userId,
                update
            );
            var history = new PointHistory
            {
                UserId = userId,
                PointChanged = points,
                ActionType = "Earn",
                Description = description
            };
            await _pointHistory.InsertOneAsync(history);
            await UpdateTier(userId);
        }

        // Redeem points
        public async Task RedeemPoints(int userId, int points,
            string description)
        {
            await EnsureCustomerExists(userId);
            var update = Builders<Customer>.Update.Inc(x => x.Points, -points);
            await _customers.UpdateOneAsync(
                x => x.UserId == userId,
                update
            );
            var history = new PointHistory
            {
                UserId = userId,
                PointChanged = -points,
                ActionType = "Redeem",
                Description = description
            };
            await _pointHistory.InsertOneAsync(history);
            await UpdateTier(userId);
        }

        // Update tier automatically
        public async Task UpdateTier(int userId)
        {
            var customer = await GetCustomer(userId);

            if (customer == null) return;

            string tier = "Silver";

            if (customer.Points >= 1000)
                tier = "Platinum";
            else if (customer.Points >= 500)
                tier = "Gold";

            var update = Builders<Customer>.Update.Set(x => x.Tier, tier);

            await _customers.UpdateOneAsync(
                x => x.UserId == userId,
                update
            );
        }

        // Get point history
        public async Task<List<PointHistory>> GetPointHistory(int userId)
        {
            return await _pointHistory
                .Find(x => x.UserId == userId)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}