using System;
using System.Collections.Generic;

namespace GameMonitoring.Data.Models;


/// Entity model that represents application log records stored in the database.

public partial class Log
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;
}
