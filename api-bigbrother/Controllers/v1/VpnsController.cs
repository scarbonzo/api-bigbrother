using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Produces("application/json")]
public class VpnsController : Controller
{
    string Server = "mongodb://192.168.50.225";
    string Database = "syslogs";
    string Source = "192.168.100.6";

    [Route("api/v1/vpns")]
    [HttpGet]
    public ActionResult GetAll()
    {
        var vpns = GetVpns();

        return Ok(vpns);
    }

    [Route("api/v1/vpns/{start}/{end}")]
    [HttpGet]
    public ActionResult GetRange(DateTime start, DateTime end)
    {
        return Ok(GetVpns()
            .Where(v => v.Start < end && v.End > start)
            );
    }

    [Route("api/v1/vpns/{username}")]
    public ActionResult GetUser(string username)
    {
        return Ok(GetVpns()
            .Where(v => v.Username == username)
            );
    }

    [Route("api/v1/vpns/{username}/{start}/{end}")]
    public ActionResult GetUserRange(string username, DateTime start, DateTime end)
    {
        return Ok(GetVpns()
            .Where(v => v.Username == username)
            .Where(v => v.Start < end && v.End > start)
            );
    }

    public List<Vpn> GetVpns()
    {
        var collection = new MongoClient(Server).GetDatabase(Database).GetCollection<syslog>(Source);

        var syslogs = collection.AsQueryable<syslog>()
                .Where(s => s.Data.Contains("NetExtender disconnected"))
                .OrderByDescending(s => s.Timestamp)
                .ToList();

        var vpns = new List<Vpn>();

        try
        {
            foreach (syslog s in syslogs)
            {
                try
                {
                    var fields = s.Data.Split(' ');

                    var _bin = fields[19].Remove(0, 8).TrimEnd('"');
                    var _bout = fields[20].Remove(0, 9).TrimEnd('"');

                    var username = fields[14].Remove(0, 6).TrimEnd('"');
                    var end = Convert.ToDateTime((fields[3].Remove(0, 6).TrimEnd('"')) + " " + (fields[4].TrimEnd('"')));
                    var duration = Convert.ToInt32(fields[18].Remove(0, 9).TrimEnd('"'));
                    var start = end.AddSeconds(-duration);
                    var bytesin = Convert.ToInt64(_bin);
                    var bytesout = Convert.ToInt64(_bout);
                    var ip = fields[12].Remove(0, 4).TrimEnd('"');

                    vpns.Add(new Vpn
                    {
                        Username = username,
                        Start = start,
                        End = end,
                        Duration = duration,
                        BytesIn = bytesin,
                        BytesOut = bytesout,
                        ClientIp = ip
                    });
                }
                catch { }
            }
        }
        catch { }

        return vpns;
    }
}
