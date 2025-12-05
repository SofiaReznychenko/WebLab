using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReznichenkoWeb.Migrations
{
    /// <inheritdoc />
    public partial class FixSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Members\"', 'Id'), COALESCE((SELECT MAX(\"Id\") + 1 FROM \"Members\"), 1), false);");
            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Workouts\"', 'Id'), COALESCE((SELECT MAX(\"Id\") + 1 FROM \"Workouts\"), 1), false);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
