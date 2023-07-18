
using BalanceService.Models;
using Microsoft.EntityFrameworkCore;
using BalanceService.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace UnitTestBalance
{
    public class UnitTests
    {
        public static DbContextOptions<DataContext> dbContextOptions { get; }
        public static string connectionString = "Server=localhost;Database=bank;Port=5432;User Id=postgres;Password=2254;";

        static UnitTests()
        {
            dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(connectionString)
                .Options;
        }

        [Fact]
        public void getBalancesSuccess()
        {
            DataContext context = new DataContext(dbContextOptions);
            BalanceController controller = new BalanceController(context);

            var data = controller.GetBalance("");

            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public void getBalanceSuccess()
        {
            DataContext context = new DataContext(dbContextOptions);
            BalanceController controller = new BalanceController(context);

            var data = controller.GetBalanceById(1, "");

            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public void GetTransferHistory()
        {
            DataContext context = new DataContext(dbContextOptions);
            BalanceController controller = new BalanceController(context);

            var data = controller.GetTransferHistory(1, "", "");

            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public void TransferMoney()
        {
            DataContext context = new DataContext(dbContextOptions);
            BalanceController controller = new BalanceController(context);

            TransferBalance balance = new TransferBalance();
            balance.to = 1;
            balance.from = 3;
            balance.moneyAmount = 100;
            var data = controller.TransferMoney(balance);

            Assert.IsType<OkObjectResult>(data);
        }
        [Fact]
        public void DepositeMoney()
        {
            DataContext context = new DataContext(dbContextOptions);
            BalanceController controller = new BalanceController(context);

            Balance balance = new Balance();
            balance.id = 1;
            balance.balance = 200;
            var data = controller.DepositeMoney(balance);

            Assert.IsType<OkObjectResult>(data);
        }
    }
}