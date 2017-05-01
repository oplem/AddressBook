namespace AddressBook
{
    public class Recipe : IMatchable
    {
        public Recipe(string title, RecipeType type)
        {
            _title = title;
            _type = type; 
        }

        public bool Matches(string term)
        {
            return _title.ToLower().Contains(term.ToLower());
        }

        public override string ToString()
        {
            return $"RECIPE: {_title}";
        }

        private string _title;
        private readonly RecipeType _type;
    }
}
