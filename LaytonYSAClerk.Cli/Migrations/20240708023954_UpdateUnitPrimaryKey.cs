using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaytonYSAClerk.Cli.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUnitPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_units",
                table: "units");

            migrationBuilder.AlterColumn<int>(
                name: "unitNumber",
                table: "units",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_units",
                table: "units",
                columns: new[] { "memberId", "unitNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_units",
                table: "units");

            migrationBuilder.AlterColumn<int>(
                name: "unitNumber",
                table: "units",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_units",
                table: "units",
                column: "unitNumber");
        }
    }
}
