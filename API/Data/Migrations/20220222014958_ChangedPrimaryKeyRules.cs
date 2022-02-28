using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class ChangedPrimaryKeyRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Habits");

            migrationBuilder.CreateTable(
                name: "AppUserHabits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    AppUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserHabits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserHabits_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HabitsPair",
                columns: table => new
                {
                    HabitName = table.Column<string>(type: "TEXT", nullable: false),
                    SourceUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    OtherUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitsPair", x => new { x.HabitName, x.SourceUserId, x.OtherUserId });
                    table.ForeignKey(
                        name: "FK_HabitsPair_AspNetUsers_OtherUserId",
                        column: x => x.OtherUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HabitsPair_AspNetUsers_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserHabits_AppUserId",
                table: "AppUserHabits",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitsPair_OtherUserId",
                table: "HabitsPair",
                column: "OtherUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitsPair_SourceUserId",
                table: "HabitsPair",
                column: "SourceUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserHabits");

            migrationBuilder.DropTable(
                name: "HabitsPair");

            migrationBuilder.CreateTable(
                name: "Habits",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    AppUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habits", x => new { x.Name, x.Id });
                    table.ForeignKey(
                        name: "FK_Habits_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Habits_AppUserId",
                table: "Habits",
                column: "AppUserId");
        }
    }
}
