using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace HangmanAssignment;

public partial class HangmanGamePage : ContentPage
{
    public HangmanGamePage()
    {
        InitializeComponent();
        Letters.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        BindingContext = this;
        PickWord();
    }

    // Fields
    #region Fields

    private List<string> words = new List<string>()
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

    private string answer = "";
    private List<char> guessed = new List<char>();
    private List<char> letters = new List<char>();
    private int errorsCount = 0;
    private int maximumErrors = 6;
    private string gameStatus = "";
    private string currentImage = "";
    private string spotlight = "";

    #endregion
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
    // UI Properties
    #region UI Properties

    public string Spotlight
    {
        get => spotlight;
        set
        {
            spotlight = value;
            OnPropertyChanged();
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
        get;
        private set;
    }

    public bool ResetButtonEnabled
    {
        get;
        private set;
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

    private void CalculateWord()
    {
        var temp = answer
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
            CalculateWord();
            CheckIfPlayerWins();
            OnPropertyChanged();
        }
        else
        {
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

    private void EnableOrDisableButtons(bool enabled)
    {
        ResetButtonEnabled = enabled;
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
        CalculateWord();
        EnableOrDisableButtons(true);
    }
}