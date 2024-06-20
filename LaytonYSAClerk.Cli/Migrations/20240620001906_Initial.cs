using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaytonYSAClerk.Cli.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    newMemberEmailSentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    textAddress = table.Column<string>(type: "TEXT", nullable: true),
                    birthdate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    gender = table.Column<string>(type: "TEXT", nullable: true),
                    householdPosition = table.Column<string>(type: "TEXT", nullable: true),
                    moveDateCalc = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    phone = table.Column<string>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    priorUnitName = table.Column<string>(type: "TEXT", nullable: true),
                    priorUnitNumber = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    unitNumber = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(type: "TEXT", nullable: true),
                    leaderName = table.Column<string>(type: "TEXT", nullable: false),
                    leaderCellPhone = table.Column<string>(type: "TEXT", nullable: true),
                    leaderEmail = table.Column<string>(type: "TEXT", nullable: false),
                    positionName = table.Column<string>(type: "TEXT", nullable: false),
                    memberId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units", x => x.unitNumber);
                    table.ForeignKey(
                        name: "FK_units_members_memberId",
                        column: x => x.memberId,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_units_memberId",
                table: "units",
                column: "memberId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropTable(
                name: "members");
        }
    }
}
