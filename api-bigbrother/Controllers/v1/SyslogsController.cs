using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Produces("application/json")]
public class SyslogsController : Controller
{
    string Server = "mongodb://192.168.50.225";
    string Database = "syslogs";

    [Route("api/v1/syslogs/sources")]
    [HttpGet]
    public ActionResult Get(int take = 25, int skip = 0)
    {
        var db = new MongoClient(Server).GetDatabase(Database);

        var collections = db.ListCollectionNames()
            .ToList()
            .Take(take)
            .Skip(skip);

        return Ok(collections);
    }

    [Route("api/v1/syslogs/{source}")]
    [HttpGet]
    public ActionResult Get(string source, string filter = null, DateTime? start = null, DateTime? end = null, int take = 25, int skip = 0)
    {
        var collection = new MongoClient(Server).GetDatabase(Database).GetCollection<syslog>(source);

        var query = collection.AsQueryable<syslog>();

        if (filter != null)
        {
            query = query
                .Where(s => s.Data.Contains(filter));
        }
        
        if(start != null)
        {
            query = query
                .Where(s => s.Timestamp > start);
        }

        if (end != null)
        {
            query = query
                .Where(s => s.Timestamp < end);
        }

        var results = query
            .OrderByDescending(s => s.Timestamp)
            .Take(take)
            .Skip(skip)
            .ToList();

        return Ok(results);
    }
}
