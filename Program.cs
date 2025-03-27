    using System;
    using System.Diagnostics;
    using System.IO;

    class AutoGitPush
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Write("📂 Nhập đường dẫn folder (ví dụ: D:\\Note): ");
            string folderPath = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                Console.WriteLine("❌ Đường dẫn không hợp lệ hoặc không tồn tại!");
                return;
            }

            if (!Directory.Exists(Path.Combine(folderPath, ".git")))
            {
                Console.WriteLine("❌ Thư mục này chưa được khởi tạo Git! Vui lòng chạy lệnh:");
                Console.WriteLine($"   git init && git remote add origin GITHUB_URL");
                return;
            }

            Console.WriteLine("🚀 Bắt đầu tự động commit & push...");

            while (true)
            {
                AutoCommitPush(folderPath);
                Console.WriteLine("⏳ Chờ 5 phút trước lần chạy tiếp theo...\n");
                System.Threading.Thread.Sleep(300000); // 5 phút
            }
        }

        static void AutoCommitPush(string repoPath)
        {
            Console.WriteLine("🔍 Kiểm tra thay đổi trong repo...");

            RunCommand("git pull origin master", repoPath);
            string changes = RunCommand("git status --porcelain", repoPath);

            if (string.IsNullOrEmpty(changes.Trim()))
            {
                Console.WriteLine("✅ Không có thay đổi. Thoát...");
                return;
            }

            Console.WriteLine("🔄 Có thay đổi! Tiến hành commit và push...");
            RunCommand("git add .", repoPath);
            RunCommand("git commit -m \"Auto commit update\"", repoPath);
            RunCommand("git push origin master", repoPath);
            Console.WriteLine("🚀 Push lên GitHub thành công!");
        }

        static string RunCommand(string command, string workingDir)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDir
            };

            Process process = new Process { StartInfo = psi };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
                Console.WriteLine($"❌ Lỗi: {error}");

            return output;
        }
    }
