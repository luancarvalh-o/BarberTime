using BarberTime.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BarberTime.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BarberTimeContext _context;

    public HomeController(ILogger<HomeController> logger, BarberTimeContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalAgendamentos = await _context.Agendamentos.CountAsync();
        ViewBag.TotalServicosAtivos = await _context.Servicos.CountAsync(s => s.Ativo);
        ViewBag.ProximosAgendamentos = await _context.Agendamentos
            .Include(a => a.Servico)
            .Where(a => a.DataHora >= DateTime.Now)
            .OrderBy(a => a.DataHora)
            .Take(5)
            .ToListAsync();

        return View();
    }
}