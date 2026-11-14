# ApplicationTracker
A job portal in the console. Companies are approved by an admin, recruiters post jobs for their company, and candidates apply. The project practises collections, generics, delegates, events, lambdas, file handling and exceptions.

git commands i used in terminal using Cmder

git clone <url>
dotnet new gitignore           to create .gitignore file
git status
git push
git pull


Session One Assignment (Bonus Question)
what soild pricple does the code below break? explain


    static void Main()
    {
        IList<int> values = new int[] { 10, 20, 30 };

        values[0] = 100; // Works: arrays allow replacing elements
        Console.WriteLine($"values[0] = {values[0]}");

        try { values.Add(40); }
        catch (NotSupportedException ex) { Console.WriteLine($"Add: {ex.Message}"); }

        try { values.Insert(1, 15); }
        catch (NotSupportedException ex) { Console.WriteLine($"Insert: {ex.Message}"); }

        try { values.RemoveAt(0); }
        catch (NotSupportedException ex) { Console.WriteLine($"RemoveAt: {ex.Message}"); }

        Console.WriteLine($"IsFixedSize via array: {((Array)values).IsFixedSize}");
        Console.WriteLine($"IsReadOnly: {values.IsReadOnly}"); // True for arrays via IList<T>
    }



    Answer:

    The code above breaks the **Liskov Substitution Principle (LSP)**, which is one of the SOLID principles of object-oriented design. The LSP states that objects of a superclass should be replaceable with objects of a subclass without affecting the correctness of the program.

    “ISP violation in the interface design caused the LSP violation in the implementation.” Fix the first by splitting or narrowing interfaces, and the second disappears.

    <img src="https://drive.google.com/file/d/1sOf-wiTTT2qT66tbQofiS3r709lmmte0/view?usp=sharing" alt="Liskov Substitution Principle" width="600"/>