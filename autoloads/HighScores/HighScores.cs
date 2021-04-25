using Godot;
using Godot.Collections;

namespace LD48 {
    public class HighScore: Object {
        public string Name;
        public int Score;

        public HighScore(string name, int score) {
            Name = name;
            Score = score;
        }

        public Dictionary<string, object> ToJson() {
            return new Dictionary<string, object> {
                { "Name", Name },
                { "Score", Score }
            };
        }

        public static HighScore FromJson(Dictionary dict) {
            return new HighScore((string)dict["Name"], (int)(float)dict["Score"]);
        }
    }

    public class HighScores: Node {
        private const string HIGHSCORE_FILE = "user://highscores.dat";
        private static Array<HighScore> DEFAULT_HIGHSCORES = new Array<HighScore> {
            new HighScore("AAA", 20000),
            new HighScore("BBB", 15000),
            new HighScore("CCC", 10000),
        };

        private static HighScores globalInstance;
        public static HighScores Global {
            get => globalInstance;
        }

        private Array<HighScore> currentHighscores;
        public Array<HighScore> CurrentHighscores {
            get => currentHighscores;
        }

        public HighScores() {
            currentHighscores = LoadHighscoresFromStorage();

            if (globalInstance == null) {
                globalInstance = this;
            }
        }

        public int SubmitScoreToList(string name, int score) {
            int addPosition = -1;

            for (int i = 0; i < currentHighscores.Count; ++i) {
                var hs = currentHighscores[i];

                if (score > hs.Score) {
                    addPosition = i;
                    break;
                }
            }

            if (addPosition >= 0) {
                currentHighscores.Insert(addPosition, new HighScore(name, score));
                currentHighscores.RemoveAt(currentHighscores.Count - 1);
                StoreHighscoresToStorage(currentHighscores);
            }

            return addPosition + 1;
        }

        private Array<HighScore> LoadHighscoresFromStorage() {
            File file = new File();
            if (!file.FileExists(HIGHSCORE_FILE)) {
                return DEFAULT_HIGHSCORES;
            }

            file.Open(HIGHSCORE_FILE, File.ModeFlags.Read);
            var data = (Array)JSON.Parse(file.GetLine()).Result;
            var output = new Array<HighScore>();
            foreach (var entry in data) {
                output.Add(HighScore.FromJson((Dictionary)entry));
            }
            return output;
        }

        private void StoreHighscoresToStorage(Array<HighScore> scores) {
            File file = new File();
            file.Open(HIGHSCORE_FILE, File.ModeFlags.Write);
            var output = new Array<Dictionary<string, object>>();
            foreach (var score in scores) {
                output.Add(score.ToJson());
            }
            file.StoreLine(JSON.Print(output));
        }
    }
}
