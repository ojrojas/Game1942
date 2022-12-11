namespace Core.Levels
{
    public class LevelReader
    {
        private int _viewportWidth;

        private const int NB_ROWS = 11;
        private const int NB_TILE_ROWS = 10;

        public LevelReader(int viewportWidth)
        {
            _viewportWidth = viewportWidth;
        }

        public List<List<IBaseGameStateEvent>> LoadLevel(int nb)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.FullName.Split(',')[0];
            var fileName = $"{assemblyName}.Levels.LevelData.Level{nb}.txt";

            var stream = assembly.GetManifestResourceStream(fileName);

            string levelString;
            using (var reader = new StreamReader(stream))
            {
                levelString = reader.ReadToEnd();
            }

            if (levelString != null && levelString.Length > 0)
            {
                var rows = levelString.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var convertedRows = from r in rows
                                    select ToEventRow(r);

                return convertedRows.Reverse().ToList();
            }
            else
            {
                return new List<List<IBaseGameStateEvent>>();
            }
        }

        private List<IBaseGameStateEvent> ToEventRow(string rowString)
        {
            var elements = rowString.Split(',');

            var newRow = new List<IBaseGameStateEvent>();
            for (int i = 0; i < NB_ROWS; i++)
            {
                newRow.Add(ToEvent(i, elements[i]));
            }

            return newRow;
        }

        private IBaseGameStateEvent ToEvent(int elementNumber, string input)
        {
            switch (input)
            {
                case "0":
                    return new BaseGameStateEvent.Nothing();

                case "_":
                    return new LevelEvents.NoRowEvent();

                case "1":
                    var xPosition = elementNumber * _viewportWidth / NB_TILE_ROWS;
                    return new LevelEvents.GenerateTurret(xPosition);

                case "s":
                    return new LevelEvents.StartLevel();

                case "e":
                    return new LevelEvents.EndLevel();

                case string g when g.StartsWith("g"):
                    var nb = int.Parse(g.Substring(1));
                    return new LevelEvents.GenerateEnemies(nb);

                default:
                    return new BaseGameStateEvent.Nothing();
            }
        }
    }
}
