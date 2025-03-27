using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class AutoGitPush
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.TreatControlCAsInput = true;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("🔹 Nhấn ESC bất kỳ lúc nào để thoát chương trình.\n");

            string folderPath = ReadValidFolderPath();
            if (folderPath == null) return;

            string branch = ReadInput("🌿 Nhập tên nhánh (mặc định: master): ", "master");
            if (branch == null) return;

            string commitMessage = ReadInput("📝 Nhập commit message (mặc định: Auto commit update): ", "Auto commit update");
            if (commitMessage == null) return;

            Console.WriteLine($"\n🚀 Bắt đầu commit & push lên nhánh '{branch}' với message: \"{commitMessage}\"...\n");

            AutoCommitPush(folderPath, branch, commitMessage);

            Console.WriteLine("🔁 Hoàn thành! Nhấn Enter để nhập lại folder mới hoặc ESC để thoát...");
            if (WaitForEscOrEnter()) return;
        }
    }

    static string ReadValidFolderPath()
    {
        while (true)
        {
            Console.Write("📂 Nhập đường dẫn folder (ví dụ: D:\\Note): ");
            string folderPath = ReadInputWithEsc();
            if (folderPath == null) return null;

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("❌ Thư mục không tồn tại! Hãy nhập lại.\n");
                continue;
            }

            if (!Directory.Exists(Path.Combine(folderPath, ".git")))
            {
                Console.WriteLine("❌ Thư mục này chưa được khởi tạo Git!");
                Console.WriteLine("⚡ Vui lòng chạy tạo repository trên github và thử lại:");
                Console.WriteLine("🔄 Nhấn Enter để thoát chương trình...");
                Console.ReadKey();
                return null;
            }

            return folderPath;
        }
    }

    static void AutoCommitPush(string repoPath, string branch, string commitMessage)
    {
        Console.WriteLine("🔍 Kiểm tra thay đổi trong repo...");

        // Thực hiện git pull --rebase để đồng bộ với remote
        string pullOutput = RunCommand($"git pull --rebase origin {branch}", repoPath);

        // Kiểm tra nếu có conflict sau khi pull
        if (pullOutput.Contains("CONFLICT"))
        {
            Console.WriteLine("❌ Gặp phải conflict! Hãy giải quyết conflict và thử lại.\n");
            return;
        }

        string changes = RunCommand("git status --porcelain", repoPath);

        if (string.IsNullOrEmpty(changes.Trim()))
        {
            Console.WriteLine("✅ Không có thay đổi. Đợi lần kiểm tra tiếp theo...\n");
            return;
        }

        Console.WriteLine("🔄 Có thay đổi! Tiến hành commit và push...");

        // Thực hiện git add, commit và push
        RunCommand("git add .", repoPath);
        string commitOutput = RunCommand($"git commit -m \"{commitMessage}\"", repoPath);

        // Kiểm tra nếu commit thành công, sau đó thực hiện git push
        if (!commitOutput.Contains("nothing to commit"))
        {
            string pushOutput = RunCommand($"git push origin {branch}", repoPath);
            Console.WriteLine($"🚀 Push lên GitHub thành công trên nhánh '{branch}'!\n");
        }
        else
        {
            Console.WriteLine("✅ Không có thay đổi để commit.\n");
        }
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

    static string ReadInputWithEsc()
    {
        string input = "";
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return input.Trim();
            }
            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\n👋 Thoát chương trình...");
                return null;
            }
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }
    }

    static string ReadInput(string message, string defaultValue)
    {
        Console.Write(message);
        string input = ReadInputWithEsc();
        return input == "" ? defaultValue : input;
    }

    static bool WaitForEscOrEnter()
    {
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter) return false;
            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\n👋 Thoát chương trình...");
                return true;
            }
        }
    }
}
