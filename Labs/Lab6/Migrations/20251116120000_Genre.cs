using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab6.Migrations
{
    public partial class Genre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(type: "INTEGER", nullable: false)
                            .Annotation("Sqlite:Autoincrement", true),
                        Name = table.Column<string>(type: "TEXT", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                }
            );

            // Seed default genre so existing movies can reference it
            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "unknown" }
            );

            // Add GenreId column to Movies with default value pointing to seeded genre
            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Movies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1
            );

            // Create index and foreign key constraint
            migrationBuilder.CreateIndex(
                name: "IX_Movies_GenreId",
                table: "Movies",
                column: "GenreId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Genres_GenreId",
                table: "Movies",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Genres_GenreId",
                table: "Movies"
            );

            migrationBuilder.DropIndex(
                name: "IX_Movies_GenreId",
                table: "Movies"
            );

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Movies"
            );

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1
            );

            migrationBuilder.DropTable(
                name: "Genres"
            );
        }
    }
}