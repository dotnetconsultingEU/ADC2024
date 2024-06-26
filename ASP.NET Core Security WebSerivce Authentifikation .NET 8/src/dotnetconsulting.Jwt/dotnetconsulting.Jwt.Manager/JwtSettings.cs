﻿// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschränkungen verwendet oder verändert werden.
// Jedoch wird keine Garantie übernommen, dass eine Funktionsfähigkeit mit aktuellen und 
// zukünftigen API-Versionen besteht. Der Autor übernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgeführt wird.
// Für Anregungen und Fragen stehe ich jedoch gerne zur Verfügung.

// Thorsten Kansy, www.dotnetconsulting.eu

#nullable disable

namespace dotnetconsulting.Jwt.Manager;

/// <summary>
/// Settings for Jwt.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// The secret for Jwt
    /// </summary>
    public string Secret { get; set; }

    public int LiveTime { get; set; } = 14 * 24 * 3600; // 14 Tage

    public string FailedUrl { get; set; }
}