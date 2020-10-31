using WebApi.DAL.Entities;

namespace WebApi.DAL.Extensions
{
    public static class CharacterExt
    {
        public static string GetName(this Character character)
        {
            string result = "";
            var type = character.GetType();
            if (type == typeof(Human))
            {
                var human = (Human)character;
                result = human.Firstname + " " + human.Lastname;
            }
            else if (type == typeof(Machine))
            {
                var machine = (Machine)character;
                result = machine.Name;
            }

            return result;
        }
    }
}
