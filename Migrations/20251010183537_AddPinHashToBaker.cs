using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carlile_Cookie_Competition.Migrations
{
    /// <inheritdoc />
    public partial class AddPinHashToBaker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PinHash",
                table: "Baker",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinHash",
                table: "Baker");
        }
    }
}
