using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapiSBIFS.Migrations
{
    /// <inheritdoc />
    public partial class Whoops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Group_GroupID",
                table: "Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityUser_Activity_ActivitiesActivityID",
                table: "ActivityUser");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupUser_Group_GroupsGroupID",
                table: "GroupUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Activity",
                table: "Activity");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameTable(
                name: "Activity",
                newName: "Activities");

            migrationBuilder.RenameIndex(
                name: "IX_Activity_GroupID",
                table: "Activities",
                newName: "IX_Activities_GroupID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "GroupID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activities",
                table: "Activities",
                column: "ActivityID");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Groups_GroupID",
                table: "Activities",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityUser_Activities_ActivitiesActivityID",
                table: "ActivityUser",
                column: "ActivitiesActivityID",
                principalTable: "Activities",
                principalColumn: "ActivityID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUser_Groups_GroupsGroupID",
                table: "GroupUser",
                column: "GroupsGroupID",
                principalTable: "Groups",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Groups_GroupID",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityUser_Activities_ActivitiesActivityID",
                table: "ActivityUser");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupUser_Groups_GroupsGroupID",
                table: "GroupUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Activities",
                table: "Activities");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameTable(
                name: "Activities",
                newName: "Activity");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_GroupID",
                table: "Activity",
                newName: "IX_Activity_GroupID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "GroupID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activity",
                table: "Activity",
                column: "ActivityID");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Group_GroupID",
                table: "Activity",
                column: "GroupID",
                principalTable: "Group",
                principalColumn: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityUser_Activity_ActivitiesActivityID",
                table: "ActivityUser",
                column: "ActivitiesActivityID",
                principalTable: "Activity",
                principalColumn: "ActivityID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUser_Group_GroupsGroupID",
                table: "GroupUser",
                column: "GroupsGroupID",
                principalTable: "Group",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
