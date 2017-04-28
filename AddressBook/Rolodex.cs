using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace AddressBook
{
    public class Rolodex
    {
        public Rolodex(string connectionString)
        {
            _connectionString = connectionString;
            _contacts = new List<Contact>();
            _recipes = new Dictionary<RecipeType, List<Recipe>>();

            _recipes.Add(RecipeType.Appetizers, new List<Recipe>());
            _recipes.Add(RecipeType.Entrees, new List<Recipe>());
            _recipes.Add(RecipeType.Deserts, new List<Recipe>());
        }

        public void DoStuff()
        {
            // Print a menu
            ShowMenu();
            // Get the user's choice
            MenuOption choice = GetMenuOption();
            
            // while the user does not want to exit
            while (choice != MenuOption.Exit)
            {
                // figure out what they want to do
                // get information
                // do stuff
                switch(choice)
                {
                    case MenuOption.AddPerson:
                        DoAddPerson();
                        break;
                    case MenuOption.AddCompany:
                        DoAddCompany();
                        break;
                    case MenuOption.ListContacts:
                        DoListContacts();
                        break;
                    case MenuOption.SearchContacts:
                        DoSearchContacts();
                        break;
                    case MenuOption.RemoveContact:
                        DoRemoveContact();
                        break;
                    case MenuOption.AddRecipe:
                        DoAddRecipe();
                        break;
                    case MenuOption.SearchEverything:
                        DoSearchEverything();
                        break;
                }
                ShowMenu();
                choice = GetMenuOption();
            }
        }


        private void DoSearchEverything()
        {
            Console.Clear();
            Console.WriteLine("SEARCH!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            List<IMatchable> matchables = new List<IMatchable>();
            matchables.AddRange(_contacts);

            using (SqlConnection connection = new SqlConnection(_connectionString))

            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"
                                        select RecipeTypeId
                                               , Name
                                           from Recipes";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string recipeTitle = reader.GetString(1);
                    Recipe recipe = new Recipe(recipeTitle);
                    matchables.Add(recipe);
                }
            }
                
            foreach (IMatchable matcher in matchables)
            {
                if (matcher.Matches(term))
                {
                    Console.WriteLine($"> {matcher}");
                }
            }
            Console.ReadLine();
        }

        private void DoAddRecipe()
        {
            Console.Clear();
            Console.WriteLine("Please enter your recipe title:");
            string title = GetNonEmptyStringFromUser();
            Recipe recipe = new Recipe(title);

            Console.WriteLine("What kind of recipe is this?");
            for (int i = 0; i < (int)RecipeType.UPPER_LIMIT; i += 1)
            {
                Console.WriteLine($"{i}. {(RecipeType)i}");
            }
            RecipeType choice = (RecipeType) int.Parse(Console.ReadLine());
            List<Recipe> specificRecipes = _recipes[choice];
            specificRecipes.Add(recipe);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"insert into recipes(recipetypeid, name)
                                        values(@typeid, @name)";
                command.Parameters.AddWithValue("@giraffe", choice);
                command.Parameters.AddWithValue("@lemur", title);
                command.ExecuteNonQuery();
            }
        }

        private void DoRemoveContact()
        {
            Console.Clear();
            Console.WriteLine("REMOVE A CONTACT!");
            Console.Write("Search for a contact: ");
            string term = GetNonEmptyStringFromUser();

            foreach (Contact contact in _contacts)
            {
                if (contact.Matches(term))
                {
                    Console.Write($"Remove {contact}? (y/N)");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        _contacts.Remove(contact);
                        return;
                    }
                }
            }

            Console.WriteLine("No more contacts found.");
            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
        }

        private void DoSearchContacts()
        {
            Console.Clear();
            Console.WriteLine("SEARCH!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser().ToLower();

            /*foreach (Contact contact in _contacts)
            {
                if (contact.Matches(term))
                {
                    Console.WriteLine($"> {contact}");
                }
            }
            */

            using (StreamReader reader = File.OpenText(contactFile))
            {
                while (!reader.EndOfStream)
                {
                    string orgLine = reader.ReadLine();
                    string lowerLine = orgLine.ToLower();

                    if (lowerLine.Contains(term))
                    {
                        Console.WriteLine(orgLine);
                    }
                }
            }
            Console.ReadLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private void DoListContacts()
        {
            Console.Clear();
            Console.WriteLine("YOUR CONTACTS");

            using (StreamReader reader = File.OpenText(contactFile))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    Console.WriteLine(line);
                }
            }
            Console.ReadLine();
        }

        private void DoListRecipe()
        {
            Console.Clear();
            Console.WriteLine("Recipes");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"
                    Select RecipeTypeid
                           , Name
                       from Recipes
                    order by RecipeTypeId
                            , Name
                    ";

                int currentRecipeTypeId = -1;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int recipeTypeId = reader.GetInt32(0);
                    string title = reader.GetString(1);

                    if (recipeTypeId != currentRecipeTypeId)
                    {
                        currentRecipeTypeId = recipeTypeId;
                        RecipeType pretty = (RecipeType)currentRecipeTypeId;
                        Console.WriteLine(pretty.ToString().ToUpper());
                    }
                        Console.WriteLine($" {title}");
                }
            }
            

            Console.ReadLine();
        }

        private void DoAddCompany()
        {
            Console.Clear();
            Console.WriteLine("Please enter information about the company.");
            Console.Write("Company name: ");
            string name = GetNonEmptyStringFromUser();

            Console.Write("Phone number: ");
            string phoneNumber = GetNonEmptyStringFromUser();

            _contacts.Add(new Company(name, phoneNumber));

            using (StreamWriter writer = new StreamWriter(contactFile, true))
            {
                writer.WriteLine(string.Join("|", "2", name, phoneNumber));
            }
        }

        private void DoAddPerson()
        {
            Console.Clear();
            Console.WriteLine("Please enter information about the person.");
            Console.Write("First name: ");
            string firstName = GetNonEmptyStringFromUser(); 

            Console.Write("Last name: ");
            string lastName = GetNonEmptyStringFromUser();

            Console.Write("Phone number: ");
            string phoneNumber = GetNonEmptyStringFromUser();

            _contacts.Add(new Person(firstName, lastName, phoneNumber));

            using (StreamWriter writer = new StreamWriter(contactFile, true))
            {
                writer.WriteLine(string.Join("|", "1", firstName, lastName, phoneNumber));
            }
        }

        private string GetNonEmptyStringFromUser()
        {
            string input = Console.ReadLine();
            while (input.Length == 0)
            {
                Console.WriteLine("That is not valid.");
                input = Console.ReadLine();
            }
            return input;
        }

        private int GetNumberFromUser()
        {
            while (true)
            {
                try
                {
                    string input = Console.ReadLine();
                    return int.Parse(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine("You should type a valid option number.");
                }
            }
        }

        private MenuOption GetMenuOption()
        {
            int choice = GetNumberFromUser(); 

            while (choice < 0 || choice >= (int)MenuOption.UPPER_LIMIT)
            {
                Console.WriteLine("That is not valid.");
                choice = GetNumberFromUser();
            }

            return (MenuOption)choice;
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine($"ROLODEX! ({_contacts.Count}) ({_recipes.Count})");
            Console.WriteLine("1. Add a person");
            Console.WriteLine("2. Add a company");
            Console.WriteLine("3. List all contacts");
            Console.WriteLine("4. Search contacts");
            Console.WriteLine("5. Remove a contact");
            Console.WriteLine("6. Add a recipe");
            Console.WriteLine("7. Search everything!!!");
            Console.WriteLine();
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("What would you like to do? ");
        }

        private readonly List<Contact> _contacts;
        private Dictionary<RecipeType, List<Recipe>> _recipes;
        private readonly string _connectionString;
        private string contactFile = "CONTACT.DAT";

    }
}
