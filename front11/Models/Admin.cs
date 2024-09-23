using System;
using System.Collections.Generic;

namespace front11.Models;

public partial class Admin
{
    /// <summary>
    /// 管理员代码（唯一标识）
    /// </summary>
    public string Aid { get; set; } = null!;

    /// <summary>
    /// 管理员账号
    /// </summary>
    public string? Aaccount { get; set; }

    /// <summary>
    /// 账号密码
    /// </summary>
    public string? Apassword { get; set; }
}
