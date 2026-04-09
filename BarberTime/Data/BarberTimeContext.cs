using BarberTime.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberTime.Data;

public class BarberTimeContext : DbContext
{
    public BarberTimeContext(DbContextOptions<BarberTimeContext> options)
    : base(options)
    {
        
    }

    public DbSet<Agendamento> Agendamentos {get; set; }

}