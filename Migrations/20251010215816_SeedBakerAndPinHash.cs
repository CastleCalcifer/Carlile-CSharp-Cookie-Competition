using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carlile_Cookie_Competition.Migrations
{
    /// <inheritdoc />
    public partial class SeedBakerAndPinHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Baker_cookie_id",
                table: "Baker");

            migrationBuilder.RenameColumn(
                name: "hasVoted",
                table: "Baker",
                newName: "has_voted");

            migrationBuilder.RenameColumn(
                name: "PinHash",
                table: "Baker",
                newName: "pin_hash");

            migrationBuilder.AlterColumn<string>(
                name: "image",
                table: "Cookie",
                type: "TEXT",
                nullable: false,
                defaultValue: "/images/placeholder.jpg",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Baker_baker_name",
                table: "Baker",
                column: "baker_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baker_cookie_id",
                table: "Baker",
                column: "cookie_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Baker_baker_name",
                table: "Baker");

            migrationBuilder.DropIndex(
                name: "IX_Baker_cookie_id",
                table: "Baker");

            migrationBuilder.RenameColumn(
                name: "pin_hash",
                table: "Baker",
                newName: "PinHash");

            migrationBuilder.RenameColumn(
                name: "has_voted",
                table: "Baker",
                newName: "hasVoted");

            migrationBuilder.AlterColumn<string>(
                name: "image",
                table: "Cookie",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldDefaultValue: "/images/placeholder.jpg");

            migrationBuilder.CreateIndex(
                name: "IX_Baker_cookie_id",
                table: "Baker",
                column: "cookie_id");
        }
    }
}
