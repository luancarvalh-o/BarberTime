using BarberTime.Data;
using BarberTime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            .Include(a => a.Servico)
            .OrderBy(a => a.DataHora)
            .ToListAsync();

        return View(agendamentos);
    }

    public async Task<IActionResult> Create()
    {
        await CarregarServicosAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Agendamento agendamento)
    {
        if (ModelState.IsValid)
        {
            if (!ValidarDataHoraAgendamento(agendamento))
            {
                await CarregarServicosAsync();
                return View(agendamento);
            }

            bool existeConflito = await ExisteConflitoDeHorarioAsync(agendamento);

            if (existeConflito)
            {
                ModelState.AddModelError("DataHora", "Já existe um agendamento conflitante nesse intervalo de horário.");
                await CarregarServicosAsync();
                return View(agendamento);
            }

            _context.Add(agendamento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await CarregarServicosAsync();
        return View(agendamento);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var agendamento = await _context.Agendamentos.FindAsync(id);

        if (agendamento == null)
            return NotFound();

        await CarregarServicosAsync();
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
        {
            await CarregarServicosAsync();
            return View(agendamento);
        }

        bool existeConflito = await ExisteConflitoDeHorarioAsync(agendamento);

        if (existeConflito)
        {
            ModelState.AddModelError("DataHora", "Já existe um agendamento conflitante nesse intervalo de horário.");
            await CarregarServicosAsync();
            return View(agendamento);
        }

        _context.Update(agendamento);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    await CarregarServicosAsync();
    return View(agendamento);
}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var agendamento = await _context.Agendamentos
            .Include(a => a.Servico)
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

    private async Task CarregarServicosAsync()
    {
        var servicos = await _context.Servicos
            .Where(s => s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync();

        ViewBag.Servicos = new SelectList(servicos, "Id", "Nome");
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

    private async Task<bool> ExisteConflitoDeHorarioAsync(Agendamento agendamento)
    {
        var servicoAtual = await _context.Servicos
            .FirstOrDefaultAsync(s => s.Id == agendamento.ServicoId);

        if (servicoAtual == null)
        {
            ModelState.AddModelError("ServicoId", "Serviço inválido.");
            return true;
        }

        var novoInicio = agendamento.DataHora;
        var novoFim = agendamento.DataHora.AddMinutes(servicoAtual.DuracaoEmMinutos);

        var agendamentosExistentes = await _context.Agendamentos
            .Include(a => a.Servico)
            .Where(a => a.Id != agendamento.Id)
            .ToListAsync();

        foreach (var agendamentoExistente in agendamentosExistentes)
        {
            if (agendamentoExistente.Servico == null)
                continue;

            var inicioExistente = agendamentoExistente.DataHora;
            var fimExistente = agendamentoExistente.DataHora
                .AddMinutes(agendamentoExistente.Servico.DuracaoEmMinutos);

            bool temConflito = novoInicio < fimExistente && novoFim > inicioExistente;

            if (temConflito)
                return true;
        }

        return false;
    }
}