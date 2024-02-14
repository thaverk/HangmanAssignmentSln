using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace HangmanAssignment;

public partial class HangmanGamePage : ContentPage
{
    public void MainPage()
    {
        InitializeComponent();
        Letters.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        BindingContext = this;
        PickWord();
        CalculateWord(answer, guessed);
        UpdateGameStatus();
    }

    // Fields
    #region Fields

    List<string> words = new List<string>()
     {
        "python",
        "javascript",
        "maui",
        "csharp",
        "mongodb",
        "sql",
        "xaml",
        "word",
        "excel",
        "powerpoint",
        "code",
        "hotreload",
        "snippets"
     };
    string answer = "";
    private string spotlight = "";
    List<char> guessed = new List<char>();
    private List<char> letters = new List<char>();
    private string message = "";
    int errorsCount = 0;
    int maximumErrors = 6;
    private string gameStatus = "";
    private string currentImage = "";
    private bool resetButtonEnabled = false;


    #endregion

    // UI Properties
    #region UI Properties

    public string Spotlight
    {
        get => spotlight;
        set
        {
            spotlight = value;
            OnPropertyChanged();
        }
    }

    public List<char> Letters
    {
        get => letters;
        set
        {
            letters = value;
            OnPropertyChanged();
        }
    }

    public string Message
    {
        get => message;
        set
        {
            message = value;
            OnPropertyChanged();
        }
    }

    public string GameStatus
    {
        get => gameStatus;
        set
        {
            gameStatus = value;
            OnPropertyChanged();
        }
    }

    public string CurrentImage
    {
        get => currentImage;
        set
        {
            currentImage = value;
            OnPropertyChanged();
        }
    }

    public bool ResetButtonEnabled
    {
        get => resetButtonEnabled;
        set
        {
            resetButtonEnabled = value;
            OnPropertyChanged();
        }
    }

    #endregion

    // Game engine
    #region Game engine

    private void PickWord()
    {
        answer = "";
        var rand = new Random();
        int index = rand.Next(words.Count);
        answer = words[index].ToUpper();
        Debug.WriteLine(answer);
    }

    private void CalculateWord(string _answer, List<char> guessed)
    {
        var temp = _answer
            .Select(x => (guessed.IndexOf(x) >= 0 ? x : '_'))
            .ToArray();

        Spotlight = String.Join(' ', temp);
    }

    private void HandleGuess(char letter)
    {
        if (!guessed.Contains(letter))
        {
            guessed.Add(letter);
        }
        if (answer.Contains(letter))
        {
            CalculateWord(answer, guessed);
            SemanticScreenReader.Announce($"{letter} exists!. {ReadSpotlight()}");
            CheckIfPlayerWins();
        }
        else
        {
            SemanticScreenReader.Announce($"{letter} does not exist. {ReadSpotlight()}");
            errorsCount++;
            UpdateGameStatus();
            if (errorsCount >= maximumErrors)
            {
                Message = "You Lost!";
                SemanticScreenReader.Announce($"You lost! The word was: {answer}");
                EnableOrDisableButtons(false);
            }
        }
    }

    private void UpdateGameStatus()
    {
        GameStatus = $"Errors: {errorsCount} / {maximumErrors}";
        CurrentImage = $"img{errorsCount}.jpg";
    }

    private void CheckIfPlayerWins()
    {
        if (answer.ToUpper() == Spotlight.ToUpper().Replace(" ", ""))
        {
            Message = "You Won!";
            SemanticScreenReader.Announce($"You won! The word was: {answer}");
            EnableOrDisableButtons(false);
        }
    }

    private string ReadSpotlight()
    {
        return Spotlight.Replace("_", "[, line]");
    }

    private void EnableOrDisableButtons(bool enabled)
    {
        ResetButtonEnabled = !enabled;
        foreach (var children in lettersContainer.Children)
        {
            var btn = children as Button;
            if (btn != null)
            {
                btn.IsEnabled = enabled;
            }
        }
    }

    #endregion

    private void Button_Clicked(object sender, EventArgs e)
    {
        var btn = sender as Button;
        if (btn != null)
        {
            var letter = btn.Text;
            btn.IsEnabled = false;
            HandleGuess(letter[0]);
        }
    }

    private void reset_Clicked(object sender, EventArgs e)
    {
        guessed.Clear();
        errorsCount = 0;
        UpdateGameStatus();
        PickWord();
        CalculateWord(answer, guessed);
        EnableOrDisableButtons(true);
    }
}