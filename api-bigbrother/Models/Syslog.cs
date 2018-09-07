using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class syslog
{
    public ObjectId _id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Data { get; set; }

}
