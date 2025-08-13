using System.Diagnostics;

namespace ConsoleApp19
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //
            Subject subject = new Subject();
            subject.CreateExam();
            Console.Clear();
            Console.Write("\nStart Exam now? (y/n): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                
                subject.StartExam();
            }

            Console.WriteLine("\n--- Exam Finished ---");
        }
    }
}

public class Answer
{
    public int AnswerId { get; set; }
    public string AnswerText { get; set; }

    public Answer(int id, string text)
    {
        AnswerId = id;
        AnswerText = text;
    }

    public override string ToString()
    { return $"{AnswerId}. {AnswerText}"; }
}

public abstract class Question
{
    public string Header { get; set; }
    public string Body { get; set; }
    public int Mark { get; set; }
    public Answer[] Answers { get; set; }
    public Answer RightAnswer { get; set; }
    public Answer UserAnswer { get; set; }

    public Question(string body, int mark)
    {
        Body = body;
        Mark = mark;
    }

    public abstract void Display();
}

public class TrueFalse : Question
{
    public TrueFalse(string body, int mark)
        : base(body, mark)
    {
        Answers = new[]
        {
            new Answer(1, "True"),
            new Answer(2, "False")
        };
    }

    public override void Display()
    {
        Console.WriteLine($"\n {Body} ({Mark} Marks)");
        foreach (var ans in Answers)
            Console.WriteLine(ans);
    }
}

public class MCQ : Question
{
    public MCQ( string body, int mark)
        : base( body, mark) { }

    public override void Display()
    {
        Console.WriteLine($"\n{Body} ({Mark} Marks)");
        foreach (var ans in Answers)
            Console.WriteLine(ans);
    }
}

public abstract class Exam
{
    public TimeSpan Duration { get; set; }
    public int NumberOfQuestions { get; set; }

   public Question[] Questions;

    public Exam(TimeSpan duration, int numberOfQuestions)
    {
        Duration = duration;
        NumberOfQuestions = numberOfQuestions;
        Questions = new Question[NumberOfQuestions];
    }

    public abstract void ShowExam();
}

public class PracticalExam : Exam
{
    public PracticalExam(TimeSpan duration, int numberOfQuestions)
        : base(duration, numberOfQuestions) { }

    public override void ShowExam()
    {
        int total = 0;
        Stopwatch stopwatch = Stopwatch.StartNew();
        foreach (var q in Questions)
        {
            if (stopwatch.Elapsed >= Duration)
            {
                Console.WriteLine("Time End !");
                break;
            }
            q.Display();
            Console.Write("Your answer ( Number ): ");
            int userChoice = int.Parse(Console.ReadLine());
            if (stopwatch.Elapsed >= Duration)
            {
                Console.WriteLine("Time End !");
                break;
            }
            q.UserAnswer = q.Answers[userChoice - 1];
            if (q.UserAnswer.AnswerText == q.RightAnswer.AnswerText)
                total += q.Mark;
           
        }
        stopwatch.Stop();
        Console.WriteLine($"\n--- Exam Review ---");
        foreach (var q in Questions)
        {
            Console.WriteLine($"\n{q.Body} ({q.Mark} Marks)");
            Console.WriteLine($"Your Answer: {q.UserAnswer.AnswerText}");
            Console.WriteLine($"Correct Answer: {q.RightAnswer.AnswerText}");
        }

        Console.WriteLine($"\nYour Total Grade: {total}");

        Console.WriteLine($"\nYour Time: {stopwatch.Elapsed}");

    }
}

public class FinalExam : Exam
{
    public FinalExam(TimeSpan duration, int numberOfQuestions)
        : base(duration, numberOfQuestions) { }

    public override void ShowExam()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int total = 0;
        foreach (var q in Questions)
        {
            if (stopwatch.Elapsed >= Duration)
            {
                Console.WriteLine("Time End !");
                break;
            }
            q.Display();
            Console.Write("Your answer ( Number ): ");
            int userChoice = int.Parse(Console.ReadLine());
            if (stopwatch.Elapsed >= Duration)
            {
                Console.WriteLine("Time End !");
                break;
            }
            q.UserAnswer = q.Answers[userChoice - 1];

            if (q.UserAnswer.AnswerText == q.RightAnswer.AnswerText)
                total += q.Mark;
        }
        stopwatch.Stop();
        Console.WriteLine($"\n--- Exam Review ---");
        foreach (var q in Questions)
        {
            Console.WriteLine($"\n{q.Body} ({q.Mark} Marks)");
            Console.WriteLine($"Your Answer: {q.UserAnswer.AnswerText}");
            Console.WriteLine($"Correct Answer: {q.RightAnswer.AnswerText}");
        }

        Console.WriteLine($"\nYour Total Grade: {total}");

        Console.WriteLine($"\nYour Time: {stopwatch.Elapsed}");
    }
}

public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public Exam SubjectExam { get; set; }

    public void CreateExam()
    {
        Console.WriteLine("Choose exam type: (1) Final (2) Practical");
        int examType = int.Parse(Console.ReadLine());
        if(!(examType ==1||examType==2))
            throw new Exception("out of range");
        Console.Write("Enter Exam Duration from 60 to 180 (minutes): ");
        int minutes = int.Parse(Console.ReadLine());
        TimeSpan duration = TimeSpan.FromMinutes(minutes);

        Console.Write("Enter Number of Questions: ");
        int numberOfQuestions = int.Parse(Console.ReadLine());

        if (examType == 1)
            SubjectExam = new FinalExam(duration, numberOfQuestions);
        else if(examType == 2)
            SubjectExam = new PracticalExam(duration, numberOfQuestions);
        
        for (int i = 0; i < numberOfQuestions; i++)
            {
                Console.WriteLine($"\nEnter Question {i + 1}:");

                Console.Write("Body: ");
                string body = Console.ReadLine();

                Console.Write("Mark: ");
                int mark = int.Parse(Console.ReadLine());

                Question question;

                if (examType == 1)
                {
                    Console.Write("Choose Question Type (1) True/False (2) MCQ: ");
                    int qType = int.Parse(Console.ReadLine());

                    if (qType == 1)
                    {
                        question = new TrueFalse(body, mark);
                        Console.Write("Correct answer (1=True, 2=False): ");
                        int correct = int.Parse(Console.ReadLine());
                        question.RightAnswer = question.Answers[correct - 1];
                    }
                    else
                    {
                        question = CreateMCQ(body, mark);
                    }
                }
                else
                {
                    question = CreateMCQ(body, mark);
                }

                SubjectExam.Questions[i]=question;
            }
    }

    private MCQ CreateMCQ( string body, int mark)
    {
        
        Answer[] answers = new Answer[4];

        for (int i = 0; i < 4; i++)
        {
            Console.Write($"Enter choice {i + 1}: ");
            string txt = Console.ReadLine();
            answers[i] = new Answer(i + 1, txt);
        }

        Console.Write("Correct answer (number): ");
        int correctIndex = int.Parse(Console.ReadLine());

        return new MCQ(body, mark)
        {
            Answers = answers,
            RightAnswer = answers[correctIndex - 1]
        };
    }

    public void StartExam()
    {
        Console.WriteLine($"\n--- Starting Exam: {SubjectName} ---");
        SubjectExam.ShowExam();
    }
}



