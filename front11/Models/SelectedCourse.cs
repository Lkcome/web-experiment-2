using System;
using System.Collections.Generic;

namespace front11.Models;

public partial class SelectedCourse
{
    /// <summary>
    /// 课程代码(主键加外键)
    /// </summary>
    public string Cid { get; set; } = null!;

    /// <summary>
    /// 学号(主键加外键)
    /// </summary>
    public string Sid { get; set; } = null!;

    /// <summary>
    /// 选课日期
    /// </summary>
    public DateTime? ScDate { get; set; }

    public virtual Course CidNavigation { get; set; } = null!;

    public virtual Student SidNavigation { get; set; } = null!;
}
