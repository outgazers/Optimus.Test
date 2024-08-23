using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optimus.Services.Customers.Infrastructure.Migrations
{
    public partial class updatemodesoftransportation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModsOfTransportation",
                schema: "customers",
                table: "Customers",
                newName: "ModesOfTransportation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModesOfTransportation",
                schema: "customers",
                table: "Customers",
                newName: "ModsOfTransportation");
        }
    }
}
