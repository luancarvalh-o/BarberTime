using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberTime.Migrations
{
    /// <inheritdoc />
    public partial class FixAgendamentoColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "telefone",
                table: "Agendamentos",
                newName: "Telefone");

            migrationBuilder.RenameColumn(
                name: "Serviço",
                table: "Agendamentos",
                newName: "Servico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Telefone",
                table: "Agendamentos",
                newName: "telefone");

            migrationBuilder.RenameColumn(
                name: "Servico",
                table: "Agendamentos",
                newName: "Serviço");
        }
    }
}
