using System;
using System.Collections.Generic;

namespace front11.Models;

public partial class Student
{
    /// <summary>
    /// 学号（唯一标识）
    /// </summary>
    public string Sid { get; set; } = null!;

    /// <summary>
    /// 姓名
    /// </summary>
    public string? Sname { get; set; }

    /// <summary>
    /// 班级
    /// </summary>
    public string? Sclass { get; set; }

    /// <summary>
    /// 初始密码
    /// </summary>
    public string? Spassword { get; set; }

    public virtual ICollection<SelectedCourse> SelectedCourses { get; set; } = new List<SelectedCourse>();
}
