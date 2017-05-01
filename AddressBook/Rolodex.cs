using System;
using System.Collections.Generic;

namespace AddressBook
{
    public class Rolodex
    {
        public Rolodex(string connectionString, string contactsFileName)
        {
            _contactsRepository = new ContactsRepository(contactsFileName);
            _recipesRepository = new RecipesRepository(connectionString);

            _contacts = new List<Contact>();
            _recipes = new Dictionary<RecipeType, List<Recipe>>();

            _recipes.Add(RecipeType.Appetizers, new List<Recipe>());
            _recipes[RecipeType.Entreés] = new List<Recipe>();
            _recipes.Add(RecipeType.Desserts, new List<Recipe>());
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
                    case MenuOption.ListReceipes:
                        DoListRecipes();
                        break;
                }
                ShowMenu();
                choice = GetMenuOption();
            }
        }

        private void DoListRecipes()
        {
            Console.Clear();
            Console.WriteLine("RECIPES!");

            List<Recipe> recipes = _recipesRepository.GetAllRecipes();

            foreach (Recipe recipe in recipes)
            {
                Console.WriteLine(recipe);
            }
            Console.ReadLine();
        }

        private void DoSearchEverything()
        {
            Console.Clear();
            Console.WriteLine("SEARCH EVERYTHING!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            List<Contact> contacts = _contactsRepository.GetAllContacts();
            List<IMatchable> matchables = new List<IMatchable>();
            matchables.AddRange(contacts);

            List<Recipe> recipes = _recipesRepository.GetAllRecipes();
            matchables.AddRange(recipes);

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

            Console.WriteLine("What kind of recipe is this?");
            for (int i = 0; i < (int)RecipeType.UPPER_LIMIT; i += 1)
            {
                Console.WriteLine($"{i}. {(RecipeType)i}");
            }
            RecipeType choice = (RecipeType)int.Parse(Console.ReadLine());
            _recipesRepository.Create(title, choice);
        }

        private void DoRemoveContact()
        {
            Console.Clear();
            Console.WriteLine("REMOVE A CONTACT!");
            Console.Write("Search for a contact: ");
            string term = GetNonEmptyStringFromUser();

            List<Contact> contacts = _contactsRepository.GetAllContacts();
            List<Contact> listToSave = new List<Contact>();
            foreach (Contact contact in contacts)
            {
                if (contact.Matches(term))
                {
                    Console.Write($"Remove {contact}? (y/N)");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        continue;
                    }
                    else
                    {
                        listToSave.Add(contact);
                    }
                }
                else
                {
                    listToSave.Add(contact); 
                }
            }
            _contactsRepository.ReplaceAllContacts(listToSave);

            Console.WriteLine("No more contacts found.");
            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
        }

        private void DoSearchContacts()
        {
            Console.Clear();
            Console.WriteLine("SEARCH!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            List<Contact> contacts = _contactsRepository.GetAllContacts();
            foreach (Contact contact in contacts)
            {
                if (contact.Matches(term))
                {
                    Console.WriteLine($"> {contact}");
                }
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private void DoListContacts()
        {
            Console.Clear();
            Console.WriteLine("YOUR CONTACTS");

            List<Contact> contacts = _contactsRepository.GetAllContacts();

            foreach (Contact contact in contacts)
            {
                Console.WriteLine(contact);
            }

            Console.ReadLine();
        }


        private void DoAddCompany()
        {
            Console.Clear();
            Console.WriteLine("Please enter information about the company.");
            Console.Write("Company name: ");
            string name = Console.ReadLine();

            Console.Write("Phone number: ");
            string phoneNumber = GetNonEmptyStringFromUser();

            _contactsRepository.CreateCompany(name, phoneNumber);
        }

        private void DoAddPerson()
        {
            Console.Clear();
            Console.WriteLine("Please enter information about the person.");
            Console.Write("First name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last name: ");
            string lastName = GetNonEmptyStringFromUser();

            Console.Write("Phone number: ");
            string phoneNumber = GetNonEmptyStringFromUser();

            _contactsRepository.CreatePerson(firstName, lastName, phoneNumber);
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
                    Console.WriteLine("You should type a number.");
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("THAT WAS BAD! DO AGAIN!");
                }
                finally
                {
                    Console.WriteLine("THIS will ALWAYS be PRINTED.");
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
            Console.WriteLine("-----------------------");
            Console.WriteLine("6. Add a recipe");
            Console.WriteLine("7. List recipes");
            Console.WriteLine("-----------------------");
            Console.WriteLine("8. Search everything!");
            Console.WriteLine();
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("What would you like to do? ");
        }

        private readonly List<Contact> _contacts;
        private Dictionary<RecipeType, List<Recipe>> _recipes;
        private readonly ContactsRepository _contactsRepository;
        private readonly RecipesRepository _recipesRepository;
    }
}
