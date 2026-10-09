namespace s_tok.Services
{
    public static class ConsolePasswordReader
    {
        public static string ReadPassword()
        {
            var password = new System.Text.StringBuilder();

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Length--;
                    }

                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                }
            }

            return password.ToString();
        }
    }
}