using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapiSBIFS.Migrations
{
    /// <inheritdoc />
    public partial class BillsRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    BillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerID = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.BillID);
                    table.ForeignKey(
                        name: "FK_Bill_Group_GroupID",
                        column: x => x.GroupID,
                        principalTable: "Group",
                        principalColumn: "GroupID");
                });

            migrationBuilder.CreateTable(
                name: "BillUser",
                columns: table => new
                {
                    BillsBillID = table.Column<int>(type: "int", nullable: false),
                    ParticipantsUserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillUser", x => new { x.BillsBillID, x.ParticipantsUserID });
                    table.ForeignKey(
                        name: "FK_BillUser_Bill_BillsBillID",
                        column: x => x.BillsBillID,
                        principalTable: "Bill",
                        principalColumn: "BillID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillUser_Users_ParticipantsUserID",
                        column: x => x.ParticipantsUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bill_GroupID",
                table: "Bill",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_BillUser_ParticipantsUserID",
                table: "BillUser",
                column: "ParticipantsUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillUser");

            migrationBuilder.DropTable(
                name: "Bill");
        }
    }
}
