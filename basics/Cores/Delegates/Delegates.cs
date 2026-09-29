namespace Cores.Delegates;

// its simply a Type for any function/ method

/* 
singlecast delegate 
 */
public delegate int Calculator(int x, int y);

/* 
multicast delegate -- holds references to multiple methods simultaneously.
it automatically calls all of its registered methods sequentially, in the exact order they were added!
 */
public delegate void FileProcessorDelegate(string filePath, string content);
