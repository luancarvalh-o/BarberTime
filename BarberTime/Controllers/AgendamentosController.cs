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
}