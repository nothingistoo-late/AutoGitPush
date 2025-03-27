using System;
using System.Diagnostics;
using System.IO;

class AutoGitPush
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true) // Vòng lặp vô hạn để restart app sau mỗi lần chạy
        {
            Console.Clear(); // Xóa màn hình console để làm mới
            Console.Write("📂 Nhập đường dẫn folder (ví dụ: D:\\Note): ");
            string folderPath = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                Console.WriteLine("❌ Đường dẫn không hợp lệ hoặc không tồn tại! Hãy nhập lại.\n");
                continue;
            }

            if (!Directory.Exists(Path.Combine(folderPath, ".git")))
            {
                Console.WriteLine("❌ Thư mục này chưa được khởi tạo Git! Vui lòng chạy lệnh:");
                Console.WriteLine($"   git init && git remote add origin GITHUB_URL\n");
                continue;
            }

            Console.Write("🌿 Nhập tên nhánh (mặc định: master): ");
            string branch = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(branch)) branch = "master";

            Console.Write("📝 Nhập commit message (mặc định: Auto commit update): ");
            string commitMessage = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(commitMessage)) commitMessage = "Auto commit update";

            Console.WriteLine($"\n🚀 Bắt đầu commit & push lên nhánh '{branch}' với message: \"{commitMessage}\"...\n");

            AutoCommitPush(folderPath, branch, commitMessage);

            Console.WriteLine("🔁 Hoàn thành! Nhấn Enter để nhập lại folder mới...");
            Console.ReadLine(); // Đợi người dùng nhấn Enter để bắt đầu lại
        }
    }

    static void AutoCommitPush(string repoPath, string branch, string commitMessage)
    {
        Console.WriteLine("🔍 Kiểm tra thay đổi trong repo...");

        RunCommand($"git pull origin {branch}", repoPath);
        string changes = RunCommand("git status --porcelain", repoPath);

        if (string.IsNullOrEmpty(changes.Trim()))
        {
            Console.WriteLine("✅ Không có thay đổi. Đợi lần kiểm tra tiếp theo...\n");
            return;
        }

        Console.WriteLine("🔄 Có thay đổi! Tiến hành commit và push...");
        RunCommand("git add .", repoPath);
        RunCommand($"git commit -m \"{commitMessage}\"", repoPath);
        RunCommand($"git push origin {branch}", repoPath);
        Console.WriteLine($"🚀 Push lên GitHub thành công trên nhánh '{branch}'!\n");
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

        if (!string.IsNullOrEmpty(error) && !error.Contains("->") && !error.Contains("up to date"))
        {
            Console.WriteLine($"❌ Lỗi: {error}");
        }
        else
        {
            Console.WriteLine(output);
        }

        return output;
    }
}
