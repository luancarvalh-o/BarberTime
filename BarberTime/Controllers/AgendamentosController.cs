using BarberTime.Data;
using BarberTime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberTime.Controllers;

public class AgendamentosController : Controller
{
    private readonly BarberTimeContext _context;

    public AgendamentosController(BarberTimeContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var agendamentos = await _context.Agendamentos
            .OrderBy(a => a.DataHora)
            .ToListAsync();

        return View(agendamentos);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Agendamento agendamento)
{
    if (ModelState.IsValid)
    {
        if (!ValidarDataHoraAgendamento(agendamento))
            return View(agendamento);

        bool horarioJaExiste = await _context.Agendamentos
            .AnyAsync(a => a.DataHora == agendamento.DataHora);

        if (horarioJaExiste)
        {
            ModelState.AddModelError("DataHora", "Já existe um agendamento para este horário.");
            return View(agendamento);
        }

        _context.Add(agendamento);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    return View(agendamento);
}

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var agendamento = await _context.Agendamentos.FindAsync(id);

        if (agendamento == null)
            return NotFound();

        return View(agendamento);
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Agendamento agendamento)
{
    if (id != agendamento.Id)
        return NotFound();

    if (ModelState.IsValid)
    {
        if (!ValidarDataHoraAgendamento(agendamento))
            return View(agendamento);

        bool horarioJaExiste = await _context.Agendamentos
            .AnyAsync(a => a.DataHora == agendamento.DataHora && a.Id != agendamento.Id);

        if (horarioJaExiste)
        {
            ModelState.AddModelError("DataHora", "Já existe outro agendamento para este horário.");
            return View(agendamento);
        }

        _context.Update(agendamento);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    return View(agendamento);
}

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var agendamento = await _context.Agendamentos
            .FirstOrDefaultAsync(a => a.Id == id);

        if (agendamento == null)
            return NotFound();

        return View(agendamento);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);

        if (agendamento != null)
        {
            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ValidarDataHoraAgendamento(Agendamento agendamento)
{
    if (agendamento.DataHora < DateTime.Now)
    {
        ModelState.AddModelError("DataHora", "Não é permitido agendar em uma data ou horário passado.");
        return false;
    }

    if (agendamento.DataHora.DayOfWeek == DayOfWeek.Sunday)
    {
        ModelState.AddModelError("DataHora", "A barbearia não funciona aos domingos.");
        return false;
    }

    TimeSpan horario = agendamento.DataHora.TimeOfDay;
    TimeSpan abertura = new TimeSpan(8, 0, 0);
    TimeSpan fechamento = new TimeSpan(20, 0, 0);

    if (horario < abertura || horario > fechamento)
    {
        ModelState.AddModelError("DataHora", "Os agendamentos devem estar entre 08:00 e 20:00.");
        return false;
    }

    return true;
}
}