using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaNumVillas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumberVillas",
                columns: table => new
                {
                    NumVilla = table.Column<int>(type: "int", nullable: false),
                    IdVilla = table.Column<int>(type: "int", nullable: false),
                    DetailsVilla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberVillas", x => x.NumVilla);
                    table.ForeignKey(
                        name: "FK_NumberVillas_Villas_IdVilla",
                        column: x => x.IdVilla,
                        principalTable: "Villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NumberVillas_IdVilla",
                table: "NumberVillas",
                column: "IdVilla");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumberVillas");
        }
    }
}
