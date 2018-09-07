using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Vpn
{
    public string Username { get; set; }
    public string ClientIp { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Duration { get; set; }
    public Int64 BytesIn { get; set; }
    public Int64 BytesOut { get; set; }
}

