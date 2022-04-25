﻿using System;
using System.Threading.Tasks;

namespace Mkh.Logging.Abstractions.Providers;

/// <summary>
/// 登录日志处理器
/// </summary>
public interface ILoginLogHandler
{
    /// <summary>
    /// 写入日志
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> Write(LoginLogModel model);
}

/// <summary>
/// 登录日志模型
/// </summary>
public class LoginLogModel : BaseLogModel
{
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 登录IP
    /// </summary>
    public string IP { get; set; }

    /// <summary>
    /// 浏览器UA
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; }
}