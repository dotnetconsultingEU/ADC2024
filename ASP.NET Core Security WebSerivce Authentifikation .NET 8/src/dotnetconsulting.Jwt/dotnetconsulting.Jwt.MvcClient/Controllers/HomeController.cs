// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschränkungen verwendet oder verändert werden.
// Jedoch wird keine Garantie übernommen, dass eine Funktionsfähigkeit mit aktuellen und 
// zukünftigen API-Versionen besteht. Der Autor übernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgeführt wird.
// Für Anregungen und Fragen stehe ich jedoch gerne zur Verfügung.

// Thorsten Kansy, www.dotnetconsulting.eu

using dotnetconsulting.Jwt.MvcClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using dotnetconsulting.Jwt.Manager;
using Microsoft.AspNetCore.Authorization;

namespace dotnetconsulting.Jwt.MvcClient.Controllers;

// [Authorize(Roles = "RoleA,RoleC")]  // RoleA oder RoleB
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ClaimsPrincipal _currentPrincipal;

    public HomeController(IHttpContextAccessor HttpContextAccessor,
                          ILogger<HomeController> logger)
    {
        if (HttpContextAccessor.HttpContext is null)
            throw new NullReferenceException(nameof(HttpContextAccessor.HttpContext));

        _currentPrincipal = HttpContextAccessor.HttpContext!.User;
        _logger = logger;
    }

    [Authorize(Roles = "RoleB,RoleC")]  // (RoleB oder RoleC)
    [Authorize(Roles = "RoleA")] // und RoleA
    public IActionResult Index()
    {
        ViewBag.UserDisplayname = _currentPrincipal.UserDisplayname();
        ViewBag.TransactionId = _currentPrincipal.UserTransactionId();

        ViewBag.IsInRoleA = _currentPrincipal.IsInRole("RoleA");
        ViewBag.IsInRoleB = _currentPrincipal.IsInRole("RoleB");
        ViewBag.IsInRoleC = _currentPrincipal.IsInRole("RoleC");

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}