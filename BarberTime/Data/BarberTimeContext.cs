using BarberTime.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberTime.Data;

public class BarberTimeContext : DbContext
{
    public BarberTimeContext(DbContextOptions<BarberTimeContext> options)
        : base(options)
    {
    }

    public DbSet<Agendamento> Agendamentos { get; set; }
    public DbSet<Servico> Servicos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Agendamento>()
            .Property(a => a.DataHora)
            .HasColumnType("timestamp without time zone");
    }
}