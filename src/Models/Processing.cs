using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Avalonia.Media.Imaging;

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
                return $"NIK: {NIK}\nNama: {Nama}\nTempat Lahir: {TempatLahir}\nTanggal Lahir: {TanggalLahir:yyyy-MM-dd}\n" +
                       $"Jenis Kelamin: {JenisKelamin}\nGolongan Darah: {GolonganDarah}\nAlamat: {Alamat}\nAgama: {Agama}\n" +
                       $"Status Perkawinan: {StatusPerkawinan}\nPekerjaan: {Pekerjaan}\nKewarganegaraan: {Kewarganegaraan}";
            }
        }

        public static (string bestPath, Biodata biodata, double time, double percentage) ProcessFingerprintMatching(Bitmap inputImage, bool algorithmChoice)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var relativePath = Path.Combine(basePath, "..", "..", "test");
            var absolutePath = Path.GetFullPath(relativePath);
            
            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
            }

            string logFilePath = Path.Combine(absolutePath, "log.txt");
            string notFoundPath = Path.Combine(absolutePath, "notfound.txt");

            try
            {
                var imageFilesTask = Task.Run(() => LoadImageFilesFromDatabaseAsync());
                var biodataListTask = Task.Run(() => LoadBiodataAsync());

                Task.WaitAll(imageFilesTask, biodataListTask);

                var imageFiles = imageFilesTask.Result;
                var biodataList = biodataListTask.Result;

                if (inputImage == null)
                {
                    return (string.Empty, new Biodata(), 0, 0);
                }

                string inputPattern = FingerprintConverter.ConvertImageToBinary(inputImage);
                inputPattern = FingerprintConverter.ConvertBinaryToAscii(inputPattern);

                string pattern = FingerprintConverter.CleanPattern(inputPattern, "ÿÿÿÿÿÿÿÿÿÿÿð");
                pattern = FingerprintConverter.CleanPattern(inputPattern, "ÿ");
                // File.AppendAllText(Path.Combine(absolutePath, "pattern.txt"), pattern);
                // File.AppendAllText(Path.Combine(absolutePath, "inputpattern.txt"), inputPattern);
                int algorithm = algorithmChoice ? 2 : 1;

                string bestMatchOwner = "";
                double highestSimilarity = 0;
                string bestPathImage = "";

                Stopwatch stopwatch = Stopwatch.StartNew();

                var tasks = imageFiles.AsParallel().Select(imageFile =>
                {
                    try
                    {
                        if (!File.Exists(imageFile.imagePath))
                        {
                            // File.AppendAllText(notFoundPath, $"File not found: {imageFile.imagePath}\n");
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
                                similarity = result.positions.Count > 0 ? 1.0 : RegexString.CalculateSimilarity(inputPattern, dbText);
                            }
                            else if (algorithm == 2)
                            {
                                var result = BMAlgorithm.BoyerMooreSearch(pattern, dbText);
                                similarity = result.positions.Count > 0 ? 1.0 : RegexString.CalculateSimilarity(inputPattern, dbText);
                            }

                            // File.AppendAllText(logFilePath, $"Similarity: {similarity:P2} Owner: {imageFile.ownerName} Path: {imageFile.imagePath}\n");
                            return (similarity, imageFile.ownerName, imageFile.imagePath);
                        }
                    }
                    catch (Exception)
                    {
                        return (similarity: 0.0, ownerName: imageFile.ownerName, imagePath: imageFile.imagePath);
                    }
                }).ToArray();

                foreach (var task in tasks)
                {
                    var result = task;
                    if (result.similarity > highestSimilarity)
                    {
                        highestSimilarity = result.similarity;
                        bestMatchOwner = result.ownerName;
                        bestPathImage = result.imagePath;
                    }
                }

                List<string> namaAlay = biodataList.Select(biodata => RegexString.ConvertAlayToOriginal(biodata.Nama)).ToList();

                double similarityThreshold = 0.7;

                string bestMatch = RegexString.FindBestMatch(bestMatchOwner, namaAlay, similarityThreshold, out double similarity);

                stopwatch.Stop();

                string bestPath = bestPathImage;
                Biodata bestBiodata = new Biodata();
                double time = stopwatch.ElapsedMilliseconds;
                double percentage = highestSimilarity * 100;

                if (highestSimilarity < similarityThreshold)
                {
                    return (string.Empty, new Biodata(), time, percentage);
                }
                else
                {
                    foreach (var biodata in biodataList)
                    {
                        if (RegexString.ConvertAlayToOriginal(biodata.Nama) == bestMatch)
                        {
                            bestBiodata = biodata;
                            bestBiodata.Nama = bestMatchOwner;
                            break;
                        }
                    }
                    return (bestPath, bestBiodata, time, percentage);
                }
            }
            catch (Exception)
            {
                return (string.Empty, new Biodata(), 0, 0);
            }
        }

        public static async Task<List<(string imagePath, string ownerName)>> LoadImageFilesFromDatabaseAsync()
        {
            var imageFiles = new List<(string imagePath, string ownerName)>();
            string connectionString = "Host=localhost;Username=postgres;Password=3663;Database=Tubes3_HapHipHop";
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
                // exceptions
            }

            return imageFiles;
        }

        public static async Task<List<Biodata>> LoadBiodataAsync()
        {
            var biodataList = new List<Biodata>();
            string connectionString = "Host=localhost;Username=postgres;Password=3663;Database=Tubes3_HapHipHop";
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
                                    NIK = SimpleAES.Decrypt(reader.GetString(0), "abcdefghijklmnop"),
                                    Nama = SimpleAES.Decrypt(reader.GetString(1), "abcdefghijklmnop"),
                                    TempatLahir = SimpleAES.Decrypt(reader.GetString(2), "abcdefghijklmnop"),
                                    TanggalLahir = SimpleAES.DecryptDate(reader.GetString(3), "abcdefghijklmnop"),
                                    JenisKelamin = reader.GetString(4),
                                    GolonganDarah = SimpleAES.Decrypt(reader.GetString(5), "abcdefghijklmnop"),
                                    Alamat = SimpleAES.Decrypt(reader.GetString(6), "abcdefghijklmnop"),
                                    Agama = SimpleAES.Decrypt(reader.GetString(7), "abcdefghijklmnop"),
                                    StatusPerkawinan = reader.GetString(8),
                                    Pekerjaan = SimpleAES.Decrypt(reader.GetString(9), "abcdefghijklmnop"),
                                    Kewarganegaraan = SimpleAES.Decrypt(reader.GetString(10), "abcdefghijklmnop")
                                };
                                biodataList.Add(biodata);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // exceptions
            }

            return biodataList;
        }
    }
}
