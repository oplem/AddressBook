using System;
using System.Collections.Generic;
using System.IO;

namespace AddressBook
{
    public class ContactsRepository
    {
        private string _contactsFileName;

        public ContactsRepository(string contactsFileName)
        {
            _contactsFileName = contactsFileName;
        }

        public List<Contact> GetAllContacts()
        {
            List<Contact> contacts = new List<Contact>();
            using (StreamReader reader = File.OpenText(_contactsFileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('|');

                    if (parts[0] == "Company")
                    {
                        Company company = new Company(parts[1], parts[2]);
                        contacts.Add(company);
                    }
                    else if (parts[0] == "Person")
                    {
                        Person person = new Person(parts[1], parts[2], parts[3]);
                        contacts.Add(person);
                    }
                    else
                    {
                        Console.WriteLine("You have junk in your contacts file.");
                    }
                }
            }
            contacts.Sort();

            return contacts;
        }

        public void ReplaceAllContacts(List<Contact> listToSave)
        {
            using (StreamWriter writer = File.CreateText(_contactsFileName))
            {
                foreach (Contact contact in listToSave)
                {
                    Person person = contact as Person;
                    if (person != null)
                    {
                        PutInFile(person, writer);
                    }

                    Company company = contact as Company;
                    if (company != null)
                    {
                        PutInFile(company, writer);
                    }
                }
            }
        }

        public void CreateCompany(string name, string phoneNumber)
        {
            using (StreamWriter writer = File.AppendText(_contactsFileName))
            {
                Company company = new Company(name, phoneNumber);
                PutInFile(company, writer);
            }
        }

        public void CreatePerson(string firstName, string lastName, string phoneNumber)
        {
            using (StreamWriter writer = File.AppendText(_contactsFileName))
            {
                Person person = new Person(firstName, lastName, phoneNumber);
            }
        }

        private void PutInFile(Person person, StreamWriter writer)
        {
            string firstName = person.GetFirstName();
            string lastName = person.GetLastName();
            string phoneNumber = person.GetPhoneNumber();
            writer.WriteLine(string.Join("|", "Person", firstName, lastName, phoneNumber));
        }

        private void PutInFile(Company company, StreamWriter writer)
        {
            string name = company.GetName();
            string phoneNumber = company.GetPhoneNumber();
            writer.WriteLine(string.Join("|", "Company", name, phoneNumber));
        }
    }
}