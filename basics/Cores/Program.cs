using Cores.Concurrency;
using Cores.Delegates;

// Logging.Main();

Thread t1 = new Thread(() => DownloadFile("Report.pdf"));
Thread t2 = new Thread(() => DownloadFile("Image.png"));
Thread t3 = new Thread(() => DownloadFile("Video.mp4"));

t1.Start();
t2.Start();
t3.Start();

t1.Join();
t2.Join();
t3.Join();

Console.WriteLine("All downloads finished.");

static void DownloadFile(string fileName)
{
    Console.WriteLine($"Starting download: {fileName}");
    Thread.Sleep(3000);
    Console.WriteLine($"Completed download: {fileName}");
}


// ---------------------------------------------------------------------------

Calculator c = new Calculator(Addition);
Console.WriteLine("Addition of 10 and 20 is : {0}", c(10, 20));

Calculator d = new Calculator(Subtraction);
Console.WriteLine("Subtraction of 10 and 20 is : {0}", d.Invoke(10, 20));

// ---------------------------------------------------------------------------

string filePath = "example.txt";
string content = "Hello, File processing!";

// Create a multicast delegate
FileProcessorDelegate filePro = LogToFile;
filePro += AppendTextToFile; // Adding another method to the multicast delegate

// Invoke the multicast delegate
filePro(filePath, content);

// ---------------------------------------------------------------------------

Func<int> getRandomNumber = delegate ()
                            {
                                Random r = new();
                                return r.Next(1, 100);
                            };

Action<int> printActionDel = delegate (int i) { Console.WriteLine(i); };

Predicate<string> isUpper = delegate (string s) { return s.Equals(s.ToUpper()); };


// ---------------------------------------------------------------------------

static int Addition(int a, int b)
{
    return a + b;
}

static int Subtraction(int a, int b)
{
    return a - b;
}

static void LogToFile(string filePath, string content)
{
    File.AppendAllText(filePath, $"Log Entry: {DateTime.Now} - {content}{Environment.NewLine}");
    Console.WriteLine("Logged information to file.");
}

static void AppendTextToFile(string filePath, string content)
{
    File.AppendAllText(filePath, content + Environment.NewLine);
    Console.WriteLine("Appended text to file.");
}

