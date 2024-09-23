using System;
using System.Collections.Generic;

namespace front11.Models;

public partial class Course
{
    /// <summary>
    /// 	课程代码（唯一标识）
    /// </summary>
    public string Cid { get; set; } = null!;

    /// <summary>
    /// 课程名称
    /// </summary>
    public string? Cname { get; set; }

    /// <summary>
    /// 学分
    /// </summary>
    public string? Cscore { get; set; }

    /// <summary>
    /// 上课老师
    /// </summary>
    public string? Cteacher { get; set; }

    /// <summary>
    /// 开课学期
    /// </summary>
    public string? Csem { get; set; }

    /// <summary>
    /// 上课时间
    /// </summary>
    public string? Ctime { get; set; }

    /// <summary>
    /// 上课教室
    /// </summary>
    public string? Cclassroom { get; set; }

    public virtual ICollection<SelectedCourse> SelectedCourses { get; set; } = new List<SelectedCourse>();
}
