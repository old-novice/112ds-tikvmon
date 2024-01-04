using tikvmon;


var builder = WebApplication.CreateBuilder(args);
var dp = new DataProvider(builder
    .Configuration.GetConnectionString("MySql")!);
var app = builder.Build();

app.UseDefaultFiles();
app.UseFileServer();
app.MapGet("/mon", () =>
    new
    {
        Stores = dp.GetTiKVStoreStatus()
            .Select(o => o.ToStoreInfo())
            .OrderBy(o => o.Kind).ThenBy(o => o.Id),
        Peers = dp.GetTiKVRegionPeers().GroupBy(o => o.STORE_ID)
            .Select(o => new
            {
                StoreId = o.Key,
                Peers = o
                    .OrderBy(p => p.REGION_ID)
                    .Select(p => new
                    {
                        RegionId = p.REGION_ID,
                        PeerId = p.PEER_ID,
                        IsLeader = p.IS_LEADER
                    })
            }).OrderBy(o => o.StoreId)
        }
    );

app.Run();
