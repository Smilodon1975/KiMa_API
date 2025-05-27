using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KiMa_API.Data;
using KiMa_API.Models;

namespace RapidmailImporter.Services
{
    /// <summary>
    /// Service zum Import von Rapidmail-CSV-Daten in die KiMa-Datenbank.
    /// Führt für jede Zeile einen Upsert (Insert oder Update) aus.
    /// </summary>
    public class RapidmailImportService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<RapidmailImportService> _logger;

        public RapidmailImportService(AppDbContext dbContext, ILogger<RapidmailImportService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Liest die CSV-Datei ein und synchronisiert die Datensätze mit der Datenbank.
        /// </summary>
        /// <param name="csvPath">Pfad zur CSV-Datei.</param>
        public async Task ImportFromCsvAsync(string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                _logger.LogError("CSV file not found: {Path}", csvPath);
                return;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                BadDataFound = null,
                MissingFieldFound = null,
                HeaderValidated = null,
                PrepareHeaderForMatch = header => header.Header?.Trim().ToLower(),
            };

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, config);

            // Header einlesen
            await csv.ReadAsync();
            csv.ReadHeader();

            int inserted = 0, updated = 0, skipped = 0;

            // Datensätze verarbeiten
            while (await csv.ReadAsync())
            {
                var record = csv.GetRecord<RapidmailRecord>();
                try
                {
                    var existing = await _dbContext.Users
                        .FirstOrDefaultAsync(u => u.Email == record.Email);

                    if (existing == null)
                    {
                        // Neuer Benutzer
                        var user = new User
                        {
                            Email = record.Email,
                            UserName = record.Email,
                            FirstName = record.Firstname,
                            LastName = record.Lastname,
                            Title = record.Title,
                            Gender = record.Gender,
                            Zip = record.Zip
                        };
                        _dbContext.Users.Add(user);
                        inserted++;
                    }
                    else
                    {
                        // Update bei geänderten Feldern
                        bool changed = false;
                        if (existing.FirstName != record.Firstname)
                        {
                            existing.FirstName = record.Firstname;
                            changed = true;
                        }
                        if (existing.LastName != record.Lastname)
                        {
                            existing.LastName = record.Lastname;
                            changed = true;
                        }
                        if (existing.Title != record.Title)
                        {
                            existing.Title = record.Title;
                            changed = true;
                        }
                        if (existing.Gender != record.Gender)
                        {
                            existing.Gender = record.Gender;
                            changed = true;
                        }
                        if (existing.Zip != record.Zip)
                        {
                            existing.Zip = record.Zip;
                            changed = true;
                        }

                        if (changed)
                        {
                            _dbContext.Users.Update(existing);
                            updated++;
                        }
                        else
                        {
                            skipped++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing CSV record for email {Email}", record.Email);
                }
            }

            // Änderungen speichern
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Import finished: {Inserted} inserted, {Updated} updated, {Skipped} skipped.", inserted, updated, skipped);
        }
    }

    /// <summary>
    /// Mapping-Klasse für CsvHelper, ordnet Header zu Record-Eigenschaften.
    /// </summary>
    public sealed class RapidmailRecordMap : ClassMap<RapidmailRecord>
    {
        public RapidmailRecordMap()
        {
            Map(m => m.Email).Name("email");
            Map(m => m.Firstname).Name("firstname");
            Map(m => m.Lastname).Name("lastname");
            Map(m => m.Gender).Name("gender");
            Map(m => m.Title).Name("title");
            Map(m => m.Zip).Name("zip");
        }
    }

    /// <summary>
    /// DTO für die CSV-Zeilen aus Rapidmail.
    /// </summary>
    public class RapidmailRecord
    {
        public string Email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
    }
}
