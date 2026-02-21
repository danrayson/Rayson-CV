using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Entities;
using Domain.Repositories;

namespace Database.SeedData;

public class DataSeeder(DataSeedLocationSettings dataSeedLocationSettings, IRepository<Exchange> exchangeRepo) : IDataSeeder
{
    private readonly DataSeedLocationSettings dataSeedLocationSettings = dataSeedLocationSettings;
    private readonly IRepository<Exchange> exchangeRepo = exchangeRepo;

    public async Task SeedDatabase()
    {
        await SeedExchanges();
    }

    private async Task SeedExchanges()
    {
        //Load up the seed exchanges
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        var csvPath = Path.Combine(assemblyPath, dataSeedLocationSettings.RelativeFilePath, "Exchanges.csv");
        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<ExchangeMap>();
        var seedExchanges = csv.GetRecords<Exchange>().ToList();

        //Get the exchanges from the database where the name is in the seed list
        var names = seedExchanges.Select(e => e.Name);
        var query = exchangeRepo.Query().Where(e => names.Contains(e.Name));
        var databaseEchanges = await exchangeRepo.GetListAsync(query);

        //Get the exchanges that are in the seed data but not in the database
        var newExchanges = seedExchanges.Where(e => !databaseEchanges.Select(e => e.Name).Contains(e.Name)).ToArray();

        //Save all the missing exchanges
        await exchangeRepo.AddManyAsync(newExchanges);
    }

    protected sealed class ExchangeMap : ClassMap<Exchange>
    {
        public ExchangeMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.DeletedAt).Ignore();
            Map(m => m.DailyMarketDatas).Ignore();
            Map(m => m.Id).Ignore();
            Map(m => m.Symbols).Ignore();
        }
    }
}