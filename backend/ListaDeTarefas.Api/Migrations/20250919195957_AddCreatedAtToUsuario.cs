using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListaDeTarefas.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Tarefas");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tarefas",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "Tarefas",
                newName: "Concluida");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Tarefas",
                newName: "Descricao");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Tarefas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Tarefas");

            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "Tarefas",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Tarefas",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Concluida",
                table: "Tarefas",
                newName: "IsCompleted");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Tarefas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
