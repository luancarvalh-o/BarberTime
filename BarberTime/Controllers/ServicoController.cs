using BarberTime.Data;
using BarberTime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberTime.Controllers;

public class ServicosController : Controller
{
    private readonly BarberTimeContext _context;

    public ServicosController(BarberTimeContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var servicos = await _context.Servicos
            .OrderBy(s => s.Nome)
            .ToListAsync();

        return View(servicos);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Servico servico)
    {
        if (ModelState.IsValid)
        {
            _context.Add(servico);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(servico);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var servico = await _context.Servicos.FindAsync(id);

        if (servico == null)
            return NotFound();

        return View(servico);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Servico servico)
    {
        if (id != servico.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(servico);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(servico);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var servico = await _context.Servicos
            .FirstOrDefaultAsync(s => s.Id == id);

        if (servico == null)
            return NotFound();

        return View(servico);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var servico = await _context.Servicos.FindAsync(id);

        if (servico != null)
        {
            _context.Servicos.Remove(servico);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}