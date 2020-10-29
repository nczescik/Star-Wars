using WebApi.DAL.Entities;

namespace WebApi.DAL.Extensions
{
    public static class CharacterExt
    {
        public static string GetName(this Character character)
        {
            string result = "";
            if (character.Discriminator == "Human")
            {
                var human = (Human)character;
                result = human.Firstname + " " + human.Lastname;
            }
            else if (character.Discriminator == "Machine")
            {
                var machine = (Machine)character;
                result = machine.Name;
            }

            return result;
        }
    }
}
