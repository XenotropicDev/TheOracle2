using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheOracle2.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetMove",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Asset = table.Column<string>(type: "TEXT", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    ProgressMove = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetMove", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetStatOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Select = table.Column<string>(type: "TEXT", nullable: true),
                    Stats = table.Column<string>(type: "TEXT", nullable: true),
                    Resources = table.Column<string>(type: "TEXT", nullable: true),
                    Legacies = table.Column<string>(type: "TEXT", nullable: true),
                    Selection = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetStatOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConditionMeter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Max = table.Column<int>(type: "INTEGER", nullable: false),
                    Conditions = table.Column<string>(type: "TEXT", nullable: true),
                    StartsAt = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionMeter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Counter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    StartsAt = table.Column<int>(type: "INTEGER", nullable: false),
                    Max = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Oracle = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsProgressMove = table.Column<bool>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoveStatOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Method = table.Column<string>(type: "TEXT", nullable: true),
                    Stats = table.Column<string>(type: "TEXT", nullable: true),
                    Progress = table.Column<string>(type: "TEXT", nullable: true),
                    Legacies = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveStatOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OracleGuilds",
                columns: table => new
                {
                    OracleGuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OracleGuilds", x => x.OracleGuildId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    DiscordGuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Edge = table.Column<int>(type: "INTEGER", nullable: false),
                    Heart = table.Column<int>(type: "INTEGER", nullable: false),
                    Iron = table.Column<int>(type: "INTEGER", nullable: false),
                    Shadow = table.Column<int>(type: "INTEGER", nullable: false),
                    Wits = table.Column<int>(type: "INTEGER", nullable: false),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    Spirit = table.Column<int>(type: "INTEGER", nullable: false),
                    Supply = table.Column<int>(type: "INTEGER", nullable: false),
                    Momentum = table.Column<int>(type: "INTEGER", nullable: false),
                    XpGained = table.Column<int>(type: "INTEGER", nullable: false),
                    XpSpent = table.Column<int>(type: "INTEGER", nullable: false),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    Impacts = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharacters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Select",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Options = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Select", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Page = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Track",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    StartsAt = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Track", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoveTrigger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    StatOptionsId = table.Column<int>(type: "INTEGER", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    MoveId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveTrigger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoveTrigger_Moves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Moves",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MoveTrigger_MoveStatOptions_StatOptionsId",
                        column: x => x.StatOptionsId,
                        principalTable: "MoveStatOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MoveOracleGuild",
                columns: table => new
                {
                    MovesId = table.Column<int>(type: "INTEGER", nullable: false),
                    OracleGuildsOracleGuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveOracleGuild", x => new { x.MovesId, x.OracleGuildsOracleGuildId });
                    table.ForeignKey(
                        name: "FK_MoveOracleGuild_Moves_MovesId",
                        column: x => x.MovesId,
                        principalTable: "Moves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoveOracleGuild_OracleGuilds_OracleGuildsOracleGuildId",
                        column: x => x.OracleGuildsOracleGuildId,
                        principalTable: "OracleGuilds",
                        principalColumn: "OracleGuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OracleInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Aliases = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    SourceId = table.Column<int>(type: "INTEGER", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    OracleGuildId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OracleInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OracleInfo_OracleGuilds_OracleGuildId",
                        column: x => x.OracleGuildId,
                        principalTable: "OracleGuilds",
                        principalColumn: "OracleGuildId");
                    table.ForeignKey(
                        name: "FK_OracleInfo_Source_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AlterProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConditionMeterId = table.Column<int>(type: "INTEGER", nullable: true),
                    TrackId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlterProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlterProperties_ConditionMeter_ConditionMeterId",
                        column: x => x.ConditionMeterId,
                        principalTable: "ConditionMeter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AlterProperties_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Aliases = table.Column<string>(type: "TEXT", nullable: true),
                    AssetType = table.Column<string>(type: "TEXT", nullable: true),
                    ConditionMeterId = table.Column<int>(type: "INTEGER", nullable: true),
                    CounterId = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Input = table.Column<string>(type: "TEXT", nullable: true),
                    Modules = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    SelectId = table.Column<int>(type: "INTEGER", nullable: true),
                    TrackId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_ConditionMeter_ConditionMeterId",
                        column: x => x.ConditionMeterId,
                        principalTable: "ConditionMeter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Counter_CounterId",
                        column: x => x.CounterId,
                        principalTable: "Counter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Select_SelectId",
                        column: x => x.SelectId,
                        principalTable: "Select",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Special",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    AssetStatOptionsId = table.Column<int>(type: "INTEGER", nullable: true),
                    MoveTriggerId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Special", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Special_AssetStatOptions_AssetStatOptionsId",
                        column: x => x.AssetStatOptionsId,
                        principalTable: "AssetStatOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Special_MoveTrigger_MoveTriggerId",
                        column: x => x.MoveTriggerId,
                        principalTable: "MoveTrigger",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Subcategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleInfoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Aliases = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    ContentTags = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Displayname = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Requires = table.Column<string>(type: "TEXT", nullable: true),
                    SampleNames = table.Column<string>(type: "TEXT", nullable: true),
                    SourceId = table.Column<int>(type: "INTEGER", nullable: true),
                    SubcategoryOf = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subcategory_OracleInfo_OracleInfoId",
                        column: x => x.OracleInfoId,
                        principalTable: "OracleInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subcategory_Source_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetAbilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlterPropertiesId = table.Column<int>(type: "INTEGER", nullable: true),
                    CounterId = table.Column<int>(type: "INTEGER", nullable: true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Input = table.Column<string>(type: "TEXT", nullable: true),
                    MoveId = table.Column<int>(type: "INTEGER", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    AssetId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAbilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetAbilities_AlterProperties_AlterPropertiesId",
                        column: x => x.AlterPropertiesId,
                        principalTable: "AlterProperties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetAbilities_AssetMove_MoveId",
                        column: x => x.MoveId,
                        principalTable: "AssetMove",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetAbilities_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetAbilities_Counter_CounterId",
                        column: x => x.CounterId,
                        principalTable: "Counter",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetOracleGuild",
                columns: table => new
                {
                    AssetsId = table.Column<int>(type: "INTEGER", nullable: false),
                    OracleGuildsOracleGuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetOracleGuild", x => new { x.AssetsId, x.OracleGuildsOracleGuildId });
                    table.ForeignKey(
                        name: "FK_AssetOracleGuild_Assets_AssetsId",
                        column: x => x.AssetsId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetOracleGuild_OracleGuilds_OracleGuildsOracleGuildId",
                        column: x => x.OracleGuildsOracleGuildId,
                        principalTable: "OracleGuilds",
                        principalColumn: "OracleGuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inherit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleInfoId = table.Column<int>(type: "INTEGER", nullable: true),
                    SubcategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Exclude = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Requires = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inherit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inherit_OracleInfo_OracleInfoId",
                        column: x => x.OracleInfoId,
                        principalTable: "OracleInfo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Inherit_Subcategory_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "Subcategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Oracles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleInfoId = table.Column<int>(type: "INTEGER", nullable: true),
                    SubcategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    Aliases = table.Column<string>(type: "TEXT", nullable: true),
                    AllowDuplicateRolls = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Initial = table.Column<bool>(type: "INTEGER", nullable: false),
                    Maxrolls = table.Column<int>(type: "INTEGER", nullable: false),
                    Minrolls = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    OracleType = table.Column<string>(type: "TEXT", nullable: true),
                    Repeatable = table.Column<bool>(type: "INTEGER", nullable: false),
                    Requires = table.Column<string>(type: "TEXT", nullable: true),
                    SelectTableBy = table.Column<string>(type: "TEXT", nullable: true),
                    SourceId = table.Column<int>(type: "INTEGER", nullable: true),
                    Subgroup = table.Column<string>(type: "TEXT", nullable: true),
                    PartOfSpeech = table.Column<string>(type: "TEXT", nullable: true),
                    ContentTags = table.Column<string>(type: "TEXT", nullable: true),
                    Group = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oracles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Oracles_OracleInfo_OracleInfoId",
                        column: x => x.OracleInfoId,
                        principalTable: "OracleInfo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Oracles_Source_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Source",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Oracles_Subcategory_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "Subcategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AlterMove",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnyMove = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    AbilityId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlterMove", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlterMove_AssetAbilities_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "AssetAbilities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleId = table.Column<int>(type: "INTEGER", nullable: false),
                    Aliases = table.Column<string>(type: "TEXT", nullable: true),
                    Displayname = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Requires = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tables_Oracles_OracleId",
                        column: x => x.OracleId,
                        principalTable: "Oracles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UseWith",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleId = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Group = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UseWith", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UseWith_Oracles_OracleId",
                        column: x => x.OracleId,
                        principalTable: "Oracles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetTrigger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    StatOptionsId = table.Column<int>(type: "INTEGER", nullable: true),
                    AlterMoveId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssetMoveId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTrigger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetTrigger_AlterMove_AlterMoveId",
                        column: x => x.AlterMoveId,
                        principalTable: "AlterMove",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetTrigger_AssetMove_AssetMoveId",
                        column: x => x.AssetMoveId,
                        principalTable: "AssetMove",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetTrigger_AssetStatOptions_StatOptionsId",
                        column: x => x.StatOptionsId,
                        principalTable: "AssetStatOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChanceTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OracleId = table.Column<int>(type: "INTEGER", nullable: true),
                    TableId = table.Column<int>(type: "INTEGER", nullable: true),
                    TablesId = table.Column<int>(type: "INTEGER", nullable: true),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Assets = table.Column<string>(type: "TEXT", nullable: true),
                    Chance = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    PartOfSpeech = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChanceTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChanceTables_Oracles_OracleId",
                        column: x => x.OracleId,
                        principalTable: "Oracles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChanceTables_Tables_TablesId",
                        column: x => x.TablesId,
                        principalTable: "Tables",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AddTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChanceTableId = table.Column<int>(type: "INTEGER", nullable: false),
                    Templatetype = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddTemplate_ChanceTables_ChanceTableId",
                        column: x => x.ChanceTableId,
                        principalTable: "ChanceTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MultipleRolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChanceTableId = table.Column<int>(type: "INTEGER", nullable: false),
                    Allowduplicates = table.Column<bool>(type: "INTEGER", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleRolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MultipleRolls_ChanceTables_ChanceTableId",
                        column: x => x.ChanceTableId,
                        principalTable: "ChanceTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suggest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChanceTableId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationTheme = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suggest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suggest_ChanceTables_ChanceTableId",
                        column: x => x.ChanceTableId,
                        principalTable: "ChanceTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameObject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChanceTableId = table.Column<int>(type: "INTEGER", nullable: true),
                    SuggestId = table.Column<int>(type: "INTEGER", nullable: true),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    Objecttype = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameObject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameObject_ChanceTables_ChanceTableId",
                        column: x => x.ChanceTableId,
                        principalTable: "ChanceTables",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameObject_Suggest_SuggestId",
                        column: x => x.SuggestId,
                        principalTable: "Suggest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OracleStubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChanceTableId = table.Column<int>(type: "INTEGER", nullable: true),
                    SuggestId = table.Column<int>(type: "INTEGER", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Subcategory = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OracleStubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OracleStubs_ChanceTables_ChanceTableId",
                        column: x => x.ChanceTableId,
                        principalTable: "ChanceTables",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OracleStubs_Suggest_SuggestId",
                        column: x => x.SuggestId,
                        principalTable: "Suggest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddTemplateId = table.Column<int>(type: "INTEGER", nullable: true),
                    GameObjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    DerelictType = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attributes_AddTemplate_AddTemplateId",
                        column: x => x.AddTemplateId,
                        principalTable: "AddTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attributes_GameObject_GameObjectId",
                        column: x => x.GameObjectId,
                        principalTable: "GameObject",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddTemplate_ChanceTableId",
                table: "AddTemplate",
                column: "ChanceTableId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlterMove_AbilityId",
                table: "AlterMove",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_AlterProperties_ConditionMeterId",
                table: "AlterProperties",
                column: "ConditionMeterId");

            migrationBuilder.CreateIndex(
                name: "IX_AlterProperties_TrackId",
                table: "AlterProperties",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAbilities_AlterPropertiesId",
                table: "AssetAbilities",
                column: "AlterPropertiesId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAbilities_AssetId",
                table: "AssetAbilities",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAbilities_CounterId",
                table: "AssetAbilities",
                column: "CounterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAbilities_MoveId",
                table: "AssetAbilities",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOracleGuild_OracleGuildsOracleGuildId",
                table: "AssetOracleGuild",
                column: "OracleGuildsOracleGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ConditionMeterId",
                table: "Assets",
                column: "ConditionMeterId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CounterId",
                table: "Assets",
                column: "CounterId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SelectId",
                table: "Assets",
                column: "SelectId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TrackId",
                table: "Assets",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTrigger_AlterMoveId",
                table: "AssetTrigger",
                column: "AlterMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTrigger_AssetMoveId",
                table: "AssetTrigger",
                column: "AssetMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTrigger_StatOptionsId",
                table: "AssetTrigger",
                column: "StatOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_AddTemplateId",
                table: "Attributes",
                column: "AddTemplateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_GameObjectId",
                table: "Attributes",
                column: "GameObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChanceTables_OracleId",
                table: "ChanceTables",
                column: "OracleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChanceTables_TablesId",
                table: "ChanceTables",
                column: "TablesId");

            migrationBuilder.CreateIndex(
                name: "IX_GameObject_ChanceTableId",
                table: "GameObject",
                column: "ChanceTableId");

            migrationBuilder.CreateIndex(
                name: "IX_GameObject_SuggestId",
                table: "GameObject",
                column: "SuggestId");

            migrationBuilder.CreateIndex(
                name: "IX_Inherit_OracleInfoId",
                table: "Inherit",
                column: "OracleInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inherit_SubcategoryId",
                table: "Inherit",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveOracleGuild_OracleGuildsOracleGuildId",
                table: "MoveOracleGuild",
                column: "OracleGuildsOracleGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveTrigger_MoveId",
                table: "MoveTrigger",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveTrigger_StatOptionsId",
                table: "MoveTrigger",
                column: "StatOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleRolls_ChanceTableId",
                table: "MultipleRolls",
                column: "ChanceTableId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OracleInfo_OracleGuildId",
                table: "OracleInfo",
                column: "OracleGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_OracleInfo_SourceId",
                table: "OracleInfo",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Oracles_OracleInfoId",
                table: "Oracles",
                column: "OracleInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Oracles_SourceId",
                table: "Oracles",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Oracles_SubcategoryId",
                table: "Oracles",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OracleStubs_ChanceTableId",
                table: "OracleStubs",
                column: "ChanceTableId");

            migrationBuilder.CreateIndex(
                name: "IX_OracleStubs_SuggestId",
                table: "OracleStubs",
                column: "SuggestId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacters_MessageId",
                table: "PlayerCharacters",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Special_AssetStatOptionsId",
                table: "Special",
                column: "AssetStatOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Special_MoveTriggerId",
                table: "Special",
                column: "MoveTriggerId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategory_OracleInfoId",
                table: "Subcategory",
                column: "OracleInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategory_SourceId",
                table: "Subcategory",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Suggest_ChanceTableId",
                table: "Suggest",
                column: "ChanceTableId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_OracleId",
                table: "Tables",
                column: "OracleId");

            migrationBuilder.CreateIndex(
                name: "IX_UseWith_OracleId",
                table: "UseWith",
                column: "OracleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetOracleGuild");

            migrationBuilder.DropTable(
                name: "AssetTrigger");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropTable(
                name: "Inherit");

            migrationBuilder.DropTable(
                name: "MoveOracleGuild");

            migrationBuilder.DropTable(
                name: "MultipleRolls");

            migrationBuilder.DropTable(
                name: "OracleStubs");

            migrationBuilder.DropTable(
                name: "PlayerCharacters");

            migrationBuilder.DropTable(
                name: "Special");

            migrationBuilder.DropTable(
                name: "UseWith");

            migrationBuilder.DropTable(
                name: "AlterMove");

            migrationBuilder.DropTable(
                name: "AddTemplate");

            migrationBuilder.DropTable(
                name: "GameObject");

            migrationBuilder.DropTable(
                name: "AssetStatOptions");

            migrationBuilder.DropTable(
                name: "MoveTrigger");

            migrationBuilder.DropTable(
                name: "AssetAbilities");

            migrationBuilder.DropTable(
                name: "Suggest");

            migrationBuilder.DropTable(
                name: "Moves");

            migrationBuilder.DropTable(
                name: "MoveStatOptions");

            migrationBuilder.DropTable(
                name: "AlterProperties");

            migrationBuilder.DropTable(
                name: "AssetMove");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "ChanceTables");

            migrationBuilder.DropTable(
                name: "ConditionMeter");

            migrationBuilder.DropTable(
                name: "Counter");

            migrationBuilder.DropTable(
                name: "Select");

            migrationBuilder.DropTable(
                name: "Track");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Oracles");

            migrationBuilder.DropTable(
                name: "Subcategory");

            migrationBuilder.DropTable(
                name: "OracleInfo");

            migrationBuilder.DropTable(
                name: "OracleGuilds");

            migrationBuilder.DropTable(
                name: "Source");
        }
    }
}
