using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManualMovementsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialManualMovementsManagerMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FavoriteSport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FavoriteClub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcceptTermsUse = table.Column<bool>(type: "bit", nullable: false),
                    AcceptPrivacyPolicy = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PRODUTO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    COD_PRODUTO = table.Column<string>(type: "nchar(4)", fixedLength: true, maxLength: 4, nullable: false),
                    DES_PRODUTO = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    STA_STATUS = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTO", x => x.Id);
                    table.UniqueConstraint("AK_PRODUTO_COD_PRODUTO", x => x.COD_PRODUTO);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Complement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRODUTO_COSIF",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    COD_PRODUTO = table.Column<string>(type: "nchar(4)", fixedLength: true, maxLength: 4, nullable: false),
                    COD_COSIF = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false),
                    COD_CLASSIFICACAO = table.Column<string>(type: "nchar(6)", fixedLength: true, maxLength: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    STA_STATUS = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTO_COSIF", x => x.Id);
                    table.UniqueConstraint("AK_PRODUTO_COSIF_COD_PRODUTO_COD_COSIF", x => new { x.COD_PRODUTO, x.COD_COSIF });
                    table.ForeignKey(
                        name: "FK_PRODUTO_COSIF_PRODUTO_COD_PRODUTO",
                        column: x => x.COD_PRODUTO,
                        principalTable: "PRODUTO",
                        principalColumn: "COD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MOVIMENTO_MANUAL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DAT_MES = table.Column<int>(type: "int", nullable: false),
                    DAT_ANO = table.Column<int>(type: "int", nullable: false),
                    NUM_LANCAMENTO = table.Column<int>(type: "int", nullable: false),
                    COD_PRODUTO = table.Column<string>(type: "nchar(4)", fixedLength: true, maxLength: 4, nullable: false),
                    COD_COSIF = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false),
                    DES_DESCRICAO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DAT_MOVIMENTO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    COD_USUARIO = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    VAL_VALOR = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOVIMENTO_MANUAL", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MOVIMENTO_MANUAL_PRODUTO_COD_PRODUTO",
                        column: x => x.COD_PRODUTO,
                        principalTable: "PRODUTO",
                        principalColumn: "COD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MOVIMENTO_MANUAL_PRODUTO_COSIF_COD_PRODUTO_COD_COSIF",
                        columns: x => new { x.COD_PRODUTO, x.COD_COSIF },
                        principalTable: "PRODUTO_COSIF",
                        principalColumns: new[] { "COD_PRODUTO", "COD_COSIF" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_CustomerId",
                table: "Address",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_DocumentNumber",
                table: "Customer",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Email",
                table: "Customer",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTO_MANUAL_COD_PRODUTO_COD_COSIF",
                table: "MOVIMENTO_MANUAL",
                columns: new[] { "COD_PRODUTO", "COD_COSIF" });

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTO_MANUAL_DAT_MES_DAT_ANO",
                table: "MOVIMENTO_MANUAL",
                columns: new[] { "DAT_MES", "DAT_ANO" });

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTO_MANUAL_DAT_MES_DAT_ANO_NUM_LANCAMENTO_COD_PRODUTO_COD_COSIF",
                table: "MOVIMENTO_MANUAL",
                columns: new[] { "DAT_MES", "DAT_ANO", "NUM_LANCAMENTO", "COD_PRODUTO", "COD_COSIF" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTO_MANUAL_DAT_MOVIMENTO",
                table: "MOVIMENTO_MANUAL",
                column: "DAT_MOVIMENTO");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUTO_COD_PRODUTO",
                table: "PRODUTO",
                column: "COD_PRODUTO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PRODUTO_COSIF_COD_PRODUTO_COD_COSIF",
                table: "PRODUTO_COSIF",
                columns: new[] { "COD_PRODUTO", "COD_COSIF" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "MOVIMENTO_MANUAL");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "PRODUTO_COSIF");

            migrationBuilder.DropTable(
                name: "PRODUTO");
        }
    }
}
