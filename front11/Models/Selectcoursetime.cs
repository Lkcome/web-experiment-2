using System;
using System.Collections.Generic;

namespace front11.Models;

public partial class Selectcoursetime
{
    public int Id { get; set; }

    public DateTime? SelectionStartTime { get; set; }

    public DateTime? SelectionEndTime { get; set; }
}
