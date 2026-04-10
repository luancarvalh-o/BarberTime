using BarberTime.Data;
using BarberTime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BarberTime.Controllers;

public class PublicoController : Controller
{
    private readonly BarberTimeContext _context;

    public PublicoController(BarberTimeContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Agendar()
    {
        await CarregarServicosAsync();
        return View(new Agendamento
        {
            DataHora = DateTime.Now.AddHours(1),
            Status = "Confirmado"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agendar(Agendamento agendamento)
    {
        agendamento.Status = "Confirmado";

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
                ModelState.AddModelError("DataHora", "Já existe um agendamento nesse intervalo de horário.");
                await CarregarServicosAsync();
                return View(agendamento);
            }

            var servico = await _context.Servicos
                .FirstOrDefaultAsync(s => s.Id == agendamento.ServicoId && s.Ativo);

            if (servico == null)
            {
                ModelState.AddModelError("ServicoId", "Selecione um serviço válido.");
                await CarregarServicosAsync();
                return View(agendamento);
            }

            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Obrigado));
        }

        await CarregarServicosAsync();
        return View(agendamento);
    }

    public IActionResult Obrigado()
    {
        return View();
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
            .FirstOrDefaultAsync(s => s.Id == agendamento.ServicoId && s.Ativo);

        if (servicoAtual == null)
        {
            ModelState.AddModelError("ServicoId", "Serviço inválido.");
            return true;
        }

        var novoInicio = agendamento.DataHora;
        var novoFim = agendamento.DataHora.AddMinutes(servicoAtual.DuracaoEmMinutos);

        var agendamentosExistentes = await _context.Agendamentos
            .Include(a => a.Servico)
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