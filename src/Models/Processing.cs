using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace HapHipHop.Models
{
    public class Processing
    {
        public class Biodata
        {
            public string NIK { get; set; }
            public string Nama { get; set; }
            public string TempatLahir { get; set; }
            public DateTime TanggalLahir { get; set; }
            public string JenisKelamin { get; set; }
            public string GolonganDarah { get; set; }
            public string Alamat { get; set; }
            public string Agama { get; set; }
            public string StatusPerkawinan { get; set; }
            public string Pekerjaan { get; set; }
            public string Kewarganegaraan { get; set; }

            public override string ToString()
            {
                return $"NIK: {NIK}, Nama: {Nama}, Tempat Lahir: {TempatLahir}, Tanggal Lahir: {TanggalLahir:yyyy-MM-dd}, " +
                       $"Jenis Kelamin: {JenisKelamin}, Golongan Darah: {GolonganDarah}, Alamat: {Alamat}, Agama: {Agama}, " +
                       $"Status Perkawinan: {StatusPerkawinan}, Pekerjaan: {Pekerjaan}, Kewarganegaraan: {Kewarganegaraan}";
            }
        }

        public static async Task<(string bestPath, Biodata biodata, double time, double percentage)> ProcessFingerprintMatchingAsync(Bitmap inputImage, bool algorithmChoice)
        {
            // Initialize logging
            // string logFilePath = "debug_log.txt";
            // File.WriteAllText(logFilePath, "Starting ProcessFingerprintMatchingAsync\n");

            try
            {
                var loadImageTask = LoadImageFilesFromDatabaseAsync();
                var loadBiodataTask = LoadBiodataAsync();

                await Task.WhenAll(loadImageTask, loadBiodataTask);

                var imageFiles = loadImageTask.Result;
                var biodataList = loadBiodataTask.Result;

                if (inputImage == null)
                {
                    return (string.Empty, new Biodata(), 0, 0);
                }

                string inputPattern = FingerprintConverter.ConvertImageToBinary(inputImage);
                inputPattern = FingerprintConverter.ConvertBinaryToAscii(inputPattern);

                string toRemove = "ÿÿÿÿÿÿÿÿÿÿÿð";
                string pattern = FingerprintConverter.CleanPattern(inputPattern, toRemove);
                int algorithm = algorithmChoice ? 2 : 1;

                string bestMatchOwner = "";
                double highestSimilarity = 0;
                string bestPathImage = "";

                Stopwatch stopwatch = Stopwatch.StartNew();

                var tasks = imageFiles.Select(imageFile => Task.Run(() =>
                {
                    try
                    {
                        if (!File.Exists(imageFile.imagePath))
                        {
                            // File.AppendAllText(logFilePath, $"Error: {imageFile.imagePath} not found\n");
                            return (similarity: 0.0, ownerName: imageFile.ownerName, imagePath: imageFile.imagePath);
                        }

                        using (Bitmap dbImage = new Bitmap(imageFile.imagePath))
                        {
                            string dbText = FingerprintConverter.ConvertImageToBinary(dbImage);
                            dbText = FingerprintConverter.ConvertBinaryToAscii(dbText);

                            double similarity = 0;
                            if (algorithm == 1)
                            {
                                var result = KMPAlgorithm.KMPSearch(pattern, dbText);
                                similarity = RegexString.CalculateSimilarity(pattern, dbText);
                            }
                            else if (algorithm == 2)
                            {
                                var result = BMAlgorithm.BoyerMooreSearch(pattern, dbText);
                                similarity = RegexString.CalculateSimilarity(pattern, dbText);
                            }

                            File.AppendAllText(logFilePath, $"Owner: {imageFile.ownerName}, Path: {imageFile.imagePath}, Similarity: {similarity * 100}%\n");
                            return (similarity, imageFile.ownerName, imageFile.imagePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        return (similarity: 0.0, ownerName: imageFile.ownerName, imagePath: imageFile.imagePath);
                    }
                })).ToArray();

                var results = await Task.WhenAll(tasks);

                foreach (var result in results)
                {
                    if (result.similarity > highestSimilarity)
                    {
                        highestSimilarity = result.similarity;
                        bestMatchOwner = result.ownerName;
                        bestPathImage = result.imagePath;
                    }
                }

                List<string> namaAlay = new List<string>();
                foreach (var biodata in biodataList)
                {
                    string convertedName = RegexString.ConvertAlayToOriginal(biodata.Nama);
                    namaAlay.Add(convertedName);
                }

                double similarityThreshold = 0.5;

                string bestMatch = RegexString.FindBestMatch(bestMatchOwner, namaAlay, similarityThreshold, out double similarity);

                stopwatch.Stop();

                string bestPath = bestPathImage;
                Biodata bestBiodata = new Biodata();
                double time = stopwatch.ElapsedMilliseconds;
                double percentage = highestSimilarity * 100;

                if (highestSimilarity < similarityThreshold)
                {
                    // File.AppendAllText(logFilePath, "No match found\n");
                    return (string.Empty, new Biodata(), time, percentage);
                }
                else
                {
                    foreach (var biodata in biodataList)
                    {
                        if (RegexString.ConvertAlayToOriginal(biodata.Nama) == bestMatch)
                        {
                            bestBiodata = biodata;
                            break;
                        }
                    }

                    // File.AppendAllText(logFilePath, $"Best match: {bestBiodata.Nama}, Path: {bestPath}, Time: {time} ms, Similarity: {percentage}%\n");
                    return (bestPath, bestBiodata, time, percentage);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFilePath, $"Error: {ex.Message}\n");
                return (string.Empty, new Biodata(), 0, 0);
            }
        }

        public static async Task<List<(string imagePath, string ownerName)>> LoadImageFilesFromDatabaseAsync()
        {
            var imageFiles = new List<(string imagePath, string ownerName)>();
            string connectionString = "Host=localhost;Username=postgres;Password=3663;Database=tubes3_haphiphop";
            string query = "SELECT berkas_citra, nama FROM sidik_jari";
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var relativePath = Path.Combine(basePath, "..", "..", "..", "..", "test");
            var absolutePath = Path.GetFullPath(relativePath);

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string imagePath = Path.Combine(absolutePath, reader.GetString(0));
                                string ownerName = reader.GetString(1);
                                imagePath = imagePath.Replace("/", "\\");
                                imageFiles.Add((imagePath, ownerName));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }

            return imageFiles;
        }

        public static async Task<List<Biodata>> LoadBiodataAsync()
        {
            var biodataList = new List<Biodata>();
            string connectionString = "Host=localhost;Username=postgres;Password=3663;Database=tubes3_haphiphop";
            string query = "SELECT nik, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan FROM biodata";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var biodata = new Biodata
                                {
                                    NIK = reader.GetString(0),
                                    Nama = reader.GetString(1),
                                    TempatLahir = reader.GetString(2),
                                    TanggalLahir = reader.GetDateTime(3),
                                    JenisKelamin = reader.GetString(4),
                                    GolonganDarah = reader.GetString(5),
                                    Alamat = reader.GetString(6),
                                    Agama = reader.GetString(7),
                                    StatusPerkawinan = reader.GetString(8),
                                    Pekerjaan = reader.GetString(9),
                                    Kewarganegaraan = reader.GetString(10)
                                };
                                biodataList.Add(biodata);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }

            return biodataList;
        }
    }
}
