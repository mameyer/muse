using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Muse.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "album",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Popularity = table.Column<int>(nullable: false),
                    TotalTracks = table.Column<long>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "artist",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Popularity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_artist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "playlist",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpotifyId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                    CurrentPopularity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_playlist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "song",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "track",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongId = table.Column<string>(nullable: true),
                    BandId = table.Column<string>(nullable: true),
                    AlbumId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_track", x => x.Id);
                    table.ForeignKey(
                        name: "FK_track_album_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "album",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_track_artist_BandId",
                        column: x => x.BandId,
                        principalTable: "artist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_track_song_SongId",
                        column: x => x.SongId,
                        principalTable: "song",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "genre",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false),
                    SingleTrackId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_genre_track_SingleTrackId",
                        column: x => x.SingleTrackId,
                        principalTable: "track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "playlist_track",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaylistId = table.Column<long>(nullable: false),
                    TrackId = table.Column<long>(nullable: false),
                    DateAdded = table.Column<DateTimeOffset>(nullable: true),
                    CurrentPopularity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_playlist_track", x => x.Id);
                    table.ForeignKey(
                        name: "FK_playlist_track_playlist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_playlist_track_track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "band_genre",
                columns: table => new
                {
                    BandId = table.Column<string>(nullable: false),
                    GenreId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_band_genre", x => new { x.BandId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_band_genre_artist_BandId",
                        column: x => x.BandId,
                        principalTable: "artist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_band_genre_genre_GenreId",
                        column: x => x.GenreId,
                        principalTable: "genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_band_genre_GenreId",
                table: "band_genre",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_genre_SingleTrackId",
                table: "genre",
                column: "SingleTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_playlist_track_PlaylistId",
                table: "playlist_track",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_playlist_track_TrackId",
                table: "playlist_track",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_track_AlbumId",
                table: "track",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_track_BandId",
                table: "track",
                column: "BandId");

            migrationBuilder.CreateIndex(
                name: "IX_track_SongId",
                table: "track",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "band_genre");

            migrationBuilder.DropTable(
                name: "playlist_track");

            migrationBuilder.DropTable(
                name: "genre");

            migrationBuilder.DropTable(
                name: "playlist");

            migrationBuilder.DropTable(
                name: "track");

            migrationBuilder.DropTable(
                name: "album");

            migrationBuilder.DropTable(
                name: "artist");

            migrationBuilder.DropTable(
                name: "song");
        }
    }
}
